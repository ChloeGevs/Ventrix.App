using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheArtOfDevHtmlRenderer.Adapters.Entities;
using Ventrix.App.Controls;
using Ventrix.App.Popups;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using System.Threading.Tasks;

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
                RefreshLayout();
                await SwitchView("Home");
            };
        }

        private void ConfigureRuntimeUI()
        {
            // Enable Double Buffering for smooth UI
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, pnlMainContent, new object[] { true });

            pnlSidebar.BringToFront();

            // --- WIRE UP EVENTS ---
            btnCreate.Click += async (s, e) => await BtnCreate_Click(s, e);
            btnEdit.Click += async (s, e) => await BtnEdit_Click(s, e);
            btnDelete.Click += async (s, e) => await BtnDelete_Click(s, e);

            btnHome.Click += async (s, e) => await SwitchView("Home");
            btnHistoryNav.Click += async (s, e) => await SwitchView("History");
            txtSearch.TextChanged += async (s, e) => await LoadFromDatabase("All");
            btnClearActivity.Click += async (s, e) => await ClearRecentActivity();

            // Wire up the Urgent Header click logic
            lblUrgentHeader.Click += async (s, e) => await LblUrgentHeader_Click(s, e);

            pnlHomeSummary.MouseMove += async (s, e) => {
                if (lblUrgentHeader.Bounds.Contains(e.Location))
                {
                    var items = await _inventoryService.GetAllItemsAsync();
                    if (items.Any(x => x.Condition == "Damaged"))
                    {
                        Cursor = System.Windows.Forms.Cursors.Hand;
                        return;
                    }
                }
                Cursor = System.Windows.Forms.Cursors.Default;
            };

            pnlHomeSummary.MouseLeave += (s, e) => {
                Cursor = System.Windows.Forms.Cursors.Default;
            };

            // Double-click to Borrow
            dgvInventory.CellDoubleClick += async (s, e) => await DgvInventory_CellDoubleClick(s, e);

            // Sidebar Card Navigation
            cardTotal.CardClicked += async (s, e) => await SwitchView("Inventory", "All");
            cardAvailable.CardClicked += async (s, e) => await SwitchView("Inventory", "Available");
            cardPending.CardClicked += async (s, e) => await SwitchView("Inventory", "Borrowed");
            cardBorrowers.CardClicked += async (s, e) => await SwitchView("Inventory", "Borrower List");

            // Sidebar Animation
            sidebarTimer.Interval = 1;
            btnHamburger.Click += (s, e) => sidebarTimer.Start();
            sidebarTimer.Tick += SidebarTimer_Tick;

            this.Load += (s, e) =>
            {
                ApplyModernBranding();
                RefreshLayout();
            };
            this.Resize += (s, e) => {
                if (this.WindowState != FormWindowState.Minimized) RefreshLayout();
            };
        }

        #region Navigation Logic

        private async Task SwitchView(string viewName, string filter = "All")
        {
            pnlHomeSummary.Visible = (viewName == "Home");
            pnlGridContainer.Visible = (viewName == "Inventory");
            pnlHistory.Visible = (viewName == "History");

            // 2. Handle CRUD button visibility
            btnCreate.Visible = btnEdit.Visible = btnDelete.Visible =
                (viewName == "Inventory" && filter != "Borrowed" && filter != "Borrower List");

            switch (viewName)
            {
                case "Home":
                    lblDashboardHeader.Text = "SYSTEM EXECUTIVE SUMMARY";
                    pnlHomeSummary.BringToFront();
                    await LoadHomeContent();
                    break;

                case "Inventory":
                    pnlGridContainer.BringToFront();
                    lblDashboardHeader.Text = $"INVENTORY: {filter.ToUpper()}";
                    await LoadFromDatabase(filter);
                    break;

                case "History":
                    pnlHistory.BringToFront();
                    lblDashboardHeader.Text = "TRANSACTION AUDIT HISTORY";
                    await LoadHistoryData();
                    break;
            }
            await UpdateDashboardCounts();
            pnlSidebar.BringToFront();
        }

        private async Task LblUrgentHeader_Click(object sender, EventArgs e)
        {
            // 1. Get the current list of damaged items
            var damagedItems = (await _inventoryService.GetAllItemsAsync())
                .Where(i => i.Condition == "Damaged")
                .ToList();

            // 2. Only show the popup if there are actually items to repair
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
            if (e.RowIndex < 0) return;

            int id = Convert.ToInt32(dgvInventory.Rows[e.RowIndex].Cells[0].Value);
            string name = dgvInventory.Rows[e.RowIndex].Cells[1].Value.ToString();
            string status = dgvInventory.Rows[e.RowIndex].Cells[3].Value.ToString();

            // FIX: Using ToString() to safely compare DataGridView cell text to the Enum
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
            else
            {
                MessageBox.Show("This item is already borrowed or unavailable.", "Ventrix System");
            }
        }

        #endregion

        #region Data Loading

        private async Task LoadFromDatabase(string statusFilter)
        {
            dgvInventory.Rows.Clear();
            dgvInventory.Columns.Clear();

            var items = await _inventoryService.GetAllItemsAsync();

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string search = txtSearch.Text.ToLower();
                items = items.Where(i => i.Name.ToLower().Contains(search)).ToList();
            }

            switch (statusFilter)
            {
                case "Available":
                    SetupColumns("ID", "Item Name", "Category", "Condition");
                    // FIX: Replaced string "Available" with Enum ItemStatus.Available
                    foreach (var i in items.Where(x => x.Status == ItemStatus.Available))
                        dgvInventory.Rows.Add(i.Id, i.Name, i.Category, i.Condition);
                    break;

                case "Borrowed":
                    SetupColumns("ID", "Item Name", "Borrower ID", "Status", "Date Borrowed", "Return Date");
                    var records = await _borrowService.GetAllBorrowRecordsAsync();
                    // FIX: Replaced string "Active" with Enum BorrowStatus.Active
                    var active = records.Where(b => b.Status == BorrowStatus.Active);
                    foreach (var r in active)
                        dgvInventory.Rows.Add(r.Id, r.ItemName, r.BorrowerId, r.Status, r.BorrowDate.ToShortDateString());
                    break;

                case "Borrower List":
                    SetupColumns("Borrower ID", "Borrower Name", "Grade Level", "Subject/Purpose", "Items Held");
                    var borrowerRecords = await _borrowService.GetAllBorrowRecordsAsync();
                    var students = borrowerRecords.GroupBy(b => b.BorrowerId);
                    foreach (var group in students)
                        // FIX: Replaced string "Active" with Enum BorrowStatus.Active
                        dgvInventory.Rows.Add(group.Key, group.First().Borrower?.FullName ?? "Unknown", group.First().GradeLevel, group.First().Purpose, group.Count(x => x.Status == BorrowStatus.Active));
                    break;

                default: // "All"
                    SetupColumns("ID", "Item Name", "Category", "Status", "Condition");
                    foreach (var i in items)
                        dgvInventory.Rows.Add(i.Id, i.Name, i.Category, i.Status, i.Condition);
                    break;
            }
        }

        private async Task LoadRecentActivity()
        {
            if (flowRecentActivity == null) return;
            flowRecentActivity.Controls.Clear();

            var recentLogs = (await _borrowService.GetAllBorrowRecordsAsync())
                .OrderByDescending(b => b.BorrowDate)
                .Take(10);

            foreach (var log in recentLogs)
            {
                // FIX: Use BorrowStatus Enum comparisons
                string msg = log.Status == BorrowStatus.Active
                    ? $"{log.BorrowerId} borrowed {log.ItemName}"
                    : $"{log.ItemName} was returned by {log.BorrowerId}";

                Color statusColor = log.Status == BorrowStatus.Active ? Color.FromArgb(33, 150, 243) : Color.Teal;

                AddActivityCard(msg, log.BorrowDate, statusColor);
            }
        }

        private async Task LoadHomeContent()
        {
            flowRecentActivity.Controls.Clear();
            AddSectionHeader("URGENT SYSTEM ALERTS");

            var items = await _inventoryService.GetAllItemsAsync();
            var damagedItems = items.Where(i => i.Condition == "Damaged").ToList();

            if (!damagedItems.Any())
            {
                AddDashboardAlert("✓ All laboratory systems are operational.", Color.Teal);
            }
            else
            {
                string summaryMsg = $"⚠ REPAIR NEEDED: {damagedItems.Count} items require attention. (Click for details)";
                AddDashboardAlert(summaryMsg, Color.DarkRed);
            }

            AddSectionHeader("RECENT ACTIVITY LOG");
            await LoadRecentActivity();
        }

        private void AddDashboardAlert(string message, Color color)
        {
            var alert = new AlertTile(message, color);

            alert.AlertClicked += async (s, e) =>
            {
                if (message.Contains("REPAIR"))
                {
                    var damaged = (await _inventoryService.GetAllItemsAsync()).Where(i => i.Condition == "Damaged").ToList();
                    using (var popup = new RepairDetailsPopup(damaged, _inventoryService, async () => await LoadHomeContent()))
                    {
                        popup.ShowDialog();
                    }
                }
            };

            flowRecentActivity.Controls.Add(alert);
            alert.BringToFront();
        }

        private void AddActivityCard(string message, DateTime time, Color statusColor)
        {
            var card = new Ventrix.App.Controls.ActivityCard(message, time, statusColor);
            card.Width = flowRecentActivity.Width - 30;
            flowRecentActivity.Controls.Add(card);
        }

        private async Task LoadHistoryData()
        {
            dgvHistory.Rows.Clear();
            dgvHistory.Columns.Clear();
            SetupColumnsHistory();
            var logs = (await _borrowService.GetAllBorrowRecordsAsync())
                // FIX: Replaced string "Returned" with Enum BorrowStatus.Returned
                .Where(b => b.Status == BorrowStatus.Returned)
                .OrderByDescending(b => b.ReturnDate);

            foreach (var log in logs)
                dgvHistory.Rows.Add(log.Id, log.ItemName, log.BorrowerId, log.BorrowDate.ToShortDateString(), log.ReturnDate?.ToShortDateString());
        }

        private async Task UpdateDashboardCounts()
        {
            var items = (await _inventoryService.GetAllItemsAsync()).ToList();
            var records = (await _borrowService.GetAllBorrowRecordsAsync()).ToList();

            // 1. Update the Metric Cards
            // FIX: Uses Enums for filtering statuses
            cardTotal.UpdateMetrics("TOTAL ITEMS", items.Count().ToString("N0"), Color.FromArgb(13, 71, 161));
            cardAvailable.UpdateMetrics("AVAILABLE", items.Count(x => x.Status == ItemStatus.Available).ToString("N0"), Color.Teal);
            cardPending.UpdateMetrics("BORROWED", items.Count(x => x.Status == ItemStatus.Borrowed).ToString("N0"), Color.FromArgb(192, 0, 0));
            cardBorrowers.UpdateMetrics("RECORDS", records.Count().ToString("N0"), Color.Orange);

            // 2. Logic for lblUrgentHeader (Interactive Alert System)
            int damagedCount = items.Count(x => x.Condition == "Damaged");

            if (damagedCount > 0)
            {
                lblUrgentHeader.ForeColor = Color.DarkRed;
                lblUrgentHeader.Text = $"URGENT SYSTEM ALERTS ({damagedCount} ISSUES)";
                lblUrgentHeader.Cursor = System.Windows.Forms.Cursors.Hand;
            }
            else
            {
                lblUrgentHeader.ForeColor = Color.Teal;
                lblUrgentHeader.Text = "URGENT SYSTEM ALERTS";
                lblUrgentHeader.Cursor = System.Windows.Forms.Cursors.Default;
            }

            // 3. Force Refresh to prevent "Black Labels"
            cardTotal.Invalidate();
            cardAvailable.Invalidate();
            cardPending.Invalidate();
            cardBorrowers.Invalidate();
        }

        #endregion

        #region CRUD Actions

        private async Task BtnCreate_Click(object sender, EventArgs e)
        {
            using (var popup = new InventoryPopup(_inventoryService))
            {
                popup.StartPosition = FormStartPosition.CenterParent;
                SuspendLayout();

                if (popup.ShowDialog() == DialogResult.OK)
                {
                    await LoadFromDatabase("All");
                    await UpdateDashboardCounts();
                }

                ResumeLayout(true);
                Refresh();
                RefreshLayout();
            }
        }

        private async Task BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvInventory.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a record to edit.", "Ventrix System");
                return;
            }

            int id = Convert.ToInt32(dgvInventory.SelectedRows[0].Cells[0].Value);

            using (var popup = new InventoryPopup(_inventoryService, id))
            {
                popup.StartPosition = FormStartPosition.CenterParent;
                this.SuspendLayout();

                if (popup.ShowDialog() == DialogResult.OK)
                {
                    await LoadFromDatabase("All");
                    await UpdateDashboardCounts();
                }

                ResumeLayout(true);
                RefreshLayout();
            }
        }

        private async Task BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvInventory.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgvInventory.SelectedRows[0].Cells[0].Value);

            // FIX: Removed the invalid 'if' check to prevent "Cannot await 'void'" error
            if (MessageBox.Show($"Delete item #{id}?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                await _inventoryService.DeleteItemAsync(id);
                await SwitchView("Inventory", "All");
            }
        }

        #endregion

        #region UI Styling & Helpers
        private void SetupColumnsHistory()
        {
            dgvHistory.Columns.Add("ID", "ID");
            dgvHistory.Columns.Add("Item", "Item Name");
            dgvHistory.Columns.Add("Borrower", "Borrower");
            dgvHistory.Columns.Add("BDate", "Borrowed");
            dgvHistory.Columns.Add("RDate", "Returned");
            dgvHistory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }


        private void InitializeMaterialSkin()
        {
            var manager = MaterialSkinManager.Instance;
            manager.AddFormToManage(this);
            manager.Theme = MaterialSkinManager.Themes.LIGHT;
            manager.ColorScheme = new ColorScheme(Color.FromArgb(13, 71, 161), Color.FromArgb(10, 50, 120), Color.FromArgb(33, 150, 243), Color.FromArgb(30, 136, 229), TextShade.WHITE);
        }

        private void ApplyModernBranding()
        {
            lblDashboardHeader.Font = null;
            ThemeManager.ApplyCustomFont(lblDashboardHeader, ThemeManager.HeaderFont, ThemeManager.VentrixBlue);
            lblDashboardHeader.Text = "INVENTORY OVERVIEW";

            lblUrgentHeader.Font = null;
            ThemeManager.ApplyCustomFont(lblUrgentHeader, ThemeManager.SubHeaderFont);

            var buttons = new[] { btnHistoryNav, btnHome, btnCreate, btnEdit, btnDelete, btnClearActivity };
            var colors = new[] { Color.Orange, ThemeManager.VentrixLightBlue, Color.Teal, ThemeManager.VentrixLightBlue, ThemeManager.VentrixBlue, Color.FromArgb(192, 0, 0) };
            var texts = new[] { "HISTORY", "HOME PAGE", "ADD ITEM", "EDIT RECORD", "DELETE ITEM", "CLEAR ALL" };

            for (int i = 0; i < buttons.Length; i++)
            {
                StyleNavButton(buttons[i], texts[i], colors[i]);
                buttons[i].Font = null;
                buttons[i].Font = ThemeManager.ButtonFont;
            }

            cardTotal.Invalidate();
            cardAvailable.Invalidate();
            cardPending.Invalidate();
            cardBorrowers.Invalidate();

            Invalidate();
            Update();
        }

        private void StyleNavButton(Guna.UI2.WinForms.Guna2Button btn, string text, Color hover)
        {
            btn.Text = text;
            btn.Font = new Font("Sitka Banner", 11F, FontStyle.Bold);
            btn.FillColor = Color.Transparent;
            btn.HoverState.FillColor = hover;
            btn.TextAlign = HorizontalAlignment.Center;
            btn.TextOffset = new Point(15, 0);
            btn.BorderRadius = 10;
        }

        private void SetupColumns(params string[] names)
        {
            foreach (var n in names) dgvInventory.Columns.Add(n.Replace(" ", ""), n);
            dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void AddSectionHeader(string title)
        {
            Label lbl = new Label { Text = title, Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true };
            flowRecentActivity.Controls.Add(lbl);
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {

            if (!this.ContainsFocus)
            {
                sidebarTimer.Stop();
                return;
            }

            int speed = 50;

            if (isSidebarExpanded)
            {
                pnlSidebar.Width -= speed;
                if (pnlSidebar.Width <= sidebarMinWidth)
                {
                    pnlSidebar.Width = sidebarMinWidth;
                    isSidebarExpanded = false;
                    sidebarTimer.Stop();
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
                }
            }

            UpdateSidebarInternalUI();
        }

        private void RefreshLayout()
        {
            if (pnlMainContent == null || pnlSidebar == null || lblDashboardHeader == null) return;

            int rightMargin = 70;
            int contentStartX = 110;

            btnHamburger.Location = new Point(20, 30);
            lblDashboardHeader.Location = new Point(60, 22);
            txtSearch.Location = new Point(this.Width - txtSearch.Width - rightMargin, 20);

            pnlMainContent.Location = new Point(0, 64);
            pnlMainContent.Size = new Size(this.Width, this.Height - 64);

            int availableWidth = pnlMainContent.Width - contentStartX - rightMargin;
            Size shiftedSize = new Size(availableWidth, pnlMainContent.Height - 160);
            Point shiftedLocation = new Point(contentStartX, 110);

            pnlHomeSummary.Bounds = pnlGridContainer.Bounds = pnlHistory.Bounds = new Rectangle(shiftedLocation, shiftedSize); ;
            dgvInventory.Anchor = dgvHistory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvInventory.Size = dgvHistory.Size = new Size(pnlGridContainer.Width - 10, pnlGridContainer.Height - 10);
            dgvInventory.Location = dgvHistory.Location = new Point(5, 5);

            pnlSidebar.Location = new Point(0, 64);
            pnlSidebar.Height = this.Height - 64;

            int btnWidth = 150;

            btnDelete.Location = new Point(this.Width - btnWidth - rightMargin, 30);
            btnEdit.Location = new Point(btnDelete.Left - btnWidth - 40, 30);
            btnCreate.Location = new Point(btnEdit.Left - btnWidth - 40, 30);

            if (pnlHomeSummary.Visible)
            {
                pnlHomeSummary.Bounds = new Rectangle(shiftedLocation, shiftedSize);

                // 1. FORCE the button into the panel so the coordinates calculate correctly
                btnClearActivity.Parent = pnlHomeSummary;

                // 2. Tell WinForms to keep it locked to the top right corner
                btnClearActivity.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                // 3. Set the location (Panel Width - Button Width - 20px margin)
                btnClearActivity.Location = new Point(pnlHomeSummary.Width - btnClearActivity.Width - 20, 20);

                // 4. Set the Urgent Header to the top left
                lblUrgentHeader.Location = new Point(20, 30);

                // 5. Place the activity feed safely below them
                flowRecentActivity.Location = new Point(20, lblUrgentHeader.Bottom + 20);
                flowRecentActivity.Size = new Size(pnlHomeSummary.Width - 40, pnlHomeSummary.Height - flowRecentActivity.Top - 20);

                pnlHomeSummary.BringToFront();
            }

            pnlSidebar.BringToFront();
            UpdateSidebarInternalUI();
        }

        private void UpdateSidebarInternalUI()
        {
            pnlSidebar.SuspendLayout();

            int currentWidth = pnlSidebar.Width;
            int contentWidth = currentWidth - 20;
            bool expanded = currentWidth > 200;

            int picSize = expanded ? 45 : 40;
            picUser.SetBounds(expanded ? 15 : 12, 25, picSize, picSize);

            lblOwnerRole.Visible = cmbAccountActions.Visible = expanded;
            if (expanded)
            {
                lblOwnerRole.Location = new Point(70, 25);
                cmbAccountActions.SetBounds(65, 42, 160, 30);
            }

            bool showNav = currentWidth > 100;
            btnHome.Visible = showNav;
            btnHistoryNav.Visible = showNav;

            if (showNav)
            {
                btnHome.SetBounds(10, 90, contentWidth, 45);
                btnHistoryNav.SetBounds(10, 140, contentWidth, 45);
            }

            int cardY = 200;
            var cards = new[] { cardTotal, cardAvailable, cardPending, cardBorrowers };

            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].SetBounds(10, cardY, contentWidth, 110);
                cardY += 120;
            }

            pnlSidebar.ResumeLayout(false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (lblDashboardHeader.Font.Name != "Segoe UI")
            {
                ApplyModernBranding();
            }
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            ApplyModernBranding();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            if (lblDashboardHeader != null)
            {
                ApplyModernBranding();
            }
        }

        private async Task ClearRecentActivity()
        {
            if (flowRecentActivity == null) return;

            var result = MessageBox.Show(
                "Are you sure you want to permanently delete all activity logs from the database?",
                "Ventrix System | Critical Action",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (result == DialogResult.Yes)
            {
                try
                {
                    // 1. Delete from Database
                    await _borrowService.ClearAllActivityAsync();

                    // 2. Clear the UI Flow Panel
                    flowRecentActivity.Controls.Clear();

                    // 3. Refresh the UI to show the empty state or headers
                    await LoadHomeContent();
                    await UpdateDashboardCounts();

                    MessageBox.Show("All activity records have been cleared.", "Success");
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error clearing records: {ex.Message}", "System Error");
                }
            }
        }
        #endregion

    }
}