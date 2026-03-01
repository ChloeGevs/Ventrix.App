using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using TheArtOfDevHtmlRenderer.Adapters.Entities;
using Ventrix.App.Controls; // MUST include this to see your new UserControls
using Ventrix.App.Popups;   // MUST include this for the separate popup files
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
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

            this.Shown += (s, e) => {
                RefreshLayout();
            };
            // Start at Home View
            SwitchView("Home"); 
        }

        private void ConfigureRuntimeUI()
        {

            // Enable Double Buffering for smooth UI
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, pnlMainContent, new object[] { true });

            pnlSidebar.BringToFront();

            // --- WIRE UP EVENTS ---
            btnCreate.Click += BtnCreate_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;
            btnHome.Click += (s, e) => SwitchView("Home");
            btnHistoryNav.Click += (s, e) => SwitchView("History");
            txtSearch.TextChanged += (s, e) => LoadFromDatabase("All");
            btnClearActivity.Click += (s, e) => ClearRecentActivity();

            // Double-click to Borrow
            dgvInventory.CellDoubleClick += DgvInventory_CellDoubleClick;

            // Sidebar Card Navigation
            cardTotal.CardClicked += (s, e) => SwitchView("Inventory", "All");
            cardAvailable.CardClicked += (s, e) => SwitchView("Inventory", "Available");
            cardPending.CardClicked += (s, e) => SwitchView("Inventory", "Borrowed");
            cardBorrowers.CardClicked += (s, e) => SwitchView("Inventory", "Borrower List");

            // Sidebar Animation
            sidebarTimer.Interval = 1;
            btnHamburger.Click += (s, e) => sidebarTimer.Start();
            sidebarTimer.Tick += SidebarTimer_Tick;

            this.Load += (s, e) =>
            {
                ApplyModernBranding();
                RefreshLayout();
            };
            this.Resize += (s, e) =>
            {
                if (this.WindowState != FormWindowState.Minimized)
                {
                    RefreshLayout();
                }
            };
        }

        #region Navigation Logic

        private void SwitchView(string viewName, string filter = "All")
        {
            pnlHomeSummary.Visible = (viewName == "Home");
            pnlGridContainer.Visible = (viewName == "Inventory");
            pnlHistory.Visible = (viewName == "History");

            // 2. Handle CRUD button visibility
            btnCreate.Visible = btnEdit.Visible = btnDelete.Visible = (viewName == "Inventory");

            switch (viewName)
            {
                case "Home":
                    lblDashboardHeader.Text = "SYSTEM EXECUTIVE SUMMARY";
                    pnlHomeSummary.BringToFront();
                    LoadHomeContent();
                    break;

                case "Inventory":
                    pnlGridContainer.BringToFront();
                    lblDashboardHeader.Text = $"INVENTORY: {filter.ToUpper()}";
                    LoadFromDatabase(filter);
                    break;

                case "History":
                    pnlHistory.BringToFront();
                    lblDashboardHeader.Text = "TRANSACTION AUDIT HISTORY";
                    LoadHistoryData();
                    break;
            }
            UpdateDashboardCounts();
            pnlSidebar.BringToFront(); // Ensure sidebar is always on top
        }

        private void DgvInventory_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            int id = Convert.ToInt32(dgvInventory.Rows[e.RowIndex].Cells[0].Value);
            string name = dgvInventory.Rows[e.RowIndex].Cells[1].Value.ToString();
            string status = dgvInventory.Rows[e.RowIndex].Cells[3].Value.ToString();

            if (status == "Available")
            {
                using (var popup = new BorrowPopup(_borrowService, id, name))
                {
                    if (popup.ShowDialog() == DialogResult.OK)
                    {
                        LoadFromDatabase("All");
                        UpdateDashboardCounts();
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

        private void LoadFromDatabase(string statusFilter)
        {
            dgvInventory.Rows.Clear();
            dgvInventory.Columns.Clear();

            var items = _inventoryService.GetAllItems();

            if (!string.IsNullOrEmpty(txtSearch.Text))
            {
                string search = txtSearch.Text.ToLower();
                items = items.Where(i => i.Name.ToLower().Contains(search)).ToList();
            }

            switch (statusFilter)
            {
                case "Available":
                    SetupColumns("ID", "Item Name", "Category", "Condition");
                    foreach (var i in items.Where(x => x.Status == "Available"))
                        dgvInventory.Rows.Add(i.Id, i.Name, i.Category, i.Condition);
                    break;

                case "Borrowed":
                    SetupColumns("ID", "Item Name", "Borrower", "Status", "Date Borrowed");
                    var active = _borrowService.GetAllBorrowRecords().Where(b => b.Status == "Active");
                    foreach (var r in active)
                        dgvInventory.Rows.Add(r.Id, r.ItemName, r.BorrowerId, r.Status, r.BorrowDate.ToShortDateString());
                    break;

                case "Borrower List":
                    SetupColumns("Borrower ID", "Grade Level", "Items Held");
                    var students = _borrowService.GetAllBorrowRecords()
                                                 .GroupBy(b => b.BorrowerId);
                    foreach (var group in students)
                        dgvInventory.Rows.Add(group.Key, group.First().GradeLevel, group.Count(x => x.Status == "Active"));
                    break;

                default: // "All"
                    SetupColumns("ID", "Item Name", "Category", "Status", "Condition");
                    foreach (var i in items)
                        dgvInventory.Rows.Add(i.Id, i.Name, i.Category, i.Status, i.Condition);
                    break;
            }
        }

        private void LoadRecentActivity()
        {
            if (flowRecentActivity == null) return;
            flowRecentActivity.Controls.Clear();

            var recentLogs = _borrowService.GetAllBorrowRecords()
                .OrderByDescending(b => b.BorrowDate)
                .Take(10);

            foreach (var log in recentLogs)
            {
                string msg = log.Status == "Active"
                    ? $"{log.BorrowerId} borrowed {log.ItemName}"
                    : $"{log.ItemName} was returned by {log.BorrowerId}";

                Color statusColor = log.Status == "Active" ? Color.FromArgb(33, 150, 243) : Color.Teal;

                AddActivityCard(msg, log.BorrowDate, statusColor);
            }
        }
        private void LoadHomeContent()
        {
            flowRecentActivity.Controls.Clear();
            AddSectionHeader("URGENT SYSTEM ALERTS");

            var items = _inventoryService.GetAllItems();
            var damagedItems = items.Where(i => i.Condition == "Damaged").ToList();

            if (!damagedItems.Any())
            {
                AddDashboardAlert("✓ All laboratory systems are operational.", Color.Teal);
            }
            else
            {
                // One interactive alert for all damaged items
                string summaryMsg = $"⚠ REPAIR NEEDED: {damagedItems.Count} items require attention. (Click for details)";
                AddDashboardAlert(summaryMsg, Color.DarkRed);
            }

            AddSectionHeader("RECENT ACTIVITY LOG");
            LoadRecentActivity();
        }

        private void AddDashboardAlert(string message, Color color)
        {
            // 1. Create a new instance of your custom control
            var alert = new AlertTile(message, color);

            // 2. Wire up the Click event defined in your AlertTile class
            alert.AlertClicked += (s, e) =>
            {
                if (message.Contains("REPAIR"))
                {
                    var damaged = _inventoryService.GetAllItems().Where(i => i.Condition == "Damaged").ToList();
                    using (var popup = new RepairDetailsPopup(damaged, _inventoryService, () => LoadHomeContent()))
                    {
                        popup.ShowDialog();
                    }
                }
            };

            // 3. Add to the flow panel and bring to top
            flowRecentActivity.Controls.Add(alert);
            alert.BringToFront();
        }

        private void AddActivityCard(string message, DateTime time, Color statusColor)
        {
            // Using the FULL path so the program CANNOT miss it
            var card = new Ventrix.App.Controls.ActivityCard(message, time, statusColor);

            // Ensure it fits the width of your flow panel
            card.Width = flowRecentActivity.Width - 30;

            flowRecentActivity.Controls.Add(card);
        }

        private void LoadHistoryData()
        {
            dgvHistory.Rows.Clear();
            dgvHistory.Columns.Clear();
            SetupColumnsHistory();
            var logs = _borrowService.GetAllBorrowRecords()
                .Where(b => b.Status == "Returned")
                .OrderByDescending(b => b.ReturnDate);

            foreach (var log in logs)
                dgvHistory.Rows.Add(log.Id, log.ItemName, log.BorrowerId, log.BorrowDate.ToShortDateString(), log.ReturnDate?.ToShortDateString());
        }

        private void UpdateDashboardCounts()
        {
            var items = _inventoryService.GetAllItems();
            var records = _borrowService.GetAllBorrowRecords();

            // --- INTEGRATION: Updating your MetricCard UserControls ---
            cardTotal.UpdateMetrics("TOTAL ITEMS", items.Count().ToString("N0"), Color.FromArgb(13, 71, 161));
            cardAvailable.UpdateMetrics("AVAILABLE", items.Count(x => x.Status == "Available").ToString("N0"), Color.Teal);
            cardPending.UpdateMetrics("BORROWED", items.Count(x => x.Status == "Borrowed").ToString("N0"), Color.FromArgb(192, 0, 0));
            cardBorrowers.UpdateMetrics("RECORDS", records.Count().ToString("N0"), Color.Orange);

            int damaged = items.Count(x => x.Condition == "Damaged");
            lblUrgentHeader.ForeColor = damaged > 0 ? Color.DarkRed : Color.Teal;
            lblUrgentHeader.Text = damaged > 0 ? $"URGENT SYSTEM ALERTS ({damaged} ISSUES)" : "URGENT SYSTEM ALERTS";
        }

        #endregion

        #region CRUD Actions

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            using (var popup = new InventoryPopup(_inventoryService))
            {
                popup.StartPosition = FormStartPosition.CenterParent;

                SuspendLayout();

                if (popup.ShowDialog() == DialogResult.OK)
                {
                    LoadFromDatabase("All");
                    UpdateDashboardCounts();
                }

                ResumeLayout(true);

                Refresh();
                RefreshLayout();
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            // 1. Validation: Ensure a row is actually selected in the grid
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
                    LoadFromDatabase("All");
                    UpdateDashboardCounts();
                }

                ResumeLayout(true);
                RefreshLayout();
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvInventory.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgvInventory.SelectedRows[0].Cells[0].Value);

            if (MessageBox.Show($"Delete item #{id}?", "Warning", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (_inventoryService.DeleteItem(id)) SwitchView("Inventory", "All");
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
            // Force the Header
            ThemeManager.ApplyCustomFont(lblDashboardHeader, ThemeManager.HeaderFont, ThemeManager.VentrixBlue);
            lblDashboardHeader.Text = "INVENTORY OVERVIEW";

            ThemeManager.ApplyCustomFont(lblUrgentHeader, ThemeManager.SubHeaderFont);

            // Re-assert fonts for every button explicitly
            var buttons = new[] { btnHistoryNav, btnHome, btnCreate, btnEdit, btnDelete, btnClearActivity };
            var colors = new[] { Color.Orange, ThemeManager.VentrixLightBlue, Color.Teal, ThemeManager.VentrixLightBlue, ThemeManager.VentrixBlue, Color.FromArgb(192, 0, 0) };
            var texts = new[] { "HISTORY", "HOME PAGE", "ADD ITEM", "EDIT RECORD", "DELETE ITEM", "CLEAR ALL" };

            for (int i = 0; i < buttons.Length; i++)
            {
                StyleNavButton(buttons[i], texts[i], colors[i]);
                // Explicitly set the font AGAIN after the StyleNavButton call
                buttons[i].Font = ThemeManager.ButtonFont;
            }

            this.Invalidate();
            this.Update(); // Force immediate redraw
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

            // Speed of the animation
            int speed = 40;

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

            // 1. TOP BAR & HEADER: Stays fixed, does not move when sidebar expands
            btnHamburger.Location = new Point(20, 30);
            lblDashboardHeader.Location = new Point(60, 22);
            txtSearch.Location = new Point(this.Width - txtSearch.Width - rightMargin, 20);

            // 2. MAIN CONTENT: Fills the whole form background behind the sidebar
            pnlMainContent.Location = new Point(0, 64);
            pnlMainContent.Size = new Size(this.Width, this.Height - 64);

            int availableWidth = pnlMainContent.Width - contentStartX - rightMargin;
            Size shiftedSize = new Size(availableWidth, pnlMainContent.Height - 160);
            Point shiftedLocation = new Point(contentStartX, 110);

            Size contentSize = new Size(pnlMainContent.Width - 40, pnlMainContent.Height - 120);
            Point contentLoc = new Point(20, 80);

            pnlHomeSummary.Bounds = pnlGridContainer.Bounds = pnlHistory.Bounds = new Rectangle(shiftedLocation, shiftedSize); ;
            dgvInventory.Anchor = dgvHistory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvInventory.Size = dgvHistory.Size = new Size(pnlGridContainer.Width - 10, pnlGridContainer.Height - 10);
            dgvInventory.Location = dgvHistory.Location = new Point(5, 5);

            // 4. FLOATING SIDEBAR: Positioned on top (Z-order)
            pnlSidebar.Location = new Point(0, 64);
            pnlSidebar.Height = this.Height - 64;

            // 5. CRUD BUTTONS: Fixed to the top-right of the form
            int btnWidth = 150;

            btnDelete.Location = new Point(this.Width - btnWidth - rightMargin, 30);
            btnEdit.Location = new Point(btnDelete.Left - btnWidth - 40, 30);
            btnCreate.Location = new Point(btnEdit.Left - btnWidth - 40, 30);

            if (pnlHomeSummary.Visible)
            {
                pnlHomeSummary.Bounds = new Rectangle(shiftedLocation, shiftedSize); //

                lblUrgentHeader.Location = new Point(20, 20); //

                // FIX: Make the flow panel take up the majority of the Home Panel  
                flowRecentActivity.Location = new Point(20, 70);
                flowRecentActivity.Size = new Size(pnlHomeSummary.Width - 40, pnlHomeSummary.Height - 150);

                pnlHomeSummary.BringToFront(); //
            }
            // 6. UPDATE COMPONENTS INSIDE SIDEBAR
            pnlSidebar.BringToFront();
            UpdateSidebarInternalUI();
        }

        private void UpdateSidebarInternalUI()
        {
            // SuspendLayout prevents the control from redrawing until all items are moved
            pnlSidebar.SuspendLayout();

            int currentWidth = pnlSidebar.Width;
            int contentWidth = currentWidth - 20;
            bool expanded = currentWidth > 200;

            // 1. User Profile Section - Use SetBounds for atomic movement
            int picSize = expanded ? 45 : 40;
            picUser.SetBounds(expanded ? 15 : 12, 25, picSize, picSize);

            lblOwnerRole.Visible = cmbAccountActions.Visible = expanded;
            if (expanded)
            {
                lblOwnerRole.Location = new Point(70, 25);
                cmbAccountActions.SetBounds(65, 42, 160, 30);
            }

            // 2. Navigation Buttons - Ensure they only show when there is enough width
            bool showNav = currentWidth > 100;
            btnHome.Visible = showNav;
            btnHistoryNav.Visible = showNav;

            if (showNav)
            {
                btnHome.SetBounds(10, 90, contentWidth, 45);
                btnHistoryNav.SetBounds(10, 140, contentWidth, 45);
            }

            // 3. Metric Card Transitions
            // Use a fixed starting Y to prevent 'drifting' locations
            int cardY = 200;
            var cards = new[] { cardTotal, cardAvailable, cardPending, cardBorrowers };

            for (int i = 0; i < cards.Length; i++)
            {
                // SetBounds is much more stable than setting .Location and .Size separately
                cards[i].SetBounds(10, cardY, contentWidth, 110);

                // Internal label visibility is now managed by the card's Z-order fix
                // so we don't need to manually toggle titles[i] and counts[i] here.

                cardY += 120;
            }

            // ResumeLayout(false) tells the sidebar to perform ONE single refresh pass
            pnlSidebar.ResumeLayout(false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            // If the manager forced 'Roboto' or 'Microsoft Sans Serif' while the popup was open
            if (lblDashboardHeader.Font.Name != "Segoe UI")
            {
                ApplyModernBranding();
            }
        }

        private void ClearRecentActivity()
        {
            if (flowRecentActivity == null) return;

            if (MessageBox.Show("Are you sure you want to clear the activity log?", "Clear History",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // 1. Clear the UI controls
                flowRecentActivity.Controls.Clear();
                LoadRecentActivity();
            }
        }
        #endregion

    }
}

