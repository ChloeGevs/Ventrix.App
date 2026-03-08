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

// Explicitly define Drawing elements to avoid iTextSharp conflicts
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

        public AdminDashboard(InventoryService inventoryService, BorrowService borrowService, UserService userService)
        {
            _inventoryService = inventoryService;
            _borrowService = borrowService;
            _userService = userService;

            InitializeComponent();
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

            if (keyData == Keys.Enter)
            {
                if (txtSearch != null && txtSearch.Focused)
                {
                    _ = LoadFromDatabase("All");
                    dgvInventory?.Focus();
                    return true;
                }

                if (dgvInventory != null && dgvInventory.Focused && dgvInventory.SelectedRows.Count > 0)
                {
                    _ = OpenItemGroupDetails();
                    return true;
                }
            }

            if (keyData == Keys.Escape)
            {
                if (txtSearch != null && txtSearch.Focused)
                {
                    txtSearch.Clear();
                    dgvInventory?.Focus();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        private void ConfigureRuntimeUI()
        {
            FormClosed += (s, e) => { if (!isSigningOut) System.Windows.Forms.Application.Exit(); };

            var controlsToBuffer = new Control[] { pnlMainContent, pnlGridContainer, pnlHistory, pnlHomeSummary, flowRecentActivity, pnlSidebar, dgvInventory, dgvHistory };
            foreach (var ctrl in controlsToBuffer)
            {
                if (ctrl != null)
                    typeof(Control).InvokeMember("DoubleBuffered", System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic, null, ctrl, new object[] { true });
            }

            if (btnCreate != null) btnCreate.Click += async (s, e) => await btnCreate_Click(s, e);
            if (btnEdit != null) btnEdit.Click += async (s, e) => await btnEdit_Click(s, e);
            if (btnDelete != null) btnDelete.Click += async (s, e) => await btnDelete_Click(s, e);

            if (btnExportExcel != null) btnExportExcel.Click += (s, e) => ExportToExcel();
            if (btnExportPDF != null) btnExportPDF.Click += (s, e) => ExportToPDF();

            if (btnHome != null) btnHome.Click += async (s, e) => await SwitchView("Home");
            if (btnHistoryNav != null) btnHistoryNav.Click += async (s, e) => await SwitchView("History");

            if (btnNavAllItems != null) btnNavAllItems.Click += async (s, e) => await SwitchView("Inventory", "All");
            if (btnNavAvailable != null) btnNavAvailable.Click += async (s, e) => await SwitchView("Inventory", "Available");
            if (btnNavBorrowed != null) btnNavBorrowed.Click += async (s, e) => await SwitchView("Inventory", "Borrowed");
            if (btnNavBorrowers != null) btnNavBorrowers.Click += async (s, e) => await SwitchView("Inventory", "Borrowers");

            if (btnClearActivity != null) btnClearActivity.Click += async (s, e) => await ClearRecentActivity();
            if (cmbAccountActions != null) cmbAccountActions.SelectedIndexChanged += CmbAccountActions_SelectedIndexChanged;
            if (lblUrgentHeader != null) lblUrgentHeader.Click += async (s, e) => await LblUrgentHeader_Click(s, e);

            if (cardTotal != null) cardTotal.CardClicked += async (s, e) => await SwitchView("Inventory", "All");
            if (cardAvailable != null) cardAvailable.CardClicked += async (s, e) => await SwitchView("Inventory", "Available");
            if (cardPending != null) cardPending.CardClicked += async (s, e) => await SwitchView("Inventory", "Borrowed");
            if (cardBorrowers != null) cardBorrowers.CardClicked += async (s, e) => await SwitchView("Inventory", "Borrowers");

            if (sidebarTimer != null && btnHamburger != null)
            {
                sidebarTimer.Interval = 10;
                btnHamburger.Click += (s, e) => sidebarTimer.Start();
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

            if (dgvHistory != null) dgvHistory.CellFormatting += DgvHistory_CellFormatting;

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

            if (txtSearch != null) txtSearch.Location = new DrawPoint(pnlTopBar.Width - txtSearch.Width - 30, 20);
            if (badgeHealth != null && txtSearch != null) badgeHealth.Location = new DrawPoint(txtSearch.Left - badgeHealth.Width - 20, 26);

            int margin = 30;
            DrawRect safeArea = new DrawRect(margin, margin, pnlMainContent.Width - (margin * 2), pnlMainContent.Height - (margin * 2));

            if (pnlHomeSummary != null) pnlHomeSummary.Bounds = safeArea;
            if (pnlGridContainer != null) pnlGridContainer.Bounds = safeArea;
            if (pnlHistory != null) pnlHistory.Bounds = safeArea;

            if (pnlHomeSummary != null && pnlHomeSummary.Visible) ArrangeHomeView();
            if (pnlGridContainer != null && pnlGridContainer.Visible) ArrangeInventoryView();
            if (pnlHistory != null && pnlHistory.Visible) ArrangeHistoryView();

            UpdateSidebarInternalUI();
        }

        private void ArrangeHomeView()
        {
            if (pnlHomeSummary == null) return;

            if (lblUrgentHeader != null) { lblUrgentHeader.Parent = pnlHomeSummary; lblUrgentHeader.Location = new DrawPoint(25, 25); }
            if (btnClearActivity != null) { btnClearActivity.Parent = pnlHomeSummary; btnClearActivity.Location = new DrawPoint(pnlHomeSummary.Width - btnClearActivity.Width - 25, 20); }

            int cardY = 80;
            int spacing = 20;
            int cardWidth = (pnlHomeSummary.Width - (spacing * 5)) / 4;

            if (cardTotal != null) { cardTotal.Parent = pnlHomeSummary; cardTotal.SetBounds(spacing, cardY, cardWidth, 110); }
            if (cardAvailable != null) { cardAvailable.Parent = pnlHomeSummary; cardAvailable.SetBounds(cardTotal.Right + spacing, cardY, cardWidth, 110); }
            if (cardPending != null) { cardPending.Parent = pnlHomeSummary; cardPending.SetBounds(cardAvailable.Right + spacing, cardY, cardWidth, 110); }
            if (cardBorrowers != null) { cardBorrowers.Parent = pnlHomeSummary; cardBorrowers.SetBounds(cardPending.Right + spacing, cardY, cardWidth, 110); }

            if (flowRecentActivity != null)
            {
                flowRecentActivity.Parent = pnlHomeSummary;
                flowRecentActivity.Location = new DrawPoint(20, cardTotal.Bottom + 30);
                flowRecentActivity.Size = new DrawSize(pnlHomeSummary.Width - 40, pnlHomeSummary.Height - flowRecentActivity.Top - 20);
            }
        }

        private void ArrangeInventoryView()
        {
            if (pnlGridContainer == null) return;

            int topRowY = 20;
            int margin = 25;

            // Re-parent Export Buttons to Inventory Container
            if (btnExportExcel != null) { btnExportExcel.Parent = pnlGridContainer; btnExportExcel.Location = new DrawPoint(margin, topRowY); btnExportExcel.BringToFront(); }
            if (btnExportPDF != null) { btnExportPDF.Parent = pnlGridContainer; btnExportPDF.Location = new DrawPoint(btnExportExcel.Right + 15, topRowY); btnExportPDF.BringToFront(); }

            if (btnCreate != null) { btnCreate.Parent = pnlGridContainer; btnCreate.Location = new DrawPoint(pnlGridContainer.Width - btnCreate.Width - margin, topRowY); btnCreate.BringToFront(); }
            if (btnEdit != null) { btnEdit.Parent = pnlGridContainer; btnEdit.Location = new DrawPoint(btnCreate.Left - btnEdit.Width - 15, topRowY); btnEdit.BringToFront(); }
            if (btnDelete != null) { btnDelete.Parent = pnlGridContainer; btnDelete.Location = new DrawPoint(btnEdit.Left - btnDelete.Width - 15, topRowY); btnDelete.BringToFront(); }

            if (dgvInventory != null)
            {
                int gridY = topRowY + 50;
                dgvInventory.Parent = pnlGridContainer;
                dgvInventory.Location = new DrawPoint(margin, gridY);
                dgvInventory.Size = new DrawSize(pnlGridContainer.Width - (margin * 2), pnlGridContainer.Height - gridY - margin);
                dgvInventory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                dgvInventory.BringToFront();
            }
        }

        private void ArrangeHistoryView()
        {
            if (pnlHistory == null) return;

            // Move Export Buttons to the History container
            if (btnExportExcel != null) { btnExportExcel.Parent = pnlHistory; btnExportExcel.Location = new DrawPoint(25, 20); btnExportExcel.BringToFront(); }
            if (btnExportPDF != null) { btnExportPDF.Parent = pnlHistory; btnExportPDF.Location = new DrawPoint(btnExportExcel.Right + 15, 20); btnExportPDF.BringToFront(); }

            if (dgvHistory != null)
            {
                int gridY = 75;
                dgvHistory.Parent = pnlHistory;
                dgvHistory.Location = new DrawPoint(25, gridY);
                dgvHistory.Size = new DrawSize(pnlHistory.Width - 50, pnlHistory.Height - gridY - 25);
                dgvHistory.BringToFront();
            }
        }

        private void UpdateSidebarInternalUI()
        {
            var navBtns = new[] { btnHome, btnHistoryNav, btnNavAllItems, btnNavAvailable, btnNavBorrowed, btnNavBorrowers };
            string[] navTexts = { "HOME PAGE", "HISTORY", "ALL ITEMS", "AVAILABLE", "BORROWED", "BORROWERS" };

            for (int i = 0; i < navBtns.Length; i++)
            {
                if (navBtns[i] != null)
                {
                    navBtns[i].Width = pnlSidebar.Width - 20;
                    navBtns[i].Text = isSidebarExpanded ? "   " + navTexts[i] : "";
                }
            }
            if (picUser != null) picUser.SetBounds((pnlSidebar.Width > 150) ? 15 : 12, 25, (pnlSidebar.Width > 150) ? 45 : 40, (pnlSidebar.Width > 150) ? 45 : 40);
            if (lblOwnerRole != null) lblOwnerRole.Visible = isSidebarExpanded;
            if (cmbAccountActions != null) cmbAccountActions.Visible = isSidebarExpanded;
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            int step = 25;
            if (isSidebarExpanded)
            {
                pnlSidebar.Width -= step;
                if (pnlSidebar.Width <= sidebarMinWidth)
                {
                    pnlSidebar.Width = sidebarMinWidth;
                    isSidebarExpanded = false;
                    sidebarTimer.Stop();
                    UpdateSidebarInternalUI();
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
                    UpdateSidebarInternalUI();
                }
            }

            pnlMainContent.Left = pnlSidebar.Width;
            pnlMainContent.Width = this.ClientSize.Width - pnlSidebar.Width;
        }
        #endregion

        #region Navigation & Data Loading
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

                // Dynamic Admin Greeting
                var adminUser = (await _userService.GetAllUsersAsync()).FirstOrDefault(u => u.Role == UserRole.Admin);
                string adminName = adminUser != null ? adminUser.FirstName.ToUpper() : "ADMIN";
                if (lblDashboardHeader != null) lblDashboardHeader.Text = $"{GetGreeting()}, {adminName}";
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
                SetupColumns("Borrower ID", "Borrower Name", "Role", "Items Held");

                // FIX: This strictly filters out any Admin accounts from the grid
                var users = (await _userService.GetAllUsersAsync())
                            .Where(u => u.Role != UserRole.Admin)
                            .ToList();

                if (!string.IsNullOrEmpty(search))
                    users = users.Where(u => u.FirstName.ToLower().Contains(search) || u.LastName.ToLower().Contains(search) || u.UserId.ToLower().Contains(search)).ToList();

                var records = await _borrowService.GetAllBorrowRecordsAsync();

                foreach (var u in users)
                {
                    int itemsHeld = records.Count(r => r.BorrowerId == u.UserId && r.Status == BorrowStatus.Active);
                    dgvInventory.Rows.Add(u.UserId, $"{u.FirstName} {u.LastName}", u.Role.ToString(), itemsHeld);
                }
            }
            else if (filter == "Borrowed")
            {
                SetupColumns("Record ID", "Item Name", "Borrower Name", "Time Borrowed");
                var activeRecords = (await _borrowService.GetAllBorrowRecordsAsync()).Where(b => b.Status == BorrowStatus.Active).ToList();

                foreach (var r in activeRecords)
                {
                    string bName = r.Borrower != null ? r.Borrower.FullName : r.BorrowerId;
                    // Precise Time Formatting
                    dgvInventory.Rows.Add(r.Id, r.ItemName, bName, r.BorrowDate.ToString("MMM dd, yyyy - hh:mm tt"));
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

            if (lblEmptyState != null) lblEmptyState.Visible = (dgvInventory.Rows.Count == 0);
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
            var damagedItems = (await _inventoryService.GetAllItemsAsync()).Where(i => i.Condition == Condition.Damaged).ToList();

            flowRecentActivity.SuspendLayout();
            flowRecentActivity.Controls.Clear();

            if (!damagedItems.Any()) AddDashboardAlert("✓ All laboratory systems are operational.", DrawColor.Teal);
            else AddDashboardAlert($"⚠ REPAIR NEEDED: {damagedItems.Count} items require attention. (Click for details)", DrawColor.DarkRed);

            flowRecentActivity?.Controls.Add(new Label { Text = "RECENT ACTIVITY LOG", Font = new DrawFont("Segoe UI", 12, FontStyle.Bold), AutoSize = true });

            var logs = (await _borrowService.GetAllBorrowRecordsAsync()).OrderByDescending(b => b.BorrowDate).Take(10).ToList();

            foreach (var log in logs)
            {
                // SAFETY: Fallback to ID or "Unknown User" if the database profile is missing
                string friendlyName = log.Borrower != null && !string.IsNullOrWhiteSpace(log.Borrower.FirstName)
                    ? log.Borrower.FirstName
                    : (!string.IsNullOrWhiteSpace(log.BorrowerId) ? log.BorrowerId : "Unknown User");

                // SAFETY: Fallback if the Item Name was corrupted in the database
                string safeItemName = !string.IsNullOrWhiteSpace(log.ItemName) ? log.ItemName : "[Item Data Missing]";

                string actionText = log.Status == BorrowStatus.Active
                    ? $"{friendlyName} borrowed {safeItemName}"
                    : $"{safeItemName} was returned by {friendlyName}";

                AddActivityCard(actionText, log.BorrowDate, log.Status == BorrowStatus.Active ? DrawColor.FromArgb(33, 150, 243) : DrawColor.Teal);
            }

            flowRecentActivity.ResumeLayout(true);
        }

        private void AddDashboardAlert(string message, DrawColor color)
        {
            var alert = new AlertTile(message, color);
            alert.AlertClicked += async (s, e) => {
                if (message.Contains("REPAIR"))
                {
                    using (var popup = new RepairDetailsPopup((await _inventoryService.GetAllItemsAsync()).Where(i => i.Condition == Condition.Damaged).ToList(), _inventoryService, async () => await LoadHomeContent()))
                    {
                        ShowPopupWithFade(popup);
                    }
                }
            };
            flowRecentActivity?.Controls.Add(alert);
        }

        private void AddActivityCard(string message, DateTime time, DrawColor statusColor) { var card = new Ventrix.App.Controls.ActivityCard(message, time, statusColor); card.Width = flowRecentActivity.Width - 30; flowRecentActivity?.Controls.Add(card); }

        private async Task LoadHistoryData()
        {
            if (dgvHistory == null) return;
            dgvHistory.Rows.Clear();
            dgvHistory.Columns.Clear();

            dgvHistory.Columns.Add("ID", "Record ID");
            dgvHistory.Columns.Add("Item", "Item Name");
            dgvHistory.Columns.Add("Borrower", "Borrower Name");
            dgvHistory.Columns.Add("BTime", "Time Borrowed");
            dgvHistory.Columns.Add("RTime", "Time Returned");
            dgvHistory.Columns.Add("Status", "Status");

            // Fetching will now succeed because types are synced
            var allLogs = (await _borrowService.GetAllBorrowRecordsAsync()).OrderByDescending(b => b.BorrowDate).ToList();

            foreach (var log in allLogs)
            {
                string bName = log.Borrower != null ? log.Borrower.FullName : log.BorrowerId;

                // Accurate timestamp display
                string bStamp = log.BorrowDate.ToString("MMM dd, yyyy - hh:mm tt");
                string rStamp = log.ReturnDate.HasValue ? log.ReturnDate.Value.ToString("MMM dd, yyyy - hh:mm tt") : "---";

                dgvHistory.Rows.Add(log.Id, log.ItemName, bName, bStamp, rStamp, log.Status.ToString());
            }
        }
        private async Task UpdateDashboardCounts()
        {
            var items = (await _inventoryService.GetAllItemsAsync())?.ToList() ?? new List<InventoryItem>();
            var records = (await _borrowService.GetAllBorrowRecordsAsync())?.ToList() ?? new List<BorrowRecord>();
            int damagedCount = items.Count(x => x.Condition == Condition.Damaged);

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

            if (lblUrgentHeader != null)
            {
                lblUrgentHeader.Text = damagedCount > 0 ? $"URGENT SYSTEM ALERTS ({damagedCount} ISSUES)" : "URGENT SYSTEM ALERTS";
                lblUrgentHeader.ForeColor = damagedCount > 0 ? DrawColor.DarkRed : DrawColor.Teal;
                lblUrgentHeader.Cursor = damagedCount > 0 ? Cursors.Hand : Cursors.Default;
            }
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
            var damagedItems = (await _inventoryService.GetAllItemsAsync()).Where(i => i.Condition == Condition.Damaged).ToList();
            if (damagedItems.Any())
            {
                using (var popup = new RepairDetailsPopup(damagedItems, _inventoryService, async () => await LoadHomeContent()))
                {
                    ShowPopupWithFade(popup);
                    await UpdateDashboardCounts();
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
            // Dynamically select the grid that is currently visible
            DataGridView activeGrid = (pnlHistory != null && pnlHistory.Visible) ? dgvHistory : dgvInventory;

            if (activeGrid == null || activeGrid.Rows.Count == 0) { MessageBox.Show("There is no data to export.", "Ventrix System", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx", FileName = "Ventrix_Data_Report.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (XLWorkbook workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add("Data Report");
                            int colIndex = 1;
                            for (int i = 0; i < activeGrid.Columns.Count; i++)
                            {
                                if (!activeGrid.Columns[i].Visible) continue;
                                worksheet.Cell(1, colIndex).Value = activeGrid.Columns[i].HeaderText;
                                worksheet.Cell(1, colIndex).Style.Font.Bold = true;
                                worksheet.Cell(1, colIndex).Style.Fill.BackgroundColor = XLColor.FromHtml("#0D47A1");
                                worksheet.Cell(1, colIndex).Style.Font.FontColor = XLColor.White;
                                colIndex++;
                            }

                            for (int i = 0; i < activeGrid.Rows.Count; i++)
                            {
                                int cellIndex = 1;
                                for (int j = 0; j < activeGrid.Columns.Count; j++)
                                {
                                    if (!activeGrid.Columns[j].Visible) continue;
                                    worksheet.Cell(i + 2, cellIndex).Value = activeGrid.Rows[i].Cells[j].Value?.ToString() ?? "";
                                    cellIndex++;
                                }
                            }
                            worksheet.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                            ToastNotification.Show(this, "Excel report exported successfully!", ToastType.Success);
                        }
                    }
                    catch (Exception ex) { MessageBox.Show("Error exporting to Excel: " + ex.Message, "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                }
            }
        }

        private void ExportToPDF()
        {
            // Dynamically select the grid that is currently visible
            DataGridView activeGrid = (pnlHistory != null && pnlHistory.Visible) ? dgvHistory : dgvInventory;

            if (activeGrid == null || activeGrid.Rows.Count == 0) { MessageBox.Show("There is no data to export.", "Ventrix System", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "PDF Document|*.pdf", FileName = "Ventrix_Data_Report.pdf" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
                        PdfWriter.GetInstance(pdfDoc, new FileStream(sfd.FileName, FileMode.Create));
                        pdfDoc.Open();

                        iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD);
                        Paragraph title = new Paragraph("VENTRIX SYSTEM - DATA REPORT\n\n", titleFont);
                        title.Alignment = Element.ALIGN_CENTER;
                        pdfDoc.Add(title);

                        PdfPTable pdfTable = new PdfPTable(activeGrid.Columns.Cast<DataGridViewColumn>().Count(c => c.Visible));
                        pdfTable.WidthPercentage = 100;

                        foreach (DataGridViewColumn column in activeGrid.Columns)
                        {
                            if (!column.Visible) continue;
                            PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)));
                            cell.BackgroundColor = new BaseColor(13, 71, 161);
                            cell.Padding = 5;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfTable.AddCell(cell);
                        }

                        foreach (DataGridViewRow row in activeGrid.Rows)
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                if (!activeGrid.Columns[cell.ColumnIndex].Visible) continue;
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
        #endregion
    }
}