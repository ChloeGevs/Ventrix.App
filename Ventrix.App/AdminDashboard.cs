using ClosedXML.Excel;
using Guna.UI2.WinForms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventrix.App.Controls;
using Ventrix.App.Popups;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using Ventrix.Domain.Enums;

using DrawColor = System.Drawing.Color;
using DrawPoint = System.Drawing.Point;
using DrawSize = System.Drawing.Size;
using DrawRect = System.Drawing.Rectangle;
using DrawFont = System.Drawing.Font;

namespace Ventrix.App
{
    public partial class AdminDashboard : MaterialForm
    {
        private readonly InventoryService _inventoryService;
        private readonly BorrowService _borrowService;
        private readonly UserService _userService;

        private bool isSigningOut = false;
        private bool isSidebarExpanded = true;
        private const int sidebarMaxWidth = 240;
        private const int sidebarMinWidth = 70;
        private int historyCurrentPage = 1;
        private int historyTotalPages = 1;
        private const int historyPageSize = 100;
        private string historySortColumn = "BTime"; 
        private bool historySortDescending = true;  

        private DateTimePicker dtpStartDate;
        private DateTimePicker dtpEndDate;
        private Button btnApplyFilters;
        private Button btnPrevPage;
        private Button btnNextPage;
        private Label lblPageInfo;

        public AdminDashboard(InventoryService inventoryService, BorrowService borrowService, UserService userService)
        {
            _inventoryService = inventoryService;
            _borrowService = borrowService;
            _userService = userService;

            InitializeComponent();
            SetupHistoryAdvancedControls();
            ThemeManager.Initialize(this);
            InitializeMaterialSkin();

            isSidebarExpanded = true;

            ConfigureRuntimeUI();
            ApplyModernBranding();
            StyleDataGrids();

            Shown += async (s, e) => {
                RefreshLayout();
                await SwitchView("Home");
                btnHome?.Focus();
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

        #region KEYBOARD SHORTCUTS
        private Control GetFocusedControl()
        {
            Control focused = this.ActiveControl;
            
            while (focused is ContainerControl container)
            {
                focused = container.ActiveControl;
            }
            return focused;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Tab || keyData == (Keys.Shift | Keys.Tab))
            {
                var navBtns = new[] { btnHome, btnHistoryNav, btnNavAllItems, btnNavAvailable, btnNavBorrowed, btnNavBorrowers };

                Control focusedCtrl = GetFocusedControl();

                int currentIndex = Array.IndexOf(navBtns, focusedCtrl);

                if (currentIndex != -1)
                {
                    int nextIndex;
                    if (keyData == Keys.Tab)
                    {
                        nextIndex = (currentIndex + 1) % navBtns.Length;
                    }
                    else
                    {
                        nextIndex = (currentIndex - 1 + navBtns.Length) % navBtns.Length;
                    }

                    var nextBtn = navBtns[nextIndex];
                    nextBtn.Focus();

                    if (nextBtn == btnHome) _ = SwitchView("Home");
                    else if (nextBtn == btnHistoryNav) _ = SwitchView("History");
                    else if (nextBtn == btnNavAllItems) _ = SwitchView("Inventory", "All");
                    else if (nextBtn == btnNavAvailable) _ = SwitchView("Inventory", "Available");
                    else if (nextBtn == btnNavBorrowed) _ = SwitchView("Inventory", "Borrowed");
                    else if (nextBtn == btnNavBorrowers) _ = SwitchView("Inventory", "Borrowers");

                    return true; 
                }
            }

            if (keyData == (Keys.Control | Keys.F))
            {
                if (txtSearch != null && txtSearch.Visible)
                {
                    txtSearch.Focus();
                    txtSearch.SelectAll();
                    return true;
                }
            }

            if (keyData == Keys.Enter)
            {
                if (txtSearch != null && txtSearch.Focused)
                {
                    _ = LoadFromDatabase("All");
                    if (dgvInventory != null) dgvInventory.Focus();
                    return true;
                }
            }

            if (keyData == Keys.Escape)
            {
                if (txtSearch != null && txtSearch.Focused)
                {
                    txtSearch.Clear();
                    if (dgvInventory != null) dgvInventory.Focus();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        private void ConfigureRuntimeUI()
        {
            FormClosed += (s, e) => { if (!isSigningOut) System.Windows.Forms.Application.Exit(); };

            if (flowRecentActivity != null)
            {
                flowRecentActivity.Resize += (s, e) => {
                    flowRecentActivity.SuspendLayout();

                    int targetWidth = flowRecentActivity.ClientSize.Width - flowRecentActivity.Padding.Left - flowRecentActivity.Padding.Right - 10;

                    if (targetWidth > 0)
                    {
                        foreach (Control ctrl in flowRecentActivity.Controls)
                        {
                            if (ctrl is Ventrix.App.Controls.ActivityCard || ctrl is Ventrix.App.Controls.AlertTile)
                            {
                                ctrl.Width = targetWidth;
                            }
                        }
                    }
                    flowRecentActivity.ResumeLayout(true);
                };
            }

            var controlsToBuffer = new Control[] { pnlMainContent, pnlGridContainer, pnlHistory, pnlHomeSummary, flowRecentActivity, pnlSidebar, dgvInventory, dgvHistory };
            foreach (var ctrl in controlsToBuffer)
            {
                if (ctrl != null)
                    typeof(Control).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, ctrl, new object[] { true });
            }

            if (btnCreate != null) btnCreate.Click += async (s, e) => await btnCreate_Click(s, e);
            if (btnEdit != null) btnEdit.Click += async (s, e) => await btnEdit_Click(s, e);
            if (btnDelete != null) btnDelete.Click += async (s, e) => await btnDelete_Click(s, e);

            if (btnExportExcel != null) btnExportExcel.Click += async (s, e) => {
                if (pnlHistory != null && pnlHistory.Visible) await ExportHistoryToExcelAsync();
                else ExportToExcel(); 
            };
            if (btnExportPDF != null) btnExportPDF.Click += async (s, e) => {
                if (pnlHistory != null && pnlHistory.Visible) await ExportHistoryToPDFAsync();
                else ExportToPDF(); 
            };

            if (btnHome != null) btnHome.Click += async (s, e) => await SwitchView("Home");
            if (btnHistoryNav != null) btnHistoryNav.Click += async (s, e) => await SwitchView("History");

            if (btnNavAllItems != null) btnNavAllItems.Click += async (s, e) => await SwitchView("Inventory", "All");
            if (btnNavAvailable != null) btnNavAvailable.Click += async (s, e) => await SwitchView("Inventory", "Available");
            if (btnNavBorrowed != null) btnNavBorrowed.Click += async (s, e) => await SwitchView("Inventory", "Borrowed");
            if (btnNavBorrowers != null) btnNavBorrowers.Click += async (s, e) => await SwitchView("Inventory", "Borrowers");

            var navBtns = new[] { btnHome, btnHistoryNav, btnNavAllItems, btnNavAvailable, btnNavBorrowed, btnNavBorrowers };
            foreach (var btn in navBtns)
            {
                if (btn != null)
                {
                    btn.GotFocus += (s, e) => btn.FillColor = DrawColor.FromArgb(40, 255, 255, 255);
                    btn.LostFocus += (s, e) => btn.FillColor = DrawColor.Transparent;
                }
            }

            if (btnClearActivity != null) btnClearActivity.Click += async (s, e) => await ClearRecentActivity();
            if (cmbAccountActions != null) cmbAccountActions.SelectedIndexChanged += CmbAccountActions_SelectedIndexChanged;

            if (cardTotal != null) cardTotal.CardClicked += async (s, e) => await SwitchView("Inventory", "All");
            if (cardAvailable != null) cardAvailable.CardClicked += async (s, e) => await SwitchView("Inventory", "Available");
            if (cardPending != null) cardPending.CardClicked += async (s, e) => await SwitchView("Inventory", "Borrowed");
            if (cardBorrowers != null) cardBorrowers.CardClicked += async (s, e) => await SwitchView("Inventory", "Borrowers");

            if (badgeHealth != null)
            {
                badgeHealth.Cursor = Cursors.Hand;
                badgeHealth.Click += async (s, e) => await LblUrgentHeader_Click(s, e); 
            }

            if (sidebarTimer != null && btnHamburger != null)
            {
                sidebarTimer.Interval = 10;
                btnHamburger.Click += (s, e) => {

                    if (isSidebarExpanded)
                    {
                        UpdateSidebarInternalUI(false);
                    }
                    sidebarTimer.Start();
                };
                sidebarTimer.Tick += SidebarTimer_Tick;
            }

            if (txtSearch != null)
            {
                txtSearch.IconRightCursor = Cursors.Hand;
                txtSearch.IconRightSize = new DrawSize(0, 0);

                txtSearch.TextChanged += async (s, e) => {
                    txtSearch.IconRightSize = string.IsNullOrEmpty(txtSearch.Text) ? new DrawSize(0, 0) : new DrawSize(15, 15);
                    if (pnlGridContainer != null && pnlGridContainer.Visible) await LoadFromDatabase("All");
                    else if (pnlHistory != null && pnlHistory.Visible) await LoadHistoryData();
                };

                txtSearch.IconRightClick += (s, e) => {
                    if (!string.IsNullOrEmpty(txtSearch.Text)) { txtSearch.Clear(); txtSearch.Focus(); }
                };
            }

            if (dgvInventory != null)
            {
                dgvInventory.CellDoubleClick += async (s, e) => await DgvInventory_CellDoubleClick(s, e);
                dgvInventory.CellFormatting += DgvInventory_CellFormatting;
            }

            if (dgvHistory != null)
            {
                dgvHistory.CellFormatting += DgvHistory_CellFormatting;

                dgvHistory.ColumnHeaderMouseClick += async (s, e) => {
                    string clickedCol = dgvHistory.Columns[e.ColumnIndex].Name;

                    if (historySortColumn == clickedCol)
                    {
                        historySortDescending = !historySortDescending; 
                    }
                    else
                    {
                        historySortColumn = clickedCol;
                        historySortDescending = false; 
                    }

                    historyCurrentPage = 1; 
                    await LoadHistoryData();
                };
            }
            if (dgvInventory != null)
            {
                ContextMenuStrip strikeMenu = new ContextMenuStrip();

                var addStrikeBtn = new ToolStripMenuItem("⚠️ Add 1 Strike (Penalty)");
                addStrikeBtn.Click += async (s, e) => {
                    if (dgvInventory.SelectedRows.Count > 0 && dgvInventory.Columns.Contains("BorrowerID"))
                    {
                        string userId = dgvInventory.SelectedRows[0].Cells["BorrowerID"].Value.ToString();
                        await _userService.AddStrikeAsync(userId);
                        ToastNotification.Show(this, "Strike added to student account.", ToastType.Warning);
                        await LoadFromDatabase("Borrowers"); 
                    }
                };

                var clearStrikeBtn = new ToolStripMenuItem("✅ Clear All Strikes (Forgive)");
                clearStrikeBtn.Click += async (s, e) => {
                    if (dgvInventory.SelectedRows.Count > 0 && dgvInventory.Columns.Contains("BorrowerID"))
                    {
                        string userId = dgvInventory.SelectedRows[0].Cells["BorrowerID"].Value.ToString();
                        await _userService.ClearStrikesAsync(userId);
                        ToastNotification.Show(this, "Student account strikes have been cleared.", ToastType.Success);
                        await LoadFromDatabase("Borrowers"); 
                    }
                };

                strikeMenu.Items.Add(addStrikeBtn);
                strikeMenu.Items.Add(new ToolStripSeparator());
                strikeMenu.Items.Add(clearStrikeBtn);

                dgvInventory.ContextMenuStrip = strikeMenu;
                strikeMenu.Opening += (s, e) => {
                    if (!dgvInventory.Columns.Contains("BorrowerID")) e.Cancel = true;
                };

                dgvInventory.CellDoubleClick += async (s, e) => await DgvInventory_CellDoubleClick(s, e);
                dgvInventory.CellFormatting += DgvInventory_CellFormatting;
            }

            this.Resize += (s, e) => { if (this.WindowState != FormWindowState.Minimized) RefreshLayout(); };
        }

        #region FLUID LAYOUT & ANIMATION (Strict Math, No Overlaps)
        private void RefreshLayout()
        {
            if (pnlMainContent == null || pnlSidebar == null || pnlTopBar == null) return;

            pnlTopBar.Dock = DockStyle.None;
            pnlSidebar.Dock = DockStyle.None;
            pnlMainContent.Dock = DockStyle.None;

            int topMargin = 64;
            int topBarHeight = 80;
            int contentY = topMargin + topBarHeight;
            int remainingHeight = this.ClientSize.Height - contentY;

            pnlTopBar.SetBounds(0, topMargin, this.ClientSize.Width, topBarHeight);
            pnlSidebar.SetBounds(0, contentY, isSidebarExpanded ? sidebarMaxWidth : sidebarMinWidth, remainingHeight);
            pnlMainContent.SetBounds(pnlSidebar.Width, contentY, this.ClientSize.Width - pnlSidebar.Width, remainingHeight);

            pnlTopBar.BringToFront();
            pnlSidebar.BringToFront();
            pnlMainContent.BringToFront();

            if (btnHamburger != null)
            {
                btnHamburger.Location = new DrawPoint(25, (pnlTopBar.Height - btnHamburger.Height) / 2);
            }

            if (lblDashboardHeader != null && btnHamburger != null)
            {
                lblDashboardHeader.Location = new DrawPoint(btnHamburger.Right + 20, (pnlTopBar.Height - lblDashboardHeader.Height) / 2);

            }
            if (txtSearch != null)
            {
                txtSearch.Location = new DrawPoint(pnlTopBar.Width - txtSearch.Width - 30, (pnlTopBar.Height - txtSearch.Height) / 2);
            }

            if (badgeHealth != null && txtSearch != null)
            {
                badgeHealth.Location = new DrawPoint(txtSearch.Left - badgeHealth.Width - 20, (pnlTopBar.Height - badgeHealth.Height) / 2);
            }

            int margin = 30;
            DrawRect safeArea = new DrawRect(margin, margin, pnlMainContent.Width - (margin * 2), pnlMainContent.Height - (margin * 2));

            if (pnlHomeSummary != null) pnlHomeSummary.Bounds = safeArea;
            if (pnlGridContainer != null) pnlGridContainer.Bounds = safeArea;
            if (pnlHistory != null) pnlHistory.Bounds = safeArea;

            if (pnlHomeSummary != null && pnlHomeSummary.Visible) ArrangeHomeView();
            if (pnlGridContainer != null && pnlGridContainer.Visible) ArrangeInventoryView();
            if (pnlHistory != null && pnlHistory.Visible) ArrangeHistoryView();

            UpdateSidebarInternalUI(isSidebarExpanded);
        }

        private void ArrangeHomeView()
        {
            if (pnlHomeSummary == null) return;

            int spacing = 20;
            int topMargin = 20;

            if (btnClearActivity != null)
            {
                btnClearActivity.Parent = pnlHomeSummary;

                btnClearActivity.Anchor = AnchorStyles.None;
                btnClearActivity.Location = new DrawPoint(pnlHomeSummary.Width - btnClearActivity.Width - spacing, topMargin);
                btnClearActivity.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnClearActivity.BringToFront();
            }

            int cardY = 70;
            int cardWidth = (pnlHomeSummary.Width - (spacing * 5)) / 4;

            if (cardTotal != null) { cardTotal.Parent = pnlHomeSummary; cardTotal.SetBounds(spacing, cardY, cardWidth, 110); }
            if (cardAvailable != null) { cardAvailable.Parent = pnlHomeSummary; cardAvailable.SetBounds(cardTotal.Right + spacing, cardY, cardWidth, 110); }
            if (cardPending != null) { cardPending.Parent = pnlHomeSummary; cardPending.SetBounds(cardAvailable.Right + spacing, cardY, cardWidth, 110); }
            if (cardBorrowers != null) { cardBorrowers.Parent = pnlHomeSummary; cardBorrowers.SetBounds(cardPending.Right + spacing, cardY, cardWidth, 110); }

            if (flowRecentActivity != null)
            {
                flowRecentActivity.Parent = pnlHomeSummary;

                int activityY = cardTotal.Bottom + 30; 

                flowRecentActivity.Anchor = AnchorStyles.None;
                flowRecentActivity.Location = new DrawPoint(spacing, activityY);
                flowRecentActivity.Size = new DrawSize(pnlHomeSummary.Width - (spacing * 2), pnlHomeSummary.Height - activityY - spacing);

                flowRecentActivity.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                flowRecentActivity.BringToFront();
            }
        }

        private void ArrangeInventoryView()
        {
            if (pnlGridContainer == null) return;

            int topRowY = 20;
            int margin = 25;

            if (btnExportExcel != null)
            {
                btnExportExcel.Parent = pnlGridContainer;
                btnExportExcel.Anchor = AnchorStyles.None; 
                btnExportExcel.Location = new DrawPoint(margin, topRowY);
                btnExportExcel.Anchor = AnchorStyles.Top | AnchorStyles.Left; 
                btnExportExcel.BringToFront();
            }
            if (btnExportPDF != null)
            {
                btnExportPDF.Parent = pnlGridContainer;
                btnExportPDF.Anchor = AnchorStyles.None;
                btnExportPDF.Location = new DrawPoint(btnExportExcel.Right + 15, topRowY);
                btnExportPDF.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                btnExportPDF.BringToFront();
            }

            if (btnDelete != null)
            {
                btnDelete.Parent = pnlGridContainer;
                btnDelete.Anchor = AnchorStyles.None;
               
                btnDelete.Location = new DrawPoint(pnlGridContainer.Width - btnDelete.Width - margin, topRowY);
                btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnDelete.BringToFront();
            }
            if (btnEdit != null)
            {
                btnEdit.Parent = pnlGridContainer;
                btnEdit.Anchor = AnchorStyles.None;
                
                btnEdit.Location = new DrawPoint(btnDelete.Left - btnEdit.Width - 15, topRowY);
                btnEdit.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnEdit.BringToFront();
            }
            if (btnCreate != null)
            {
                btnCreate.Parent = pnlGridContainer;
                btnCreate.Anchor = AnchorStyles.None;
                
                btnCreate.Location = new DrawPoint(btnEdit.Left - btnCreate.Width - 15, topRowY);
                btnCreate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnCreate.BringToFront();
            }

            if (dgvInventory != null)
            {
                int gridY = topRowY + 50;
                dgvInventory.Parent = pnlGridContainer;

                dgvInventory.Anchor = AnchorStyles.None;
                dgvInventory.Location = new DrawPoint(margin, gridY);
                dgvInventory.Size = new DrawSize(pnlGridContainer.Width - (margin * 2), pnlGridContainer.Height - gridY - margin);

                dgvInventory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                dgvInventory.BringToFront();
            }
        }

        private void ArrangeHistoryView()
        {
            if (pnlHistory == null) return;

            int topRowY = 20;
            int margin = 25;

            if (btnExportExcel != null)
            {
                btnExportExcel.Parent = pnlHistory;
                btnExportExcel.Anchor = AnchorStyles.None;
                btnExportExcel.Location = new DrawPoint(margin, topRowY);
                btnExportExcel.Anchor = AnchorStyles.Top | AnchorStyles.Left; 
                btnExportExcel.BringToFront();
            }
            if (btnExportPDF != null)
            {
                btnExportPDF.Parent = pnlHistory;
                btnExportPDF.Anchor = AnchorStyles.None;
                btnExportPDF.Location = new DrawPoint(btnExportExcel.Right + 15, topRowY);
                btnExportPDF.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                btnExportPDF.BringToFront();
            }

            if (dgvHistory != null)
            {
                int gridY = topRowY + 50;
                dgvHistory.Parent = pnlHistory;

                dgvHistory.Anchor = AnchorStyles.None;
                dgvHistory.Location = new DrawPoint(margin, gridY);
                dgvHistory.Size = new DrawSize(pnlHistory.Width - (margin * 2), pnlHistory.Height - gridY - margin);

                dgvHistory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                dgvHistory.BringToFront();
            }
        }

        private void UpdateSidebarInternalUI(bool showDetails)
        {
            var navBtns = new[] { btnHome, btnHistoryNav, btnNavAllItems, btnNavAvailable, btnNavBorrowed, btnNavBorrowers };
            string[] navTexts = { "HOME PAGE", "HISTORY", "ALL ITEMS", "AVAILABLE", "BORROWED", "BORROWERS" };

            if (picUser != null) picUser.SetBounds((showDetails) ? 15 : 12, 25, (showDetails) ? 45 : 40, (showDetails) ? 45 : 40);
            if (lblOwnerRole != null) lblOwnerRole.Visible = showDetails;
            if (cmbAccountActions != null) cmbAccountActions.Visible = showDetails;

            int startY = 130;
            int endY = pnlSidebar.Height - 30;
            int availableHeight = endY - startY;

            int buttonHeight = 45;
            int totalButtonHeight = navBtns.Length * buttonHeight;

            int remainingSpace = availableHeight - totalButtonHeight;
            int gap = remainingSpace / (navBtns.Length - 1);

            if (gap < 5) gap = 5;
            if (gap > 35) gap = 35;

            int currentY = startY;

            for (int i = 0; i < navBtns.Length; i++)
            {
                if (navBtns[i] != null)
                {
                    navBtns[i].Width = pnlSidebar.Width - 20;
                    navBtns[i].Location = new DrawPoint(10, currentY);

                    currentY += buttonHeight + gap;

                    navBtns[i].ImageAlign = HorizontalAlignment.Left;

                    if (showDetails)
                    {
                        navBtns[i].Text = "   " + navTexts[i];
                    }
                    else
                    {
                        navBtns[i].Text = "";
                    }
                }
            }
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            this.SuspendLayout();

            int targetWidth = isSidebarExpanded ? sidebarMinWidth : sidebarMaxWidth;
            int remainingDistance = Math.Abs(targetWidth - pnlSidebar.Width);

            int step = remainingDistance / 2;
            if (step < 40) step = 40;

            if (isSidebarExpanded) 
            {
                pnlSidebar.Width -= step;
                if (pnlSidebar.Width <= sidebarMinWidth)
                {
                    pnlSidebar.Width = sidebarMinWidth; 
                    isSidebarExpanded = false;
                    sidebarTimer.Stop();
                }
            }
            else 
            {
                pnlSidebar.Width += step;
                if (pnlSidebar.Width >= sidebarMaxWidth)
                {
                    pnlSidebar.Width = sidebarMaxWidth;
                    isSidebarExpanded = true;
                    sidebarTimer.Stop();

                    UpdateSidebarInternalUI(true);
                }
            }

            DrawRect safeClientArea = this.DisplayRectangle;
            pnlMainContent.Left = pnlSidebar.Right;
            pnlMainContent.Width = safeClientArea.Right - pnlSidebar.Right;

            int margin = 30;
            DrawRect innerSafeArea = new DrawRect(margin, margin, pnlMainContent.Width - (margin * 2), pnlMainContent.Height - (margin * 2));

            if (pnlHomeSummary != null && pnlHomeSummary.Visible)
            {
                pnlHomeSummary.Bounds = innerSafeArea;
               
                ArrangeHomeView();
            }

            if (pnlGridContainer != null && pnlGridContainer.Visible) pnlGridContainer.Bounds = innerSafeArea;
            if (pnlHistory != null && pnlHistory.Visible) pnlHistory.Bounds = innerSafeArea;

            this.ResumeLayout(true);
        }           
        #endregion

        #region Navigation & Data Loading
        private async Task<List<InventoryItem>> GetDamagedItemsAsync()
        {
            return (await _inventoryService.GetAllItemsAsync())
                .Where(i => i.Condition.ToString() == "Damaged" ||
                            i.Condition.ToString() == "Broken" ||
                            i.Condition.ToString() == "NeedsRepair")
                .ToList();
        }
        private void HighlightActiveButton(Guna2Button activeBtn)
        {
            var navBtns = new[] { btnHome, btnHistoryNav, btnNavAllItems, btnNavAvailable, btnNavBorrowed, btnNavBorrowers };
            foreach (var btn in navBtns)
            {
                if (btn != null)
                {
                    btn.FillColor = DrawColor.Transparent;
                    btn.CustomBorderThickness = new Padding(0);
                    btn.Font = new DrawFont("Segoe UI Semibold", 10.5F, FontStyle.Bold);
                    btn.ForeColor = DrawColor.White;
                }
            }

            if (activeBtn != null)
            {
                activeBtn.FillColor = DrawColor.FromArgb(40, 255, 255, 255);
                activeBtn.CustomBorderColor = DrawColor.Orange;
                activeBtn.CustomBorderThickness = new Padding(4, 0, 0, 0);
                activeBtn.Font = new DrawFont("Segoe UI", 11F, FontStyle.Bold);
                activeBtn.ForeColor = DrawColor.White;
            }
        }

        private async Task SwitchView(string viewName, string filter = "All")
        {
            if (pnlHomeSummary != null) pnlHomeSummary.Visible = false;
            if (pnlGridContainer != null) pnlGridContainer.Visible = false;
            if (pnlHistory != null) pnlHistory.Visible = false;

            if (btnCreate != null) btnCreate.Visible = false;
            if (btnEdit != null) btnEdit.Visible = false;
            if (btnDelete != null) btnDelete.Visible = false;
            if (btnExportExcel != null) btnExportExcel.Visible = false;
            if (btnExportPDF != null) btnExportPDF.Visible = false;

            if (viewName == "Home")
            {
                HighlightActiveButton(btnHome);
                if (pnlHomeSummary != null) { pnlHomeSummary.Visible = true; pnlHomeSummary.BringToFront(); }
                await LoadHomeContent();

                if (lblDashboardHeader != null) lblDashboardHeader.Text = $"{GetGreeting()}, ADMIN";
            }
            else if (viewName == "Inventory")
            {
                if (lblDashboardHeader != null) lblDashboardHeader.Text = $"INVENTORY: {filter.ToUpper()}";

                if (filter == "All") HighlightActiveButton(btnNavAllItems);
                else if (filter == "Available") HighlightActiveButton(btnNavAvailable);
                else if (filter == "Borrowed") HighlightActiveButton(btnNavBorrowed);
                else if (filter == "Borrowers") HighlightActiveButton(btnNavBorrowers);

                if (pnlGridContainer != null) { pnlGridContainer.Visible = true; pnlGridContainer.BringToFront(); }

                bool showCrud = (filter == "All" || filter == "Available");
                if (btnCreate != null) { btnCreate.Visible = showCrud; btnCreate.BringToFront(); }
                if (btnEdit != null) { btnEdit.Visible = showCrud; btnEdit.BringToFront(); }
                if (btnDelete != null) { btnDelete.Visible = showCrud; btnDelete.BringToFront(); }

                // Show Exports for Inventory
                if (btnExportExcel != null) { btnExportExcel.Visible = true; btnExportExcel.BringToFront(); }
                if (btnExportPDF != null) { btnExportPDF.Visible = true; btnExportPDF.BringToFront(); }

                await LoadFromDatabase(filter);
            }
            else if (viewName == "History")
            {
                if (lblDashboardHeader != null) lblDashboardHeader.Text = "TRANSACTION AUDIT HISTORY";
                HighlightActiveButton(btnHistoryNav);

                if (pnlHistory != null) { pnlHistory.Visible = true; pnlHistory.BringToFront(); }

                // Show Exports for History
                if (btnExportExcel != null) { btnExportExcel.Visible = true; btnExportExcel.BringToFront(); }
                if (btnExportPDF != null) { btnExportPDF.Visible = true; btnExportPDF.BringToFront(); }

                await LoadHistoryData();
            }

            RefreshLayout();
            await UpdateDashboardCounts();
        }

        private void SetupHistoryAdvancedControls()
        {
            dtpStartDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 110, Value = DateTime.Today.AddMonths(-1) };
            dtpEndDate = new DateTimePicker { Format = DateTimePickerFormat.Short, Width = 110, Value = DateTime.Today };

            btnApplyFilters = new Button { Text = "Filter Dates", BackColor = DrawColor.FromArgb(13, 71, 161), ForeColor = DrawColor.White, FlatStyle = FlatStyle.Flat, Height = 25, Width = 80 };
            btnApplyFilters.Click += async (s, e) => { historyCurrentPage = 1; await LoadHistoryData(); };

            btnPrevPage = new Button { Text = "< Prev", BackColor = DrawColor.FromArgb(240, 240, 240), FlatStyle = FlatStyle.Flat, Width = 70, Height = 30 };
            btnPrevPage.Click += async (s, e) => { if (historyCurrentPage > 1) { historyCurrentPage--; await LoadHistoryData(); } };

            btnNextPage = new Button { Text = "Next >", BackColor = DrawColor.FromArgb(240, 240, 240), FlatStyle = FlatStyle.Flat, Width = 70, Height = 30 };
            btnNextPage.Click += async (s, e) => { if (historyCurrentPage < historyTotalPages) { historyCurrentPage++; await LoadHistoryData(); } };

            lblPageInfo = new Label { Text = "Page 1 of 1", AutoSize = true, Font = new DrawFont("Segoe UI", 10, FontStyle.Bold) };
        }

        private async Task LoadFromDatabase(string filter)
        {
            if (dgvInventory == null) return;
            dgvInventory.Rows.Clear();
            dgvInventory.Columns.Clear();

            var items = await _inventoryService.GetAllItemsAsync();
            string search = txtSearch?.Text?.ToLower() ?? "";

            if (!string.IsNullOrEmpty(search))
            {
                items = items.Where(i => i.Name.ToLower().Contains(search) || i.Category.ToString().ToLower().Contains(search)).ToList();
            }

            if (filter == "Borrowers")
            {
                SetupColumns("Borrower ID", "Borrower Name", "Role", "Items Held", "Strikes", "Account Status");

                var users = (await _userService.GetAllUsersAsync())
                            .Where(u => u.Role != UserRole.Admin)
                            .ToList();

                if (!string.IsNullOrEmpty(search))
                    users = users.Where(u => u.FirstName.ToLower().Contains(search) || u.LastName.ToLower().Contains(search) || u.UserId.ToLower().Contains(search)).ToList();

                var records = await _borrowService.GetAllBorrowRecordsAsync();

                foreach (var u in users)
                {
                    int itemsHeld = records.Count(r => r.BorrowerId == u.UserId && r.Status == BorrowStatus.Active);

                    // Determine if the account is locked based on the 3-strike rule
                    string accountStatus = u.Strikes >= 3 ? "LOCKED" : "ACTIVE";

                    dgvInventory.Rows.Add(u.UserId, u.FullName, u.Role.ToString(), itemsHeld, u.Strikes, accountStatus);
                }
            }
            else if (filter == "Borrowed")
            {
                // We change the columns to show a clean summary instead of raw Record IDs
                SetupColumns("Borrower Name", "Items Held", "Specific Units", "Time Borrowed");
                var activeRecords = (await _borrowService.GetAllBorrowRecordsAsync()).Where(b => b.Status == BorrowStatus.Active).ToList();

                // Group the active records by the Borrower and the exact time they borrowed them
                var groupedBorrowed = activeRecords
                    .GroupBy(b => new { b.BorrowerId, TimeKey = b.BorrowDate.ToString("yyyyMMddHHmm") })
                    .OrderByDescending(g => g.First().BorrowDate)
                    .ToList();

                foreach (var group in groupedBorrowed)
                {
                    var first = group.First();
                    string bName = first.Borrower != null ? first.Borrower.FullName : first.BorrowerId;
                    int count = group.Count();

                    // e.g., "3 Laptops" or "Flash drive #1"
                    string baseItemName = GetBaseItemName(first.ItemName ?? "Item");
                    string summary = count > 1 ? $"{count} {baseItemName}s" : first.ItemName;

                    // A comma-separated list so the admin still knows EXACTLY which units are missing
                    string detailedUnits = string.Join(", ", group.Select(g => g.ItemName));

                    dgvInventory.Rows.Add(bName, summary, detailedUnits, first.BorrowDate.ToString("MMM dd, yyyy - hh:mm tt"));
                }
            }
            else if (filter == "Available")
            {
                SetupColumns("Item Name", "Category", "Available Units");
                var groupedItems = items.Where(i => i.Status == ItemStatus.Available).GroupBy(i => new { BaseName = GetBaseItemName(i.Name), i.Category });

                foreach (var group in groupedItems) dgvInventory.Rows.Add(group.Key.BaseName, group.Key.Category.ToString(), group.Count());
                if (dgvInventory.Columns.Contains("ItemName")) dgvInventory.Columns["ItemName"].FillWeight = 150;
            }
            else
            {
                SetupColumns("Item Name", "Category", "Total Units", "Available", "Damaged");
                var groupedItems = items.GroupBy(i => new { BaseName = GetBaseItemName(i.Name), i.Category });

                foreach (var group in groupedItems)
                {
                    int total = group.Count();
                    int avail = group.Count(x => x.Status == ItemStatus.Available);
                    int damaged = group.Count(x => x.Condition == Condition.Damaged);
                    dgvInventory.Rows.Add(group.Key.BaseName, group.Key.Category.ToString(), total, avail, damaged);
                }
                if (dgvInventory.Columns.Contains("ItemName")) dgvInventory.Columns["ItemName"].FillWeight = 150;
            }

            ToggleNoResultsState(dgvInventory.Rows.Count == 0);
        }

        private string GetBaseItemName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Unknown Item";
            int hashIndex = name.IndexOf(" #");
            return hashIndex > 0 ? name.Substring(0, hashIndex).Trim() : name.Trim();
        }

        private async Task LoadHomeContent()
        {
            if (flowRecentActivity == null) return;
            var damagedItems = await GetDamagedItemsAsync();

            flowRecentActivity.SuspendLayout();
            flowRecentActivity.Controls.Clear();

            if (!damagedItems.Any()) AddDashboardAlert("✓ All laboratory systems are operational.", DrawColor.Teal);
            else AddDashboardAlert($"⚠ REPAIR NEEDED: {damagedItems.Count} items require attention. (Click for details)", DrawColor.DarkRed);

            flowRecentActivity?.Controls.Add(new Label { Text = "RECENT ACTIVITY LOG", Font = new DrawFont("Segoe UI", 12, FontStyle.Bold), AutoSize = true });

            var rawLogs = (await _borrowService.GetAllBorrowRecordsAsync())
                  .Where(b => b.IsHiddenFromDashboard == false)
                  .ToList();

            var groupedLogs = rawLogs
                .GroupBy(b => new {
                    b.BorrowerId,
                    b.Status,
                    TimeKey = (b.Status == BorrowStatus.Active ? b.BorrowDate : (b.ReturnDate ?? b.BorrowDate)).ToString("yyyyMMddHHmm")
                })
                .OrderByDescending(g => g.Max(b => b.Status == BorrowStatus.Active ? b.BorrowDate : (b.ReturnDate ?? DateTime.MinValue)))
                .Take(10) 
                .ToList();

            foreach (var group in groupedLogs)
            {
                var firstRecord = group.First();
                int itemCount = group.Count();

                string friendlyName = firstRecord.Borrower != null && !string.IsNullOrWhiteSpace(firstRecord.Borrower.FirstName)
                    ? firstRecord.Borrower.FirstName
                    : (!string.IsNullOrWhiteSpace(firstRecord.BorrowerId) ? firstRecord.BorrowerId : "Unknown User");

                string baseItemName = GetBaseItemName(firstRecord.ItemName ?? "Item");

                string displayItem = itemCount > 1 ? $"{itemCount} {baseItemName}s" : (firstRecord.ItemName ?? "[Data Missing]");

                string actionText = firstRecord.Status == BorrowStatus.Active
                    ? $"{friendlyName} borrowed {displayItem}"
                    : $"{displayItem} were returned by {friendlyName}";

                DateTime actionTime = firstRecord.Status == BorrowStatus.Active ? firstRecord.BorrowDate : (firstRecord.ReturnDate ?? firstRecord.BorrowDate);

                AddActivityCard(actionText, actionTime, firstRecord.Status == BorrowStatus.Active ? DrawColor.FromArgb(33, 150, 243) : DrawColor.Teal);
            }

            flowRecentActivity.ResumeLayout(true);
        }

        private void AddDashboardAlert(string message, DrawColor color)
        {
            var alert = new AlertTile(message, color);

            int safeWidth = flowRecentActivity.ClientSize.Width > 0 ? flowRecentActivity.ClientSize.Width : flowRecentActivity.Width;
            alert.Width = safeWidth - flowRecentActivity.Padding.Left - flowRecentActivity.Padding.Right - 10;

            alert.AlertClicked += async (s, e) => {
                if (message.Contains("REPAIR"))
                {
                    var damagedItems = await GetDamagedItemsAsync();
                    using (var popup = new RepairDetailsPopup(damagedItems, _inventoryService, async () => {
                        await LoadHomeContent();
                        await UpdateDashboardCounts();
                    }))
                    {
                        ShowPopupWithFade(popup);
                    }
                }
            };
            flowRecentActivity?.Controls.Add(alert);
        }

        private void AddActivityCard(string message, DateTime time, DrawColor statusColor)
        {
            var card = new Ventrix.App.Controls.ActivityCard(message, time, statusColor);

            int safeWidth = flowRecentActivity.ClientSize.Width > 0 ? flowRecentActivity.ClientSize.Width : flowRecentActivity.Width;
            card.Width = safeWidth - flowRecentActivity.Padding.Left - flowRecentActivity.Padding.Right - 10;

            flowRecentActivity?.Controls.Add(card);
        }

        private async Task<IEnumerable<BorrowRecord>> GetFilteredHistoryQuery()
        {
            var query = await _borrowService.GetAllBorrowRecordsAsync();

            if (dtpStartDate != null && dtpEndDate != null)
            {
                DateTime endOfDay = dtpEndDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(b => b.BorrowDate >= dtpStartDate.Value.Date && b.BorrowDate <= endOfDay);
            }

            if (txtSearch != null && !string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string search = txtSearch.Text.ToLower();
                query = query.Where(l =>
                    (l.ItemName != null && l.ItemName.ToLower().Contains(search)) ||
                    (l.Borrower != null && l.Borrower.FullName.ToLower().Contains(search)) ||
                    (l.BorrowerId != null && l.BorrowerId.ToLower().Contains(search)));
            }

            switch (historySortColumn)
            {
                case "Item":
                    return historySortDescending ? query.OrderByDescending(b => b.ItemName) : query.OrderBy(b => b.ItemName);
                case "Borrower":
                    return historySortDescending ? query.OrderByDescending(b => b.Borrower != null ? b.Borrower.FullName : b.BorrowerId) : query.OrderBy(b => b.Borrower != null ? b.Borrower.FullName : b.BorrowerId);
                case "RTime":
                    return historySortDescending ? query.OrderByDescending(b => b.ReturnDate ?? DateTime.MaxValue) : query.OrderBy(b => b.ReturnDate ?? DateTime.MinValue);
                case "Status":
                    return historySortDescending ? query.OrderByDescending(b => b.Status.ToString()) : query.OrderBy(b => b.Status.ToString());
                case "BTime":
                default:
                    return historySortDescending ? query.OrderByDescending(b => b.BorrowDate) : query.OrderBy(b => b.BorrowDate);
            }
        }

        private async Task LoadHistoryData()
        {
            if (dgvHistory == null) return;
            dgvHistory.SuspendLayout();
            dgvHistory.Rows.Clear();

            if (dgvHistory.Columns.Count == 0)
            {
                dgvHistory.Columns.Add("Item", "Item Name");
                dgvHistory.Columns.Add("Borrower", "Borrower Name");
                dgvHistory.Columns.Add("BTime", "Time Borrowed");
                dgvHistory.Columns.Add("RTime", "Time Returned");
                dgvHistory.Columns.Add("Status", "Status");

                foreach (DataGridViewColumn col in dgvHistory.Columns)
                {
                    col.SortMode = DataGridViewColumnSortMode.Programmatic;
                }
            }

            var fullFilteredQuery = await GetFilteredHistoryQuery();

            int totalRecords = fullFilteredQuery.Count();
            historyTotalPages = (int)Math.Ceiling((double)totalRecords / historyPageSize);
            if (historyTotalPages == 0) historyTotalPages = 1;
            if (historyCurrentPage > historyTotalPages) historyCurrentPage = historyTotalPages;

            var pagedLogs = fullFilteredQuery.Skip((historyCurrentPage - 1) * historyPageSize).Take(historyPageSize).ToList();

            foreach (var log in pagedLogs)
            {
                string bName = log.Borrower != null ? log.Borrower.FullName : log.BorrowerId;
                string bStamp = log.BorrowDate.ToString("MMM dd, yyyy - hh:mm tt");
                string rStamp = log.ReturnDate.HasValue ? log.ReturnDate.Value.ToString("MMM dd, yyyy - hh:mm tt") : "---";

                dgvHistory.Rows.Add(log.ItemName, bName, bStamp, rStamp, log.Status.ToString());
            }

            foreach (DataGridViewColumn col in dgvHistory.Columns) { col.HeaderCell.SortGlyphDirection = SortOrder.None; }
            if (dgvHistory.Columns.Contains(historySortColumn))
            {
                dgvHistory.Columns[historySortColumn].HeaderCell.SortGlyphDirection = historySortDescending ? SortOrder.Descending : SortOrder.Ascending;
            }

            if (lblPageInfo != null) lblPageInfo.Text = $"Page {historyCurrentPage} of {historyTotalPages} ({totalRecords} total items)";

            ToggleNoResultsState(dgvHistory.Rows.Count == 0);

            dgvHistory.ResumeLayout();
        }
        private async Task UpdateDashboardCounts()
        {
            var items = (await _inventoryService.GetAllItemsAsync())?.ToList() ?? new List<InventoryItem>();
            var records = (await _borrowService.GetAllBorrowRecordsAsync())?.ToList() ?? new List<BorrowRecord>();
            int damagedCount = (await GetDamagedItemsAsync()).Count;

            if (badgeHealth != null)
            {
                if (damagedCount > 0)
                {
                    badgeHealth.Text = $"SYSTEM ALERTS: {damagedCount} ISSUES";
                    badgeHealth.FillColor = DrawColor.FromArgb(255, 235, 238);
                    badgeHealth.ForeColor = DrawColor.Red;
                }
                else
                {
                    badgeHealth.Text = "ALL SYSTEMS OPERATIONAL";
                    badgeHealth.FillColor = DrawColor.FromArgb(232, 245, 233);
                    badgeHealth.ForeColor = DrawColor.MediumSeaGreen;
                }
            }

            cardTotal?.UpdateMetrics("TOTAL ITEMS", items.Count.ToString("N0"), DrawColor.FromArgb(13, 71, 161));
            cardAvailable?.UpdateMetrics("AVAILABLE", items.Count(x => x.Status == ItemStatus.Available).ToString("N0"), DrawColor.Teal);
            cardPending?.UpdateMetrics("BORROWED", items.Count(x => x.Status == ItemStatus.Borrowed).ToString("N0"), DrawColor.FromArgb(192, 0, 0));
            cardBorrowers?.UpdateMetrics("RECORDS", records.Count.ToString("N0"), DrawColor.Orange);

        }

        private string GetGreeting()
        {
            int hour = DateTime.Now.Hour;
            if (hour < 12) return "GOOD MORNING";
            if (hour < 17) return "GOOD AFTERNOON";
            return "GOOD EVENING";
        }
        #endregion

        #region CRUD Actions
        private async Task btnCreate_Click(object sender, EventArgs e)
        {
            using (var popup = new InventoryPopup(_inventoryService))
            {
                if (ShowPopupWithFade(popup) == DialogResult.OK)
                {
                    await LoadFromDatabase("All");
                    await UpdateDashboardCounts();
                    ToastNotification.Show(this, "New item added to inventory!", ToastType.Success);
                }
                RefreshLayout();
            }
        }

        private async Task btnEdit_Click(object sender, EventArgs e) { await OpenItemGroupDetails(); }
        private async Task btnDelete_Click(object sender, EventArgs e) { await OpenItemGroupDetails(); }

        private async Task OpenItemGroupDetails()
        {
            if (dgvInventory?.SelectedRows.Count == 0) return;

            if (dgvInventory.Columns.Contains("ItemName"))
            {
                string itemName = dgvInventory.SelectedRows[0].Cells["ItemName"].Value?.ToString() ?? "";
                using (var popup = new ItemGroupPopup(_inventoryService, _borrowService, itemName))
                {
                    ShowPopupWithFade(popup);
                    string currentFilter = lblDashboardHeader.Text.Contains("AVAILABLE") ? "Available" : "All";
                    await SwitchView("Inventory", currentFilter);
                }
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

        private async Task DgvInventory_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvInventory == null) return;
            await OpenItemGroupDetails();
        }

        private async Task LblUrgentHeader_Click(object sender, EventArgs e)
        {
            var damagedItems = await GetDamagedItemsAsync();
            if (damagedItems.Any())
            {
                using (var popup = new RepairDetailsPopup(damagedItems, _inventoryService, async () => {
                    await LoadHomeContent();
                    await UpdateDashboardCounts();
                }))
                {
                    ShowPopupWithFade(popup);
                }
            }
        }

        private void CmbAccountActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAccountActions?.SelectedItem?.ToString() == "Sign out")
            {
                if (MessageBox.Show("Are you sure you want to sign out?", "Ventrix System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    isSigningOut = true;
                    var loginScreen = new BorrowerPortal(_inventoryService, _borrowService, _userService);
                    loginScreen.ToggleMode("Admin");
                    loginScreen.Show();
                    this.Close();
                }
                else cmbAccountActions.SelectedIndex = -1;
            }
        }
        #endregion

        #region Utilities (Export, Styling, Fade)
        private DialogResult ShowPopupWithFade(Form popup)
        {
            DialogResult result = DialogResult.Cancel;
            using (Form fadeOverlay = new Form())
            {
                fadeOverlay.StartPosition = FormStartPosition.Manual;
                fadeOverlay.FormBorderStyle = FormBorderStyle.None;
                fadeOverlay.Opacity = 0.50;
                fadeOverlay.BackColor = DrawColor.Black;
                fadeOverlay.ShowInTaskbar = false;
                fadeOverlay.Location = this.Location;
                fadeOverlay.Size = this.Size;
                fadeOverlay.Show(this);

                popup.StartPosition = FormStartPosition.CenterParent;
                result = popup.ShowDialog(fadeOverlay);
            }
            return result;
        }

        private void SetupColumns(params string[] names) { foreach (var n in names) dgvInventory.Columns.Add(n.Replace(" ", ""), n); dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; }

        private void ExportToExcel()
        {
            if (dgvInventory == null || dgvInventory.Rows.Count == 0) { MessageBox.Show("There is no inventory data to export.", "Ventrix System", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx", FileName = "Ventrix_Inventory_Report.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (XLWorkbook workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("Inventory Report");
                            int colIndex = 1;
                            for (int i = 0; i < dgvInventory.Columns.Count; i++)
                            {
                                if (!dgvInventory.Columns[i].Visible) continue;
                                worksheet.Cell(1, colIndex).Value = dgvInventory.Columns[i].HeaderText;
                                worksheet.Cell(1, colIndex).Style.Font.Bold = true;
                                worksheet.Cell(1, colIndex).Style.Fill.BackgroundColor = XLColor.FromHtml("#0D47A1");
                                worksheet.Cell(1, colIndex).Style.Font.FontColor = XLColor.White;
                                colIndex++;
                            }

                            for (int i = 0; i < dgvInventory.Rows.Count; i++)
                            {
                                int cellIndex = 1;
                                for (int j = 0; j < dgvInventory.Columns.Count; j++)
                                {
                                    if (!dgvInventory.Columns[j].Visible) continue;
                                    worksheet.Cell(i + 2, cellIndex).Value = dgvInventory.Rows[i].Cells[j].Value?.ToString() ?? "";
                                    cellIndex++;
                                }
                            }
                            worksheet.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                            ToastNotification.Show(this, "Inventory Excel exported successfully!", ToastType.Success);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Log(ex, "AdminDashboard - Export to Excel Failed");
                        MessageBox.Show("Failed to save the Excel file. Please ensure the file is not currently open in another program.", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async Task ExportHistoryToExcelAsync()
        {
            var allData = (await GetFilteredHistoryQuery()).ToList();

            if (!allData.Any()) { MessageBox.Show("No data matching these filters was found to export.", "Ventrix System", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx", FileName = $"Ventrix_Audit_History_{DateTime.Now:yyyyMMdd}.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (XLWorkbook workbook = new XLWorkbook())
                        {
                            var ws = workbook.Worksheets.Add("Audit History");
                            string[] headers = { "Record ID", "Item Name", "Borrower ID", "Borrower Name", "Role/Grade", "Purpose", "Time Borrowed", "Time Returned", "Status" };

                            for (int i = 0; i < headers.Length; i++)
                            {
                                ws.Cell(1, i + 1).Value = headers[i];
                                ws.Cell(1, i + 1).Style.Font.Bold = true;
                                ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#0D47A1");
                                ws.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
                            }

                            for (int i = 0; i < allData.Count; i++)
                            {
                                var log = allData[i];
                                ws.Cell(i + 2, 1).Value = log.Id;
                                ws.Cell(i + 2, 2).Value = log.ItemName;
                                ws.Cell(i + 2, 3).Value = log.BorrowerId;
                                ws.Cell(i + 2, 4).Value = log.Borrower != null ? log.Borrower.FullName : "Unknown";
                                ws.Cell(i + 2, 5).Value = log.GradeLevel.ToString();
                                ws.Cell(i + 2, 6).Value = log.Purpose;
                                ws.Cell(i + 2, 7).Value = log.BorrowDate.ToString("MMM dd, yyyy - hh:mm tt");
                                ws.Cell(i + 2, 8).Value = log.ReturnDate?.ToString("MMM dd, yyyy - hh:mm tt") ?? "---";
                                ws.Cell(i + 2, 9).Value = log.Status.ToString();
                            }

                            ws.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                            ToastNotification.Show(this, $"Successfully exported {allData.Count} records!", ToastType.Success);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Log(ex, "AdminDashboard - Export to Excel Failed");
                        MessageBox.Show("Failed to save the Excel file. Please ensure the file is not currently open in another program.", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportToPDF()
        {
            if (dgvInventory == null || dgvInventory.Rows.Count == 0) { MessageBox.Show("There is no inventory data to export.", "Ventrix System", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

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

                        PdfPTable pdfTable = new PdfPTable(dgvInventory.Columns.Cast<DataGridViewColumn>().Count(c => c.Visible));
                        pdfTable.WidthPercentage = 100;

                        foreach (DataGridViewColumn column in dgvInventory.Columns)
                        {
                            if (!column.Visible) continue;
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
                                if (!dgvInventory.Columns[cell.ColumnIndex].Visible) continue;
                                PdfPCell pdfCell = new PdfPCell(new Phrase(cell.Value?.ToString() ?? "", FontFactory.GetFont("Arial", 9)));
                                pdfCell.Padding = 5;
                                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pdfTable.AddCell(pdfCell);
                            }
                        }

                        pdfDoc.Add(pdfTable);
                        pdfDoc.Close();
                        ToastNotification.Show(this, "Inventory PDF exported successfully!", ToastType.Success);
                    }
                    catch (Exception ex) { MessageBox.Show("Error exporting to PDF: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        private async Task ExportHistoryToPDFAsync()
        {
            var allData = (await GetFilteredHistoryQuery()).ToList();

            if (!allData.Any()) { MessageBox.Show("No data matching these filters was found to export.", "Ventrix System", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "PDF Document|*.pdf", FileName = $"Ventrix_Audit_History_{DateTime.Now:yyyyMMdd}.pdf" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
                        PdfWriter.GetInstance(pdfDoc, new FileStream(sfd.FileName, FileMode.Create));
                        pdfDoc.Open();

                        iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD);
                        Paragraph title = new Paragraph("VENTRIX SYSTEM - FULL AUDIT HISTORY\n\n", titleFont);
                        title.Alignment = Element.ALIGN_CENTER;
                        pdfDoc.Add(title);

                        string[] headers = { "ID", "Item Name", "Borrower ID", "Borrower Name", "Role", "Purpose", "Time Borrowed", "Time Returned", "Status" };
                        PdfPTable pdfTable = new PdfPTable(headers.Length);
                        pdfTable.WidthPercentage = 100;

                        float[] widths = new float[] { 8f, 15f, 12f, 15f, 10f, 12f, 15f, 15f, 10f };
                        pdfTable.SetWidths(widths);

                        foreach (var header in headers)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(header, FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD)));
                            cell.BackgroundColor = new BaseColor(13, 71, 161);
                            cell.Padding = 5;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfTable.AddCell(cell);
                        }

                        foreach (var log in allData)
                        {
                            string[] rowData = {
                                log.Id.ToString(),
                                log.ItemName,
                                log.BorrowerId,
                                log.Borrower != null ? log.Borrower.FullName : "Unknown",
                                log.GradeLevel.ToString(),
                                log.Purpose,
                                log.BorrowDate.ToString("MMM dd, yyyy - hh:mm tt"),
                                log.ReturnDate?.ToString("MMM dd, yyyy - hh:mm tt") ?? "---",
                                log.Status.ToString()
                            };

                            foreach (var data in rowData)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(data ?? "", FontFactory.GetFont("Arial", 8)));
                                cell.Padding = 5;
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pdfTable.AddCell(cell);
                            }
                        }

                        pdfDoc.Add(pdfTable);
                        pdfDoc.Close();
                        ToastNotification.Show(this, $"Successfully exported {allData.Count} PDF records!", ToastType.Success);
                    }
                    catch (Exception ex) { MessageBox.Show("Error exporting to PDF: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        private void InitializeMaterialSkin()
        {
            var manager = MaterialSkinManager.Instance; manager.AddFormToManage(this); manager.Theme = MaterialSkinManager.Themes.LIGHT; manager.ColorScheme = new ColorScheme(DrawColor.FromArgb(13, 71, 161), DrawColor.FromArgb(10, 50, 120), DrawColor.FromArgb(33, 150, 243), DrawColor.FromArgb(30, 136, 229), TextShade.WHITE);
        }

        private void ApplyModernBranding()
        {
            if (lblDashboardHeader != null)
            {
                lblDashboardHeader.Font = null;
                ThemeManager.ApplyCustomFont(lblDashboardHeader, ThemeManager.HeaderFont, ThemeManager.VentrixBlue);
            }

            var actionBtns = new[] { btnCreate, btnEdit, btnDelete, btnClearActivity };
            var actionTxts = new[] { "ADD ITEM", "EDIT RECORD", "DELETE ITEM", "CLEAR ALL" };
            var actionClrs = new[] { DrawColor.Teal, ThemeManager.VentrixLightBlue, DrawColor.IndianRed, DrawColor.IndianRed };

            for (int i = 0; i < actionBtns.Length; i++)
            {
                if (actionBtns[i] != null)
                {
                    actionBtns[i].Text = actionTxts[i];
                    actionBtns[i].Font = ThemeManager.ButtonFont;
                    actionBtns[i].FillColor = DrawColor.Transparent;
                    actionBtns[i].HoverState.FillColor = actionClrs[i];
                    actionBtns[i].HoverState.ForeColor = DrawColor.White;
                    actionBtns[i].TextAlign = HorizontalAlignment.Left;
                    actionBtns[i].TextOffset = new DrawPoint(10, 0);
                    actionBtns[i].BorderRadius = 8;
                    actionBtns[i].Animated = true;
                }
            }
            if (btnClearActivity != null) { btnClearActivity.TextAlign = HorizontalAlignment.Center; btnClearActivity.Image = null; btnClearActivity.TextOffset = new DrawPoint(0, 0); }

            if (txtSearch != null)
            {
                txtSearch.BorderRadius = txtSearch.Height / 2;
                txtSearch.FillColor = DrawColor.FromArgb(245, 248, 252);
                txtSearch.HoverState.FillColor = DrawColor.FromArgb(250, 252, 255);
                txtSearch.FocusedState.FillColor = DrawColor.White;
            }
            if (pnlTopBar != null)
            {
                pnlTopBar.FillColor = DrawColor.White;
                pnlTopBar.ShadowDecoration.Enabled = false;
                pnlTopBar.CustomBorderColor = DrawColor.FromArgb(230, 235, 240);
                pnlTopBar.CustomBorderThickness = new Padding(0, 0, 0, 1);
            }
        }

        private void StyleDataGrids()
        {
            var grids = new[] { dgvInventory, dgvHistory };
            foreach (var grid in grids)
            {
                if (grid == null) continue;
                grid.BackgroundColor = DrawColor.White;
                grid.BorderStyle = BorderStyle.None;
                grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                grid.GridColor = DrawColor.FromArgb(230, 235, 240);
                grid.RowHeadersVisible = false;
                grid.AllowUserToAddRows = false;
                grid.AllowUserToDeleteRows = false;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.MultiSelect = false;
                grid.ReadOnly = true;

                grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                grid.ColumnHeadersHeight = 55;
                grid.ThemeStyle.HeaderStyle.BackColor = DrawColor.FromArgb(13, 71, 161);
                grid.ThemeStyle.HeaderStyle.ForeColor = DrawColor.White;
                grid.ThemeStyle.HeaderStyle.Font = new DrawFont("Segoe UI", 11F, FontStyle.Bold);
                grid.ColumnHeadersDefaultCellStyle.Font = new DrawFont("Segoe UI", 11F, FontStyle.Bold);

                grid.RowTemplate.Height = 50;
                grid.ThemeStyle.RowsStyle.BackColor = DrawColor.White;
                grid.ThemeStyle.RowsStyle.ForeColor = DrawColor.FromArgb(64, 64, 64);
                grid.ThemeStyle.RowsStyle.Font = new DrawFont("Segoe UI", 10.5F, FontStyle.Regular);
                grid.DefaultCellStyle.Font = new DrawFont("Segoe UI", 10.5F, FontStyle.Regular);
                grid.ThemeStyle.AlternatingRowsStyle.BackColor = DrawColor.FromArgb(250, 252, 255);
                grid.DefaultCellStyle.Padding = new Padding(10, 0, 10, 0);

                grid.DefaultCellStyle.SelectionBackColor = DrawColor.FromArgb(220, 235, 255);
                grid.DefaultCellStyle.SelectionForeColor = DrawColor.FromArgb(13, 71, 161);
                grid.ThemeStyle.RowsStyle.SelectionBackColor = DrawColor.FromArgb(220, 235, 255);
                grid.ThemeStyle.RowsStyle.SelectionForeColor = DrawColor.FromArgb(13, 71, 161);
                grid.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = DrawColor.FromArgb(220, 235, 255);
                grid.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = DrawColor.FromArgb(13, 71, 161);

                grid.CellMouseEnter += (s, e) => { if (e.RowIndex >= 0) { grid.Cursor = Cursors.Hand; } };
                grid.CellMouseLeave += (s, e) => { if (e.RowIndex >= 0) { grid.Cursor = Cursors.Default; } };
            }
        }

        private void DgvInventory_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.Value != null)
            {
                DataGridView dgv = sender as DataGridView;
                string colName = dgv.Columns[e.ColumnIndex].Name;
                string value = e.Value.ToString();

                if (txtSearch != null && !string.IsNullOrEmpty(txtSearch.Text) && value.ToLower().Contains(txtSearch.Text.ToLower()))
                {
                    e.CellStyle.BackColor = DrawColor.FromArgb(255, 248, 200);
                    e.CellStyle.Font = new DrawFont("Segoe UI", 10.5F, FontStyle.Bold);
                }

                if (colName == "Status" || colName == "Condition")
                {
                    e.CellStyle.Font = new DrawFont("Segoe UI", 10.5F, FontStyle.Bold);
                    if (value == nameof(ItemStatus.Available) || value == nameof(Condition.Good)) e.CellStyle.ForeColor = DrawColor.MediumSeaGreen;
                    else if (value == nameof(ItemStatus.Borrowed)) e.CellStyle.ForeColor = DrawColor.DarkOrange;
                    else e.CellStyle.ForeColor = DrawColor.IndianRed;
                }
                if (colName == "AccountStatus")
                {
                    e.CellStyle.Font = new DrawFont("Segoe UI", 10.5F, FontStyle.Bold);
                    if (value == "ACTIVE") e.CellStyle.ForeColor = DrawColor.MediumSeaGreen;
                    else if (value == "LOCKED") e.CellStyle.ForeColor = DrawColor.IndianRed;
                }
                if (colName == "Strikes" && value == "2")
                {
                    e.CellStyle.ForeColor = DrawColor.DarkOrange;
                    e.CellStyle.Font = new DrawFont("Segoe UI", 10.5F, FontStyle.Bold);
                }
            }
        }

        private void DgvHistory_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.Value != null)
            {
                DataGridView dgv = sender as DataGridView;
                string colName = dgv.Columns[e.ColumnIndex].Name;
                string value = e.Value.ToString();

                if (colName == "Status")
                {
                    e.CellStyle.Font = new DrawFont("Segoe UI", 10.5F, FontStyle.Bold);
                    if (value == nameof(BorrowStatus.Returned)) e.CellStyle.ForeColor = DrawColor.MediumSeaGreen;
                    else if (value == nameof(BorrowStatus.Active)) e.CellStyle.ForeColor = DrawColor.DarkOrange;
                }
                if (colName == "RDate" && value == "---") e.CellStyle.ForeColor = DrawColor.LightGray;
            }
        }

        private void ToggleNoResultsState(bool showNoResults)
        {
            if (pnlNoResults != null)
            {
                pnlNoResults.Visible = showNoResults;

                if (showNoResults)
                {
                    pnlNoResults.BringToFront();
                }
            }
        }
        #endregion
    }
}