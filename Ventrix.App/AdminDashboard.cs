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
                await SwitchView("Home");
                RefreshLayout();
            };
        }

        private void ConfigureRuntimeUI()
        {
            // Safely enable Double Buffering
            if (pnlHomeSummary != null)
            {
                typeof(Control).InvokeMember("DoubleBuffered",
                    System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                    null, pnlHomeSummary, new object[] { true });
            }

            if (flowRecentActivity != null)
            {
                typeof(Control).InvokeMember("DoubleBuffered",
                    System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                    null, flowRecentActivity, new object[] { true });
            }

            pnlSidebar?.BringToFront();

            // --- SAFELY WIRE UP EVENTS ---
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

                pnlHomeSummary.MouseLeave += (s, e) => {
                    Cursor = Cursors.Default;
                };
            }

            if (dgvInventory != null) dgvInventory.CellDoubleClick += async (s, e) => await DgvInventory_CellDoubleClick(s, e);

            // Sidebar Card Navigation
            if (cardTotal != null) cardTotal.CardClicked += async (s, e) => await SwitchView("Inventory", "All");
            if (cardAvailable != null) cardAvailable.CardClicked += async (s, e) => await SwitchView("Inventory", "Available");
            if (cardPending != null) cardPending.CardClicked += async (s, e) => await SwitchView("Inventory", "Borrowed");
            if (cardBorrowers != null) cardBorrowers.CardClicked += async (s, e) => await SwitchView("Inventory", "Borrower List");

            // Sidebar Animation
            if (sidebarTimer != null && btnHamburger != null)
            {
                sidebarTimer.Interval = 10;
                btnHamburger.Click += (s, e) => sidebarTimer.Start();
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

        #region Navigation Logic

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
            var damagedItems = (await _inventoryService.GetAllItemsAsync())
                .Where(i => i.Condition == "Damaged")
                .ToList();

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
            else
            {
                MessageBox.Show("This item is already borrowed or unavailable.", "Ventrix System");
            }
        }

        #endregion

        #region Data Loading

        private void CmbAccountActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAccountActions?.SelectedItem?.ToString() == "Sign out")
            {
                var result = MessageBox.Show("Are you sure you want to sign out?", "Ventrix System",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (result == DialogResult.Yes)
                {
                    BorrowerPortal loginPortal = new BorrowerPortal(_inventoryService, _borrowService, new UserService(new Ventrix.Infrastructure.Data.AppDbContext()));
                    loginPortal.Show();
                    this.Close();
                }
                else
                {
                    cmbAccountActions.SelectedIndex = -1;
                }
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
                    foreach (var i in items.Where(x => x.Status == ItemStatus.Available))
                        dgvInventory.Rows.Add(i.Id, i.Name, i.Category, i.Condition);
                    break;

                case "Borrowed":
                    SetupColumns("ID", "Item Name", "Borrower ID", "Status", "Date Borrowed", "Return Date");
                    var records = await _borrowService.GetAllBorrowRecordsAsync();
                    var active = records.Where(b => b.Status == BorrowStatus.Active);
                    foreach (var r in active)
                        dgvInventory.Rows.Add(r.Id, r.ItemName, r.BorrowerId, r.Status, r.BorrowDate.ToShortDateString());
                    break;

                case "Borrower List":
                    SetupColumns("Borrower ID", "Borrower Name", "Grade Level", "Subject/Purpose", "Items Held");
                    var borrowerRecords = await _borrowService.GetAllBorrowRecordsAsync();
                    var students = borrowerRecords.GroupBy(b => b.BorrowerId);
                    foreach (var group in students)
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
                string msg = log.Status == BorrowStatus.Active
                    ? $"{log.BorrowerId} borrowed {log.ItemName}"
                    : $"{log.ItemName} was returned by {log.BorrowerId}";

                Color statusColor = log.Status == BorrowStatus.Active ? Color.FromArgb(33, 150, 243) : Color.Teal;
                AddActivityCard(msg, log.BorrowDate, statusColor);
            }
        }

        private async Task LoadHomeContent()
        {
            if (flowRecentActivity == null) return;

            var items = await _inventoryService.GetAllItemsAsync();
            var damagedItems = items.Where(i => i.Condition == "Damaged").ToList();
            var recentLogs = (await _borrowService.GetAllBorrowRecordsAsync())
                .OrderByDescending(b => b.BorrowDate)
                .Take(10).ToList();

            flowRecentActivity.SuspendLayout();
            flowRecentActivity.Controls.Clear();

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

            foreach (var log in recentLogs)
            {
                string msg = log.Status == BorrowStatus.Active
                    ? $"{log.BorrowerId} borrowed {log.ItemName}"
                    : $"{log.ItemName} was returned by {log.BorrowerId}";

                Color statusColor = log.Status == BorrowStatus.Active ? Color.FromArgb(33, 150, 243) : Color.Teal;
                AddActivityCard(msg, log.BorrowDate, statusColor);
            }

            await LoadRecentActivity();
            flowRecentActivity.ResumeLayout(true);
        }

        private void AddDashboardAlert(string message, Color color)
        {
            if (flowRecentActivity == null) return;
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
            if (flowRecentActivity == null) return;
            var card = new Ventrix.App.Controls.ActivityCard(message, time, statusColor);
            card.Width = flowRecentActivity.Width - 30;
            flowRecentActivity.Controls.Add(card);
        }

        private async Task LoadHistoryData()
        {
            if (dgvHistory == null) return;
            dgvHistory.Rows.Clear();
            dgvHistory.Columns.Clear();
            SetupColumnsHistory();

            var logs = (await _borrowService.GetAllBorrowRecordsAsync())
                .Where(b => b.Status == BorrowStatus.Returned)
                .OrderByDescending(b => b.ReturnDate);

            foreach (var log in logs)
                dgvHistory.Rows.Add(log.Id, log.ItemName, log.BorrowerId, log.BorrowDate.ToShortDateString(), log.ReturnDate?.ToShortDateString());
        }

        private async Task UpdateDashboardCounts()
        {
            var rawItems = await _inventoryService.GetAllItemsAsync();
            var items = rawItems != null ? rawItems.ToList() : new List<InventoryItem>();

            var rawRecords = await _borrowService.GetAllBorrowRecordsAsync();
            var records = rawRecords != null ? rawRecords.ToList() : new List<BorrowRecord>();

            // SAFELY update metrics
            cardTotal?.UpdateMetrics("TOTAL ITEMS", items.Count.ToString("N0"), Color.FromArgb(13, 71, 161));
            cardAvailable?.UpdateMetrics("AVAILABLE", items.Count(x => x.Status == ItemStatus.Available).ToString("N0"), Color.Teal);
            cardPending?.UpdateMetrics("BORROWED", items.Count(x => x.Status == ItemStatus.Borrowed).ToString("N0"), Color.FromArgb(192, 0, 0));
            cardBorrowers?.UpdateMetrics("RECORDS", records.Count.ToString("N0"), Color.Orange);

            if (lblUrgentHeader != null)
            {
                int damagedCount = items.Count(x => x.Condition == "Damaged");
                string newHeaderText = damagedCount > 0 ? $"URGENT SYSTEM ALERTS ({damagedCount} ISSUES)" : "URGENT SYSTEM ALERTS";
                Color newHeaderColor = damagedCount > 0 ? Color.DarkRed : Color.Teal;
                Cursor newCursor = damagedCount > 0 ? Cursors.Hand : Cursors.Default;

                if (lblUrgentHeader.Text != newHeaderText) lblUrgentHeader.Text = newHeaderText;
                if (lblUrgentHeader.ForeColor != newHeaderColor) lblUrgentHeader.ForeColor = newHeaderColor;
                if (lblUrgentHeader.Cursor != newCursor) lblUrgentHeader.Cursor = newCursor;
            }

            if (this.Visible)
            {
                cardTotal?.Invalidate();
                cardAvailable?.Invalidate();
                cardPending?.Invalidate();
                cardBorrowers?.Invalidate();
            }
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
            if (dgvInventory == null || dgvInventory.SelectedRows.Count == 0)
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
            if (dgvInventory == null || dgvInventory.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgvInventory.SelectedRows[0].Cells[0].Value);

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
            if (dgvHistory == null) return;
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

            var buttons = new[] { btnHistoryNav, btnHome, btnCreate, btnEdit, btnDelete, btnClearActivity };
            var colors = new[] { Color.Orange, ThemeManager.VentrixLightBlue, Color.Teal, ThemeManager.VentrixLightBlue, ThemeManager.VentrixBlue, Color.FromArgb(192, 0, 0) };
            var texts = new[] { "HISTORY", "HOME PAGE", "ADD ITEM", "EDIT RECORD", "DELETE ITEM", "CLEAR ALL" };

            for (int i = 0; i < buttons.Length; i++)
            {
                if (buttons[i] != null)
                {
                    StyleNavButton(buttons[i], texts[i], colors[i]);
                    buttons[i].Font = null;
                    buttons[i].Font = ThemeManager.ButtonFont;
                }
            }

            cardTotal?.Invalidate();
            cardAvailable?.Invalidate();
            cardPending?.Invalidate();
            cardBorrowers?.Invalidate();

            Invalidate();
            Update();
        }

        private void StyleNavButton(Guna.UI2.WinForms.Guna2Button btn, string text, Color hover)
        {
            if (btn == null) return;
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
            if (dgvInventory == null) return;
            foreach (var n in names) dgvInventory.Columns.Add(n.Replace(" ", ""), n);
            dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void AddSectionHeader(string title)
        {
            if (flowRecentActivity == null) return;
            Label lbl = new Label { Text = title, Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true };
            flowRecentActivity.Controls.Add(lbl);
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            if (pnlSidebar == null || sidebarTimer == null) return;

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

            this.SuspendLayout();
            pnlMainContent.SuspendLayout();
            if (pnlHomeSummary != null) pnlHomeSummary.SuspendLayout();

            int rightMargin = 70;
            int contentStartX = 110;

            if (btnHamburger != null) btnHamburger.Location = new Point(20, 30);
            lblDashboardHeader.Location = new Point(65, 22);
            if (txtSearch != null) txtSearch.Location = new Point(this.Width - txtSearch.Width - rightMargin, 20);

            pnlMainContent.Location = new Point(0, 64);
            pnlMainContent.Size = new Size(this.Width, this.Height - 64);

            int availableWidth = pnlMainContent.Width - contentStartX - rightMargin;
            Size shiftedSize = new Size(availableWidth, pnlMainContent.Height - 250);
            Point shiftedLocation = new Point(contentStartX, 110);

            if (pnlHomeSummary != null) pnlHomeSummary.Bounds = new Rectangle(shiftedLocation, shiftedSize);
            if (pnlGridContainer != null) pnlGridContainer.Bounds = new Rectangle(shiftedLocation, shiftedSize);
            if (pnlHistory != null) pnlHistory.Bounds = new Rectangle(shiftedLocation, shiftedSize);

            if (dgvInventory != null && pnlGridContainer != null)
            {
                dgvInventory.Size = new Size(pnlGridContainer.Width - 10, pnlGridContainer.Height - 10);
                dgvInventory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
                dgvInventory.Location = new Point(5, 5);
            }
            if (dgvHistory != null && pnlGridContainer != null)
            {
                dgvHistory.Size = new Size(pnlGridContainer.Width - 10, pnlGridContainer.Height - 10);
                dgvInventory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left;
                dgvHistory.Location = new Point(5, 5);
            }

            pnlSidebar.Location = new Point(0, 64);
            pnlSidebar.Height = this.Height - 64;

            int btnWidth = 150;
            if (btnDelete != null) btnDelete.Location = new Point(this.Width - btnWidth - rightMargin, 30);
            if (btnEdit != null && btnDelete != null) btnEdit.Location = new Point(btnDelete.Left - btnWidth - 40, 30);
            if (btnCreate != null && btnEdit != null) btnCreate.Location = new Point(btnEdit.Left - btnWidth - 40, 30);

            if (btnClearActivity != null && pnlHomeSummary != null)
            {
                btnClearActivity.Parent = pnlHomeSummary;
                btnClearActivity.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnClearActivity.Location = new Point(pnlHomeSummary.Width - btnClearActivity.Width - 20, 20);
            }
            if (lblUrgentHeader != null) lblUrgentHeader.Location = new Point(20, 30);

            if (pnlHomeSummary != null && pnlHomeSummary.Visible)
            {
                if (flowRecentActivity != null && lblUrgentHeader != null)
                {
                    flowRecentActivity.Location = new Point(20, lblUrgentHeader.Bottom + 20);
                    flowRecentActivity.Size = new Size(pnlHomeSummary.Width - 40, pnlHomeSummary.Height - flowRecentActivity.Top - 20);
                }
                pnlHomeSummary.BringToFront();
            }

            pnlSidebar.BringToFront();
            UpdateSidebarInternalUI();

            if (pnlHomeSummary != null) pnlHomeSummary.ResumeLayout(false);
            pnlMainContent.ResumeLayout(false);
            this.ResumeLayout(true);
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

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);
            ApplyModernBranding();
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            base.OnLayout(levent);
            // FIX: Removed ApplyModernBranding() from here as it creates an infinite layout loop causing UI freezes.
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
                    await _borrowService.ClearAllActivityAsync();
                    flowRecentActivity.Controls.Clear();
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