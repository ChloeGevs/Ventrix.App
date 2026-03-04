using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Ventrix.App.Controls;
using Ventrix.App.Popups;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using System.Threading.Tasks;
using System.Collections.Generic;

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

            Shown += async (s, e) => {
                this.PerformLayout();
                RefreshLayout();
                SetupInitialInnerLayout(); // Setup native anchors once
                await SwitchView("Home");
            };
        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                // Turn on WS_EX_COMPOSITED to double-buffer the entire form to eliminate tearing
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        private void ConfigureRuntimeUI()
        {
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
            if (btnCreate != null) btnCreate.Click += async (s, e) => await BtnCreate_Click(s, e);
            if (btnEdit != null) btnEdit.Click += async (s, e) => await BtnEdit_Click(s, e);
            if (btnDelete != null) btnDelete.Click += async (s, e) => await BtnDelete_Click(s, e);

            if (btnHome != null) btnHome.Click += async (s, e) => await SwitchView("Home");
            if (btnHistoryNav != null) btnHistoryNav.Click += async (s, e) => await SwitchView("History");
            if (txtSearch != null) txtSearch.TextChanged += async (s, e) => await LoadFromDatabase("All");
            if (btnClearActivity != null) btnClearActivity.Click += async (s, e) => await ClearRecentActivity();
            if (cmbAccountActions != null) cmbAccountActions.SelectedIndexChanged += CmbAccountActions_SelectedIndexChanged;

            if (lblUrgentHeader != null)
            {
                lblUrgentHeader.Click += async (s, e) => await LblUrgentHeader_Click(s, e);
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

            if (dgvInventory != null) dgvInventory.CellDoubleClick += async (s, e) => await DgvInventory_CellDoubleClick(s, e);

            if (cardTotal != null) cardTotal.CardClicked += async (s, e) => await SwitchView("Inventory", "All");
            if (cardAvailable != null) cardAvailable.CardClicked += async (s, e) => await SwitchView("Inventory", "Available");
            if (cardPending != null) cardPending.CardClicked += async (s, e) => await SwitchView("Inventory", "Borrowed");
            if (cardBorrowers != null) cardBorrowers.CardClicked += async (s, e) => await SwitchView("Inventory", "Borrower List");

            if (sidebarTimer != null && btnHamburger != null)
            {
                sidebarTimer.Interval = 10;
                btnHamburger.Click += (s, e) =>
                {
                    // Disable heavy shadow effect during slide to prevent container drag/lag
                    if (pnlGridContainer != null) pnlGridContainer.ShadowDecoration.Enabled = false;
                    sidebarTimer.Start();
                };
                sidebarTimer.Tick += SidebarTimer_Tick;
            }

            this.Load += (s, e) =>
            {
                ApplyModernBranding();
                RefreshLayout();
            };
            this.Resize += (s, e) => {
                if (this.WindowState != FormWindowState.Minimized) RefreshLayout();
            };
        }

        // Sets up WinForms Native Anchors ONCE so controls automatically stretch alongside the Main Panel
        private void SetupInitialInnerLayout()
        {
            if (pnlMainContent == null) return;

            int innerMargin = 20;
            int availableWidth = pnlMainContent.Width - (innerMargin * 2);
            Size shiftedSize = new Size(availableWidth, pnlMainContent.Height - 100);
            Point shiftedLocation = new Point(innerMargin, 80);

            var fillAnchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;

            if (pnlHomeSummary != null)
            {
                pnlHomeSummary.Bounds = new Rectangle(shiftedLocation, shiftedSize);
                pnlHomeSummary.Anchor = fillAnchor;
            }
            if (pnlGridContainer != null)
            {
                pnlGridContainer.Bounds = new Rectangle(shiftedLocation, shiftedSize);
                pnlGridContainer.Anchor = fillAnchor;
            }
            if (pnlHistory != null)
            {
                pnlHistory.Bounds = new Rectangle(shiftedLocation, shiftedSize);
                pnlHistory.Anchor = fillAnchor;
            }

            if (dgvInventory != null && pnlGridContainer != null)
            {
                dgvInventory.Bounds = new Rectangle(5, 5, pnlGridContainer.Width - 10, pnlGridContainer.Height - 10);
                dgvInventory.Anchor = fillAnchor;
            }
            if (dgvHistory != null && pnlHistory != null)
            {
                dgvHistory.Bounds = new Rectangle(5, 5, pnlHistory.Width - 10, pnlHistory.Height - 10);
                dgvHistory.Anchor = fillAnchor;
            }

            int btnWidth = 150;
            int rightEdgeMargin = 50;

            if (btnDelete != null)
            {
                btnDelete.Location = new Point(pnlMainContent.Width - btnWidth - rightEdgeMargin, 20);
                btnDelete.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            }
            if (btnEdit != null && btnDelete != null)
            {
                btnEdit.Location = new Point(btnDelete.Left - btnWidth - 20, 20);
                btnEdit.Anchor = AnchorStyles.Top | AnchorStyles.Right; 
            }
            if (btnCreate != null && btnEdit != null)
            {
                btnCreate.Location = new Point(btnEdit.Left - btnWidth - 20, 20);
                btnCreate.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            }

            if (lblUrgentHeader != null) lblUrgentHeader.Location = new Point(innerMargin, 35);

            if (btnClearActivity != null && pnlHomeSummary != null)
            {
                btnClearActivity.Parent = pnlHomeSummary;
                btnClearActivity.Location = new Point(pnlHomeSummary.Width - btnClearActivity.Width - 20, 20);
                btnClearActivity.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            }

            if (flowRecentActivity != null && lblUrgentHeader != null && pnlHomeSummary != null)
            {
                flowRecentActivity.Location = new Point(20, lblUrgentHeader.Bottom + 20);
                flowRecentActivity.Size = new Size(pnlHomeSummary.Width - 40, pnlHomeSummary.Height - flowRecentActivity.Top - 20);
                flowRecentActivity.Anchor = fillAnchor;
            }
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            if (pnlSidebar == null || sidebarTimer == null) return;

            if (!this.ContainsFocus)
            {
                sidebarTimer.Stop();
                if (pnlGridContainer != null) pnlGridContainer.ShadowDecoration.Enabled = true;
                return;
            }

            int speed = 25;

            if (isSidebarExpanded)
            {
                pnlSidebar.Width -= speed;
                if (pnlSidebar.Width <= sidebarMinWidth)
                {
                    pnlSidebar.Width = sidebarMinWidth;
                    isSidebarExpanded = false;
                    sidebarTimer.Stop();
                    // Restore container shadow
                    if (pnlGridContainer != null) pnlGridContainer.ShadowDecoration.Enabled = true;
                }
            }
            else
            {
                pnlSidebar.Width += speed;
                if (pnlSidebar.Width >= sidebarMaxWidth)
                {
                    pnlSidebar.Width = sidebarMaxWidth;
                    isSidebarExpanded = true;
                    sidebarTimer.Stop();
                    // Restore container shadow
                    if (pnlGridContainer != null) pnlGridContainer.ShadowDecoration.Enabled = true;
                }
            }

            RefreshLayout();
        }

        private void RefreshLayout()
        {
            if (pnlMainContent == null || pnlSidebar == null) return;

            int topMargin = 64;

            // 1. Position TopBar
            if (pnlTopBar != null)
            {
                pnlTopBar.Location = new Point(0, topMargin);
                pnlTopBar.Size = new Size(this.Width, 80);

                if (btnHamburger != null) btnHamburger.Location = new Point(20, 22);
                if (lblDashboardHeader != null) lblDashboardHeader.Location = new Point(75, 22);
                if (txtSearch != null) txtSearch.Location = new Point(pnlTopBar.Width - txtSearch.Width - 40, 20);
            }

            int contentY = topMargin + (pnlTopBar != null ? pnlTopBar.Height : 0);
            int remainingHeight = this.Height - contentY;

            // 2. Position Sidebar
            pnlSidebar.Location = new Point(0, contentY);
            pnlSidebar.Height = remainingHeight;

            // 3. Position Main Content perfectly next to the Sidebar
            // Note: Since SetupInitialInnerLayout() anchored the child controls, adjusting this Width resizes everything inside automatically!
            pnlMainContent.Location = new Point(pnlSidebar.Width, contentY);
            pnlMainContent.Size = new Size(this.Width - pnlSidebar.Width, remainingHeight);

            UpdateSidebarInternalUI();
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

            bool showNav = currentWidth > 100;
            if (btnHome != null) btnHome.Visible = showNav;
            if (btnHistoryNav != null) btnHistoryNav.Visible = showNav;

            if (showNav)
            {
                if (btnHome != null) btnHome.SetBounds(10, 90, contentWidth, 45);
                if (btnHistoryNav != null) btnHistoryNav.SetBounds(10, 140, contentWidth, 45);
            }

            int cardY = 200;
            var cards = new[] { cardTotal, cardAvailable, cardPending, cardBorrowers };

            for (int i = 0; i < cards.Length; i++)
            {
                if (cards[i] != null)
                {
                    cards[i].SetBounds(10, cardY, contentWidth, 110);
                    cardY += 120;
                }
            }

            pnlSidebar.ResumeLayout(false);
        }

        #region Navigation & Data Loading

        private async Task SwitchView(string viewName, string filter = "All")
        {
            if (pnlHomeSummary != null) pnlHomeSummary.Visible = (viewName == "Home");
            if (pnlGridContainer != null) pnlGridContainer.Visible = (viewName == "Inventory");
            if (pnlHistory != null) pnlHistory.Visible = (viewName == "History");

            bool showCrud = (viewName == "Inventory" && filter != "Borrowed" && filter != "Borrower List");
            if (btnCreate != null) btnCreate.Visible = showCrud;
            if (btnEdit != null) btnEdit.Visible = showCrud;
            if (btnDelete != null) btnDelete.Visible = showCrud;

            switch (viewName)
            {
                case "Home":
                    if (lblDashboardHeader != null) lblDashboardHeader.Text = "SYSTEM EXECUTIVE SUMMARY";
                    pnlHomeSummary?.BringToFront();
                    await LoadHomeContent();
                    break;
                case "Inventory":
                    pnlGridContainer?.BringToFront();
                    if (lblDashboardHeader != null) lblDashboardHeader.Text = $"INVENTORY: {filter.ToUpper()}";
                    await LoadFromDatabase(filter);
                    break;
                case "History":
                    pnlHistory?.BringToFront();
                    if (lblDashboardHeader != null) lblDashboardHeader.Text = "TRANSACTION AUDIT HISTORY";
                    await LoadHistoryData();
                    break;
            }
            await UpdateDashboardCounts();
            pnlSidebar?.BringToFront();
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

            switch (statusFilter)
            {
                case "Available":
                    SetupColumns("ID", "Item Name", "Category", "Condition");
                    foreach (var i in items.Where(x => x.Status == ItemStatus.Available)) dgvInventory.Rows.Add(i.Id, i.Name, i.Category, i.Condition);
                    break;
                case "Borrowed":
                    SetupColumns("ID", "Item Name", "Borrower ID", "Status", "Date Borrowed", "Return Date");
                    foreach (var r in (await _borrowService.GetAllBorrowRecordsAsync()).Where(b => b.Status == BorrowStatus.Active)) dgvInventory.Rows.Add(r.Id, r.ItemName, r.BorrowerId, r.Status, r.BorrowDate.ToShortDateString());
                    break;
                case "Borrower List":
                    SetupColumns("Borrower ID", "Borrower Name", "Grade Level", "Subject/Purpose", "Items Held");
                    foreach (var group in (await _borrowService.GetAllBorrowRecordsAsync()).GroupBy(b => b.BorrowerId))
                        dgvInventory.Rows.Add(group.Key, group.First().Borrower?.FullName ?? "Unknown", group.First().GradeLevel, group.First().Purpose, group.Count(x => x.Status == BorrowStatus.Active));
                    break;
                default:
                    SetupColumns("ID", "Item Name", "Category", "Status", "Condition");
                    foreach (var i in items) dgvInventory.Rows.Add(i.Id, i.Name, i.Category, i.Status, i.Condition);
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

        private async Task BtnCreate_Click(object sender, EventArgs e)
        {
            using (var popup = new InventoryPopup(_inventoryService)) { popup.StartPosition = FormStartPosition.CenterParent; if (popup.ShowDialog() == DialogResult.OK) { await LoadFromDatabase("All"); await UpdateDashboardCounts(); } RefreshLayout(); }
        }

        private async Task BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvInventory?.SelectedRows.Count == 0) { MessageBox.Show("Please select a record to edit.", "Ventrix System"); return; }
            using (var popup = new InventoryPopup(_inventoryService, Convert.ToInt32(dgvInventory.SelectedRows[0].Cells[0].Value))) { popup.StartPosition = FormStartPosition.CenterParent; if (popup.ShowDialog() == DialogResult.OK) { await LoadFromDatabase("All"); await UpdateDashboardCounts(); } RefreshLayout(); }
        }

        private async Task BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvInventory?.SelectedRows.Count > 0 && MessageBox.Show($"Delete item #{dgvInventory.SelectedRows[0].Cells[0].Value}?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes) { await _inventoryService.DeleteItemAsync(Convert.ToInt32(dgvInventory.SelectedRows[0].Cells[0].Value)); await SwitchView("Inventory", "All"); }
        }

        private async Task ClearRecentActivity()
        {
            if (MessageBox.Show("Delete all activity logs?", "Critical Action", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes) { await _borrowService.ClearAllActivityAsync(); flowRecentActivity?.Controls.Clear(); await LoadHomeContent(); await UpdateDashboardCounts(); MessageBox.Show("Records cleared."); }
        }

        #endregion

        #region UI Styling
        private void SetupColumnsHistory() { dgvHistory.Columns.Add("ID", "ID"); dgvHistory.Columns.Add("Item", "Item Name"); dgvHistory.Columns.Add("Borrower", "Borrower"); dgvHistory.Columns.Add("BDate", "Borrowed"); dgvHistory.Columns.Add("RDate", "Returned"); dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; }
        private void SetupColumns(params string[] names) { foreach (var n in names) dgvInventory.Columns.Add(n.Replace(" ", ""), n); dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; }
        private void AddSectionHeader(string title) { flowRecentActivity?.Controls.Add(new Label { Text = title, Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true }); }

        private void InitializeMaterialSkin()
        {
            var manager = MaterialSkinManager.Instance; manager.AddFormToManage(this); manager.Theme = MaterialSkinManager.Themes.LIGHT; manager.ColorScheme = new ColorScheme(Color.FromArgb(13, 71, 161), Color.FromArgb(10, 50, 120), Color.FromArgb(33, 150, 243), Color.FromArgb(30, 136, 229), TextShade.WHITE);
        }

        private void ApplyModernBranding()
        {
            if (lblDashboardHeader != null) { lblDashboardHeader.Font = null; ThemeManager.ApplyCustomFont(lblDashboardHeader, ThemeManager.HeaderFont, ThemeManager.VentrixBlue); lblDashboardHeader.Text = "INVENTORY OVERVIEW"; }
            if (lblUrgentHeader != null) { lblUrgentHeader.Font = null; ThemeManager.ApplyCustomFont(lblUrgentHeader, ThemeManager.SubHeaderFont); }
            var btns = new[] { btnHistoryNav, btnHome, btnCreate, btnEdit, btnDelete, btnClearActivity }; var clrs = new[] { Color.Orange, ThemeManager.VentrixLightBlue, Color.Teal, ThemeManager.VentrixLightBlue, ThemeManager.VentrixBlue, Color.FromArgb(192, 0, 0) }; var txts = new[] { "HISTORY", "HOME PAGE", "ADD ITEM", "EDIT RECORD", "DELETE ITEM", "CLEAR ALL" };
            for (int i = 0; i < btns.Length; i++) if (btns[i] != null) { btns[i].Text = txts[i]; btns[i].Font = ThemeManager.ButtonFont; btns[i].FillColor = Color.Transparent; btns[i].HoverState.FillColor = clrs[i]; btns[i].TextAlign = HorizontalAlignment.Center; btns[i].TextOffset = new Point(15, 0); btns[i].BorderRadius = 10; }
        }

        protected override void OnActivated(EventArgs e) { base.OnActivated(e); ApplyModernBranding(); }
        #endregion
    }
}