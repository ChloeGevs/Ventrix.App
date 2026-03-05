using ClosedXML.Excel;
using Guna.UI2.WinForms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventrix.App.Controls;
using Ventrix.App.Popups;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using Ventrix.Domain.Enums;

namespace Ventrix.App
{
    public partial class AdminDashboard : MaterialForm
    {
        private readonly InventoryService _inventoryService;
        private readonly BorrowService _borrowService;

        private bool isSidebarExpanded = true;
        private const int sidebarMaxWidth = 240;
        private const int sidebarMinWidth = 70;

        public AdminDashboard(InventoryService inventoryService, BorrowService borrowService)
        {
            _inventoryService = inventoryService;
            _borrowService = borrowService;

            InitializeComponent();
            ThemeManager.Initialize(this);
            InitializeMaterialSkin();

            ConfigureRuntimeUI();
            ApplyModernBranding();
            StyleDataGrids();

            Shown += async (s, e) => {
                this.PerformLayout();
                RefreshLayout();
                SetupInitialInnerLayout();
                await SwitchView("Home");
            };
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }
                
        private void ConfigureRuntimeUI()
        {
            FormClosed += (s, e) => System.Windows.Forms.Application.Exit();

            // Remove built-in docking so we can manually animate bounds flawlessly
            if (pnlSidebar != null) pnlSidebar.Dock = DockStyle.None;
            if (pnlTopBar != null) pnlTopBar.Dock = DockStyle.None;
            if (pnlMainContent != null) pnlMainContent.Dock = DockStyle.None;

            var controlsToBuffer = new Control[]
            {
                pnlMainContent, pnlGridContainer, pnlHistory,
                pnlHomeSummary, flowRecentActivity, pnlSidebar,
                dgvInventory, dgvHistory
            };

            foreach (var ctrl in controlsToBuffer)
            {
                if (ctrl != null)
                {
                    typeof(Control).InvokeMember("DoubleBuffered",
                        System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                        null, ctrl, new object[] { true });
                }
            }

            pnlSidebar?.BringToFront();

            // --- WIRE UP EVENTS ---
            if (btnCreate != null) btnCreate.Click += async (s, e) => await btnCreate_Click(s, e);
            if (btnEdit != null) btnEdit.Click += async (s, e) => await btnEdit_Click(s, e);
            if (btnDelete != null) btnDelete.Click += async (s, e) => await BtnDelete_Click(s, e);

            if (btnExportExcel != null) btnExportExcel.Click += (s, e) => ExportToExcel();
            if (btnExportPDF != null) btnExportPDF.Click += (s, e) => ExportToPDF();

            // --- SIDEBAR NAVIGATION CLICKS ---
            if (btnHome != null) btnHome.Click += async (s, e) => await SwitchView("Home");
            if (btnHistoryNav != null) btnHistoryNav.Click += async (s, e) => await SwitchView("History");
            if (btnNavAllItems != null) btnNavAllItems.Click += async (s, e) => await SwitchView("Inventory", "All");
            if (btnNavAvailable != null) btnNavAvailable.Click += async (s, e) => await SwitchView("Inventory", "Available");
            if (btnNavBorrowed != null) btnNavBorrowed.Click += async (s, e) => await SwitchView("Inventory", "Borrowed");
            if (btnNavBorrowers != null) btnNavBorrowers.Click += async (s, e) => await SwitchView("Inventory", "Borrower List");

            if (btnClearActivity != null) btnClearActivity.Click += async (s, e) => await ClearRecentActivity();
            if (cmbAccountActions != null) cmbAccountActions.SelectedIndexChanged += CmbAccountActions_SelectedIndexChanged;

            if (lblUrgentHeader != null)
            {
                lblUrgentHeader.Click += async (s, e) => await LblUrgentHeader_Click(s, e);
            }

            if (pnlMainContent != null)
            {
                pnlMainContent.UseTransparentBackground = true;
            }

            if (pnlHomeSummary != null && lblUrgentHeader != null)
            {
                pnlHomeSummary.MouseMove += async (s, e) => {
                    if (lblUrgentHeader.Bounds.Contains(e.Location))
                    {
                        var items = await _inventoryService.GetAllItemsAsync();
                        if (items.Any(x => x.Condition == "Damaged"))
                        {
                            Cursor = Cursors.Hand;
                            return;
                        }
                    }
                    Cursor = Cursors.Default;
                };
                pnlHomeSummary.MouseLeave += (s, e) => Cursor = Cursors.Default;
            }

            if (dgvInventory != null)
            {
                dgvInventory.CellDoubleClick += async (s, e) => await DgvInventory_CellDoubleClick(s, e);
                dgvInventory.CellFormatting += DgvInventory_CellFormatting;
            }

            // Cards now click to switch view too
            if (cardTotal != null) cardTotal.CardClicked += async (s, e) => await SwitchView("Inventory", "All");
            if (cardAvailable != null) cardAvailable.CardClicked += async (s, e) => await SwitchView("Inventory", "Available");
            if (cardPending != null) cardPending.CardClicked += async (s, e) => await SwitchView("Inventory", "Borrowed");
            if (cardBorrowers != null) cardBorrowers.CardClicked += async (s, e) => await SwitchView("Inventory", "Borrower List");

            if (sidebarTimer != null && btnHamburger != null)
            {
                sidebarTimer.Interval = 10;
                btnHamburger.Click += (s, e) =>
                {
                    if (pnlGridContainer != null) pnlGridContainer.ShadowDecoration.Enabled = false;
                    if (pnlHomeSummary != null) pnlHomeSummary.ShadowDecoration.Enabled = false;
                    if (pnlHistory != null) pnlHistory.ShadowDecoration.Enabled = false;

                    sidebarTimer.Start();
                };
                sidebarTimer.Tick += SidebarTimer_Tick;
            }

            if (txtSearch != null)
            {
                txtSearch.IconRightCursor = Cursors.Hand;
                txtSearch.IconRightSize = new Size(0, 0);

                if (searchTimer != null)
                {
                    searchTimer.Interval = 300;
                    searchTimer.Tick += async (sender, args) =>
                    {
                        searchTimer.Stop();
                        await LoadFromDatabase("All");
                    };
                }

                txtSearch.TextChanged += (s, e) =>
                {
                    txtSearch.IconRightSize = string.IsNullOrEmpty(txtSearch.Text) ? new Size(0, 0) : new Size(15, 15);
                    searchTimer?.Stop();
                    searchTimer?.Start();
                };

                txtSearch.KeyDown += async (s, e) =>
                {
                    if (e.KeyCode == Keys.Enter)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        searchTimer?.Stop();
                        await LoadFromDatabase("All");
                    }
                    else if (e.KeyCode == Keys.Escape)
                    {
                        e.Handled = true;
                        e.SuppressKeyPress = true;
                        txtSearch.Clear();
                        this.Focus();
                    }
                };

                txtSearch.IconRightClick += (s, e) =>
                {
                    if (!string.IsNullOrEmpty(txtSearch.Text))
                    {
                        txtSearch.Clear();
                        txtSearch.Focus();
                    }
                };
            }

            // --- ULTRA-SOFT "GLASS" DROP SHADOWS FOR CONTAINERS ---
            var containers = new[] { pnlGridContainer, pnlHomeSummary, pnlHistory };

            foreach (var container in containers)
            {
                if (container != null)
                {
                    container.BorderRadius = 12;
                    container.FillColor = Color.White;
                    container.ShadowDecoration.Enabled = true;
                    container.ShadowDecoration.BorderRadius = 12;
                    container.ShadowDecoration.Color = Color.FromArgb(15, 0, 0, 0);
                    container.ShadowDecoration.Depth = 10;
                    container.ShadowDecoration.Shadow = new Padding(4, 4, 10, 10);
                }
            }

            var metricCards = new[] { cardTotal, cardAvailable, cardPending, cardBorrowers };
            foreach (var card in metricCards)
            {
                if (card != null)
                {
                    card.MouseEnter += (s, e) => { card.Location = new Point(card.Location.X, card.Location.Y - 4); };
                    card.MouseLeave += (s, e) => { card.Location = new Point(card.Location.X, card.Location.Y + 4); };
                }
            }

            // --- CUSTOM MODERN SCROLLBAR FOR RECENT ACTIVITY ---
            if (flowRecentActivity != null && pnlHomeSummary != null)
            {
                flowRecentActivity.AutoScroll = false;
                Guna.UI2.WinForms.Guna2VScrollBar customScroll = new Guna.UI2.WinForms.Guna2VScrollBar
                {
                    BindingContainer = flowRecentActivity,
                    BorderRadius = 4,
                    ThumbColor = Color.FromArgb(200, 200, 200),
                    FillColor = Color.Transparent,
                    Width = 8,
                    Margin = new Padding(0, 10, 5, 10),
                    Dock = DockStyle.Right
                };
                customScroll.HoverState.ThumbColor = ThemeManager.VentrixLightBlue;
                pnlHomeSummary.Controls.Add(customScroll);
                customScroll.BringToFront();
            }

            // --- SLEEK CUSTOM DATAGRID SCROLLBAR ---
            if (dgvInventory != null && pnlGridContainer != null)
            {
                dgvInventory.ScrollBars = ScrollBars.None;
                Guna.UI2.WinForms.Guna2VScrollBar gridScroll = new Guna.UI2.WinForms.Guna2VScrollBar
                {
                    BindingContainer = dgvInventory,
                    BorderRadius = 4,
                    ThumbColor = Color.FromArgb(200, 200, 200),
                    FillColor = Color.Transparent,
                    Width = 8,
                    Margin = new Padding(0, 10, 5, 10),
                    Dock = DockStyle.Right
                };
                gridScroll.HoverState.ThumbColor = ThemeManager.VentrixLightBlue;
                pnlGridContainer.Controls.Add(gridScroll);
                gridScroll.BringToFront();
            }

            // --- MODERN RIGHT-CLICK CONTEXT MENU ---
            Guna.UI2.WinForms.Guna2ContextMenuStrip gridMenu = new Guna.UI2.WinForms.Guna2ContextMenuStrip();
            gridMenu.RenderStyle.SelectionBackColor = ThemeManager.VentrixLightBlue;
            gridMenu.RenderStyle.SelectionForeColor = Color.White;
            gridMenu.Font = new System.Drawing.Font("Segoe UI", 10F);

            ToolStripMenuItem editOption = new ToolStripMenuItem("✏️ Edit Record");
            ToolStripMenuItem deleteOption = new ToolStripMenuItem("🗑️ Delete Item");
            deleteOption.ForeColor = Color.IndianRed;

            gridMenu.Items.Add(editOption);
            gridMenu.Items.Add(deleteOption);

            if (btnEdit != null) editOption.Click += (s, e) => btnEdit.PerformClick();
            if (btnDelete != null) deleteOption.Click += (s, e) => btnDelete.PerformClick();

            if (dgvInventory != null)
            {
                dgvInventory.CellMouseDown += (s, e) =>
                {
                    if (e.Button == MouseButtons.Right && e.RowIndex >= 0)
                    {
                        dgvInventory.ClearSelection();
                        dgvInventory.Rows[e.RowIndex].Selected = true;
                        gridMenu.Show(Cursor.Position);
                    }
                };
            }

            this.Load += (s, e) =>
            {
                ApplyModernBranding();
                RefreshLayout();
            };

            Resize += (s, e) => {
                if (WindowState != FormWindowState.Minimized) RefreshLayout();
            };
        }

        private void SetupInitialInnerLayout()
        {
            if (pnlMainContent == null) return;

            int horizontalMargin = 30;
            int topMargin = 85;
            int bottomMargin = 40;

            int availableWidth = pnlMainContent.Width - (horizontalMargin * 2);
            int availableHeight = pnlMainContent.Height - topMargin - bottomMargin;

            Size shiftedSize = new Size(availableWidth, availableHeight);
            Point shiftedLocation = new Point(horizontalMargin, topMargin);

            var fillAnchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            if (pnlHomeSummary != null)
            {
                pnlHomeSummary.Bounds = new System.Drawing.Rectangle(shiftedLocation, shiftedSize);
                pnlHomeSummary.Anchor = fillAnchor;
            }
            if (pnlGridContainer != null)
            {
                pnlGridContainer.Bounds = new System.Drawing.Rectangle(shiftedLocation, shiftedSize);
                pnlGridContainer.Anchor = fillAnchor;
            }
            if (pnlHistory != null)
            {
                pnlHistory.Bounds = new System.Drawing.Rectangle(shiftedLocation, shiftedSize);
                pnlHistory.Anchor = fillAnchor;
            }

            if (dgvInventory != null && pnlGridContainer != null)
            {
                dgvInventory.Bounds = new System.Drawing.Rectangle(5, 5, pnlGridContainer.Width - 10, pnlGridContainer.Height - 10);
                dgvInventory.Anchor = fillAnchor;
            }
            if (dgvHistory != null && pnlHistory != null)
            {
                dgvHistory.Bounds = new System.Drawing.Rectangle(5, 5, pnlHistory.Width - 10, pnlHistory.Height - 10);
                dgvHistory.Anchor = fillAnchor;
            }

            int spacing = 15;
            int rightEdgeMargin = horizontalMargin;
            int buttonY = 25;

            if (btnExportExcel != null)
            {
                btnExportExcel.Location = new Point(horizontalMargin, buttonY);
                btnExportExcel.BringToFront();
            }
            if (btnExportPDF != null && btnExportExcel != null)
            {
                btnExportPDF.Location = new Point(btnExportExcel.Right + spacing, buttonY);
                btnExportPDF.BringToFront();
            }

            if (btnDelete != null)
            {
                btnDelete.Location = new Point(pnlMainContent.Width - btnDelete.Width - rightEdgeMargin, buttonY);
                btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnDelete.BringToFront();
            }
            if (btnEdit != null && btnDelete != null)
            {
                btnEdit.Location = new Point(btnDelete.Left - btnEdit.Width - spacing, buttonY);
                btnEdit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnEdit.BringToFront();
            }
            if (btnCreate != null && btnEdit != null)
            {
                btnCreate.Location = new Point(btnEdit.Left - btnCreate.Width - spacing, buttonY);
                btnCreate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnCreate.BringToFront();
            }

            if (lblUrgentHeader != null) lblUrgentHeader.Location = new Point(horizontalMargin, 35);

            if (btnClearActivity != null && pnlHomeSummary != null)
            {
                btnClearActivity.Parent = pnlHomeSummary;
                btnClearActivity.Location = new Point(pnlHomeSummary.Width - btnClearActivity.Width - 20, 20);
                btnClearActivity.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            }

            // --- ARRANGE METRIC CARDS HORIZONTALLY ON THE HOME PAGE ---
            if (pnlHomeSummary != null && cardTotal != null && cardAvailable != null && cardPending != null && cardBorrowers != null)
            {
                cardTotal.Parent = pnlHomeSummary;
                cardAvailable.Parent = pnlHomeSummary;
                cardPending.Parent = pnlHomeSummary;
                cardBorrowers.Parent = pnlHomeSummary;

                int cardSpacing = 20;
                int startX = 20;
                int startY = 120;

                int totalAvailableWidth = pnlHomeSummary.Width - (startX * 2) - (cardSpacing * 3);
                int exactCardWidth = totalAvailableWidth / 4;

                cardTotal.Bounds = new System.Drawing.Rectangle(startX, startY, exactCardWidth, 110);
                cardAvailable.Bounds = new System.Drawing.Rectangle(cardTotal.Right + cardSpacing, startY, exactCardWidth, 110);
                cardPending.Bounds = new System.Drawing.Rectangle(cardAvailable.Right + cardSpacing, startY, exactCardWidth, 110);
                cardBorrowers.Bounds = new System.Drawing.Rectangle(cardPending.Right + cardSpacing, startY, exactCardWidth, 110);

                if (flowRecentActivity != null)
                {
                    flowRecentActivity.Location = new Point(startX, cardTotal.Bottom + 30);
                    flowRecentActivity.Size = new Size(pnlHomeSummary.Width - 40, pnlHomeSummary.Height - flowRecentActivity.Top - 20);
                }
            }
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            if (pnlSidebar == null || sidebarTimer == null) return;

            Action finalizeAnimation = () => {
                if (pnlGridContainer != null) pnlGridContainer.ShadowDecoration.Enabled = true;
                if (pnlHomeSummary != null) pnlHomeSummary.ShadowDecoration.Enabled = true;
                if (pnlHistory != null) pnlHistory.ShadowDecoration.Enabled = true;
                RefreshLayout();
            };

            if (!this.ContainsFocus)
            {
                sidebarTimer.Stop();
                finalizeAnimation();
                return;
            }

            int targetWidth = isSidebarExpanded ? sidebarMinWidth : sidebarMaxWidth;
            int distanceRemaining = Math.Abs(targetWidth - pnlSidebar.Width);
            int easingFactor = 4;

            if (distanceRemaining <= 2)
            {
                pnlSidebar.Width = targetWidth;
                isSidebarExpanded = !isSidebarExpanded;
                sidebarTimer.Stop();
                finalizeAnimation();
            }
            else
            {
                int step = (distanceRemaining / easingFactor) + 1;

                if (isSidebarExpanded) pnlSidebar.Width -= step;
                else pnlSidebar.Width += step;

                pnlMainContent.Location = new Point(pnlSidebar.Width, pnlMainContent.Location.Y);
                pnlMainContent.Width = this.Width - pnlSidebar.Width;
            }
        }

        private void RefreshLayout()
        {
            if (pnlMainContent == null || pnlSidebar == null) return;

            int formWidth = this.ClientSize.Width;
            int formHeight = this.ClientSize.Height;
            int topMargin = 64;
            int topBarHeight = 80;

            if (pnlTopBar != null)
            {
                pnlTopBar.SetBounds(0, topMargin, formWidth, topBarHeight);

                if (btnHamburger != null) btnHamburger.Location = new Point(20, (pnlTopBar.Height - btnHamburger.Height) / 2);
                if (lblDashboardHeader != null) lblDashboardHeader.Location = new Point(75, (pnlTopBar.Height - lblDashboardHeader.Height) / 2);

                if (txtSearch != null)
                {
                    txtSearch.Location = new Point(pnlTopBar.Width - txtSearch.Width - 40, (pnlTopBar.Height - txtSearch.Height) / 2);
                }
            }

            int contentY = topMargin + (pnlTopBar != null ? pnlTopBar.Height : 0);
            int remainingHeight = formHeight - contentY;

            pnlSidebar.SetBounds(0, contentY, pnlSidebar.Width, remainingHeight);

            int mainContentWidth = formWidth - pnlSidebar.Width;
            pnlMainContent.SetBounds(pnlSidebar.Width, contentY, mainContentWidth, remainingHeight);

            UpdateSidebarInternalUI();

            lblDashboardHeader?.Invalidate();

            if (badgeHealth != null && txtSearch != null)
            {
                badgeHealth.Location = new Point(txtSearch.Left - badgeHealth.Width - 20, (pnlTopBar.Height - badgeHealth.Height) / 2);
            }
        }

        private void UpdateSidebarInternalUI()
        {
            if (pnlSidebar == null) return;
            pnlSidebar.SuspendLayout();

            int currentWidth = pnlSidebar.Width;
            int contentWidth = currentWidth - 20;
            bool expanded = currentWidth > 200;

            int picSize = expanded ? 45 : 40;
            if (picUser != null) picUser.SetBounds(expanded ? 15 : 12, 25, picSize, picSize);

            if (lblOwnerRole != null) lblOwnerRole.Visible = expanded;
            if (cmbAccountActions != null) cmbAccountActions.Visible = expanded;

            if (expanded)
            {
                if (lblOwnerRole != null) lblOwnerRole.Location = new Point(70, 25);
                if (cmbAccountActions != null) cmbAccountActions.SetBounds(65, 42, 160, 30);
            }

            // --- STACK THE NAVIGATION BUTTONS ---
            bool showNav = currentWidth > 100;
            var navBtns = new[] { btnHome, btnHistoryNav, btnNavAllItems, btnNavAvailable, btnNavBorrowed, btnNavBorrowers };

            int btnY = 120;
            foreach (var btn in navBtns)
            {
                if (btn != null)
                {
                    btn.Visible = showNav;
                    if (showNav)
                    {
                        btn.SetBounds(10, btnY, contentWidth, 45);
                        btnY += 80; // Spacing between buttons
                    }
                }
            }

            pnlSidebar.ResumeLayout(false);
        }

        private string GetGreeting()
        {
            int hour = DateTime.Now.Hour;
            if (hour < 12) return "GOOD MORNING";
            if (hour < 17) return "GOOD AFTERNOON";
            return "GOOD EVENING";
        }

        #region Navigation & Data Loading

        private void HighlightNavButton(Guna.UI2.WinForms.Guna2Button activeBtn)
        {
            if (activeBtn == null) return;
            activeBtn.FillColor = Color.FromArgb(40, 255, 255, 255);
            activeBtn.CustomBorderColor = Color.Orange;
            activeBtn.CustomBorderThickness = new Padding(4, 0, 0, 0);
        }

        private async Task SwitchView(string viewName, string filter = "All")
        {
            if (pnlHomeSummary != null) pnlHomeSummary.Visible = (viewName == "Home");
            if (pnlGridContainer != null) pnlGridContainer.Visible = (viewName == "Inventory");
            if (pnlHistory != null) pnlHistory.Visible = (viewName == "History");

            bool showCrud = (viewName == "Inventory" && filter != "Borrowed" && filter != "Borrower List");

            if (btnCreate != null) btnCreate.Visible = showCrud;
            if (btnEdit != null) btnEdit.Visible = showCrud;
            if (btnDelete != null) btnDelete.Visible = showCrud;

            if (txtSearch != null)
            {
                if (viewName == "Inventory")
                    txtSearch.PlaceholderText = "Search by item name, category, or ID...";
                else if (viewName == "History")
                    txtSearch.PlaceholderText = "Search by borrower name or action...";
                else
                    txtSearch.PlaceholderText = "Search records...";
            }

            // Reset all nav button highlights
            var navButtons = new[] { btnHome, btnHistoryNav, btnNavAllItems, btnNavAvailable, btnNavBorrowed, btnNavBorrowers };
            foreach (var btn in navButtons)
            {
                if (btn != null)
                {
                    btn.FillColor = Color.Transparent;
                    btn.CustomBorderThickness = new Padding(0);
                }
            }

            switch (viewName)
            {
                case "Home":
                    if (lblDashboardHeader != null) lblDashboardHeader.Text = $"{GetGreeting()}, ADMIN";
                    pnlHomeSummary?.BringToFront();
                    HighlightNavButton(btnHome);
                    await LoadHomeContent();
                    break;

                case "Inventory":
                    pnlGridContainer?.BringToFront();
                    if (lblDashboardHeader != null) lblDashboardHeader.Text = $"INVENTORY: {filter.ToUpper()}";

                    if (filter == "All") HighlightNavButton(btnNavAllItems);
                    else if (filter == "Available") HighlightNavButton(btnNavAvailable);
                    else if (filter == "Borrowed") HighlightNavButton(btnNavBorrowed);
                    else if (filter == "Borrower List") HighlightNavButton(btnNavBorrowers);

                    await LoadFromDatabase(filter);
                    break;

                case "History":
                    pnlHistory?.BringToFront();
                    if (lblDashboardHeader != null) lblDashboardHeader.Text = "TRANSACTION AUDIT HISTORY";
                    HighlightNavButton(btnHistoryNav);
                    await LoadHistoryData();
                    break;
            }

            bool isInventory = (viewName == "Inventory");
            if (btnExportExcel != null) btnExportExcel.Visible = isInventory;
            if (btnExportPDF != null) btnExportPDF.Visible = isInventory;

            if (showCrud)
            {
                btnCreate?.BringToFront();
                btnEdit?.BringToFront();
                btnDelete?.BringToFront();
            }

            await UpdateDashboardCounts();
            pnlSidebar?.BringToFront();
        }

        private async Task UpdateSystemHealthBadge()
        {
            var items = await _inventoryService.GetAllItemsAsync();
            int damagedCount = items.Count(x => x.Condition == "Damaged");

            if (badgeHealth != null)
            {
                if (damagedCount > 0)
                {
                    badgeHealth.Text = $"SYSTEM ALERTS: {damagedCount} ISSUES";
                    badgeHealth.FillColor = Color.FromArgb(255, 235, 238);
                    badgeHealth.ForeColor = Color.Red;
                }
                else
                {
                    badgeHealth.Text = "ALL SYSTEMS OPERATIONAL";
                    badgeHealth.FillColor = Color.FromArgb(232, 245, 233);
                    badgeHealth.ForeColor = Color.MediumSeaGreen;
                }
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.F))
            {
                if (txtSearch != null && txtSearch.Visible)
                {
                    txtSearch.Focus();
                    txtSearch.SelectAll();
                    return true;
                }
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private async Task LblUrgentHeader_Click(object sender, EventArgs e)
        {
            var damagedItems = (await _inventoryService.GetAllItemsAsync()).Where(i => i.Condition == "Damaged").ToList();
            if (damagedItems.Any())
            {
                using (var popup = new RepairDetailsPopup(damagedItems, _inventoryService, async () => await LoadHomeContent()))
                {
                    popup.StartPosition = FormStartPosition.CenterParent;
                    popup.ShowDialog();
                    await UpdateDashboardCounts();
                }
            }
            else
            {
                MessageBox.Show("All systems are currently operational. No repairs needed.", "Ventrix System");
            }
        }

        private async Task DgvInventory_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvInventory == null) return;
            int id = Convert.ToInt32(dgvInventory.Rows[e.RowIndex].Cells[0].Value);
            string name = dgvInventory.Rows[e.RowIndex].Cells[1].Value?.ToString() ?? "";
            string status = dgvInventory.Rows[e.RowIndex].Cells[3].Value?.ToString() ?? "";

            if (status == ItemStatus.Available.ToString())
            {
                using (var popup = new BorrowPopup(_borrowService, id, name))
                {
                    if (popup.ShowDialog() == DialogResult.OK)
                    {
                        await LoadFromDatabase("All");
                        await UpdateDashboardCounts();
                    }
                }
            }
            else MessageBox.Show("This item is already borrowed or unavailable.", "Ventrix System");
        }

        private void CmbAccountActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAccountActions?.SelectedItem?.ToString() == "Sign out")
            {
                if (MessageBox.Show("Are you sure you want to sign out?", "Ventrix System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    new BorrowerPortal(_inventoryService, _borrowService, new UserService(new Ventrix.Infrastructure.Data.AppDbContext())).Show();
                    this.Close();
                }
                else cmbAccountActions.SelectedIndex = -1;
            }
        }

        private async Task LoadFromDatabase(string statusFilter)
        {
            if (dgvInventory == null) return;

            dgvInventory.Rows.Clear();
            dgvInventory.Columns.Clear();

            var items = await _inventoryService.GetAllItemsAsync();

            if (txtSearch != null && !string.IsNullOrEmpty(txtSearch.Text))
            {
                string search = txtSearch.Text.ToLower();
                items = items.Where(i => i.Name.ToLower().Contains(search)).ToList();
            }

            var filteredItems = items;
            if (statusFilter == "Available")
            {
                filteredItems = items.Where(x => x.Status == ItemStatus.Available).ToList();
            }

            if (lblEmptyState != null)
            {
                bool isEmpty = (statusFilter == "Borrowed" || statusFilter == "Borrower List")
                    ? !(await _borrowService.GetAllBorrowRecordsAsync()).Any()
                    : !filteredItems.Any();

                lblEmptyState.Visible = isEmpty;

                if (txtSearch != null)
                {
                    if (isEmpty && !string.IsNullOrEmpty(txtSearch.Text))
                    {
                        txtSearch.FocusedState.BorderColor = Color.IndianRed;
                        txtSearch.BorderColor = Color.FromArgb(255, 200, 200);
                    }
                    else
                    {
                        txtSearch.FocusedState.BorderColor = ThemeManager.VentrixBlue;
                        txtSearch.BorderColor = Color.Transparent;
                    }
                }
            }

            switch (statusFilter)
            {
                case "Available":
                    SetupColumns("ID", "Item Name", "Category", "Condition");
                    foreach (var i in filteredItems)
                    {
                        dgvInventory.Rows.Add(i.Id, i.Name, i.Category, i.Condition);
                        if (filteredItems.Count < 30) await Task.Delay(15);
                    }
                    break;

                case "Borrowed":
                    SetupColumns("ID", "Item Name", "Borrower ID", "Status", "Date Borrowed");
                    var borrowedRecords = (await _borrowService.GetAllBorrowRecordsAsync())
                        .Where(b => b.Status == BorrowStatus.Active).ToList();

                    foreach (var r in borrowedRecords)
                    {
                        dgvInventory.Rows.Add(r.Id, r.ItemName, r.BorrowerId, r.Status, r.BorrowDate.ToShortDateString());
                        if (borrowedRecords.Count < 30) await Task.Delay(15);
                    }
                    break;

                case "Borrower List":
                    SetupColumns("Borrower ID", "Borrower Name", "Grade Level", "Purpose", "Items Held");
                    var groups = (await _borrowService.GetAllBorrowRecordsAsync())
                        .GroupBy(b => b.BorrowerId).ToList();

                    foreach (var group in groups)
                    {
                        dgvInventory.Rows.Add(group.Key, group.First().Borrower?.FullName ?? "Unknown",
                            group.First().GradeLevel, group.First().Purpose, group.Count(x => x.Status == BorrowStatus.Active));
                        if (groups.Count < 30) await Task.Delay(15);
                    }
                    break;

                default:
                    SetupColumns("ID", "Item Name", "Category", "Status", "Condition");
                    foreach (var i in filteredItems)
                    {
                        dgvInventory.Rows.Add(i.Id, i.Name, i.Category, i.Status, i.Condition);
                        if (filteredItems.Count < 30) await Task.Delay(15);
                    }
                    break;
            }
        }

        private async Task LoadRecentActivity()
        {
            if (flowRecentActivity == null) return;
            flowRecentActivity.Controls.Clear();
            foreach (var log in (await _borrowService.GetAllBorrowRecordsAsync()).OrderByDescending(b => b.BorrowDate).Take(10))
                AddActivityCard(log.Status == BorrowStatus.Active ? $"{log.BorrowerId} borrowed {log.ItemName}" : $"{log.ItemName} was returned by {log.BorrowerId}", log.BorrowDate, log.Status == BorrowStatus.Active ? Color.FromArgb(33, 150, 243) : Color.Teal);
        }

        private async Task LoadHomeContent()
        {
            if (flowRecentActivity == null) return;
            var damagedItems = (await _inventoryService.GetAllItemsAsync()).Where(i => i.Condition == "Damaged").ToList();

            flowRecentActivity.SuspendLayout();
            flowRecentActivity.Controls.Clear();

            if (!damagedItems.Any()) AddDashboardAlert("✓ All laboratory systems are operational.", Color.Teal);
            else AddDashboardAlert($"⚠ REPAIR NEEDED: {damagedItems.Count} items require attention. (Click for details)", Color.DarkRed);

            AddSectionHeader("RECENT ACTIVITY LOG");
            await LoadRecentActivity();
            flowRecentActivity.ResumeLayout(true);
        }

        private void AddDashboardAlert(string message, Color color)
        {
            var alert = new AlertTile(message, color);
            alert.AlertClicked += async (s, e) => { if (message.Contains("REPAIR")) { using (var popup = new RepairDetailsPopup((await _inventoryService.GetAllItemsAsync()).Where(i => i.Condition == "Damaged").ToList(), _inventoryService, async () => await LoadHomeContent())) popup.ShowDialog(); } };
            flowRecentActivity?.Controls.Add(alert);
            alert.BringToFront();
        }

        private void AddActivityCard(string message, DateTime time, Color statusColor) { var card = new Ventrix.App.Controls.ActivityCard(message, time, statusColor); card.Width = flowRecentActivity.Width - 30; flowRecentActivity?.Controls.Add(card); }

        private async Task LoadHistoryData()
        {
            if (dgvHistory == null) return;
            dgvHistory.Rows.Clear();
            dgvHistory.Columns.Clear();
            SetupColumnsHistory();
            foreach (var log in (await _borrowService.GetAllBorrowRecordsAsync()).Where(b => b.Status == BorrowStatus.Returned).OrderByDescending(b => b.ReturnDate)) dgvHistory.Rows.Add(log.Id, log.ItemName, log.BorrowerId, log.BorrowDate.ToShortDateString(), log.ReturnDate?.ToShortDateString());
        }

        private async Task UpdateDashboardCounts()
        {
            UpdateSystemHealthBadge();
            var items = (await _inventoryService.GetAllItemsAsync())?.ToList() ?? new List<InventoryItem>();
            var records = (await _borrowService.GetAllBorrowRecordsAsync())?.ToList() ?? new List<BorrowRecord>();

            cardTotal?.UpdateMetrics("TOTAL ITEMS", items.Count.ToString("N0"), Color.FromArgb(13, 71, 161));
            cardAvailable?.UpdateMetrics("AVAILABLE", items.Count(x => x.Status == ItemStatus.Available).ToString("N0"), Color.Teal);
            cardPending?.UpdateMetrics("BORROWED", items.Count(x => x.Status == ItemStatus.Borrowed).ToString("N0"), Color.FromArgb(192, 0, 0));
            cardBorrowers?.UpdateMetrics("RECORDS", records.Count.ToString("N0"), Color.Orange);

            if (lblUrgentHeader != null)
            {
                int damagedCount = items.Count(x => x.Condition == "Damaged");
                lblUrgentHeader.Text = damagedCount > 0 ? $"URGENT SYSTEM ALERTS ({damagedCount} ISSUES)" : "URGENT SYSTEM ALERTS";
                lblUrgentHeader.ForeColor = damagedCount > 0 ? Color.DarkRed : Color.Teal;
                lblUrgentHeader.Cursor = damagedCount > 0 ? Cursors.Hand : Cursors.Default;
            }

            if (this.Visible) { cardTotal?.Invalidate(); cardAvailable?.Invalidate(); cardPending?.Invalidate(); cardBorrowers?.Invalidate(); }
        }

        #endregion

        #region CRUD Actions

        private async Task btnCreate_Click(object sender, EventArgs e)
        {
            using (var popup = new InventoryPopup(_inventoryService))
            {
                popup.StartPosition = FormStartPosition.CenterParent;
                if (popup.ShowDialog() == DialogResult.OK)
                {
                    await LoadFromDatabase("All");
                    await UpdateDashboardCounts();
                    ToastNotification.Show(this, "New item added to inventory!", ToastType.Success);
                }
                RefreshLayout();
            }
        }

        private async Task btnEdit_Click(object sender, EventArgs e)
        {
            if (dgvInventory?.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to edit.", "Ventrix System");
                return;
            }

            using (var popup = new InventoryPopup(_inventoryService, Convert.ToInt32(dgvInventory.SelectedRows[0].Cells[0].Value)))
            {
                popup.StartPosition = FormStartPosition.CenterParent;
                if (popup.ShowDialog() == DialogResult.OK)
                {
                    await LoadFromDatabase("All");
                    await UpdateDashboardCounts();
                    ToastNotification.Show(this, "Item updated successfully!", ToastType.Success);
                }
                RefreshLayout();
            }
        }

        private async Task BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvInventory?.SelectedRows.Count > 0 && MessageBox.Show($"Delete item #{dgvInventory.SelectedRows[0].Cells[0].Value}?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await _inventoryService.DeleteItemAsync(Convert.ToInt32(dgvInventory.SelectedRows[0].Cells[0].Value));
                await SwitchView("Inventory", "All");
                ToastNotification.Show(this, "Item deleted from inventory.", ToastType.Info);
            }
        }

        private async Task ClearRecentActivity()
        {
            if (MessageBox.Show("Delete all activity logs?", "Critical Action", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                await _borrowService.ClearAllActivityAsync();
                flowRecentActivity?.Controls.Clear();
                await LoadHomeContent();
                await UpdateDashboardCounts();
                ToastNotification.Show(this, "Activity records cleared successfully.", ToastType.Info);
            }
        }

        #endregion

        #region Export Methods
        private void ExportToExcel()
        {
            if (dgvInventory == null || dgvInventory.Rows.Count == 0)
            {
                MessageBox.Show("There is no data to export.", "Ventrix System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx", FileName = "Ventrix_Inventory_Report.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (XLWorkbook workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("Inventory Report");

                            for (int i = 0; i < dgvInventory.Columns.Count; i++)
                            {
                                worksheet.Cell(1, i + 1).Value = dgvInventory.Columns[i].HeaderText;
                                worksheet.Cell(1, i + 1).Style.Font.Bold = true;
                                worksheet.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#0D47A1");
                                worksheet.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
                            }

                            for (int i = 0; i < dgvInventory.Rows.Count; i++)
                            {
                                for (int j = 0; j < dgvInventory.Columns.Count; j++)
                                {
                                    worksheet.Cell(i + 2, j + 1).Value = dgvInventory.Rows[i].Cells[j].Value?.ToString() ?? "";
                                }
                            }

                            worksheet.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                            ToastNotification.Show(this, "Excel report exported successfully!", ToastType.Success);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error exporting to Excel: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportToPDF()
        {
            if (dgvInventory == null || dgvInventory.Rows.Count == 0)
            {
                MessageBox.Show("There is no data to export.", "Ventrix System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "PDF Document|*.pdf", FileName = "Ventrix_Inventory_Report.pdf" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
                        PdfWriter.GetInstance(pdfDoc, new FileStream(sfd.FileName, FileMode.Create));
                        pdfDoc.Open();

                        iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD);
                        Paragraph title = new Paragraph("VENTRIX SYSTEM - INVENTORY REPORT\n\n", titleFont);
                        title.Alignment = Element.ALIGN_CENTER;
                        pdfDoc.Add(title);

                        PdfPTable pdfTable = new PdfPTable(dgvInventory.Columns.Count);
                        pdfTable.WidthPercentage = 100;

                        foreach (DataGridViewColumn column in dgvInventory.Columns)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)));
                            cell.BackgroundColor = new BaseColor(13, 71, 161);
                            cell.Padding = 5;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfTable.AddCell(cell);
                        }

                        foreach (DataGridViewRow row in dgvInventory.Rows)
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                PdfPCell pdfCell = new PdfPCell(new Phrase(cell.Value?.ToString() ?? "", FontFactory.GetFont("Arial", 9)));
                                pdfCell.Padding = 5;
                                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pdfTable.AddCell(pdfCell);
                            }
                        }

                        pdfDoc.Add(pdfTable);
                        pdfDoc.Close();

                        ToastNotification.Show(this, "PDF report exported successfully!", ToastType.Success);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Error exporting to PDF: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }
        #endregion

        #region UI Styling
        private void SetupColumnsHistory() { dgvHistory.Columns.Add("ID", "ID"); dgvHistory.Columns.Add("Item", "Item Name"); dgvHistory.Columns.Add("Borrower", "Borrower"); dgvHistory.Columns.Add("BDate", "Borrowed"); dgvHistory.Columns.Add("RDate", "Returned"); dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; }
        private void SetupColumns(params string[] names) { foreach (var n in names) dgvInventory.Columns.Add(n.Replace(" ", ""), n); dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; }

        private void AddSectionHeader(string title) { flowRecentActivity?.Controls.Add(new Label { Text = title, Font = new System.Drawing.Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true }); }

        private void InitializeMaterialSkin()
        {
            var manager = MaterialSkinManager.Instance; manager.AddFormToManage(this); manager.Theme = MaterialSkinManager.Themes.LIGHT; manager.ColorScheme = new ColorScheme(Color.FromArgb(13, 71, 161), Color.FromArgb(10, 50, 120), Color.FromArgb(33, 150, 243), Color.FromArgb(30, 136, 229), TextShade.WHITE);
        }

        private void ApplyModernBranding()
        {
            if (lblDashboardHeader != null)
            {
                lblDashboardHeader.Font = null;
                ThemeManager.ApplyCustomFont(lblDashboardHeader, ThemeManager.HeaderFont, ThemeManager.VentrixBlue);
                lblDashboardHeader.Text = "INVENTORY OVERVIEW";
            }

            if (lblUrgentHeader != null)
            {
                lblUrgentHeader.Font = null;
                ThemeManager.ApplyCustomFont(lblUrgentHeader, ThemeManager.SubHeaderFont);
            }

            // 1. Style the Sidebar Navigation Buttons
            var sideBtns = new[] { btnHome, btnHistoryNav, btnNavAllItems, btnNavAvailable, btnNavBorrowed, btnNavBorrowers };
            var sideTxts = new[] { "HOME PAGE", "HISTORY", "ALL ITEMS", "AVAILABLE", "BORROWED", "BORROWERS" };

            for (int i = 0; i < sideBtns.Length; i++)
            {
                if (sideBtns[i] != null)
                {
                    sideBtns[i].Text = sideTxts[i];
                    sideBtns[i].Font = ThemeManager.ButtonFont;
                    sideBtns[i].FillColor = Color.Transparent;
                    sideBtns[i].HoverState.FillColor = Color.FromArgb(40, 255, 255, 255); // White glow
                    sideBtns[i].HoverState.ForeColor = Color.White;
                    sideBtns[i].TextAlign = HorizontalAlignment.Left;
                    sideBtns[i].TextOffset = new Point(10, 0);
                    sideBtns[i].BorderRadius = 8;
                    sideBtns[i].Animated = true;
                }
            }

            // 2. Style the Main Action Buttons
            var actionBtns = new[] { btnCreate, btnEdit, btnDelete, btnClearActivity };
            var actionTxts = new[] { "ADD ITEM", "EDIT RECORD", "DELETE ITEM", "CLEAR ALL" };
            var actionClrs = new[] { Color.Teal, ThemeManager.VentrixLightBlue, Color.IndianRed, Color.IndianRed };

            for (int i = 0; i < actionBtns.Length; i++)
            {
                if (actionBtns[i] != null)
                {
                    actionBtns[i].Text = actionTxts[i];
                    actionBtns[i].Font = ThemeManager.ButtonFont;
                    actionBtns[i].FillColor = Color.Transparent;
                    actionBtns[i].HoverState.FillColor = actionClrs[i];
                    actionBtns[i].HoverState.ForeColor = Color.White;
                    actionBtns[i].TextAlign = HorizontalAlignment.Left;
                    actionBtns[i].TextOffset = new Point(10, 0);
                    actionBtns[i].BorderRadius = 8;
                    actionBtns[i].Animated = true;
                }
            }

            if (txtSearch != null)
            {
                txtSearch.BorderRadius = txtSearch.Height / 2;
                txtSearch.Font = new System.Drawing.Font("Segoe UI", 10.5F, FontStyle.Regular);
                txtSearch.ForeColor = Color.FromArgb(64, 64, 64);
                txtSearch.TextOffset = new Point(5, 0);

                txtSearch.FillColor = Color.FromArgb(245, 248, 252);
                txtSearch.BorderColor = Color.Transparent;
                txtSearch.HoverState.FillColor = Color.FromArgb(250, 252, 255);
                txtSearch.HoverState.BorderColor = Color.FromArgb(200, 210, 230);
                txtSearch.FocusedState.FillColor = Color.White;
                txtSearch.FocusedState.BorderColor = ThemeManager.VentrixBlue;
                txtSearch.PlaceholderForeColor = Color.FromArgb(160, 160, 160);
                txtSearch.Animated = true;
                txtSearch.IconLeftOffset = new Point(10, 0);
                txtSearch.IconRightOffset = new Point(10, 0);
            }

            if (pnlTopBar != null)
            {
                pnlTopBar.FillColor = Color.White;
                pnlTopBar.ShadowDecoration.Enabled = false;
                pnlTopBar.CustomBorderColor = Color.FromArgb(230, 235, 240);
                pnlTopBar.CustomBorderThickness = new Padding(0, 0, 0, 1);
            }

            if (btnHamburger != null)
            {
                btnHamburger.HoverState.ImageSize = new Size(btnHamburger.ImageSize.Width - 2, btnHamburger.ImageSize.Height - 2);
            }
        }

        private void StyleDataGrids()
        {
            var grids = new[] { dgvInventory, dgvHistory };

            foreach (var grid in grids)
            {
                if (grid == null) continue;

                grid.BackgroundColor = Color.White;
                grid.BorderStyle = BorderStyle.None;
                grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                grid.GridColor = Color.FromArgb(230, 235, 240);
                grid.RowHeadersVisible = false;
                grid.AllowUserToAddRows = false;
                grid.AllowUserToDeleteRows = false;
                grid.AllowUserToResizeRows = false;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.MultiSelect = false;
                grid.ReadOnly = true;

                grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                grid.ColumnHeadersHeight = 50;
                grid.ThemeStyle.HeaderStyle.BackColor = ThemeManager.VentrixBlue;
                grid.ThemeStyle.HeaderStyle.ForeColor = Color.White;

                grid.ThemeStyle.HeaderStyle.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, FontStyle.Regular);
                grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                grid.RowTemplate.Height = 45;
                grid.ThemeStyle.RowsStyle.BackColor = Color.White;
                grid.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(64, 64, 64);

                grid.ThemeStyle.RowsStyle.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Regular);
                grid.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(240, 245, 255);
                grid.ThemeStyle.RowsStyle.SelectionForeColor = ThemeManager.VentrixBlue;

                grid.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                grid.DefaultCellStyle.Padding = new Padding(5, 0, 5, 0);

                grid.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(250, 252, 255);
                grid.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.FromArgb(64, 64, 64);
                grid.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.FromArgb(240, 245, 255);
                grid.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = ThemeManager.VentrixBlue;

                grid.CellPainting += (s, e) =>
                {
                    if (e.RowIndex >= 0 && (e.State & DataGridViewElementStates.Selected) == DataGridViewElementStates.Selected)
                    {
                        e.Paint(e.ClipBounds, DataGridViewPaintParts.All & ~DataGridViewPaintParts.Focus);
                        e.Handled = true;
                    }
                };

                grid.CellMouseEnter += (s, e) =>
                {
                    if (e.RowIndex >= 0)
                    {
                        grid.Cursor = Cursors.Hand;
                        grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.FromArgb(245, 248, 255);
                    }
                };

                grid.CellMouseLeave += (s, e) =>
                {
                    if (e.RowIndex >= 0)
                    {
                        grid.Cursor = Cursors.Default;
                        if (e.RowIndex % 2 == 0)
                            grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.White;
                        else
                            grid.Rows[e.RowIndex].DefaultCellStyle.BackColor = grid.ThemeStyle.AlternatingRowsStyle.BackColor;
                    }
                };
            }
        }

        private void DgvInventory_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.Value != null)
            {
                DataGridView dgv = sender as DataGridView;
                string colName = dgv.Columns[e.ColumnIndex].Name;
                string value = e.Value.ToString();

                if (!string.IsNullOrEmpty(txtSearch.Text) && value.ToLower().Contains(txtSearch.Text.ToLower()))
                {
                    e.CellStyle.BackColor = Color.FromArgb(255, 255, 200);
                }

                if (colName == "Status" || colName == "Condition")
                {
                    e.CellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, FontStyle.Bold);
                    if (value == "Available" || value == "Good") e.CellStyle.ForeColor = Color.MediumSeaGreen;
                    else if (value == "Borrowed") e.CellStyle.ForeColor = Color.DarkOrange;
                    else if (value == "Damaged" || value == "Missing") e.CellStyle.ForeColor = Color.IndianRed;
                }
            }
        }

        protected override void OnActivated(EventArgs e) { base.OnActivated(e); ApplyModernBranding(); }
        #endregion
    }
}