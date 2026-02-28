using MaterialSkin;
using MaterialSkin.Controls;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using Ventrix.Infrastructure;

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
            InitializeMaterialSkin();
            ConfigureRuntimeUI();
            ApplyModernBranding();  // Apply colors and fonts
            RefreshLayout();
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
            cardTotal.Click += (s, e) => SwitchView("Inventory", "All");
            cardAvailable.Click += (s, e) => SwitchView("Inventory", "Available");
            cardPending.Click += (s, e) => SwitchView("Inventory", "Borrowed");
            cardBorrowers.Click += (s, e) => SwitchView("Inventory", "Borrower List");

            // Sidebar Animation
            sidebarTimer.Interval = 1;
            btnHamburger.Click += (s, e) => sidebarTimer.Start();
            sidebarTimer.Tick += SidebarTimer_Tick;

            this.Load += (s, e) =>
            {
                ApplyModernBranding();
                RefreshLayout();
            };
            this.Resize += (s, e) => RefreshLayout();
        }

        #region Navigation Logic

        private void SwitchView(string viewName, string filter = "All")
        {
            // 1. Reset all panel visibilities
            pnlHomeSummary.Visible = false;
            pnlGridContainer.Visible = false;
            pnlHistory.Visible = false;

            // 2. Handle CRUD button visibility
            btnCreate.Visible = btnEdit.Visible = btnDelete.Visible = (viewName == "Inventory");

            switch (viewName)
            {
                case "Home":
                    pnlHomeSummary.Visible = true;
                    pnlGridContainer.Visible = false; // Explicitly hide other main panels
                    pnlHistory.Visible = false;

                    lblDashboardHeader.Text = "SYSTEM EXECUTIVE SUMMARY";

                    pnlHomeSummary.BringToFront();
                    LoadHomeContent();
                    break;

                case "Inventory":
                    pnlGridContainer.Visible = true;
                    pnlGridContainer.BringToFront();
                    lblDashboardHeader.Text = $"INVENTORY: {filter.ToUpper()}";
                    LoadFromDatabase(filter); // Load the grid data
                    break;

                case "History":
                    pnlHistory.Visible = true;
                    pnlHistory.BringToFront();
                    lblDashboardHeader.Text = "TRANSACTION AUDIT HISTORY";
                    LoadHistoryData();
                    break;
            }
            UpdateDashboardCounts();
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

        #endregion

        #region CRUD Actions

        private void BtnCreate_Click(object sender, EventArgs e)
        {
            using (var popup = new InventoryPopup(_inventoryService, _borrowService))
            {
                if (popup.ShowDialog() == DialogResult.OK) SwitchView("Inventory", "All");
            }
        }

        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvInventory.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgvInventory.SelectedRows[0].Cells[0].Value);

            using (var popup = new InventoryPopup(_inventoryService, _borrowService, id))
            {
                if (popup.ShowDialog() == DialogResult.OK) LoadFromDatabase("All");
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

        private void UpdateDashboardCounts()
        {
            var items = _inventoryService.GetAllItems();
            lblTotalCount.Text = items.Count().ToString("N0");
            lblAvailCount.Text = items.Count(x => x.Status == "Available").ToString("N0");
            lblPendingCount.Text = items.Count(x => x.Status == "Borrowed").ToString("N0");

            // Check for damaged items
            int damaged = items.Count(x => x.Condition == "Damaged");

            // If there are damaged items, make the header Red and Bold
            if (damaged > 0)
            {
                lblUrgentHeader.ForeColor = Color.DarkRed;
                lblUrgentHeader.Text = $"URGENT SYSTEM ALERTS ({damaged} ISSUES)";
            }
            else
            {
                lblUrgentHeader.ForeColor = Color.Teal; // Professional green for "All Clear"
                lblUrgentHeader.Text = "URGENT SYSTEM ALERTS";
            }
        }

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
            lblDashboardHeader.Text = "INVENTORY OVERVIEW";
            lblDashboardHeader.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            lblDashboardHeader.ForeColor = Color.FromArgb(13, 71, 161);

            // 2.APPLY STYLES TO NAVIGATION BUTTONS
            // These ensure buttons look like modern Material links instead of gray boxes
            StyleNavButton(btnHistoryNav, "HISTORY", Color.Orange);
            StyleNavButton(btnHome, "HOME PAGE", Color.FromArgb(33, 150, 243));
            StyleNavButton(btnCreate, "ADD ITEM", Color.Teal);
            StyleNavButton(btnEdit, "EDIT RECORD", Color.FromArgb(33, 150, 243));
            StyleNavButton(btnDelete, "DELETE ITEM", Color.FromArgb(192, 0, 0));
            StyleNavButton(btnClearActivity, "CLEAR ALL", Color.FromArgb(192, 0, 0));

            // 3. APPLY STYLES TO METRIC CARDS
            // This gives them the rounded white "Card" look
            SetupCard(cardTotal, lblTotalTitle, lblTotalCount, "TOTAL ITEMS", "0", Color.FromArgb(13, 71, 161));
            SetupCard(cardAvailable, lblAvailTitle, lblAvailCount, "AVAILABLE", "0", Color.Teal);
            SetupCard(cardPending, lblPendingTitle, lblPendingCount, "BORROWED", "0", Color.FromArgb(192, 0, 0));
            SetupCard(cardBorrowers, lblBorrowersTitle, lblBorrowersCount, "RECORDS", "0", Color.Orange);
            cardTotal.Parent = cardAvailable.Parent = cardPending.Parent = cardBorrowers.Parent = pnlSidebar;

            btnCreate.Visible = btnEdit.Visible = btnDelete.Visible = true;
            UpdateDashboardCounts();
        }

        private void SetupCard(Guna.UI2.WinForms.Guna2Panel card, Guna.UI2.WinForms.Guna2HtmlLabel title, Guna.UI2.WinForms.Guna2HtmlLabel count, string titleText, string countText, Color accentColor)
        {
            title.Parent = card;
            count.Parent = card;
            card.FillColor = Color.White;
            card.BorderRadius = 15;
            card.Cursor = Cursors.Hand;

            title.Text = titleText;
            title.Font = new Font("Segoe UI Semibold", 9F);
            title.ForeColor = Color.Gray;
            title.BackColor = Color.Transparent;
            // FIXED LOCATION:
            title.Location = new Point(20, 15);

            count.Text = countText;
            count.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            count.ForeColor = accentColor;
            count.BackColor = Color.Transparent;
            // FIXED LOCATION:
            count.Location = new Point(20, 55);

            title.BringToFront();
            count.BringToFront();
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

        private void AddDashboardAlert(string message, Color color)
        {
            Panel tile = new Panel
            {
                Size = new Size(flowRecentActivity.Width - 40, 60),
                BackColor = Color.White,
                Margin = new Padding(0, 0, 0, 10),
                Cursor = Cursors.Hand // Visual feedback
            };

            Label msg = new Label
            {
                Text = message,
                ForeColor = color,
                Location = new Point(10, 20),
                AutoSize = true,
                Font = new Font("Segoe UI", 18, FontStyle.Bold),
                Cursor = Cursors.Hand
            };

            // Shared Click Handler
            EventHandler alertClick = (s, e) => {
                if (message.Contains("REPAIR NEEDED"))
                {
                    var damaged = _inventoryService.GetAllItems().Where(i => i.Condition == "Damaged").ToList();
                    using (var popup = new RepairDetailsPopup(damaged, _inventoryService, () => LoadHomeContent()))
                    {
                        popup.ShowDialog();
                    }
                }
            };

            // Attach to BOTH
            tile.Click += alertClick;
            msg.Click += alertClick;

            tile.Controls.Add(msg);
            flowRecentActivity.Controls.Add(tile);

            // Ensure the new alert is visible and at the top of the flow
            tile.BringToFront();
        }

        private void AddSectionHeader(string title)
        {
            Label lbl = new Label { Text = title, Font = new Font("Segoe UI", 12, FontStyle.Bold), AutoSize = true };
            flowRecentActivity.Controls.Add(lbl);
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
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
            lblDashboardHeader.Location = new Point (60, 22);
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
            int currentWidth = pnlSidebar.Width;
            int contentWidth = currentWidth - 20;
            bool expanded = currentWidth > 200; // Logic for "Expanded" state visuals

            // User Profile Section
            picUser.Location = new Point(expanded ? 15 : 12, 25);
            picUser.Size = expanded ? new Size(45, 45) : new Size(40, 40);
            lblOwnerRole.Visible = cmbAccountActions.Visible = expanded;
            if (expanded)
            {
                lblOwnerRole.Location = new Point(70, 25);
                cmbAccountActions.Location = new Point(65, 42);
                cmbAccountActions.Size = new Size(160, 30);
            }

            // Navigation Buttons (Home & History)
            btnHome.Location = new Point(10, 90);
            btnHome.Size = new Size(contentWidth, 45);
            btnHome.Visible = (currentWidth > 100);

            btnHistoryNav.Location = new Point(10, 140);
            btnHistoryNav.Size = new Size(contentWidth, 45);
            btnHistoryNav.Visible = (currentWidth > 100);

            // Metric Card Transitions
            int cardY = 200; // Start below the nav buttons
            var cards = new[] { cardTotal, cardAvailable, cardPending, cardBorrowers };
            var titles = new[] { lblTotalTitle, lblAvailTitle, lblPendingTitle, lblBorrowersTitle };
            var counts = new[] { lblTotalCount, lblAvailCount, lblPendingCount, lblBorrowersCount };

            for (int i = 0; i < cards.Length; i++)
            {
                cards[i].Location = new Point(10, cardY);
                cards[i].Size = new Size(contentWidth, 110);
                cards[i].Width = contentWidth;

                // Sync label visibility with sidebar state
                titles[i].Visible = counts[i].Visible = expanded;

                cardY += 120; // Maintain consistent vertical gap
            }
        }

        private void LoadRecentActivity()
        {
            if (flowRecentActivity == null) return;
            flowRecentActivity.Controls.Clear();

            // Pull the latest 10 records from your BorrowService
            var recentLogs = _borrowService.GetAllBorrowRecords()
                .OrderByDescending(b => b.BorrowDate)
                .Take(10);

            foreach (var log in recentLogs)
            {
                string msg;
                Color color;

                if (log.Status == "Active")
                {
                    msg = $"{log.BorrowerId} borrowed {log.ItemName}";
                    color = Color.FromArgb(33, 150, 243); // Ventrix Blue
                }
                else
                {
                    msg = $"{log.ItemName} was returned by {log.BorrowerId}";
                    color = Color.Teal; // Returned Green
                }

                AddActivityCard(msg, log.BorrowDate, color);
            }

            flowRecentActivity.ResumeLayout(true);
            flowRecentActivity.PerformLayout();
        }

        private void AddActivityCard(string message, DateTime time, Color statusColor)
        {
            // 1. Setup the main card container
            var card = new Guna.UI2.WinForms.Guna2Panel
            {
                Size = new Size(flowRecentActivity.Width - 45, 85),
                FillColor = Color.White,
                BorderRadius = 12,
                Margin = new Padding(10, 80, 100, 10),
            };

            // FIX: ShadowDecoration.Enabled is a 'bool', not an 'int'
            card.ShadowDecoration.Enabled = true;
            card.ShadowDecoration.Color = Color.Gainsboro;

            // FIX: 'BlurResonance' is actually 'BlurRadius' in Guna UI2
            card.ShadowDecoration.Shadow = new Padding(5); // Alternative to add depth
            card.ShadowDecoration.Depth = 10;

            // 2. Status Indicator
            var indicator = new Guna.UI2.WinForms.Guna2Panel
            {
                Size = new Size(6, 85),
                FillColor = statusColor,
                BorderRadius = 12,
                Dock = DockStyle.Left
            };
   
            // 3. Message Label
            var lblMsg = new Label
            {
                Text = message,
                Location = new Point(22, 18),
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold),
                ForeColor = Color.FromArgb(45, 45, 45)
            };

            // 4. Time Label
            var lblTime = new Label
            {
                Text = $"🕒 {time.ToString("hh:mm tt")} • {time.ToString("MMM dd, yyyy")}",
                Location = new Point(22, 45),
                AutoSize = true,
                ForeColor = Color.DarkGray,
                Font = new Font("Segoe UI", 8.5F)
            };

            card.Controls.Add(indicator);
            card.Controls.Add(lblMsg);
            card.Controls.Add(lblTime);

            // Subtle Hover Effect
            card.MouseEnter += (s, e) => card.FillColor = Color.FromArgb(252, 252, 252);
            card.MouseLeave += (s, e) => card.FillColor = Color.White;

            flowRecentActivity.Controls.Add(card);
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

    // ==========================================
    //  POPUP CLASSES (Moved Outside AdminDashboard)
    // ==========================================
    public class RepairDetailsPopup : Form
    {
        private readonly InventoryService _inventoryService;
        private readonly Action _onRefresh;

        public RepairDetailsPopup(List<InventoryItem> damagedItems, InventoryService inventoryService, Action onRefresh)
        {
            _inventoryService = inventoryService;
            _onRefresh = onRefresh;

            this.Text = "Maintenance Management";
            this.Size = new Size(550, 600);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.BackColor = Color.FromArgb(242, 245, 250);

            Label lblHeader = new Label
            {
                Text = $"Maintenance Queue ({damagedItems.Count})",
                Font = new Font("Segoe UI", 14, FontStyle.Bold),
                ForeColor = Color.FromArgb(13, 71, 161),
                Location = new Point(20, 20),
                AutoSize = true
            };

            FlowLayoutPanel flow = new FlowLayoutPanel
            {
                Location = new Point(20, 60),
                Size = new Size(490, 420),
                AutoScroll = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };

            foreach (var item in damagedItems)
            {
                Panel card = new Panel { Size = new Size(460, 80), BackColor = Color.White, Margin = new Padding(0, 0, 0, 10) };

                Label name = new Label { Text = item.Name, Font = new Font("Segoe UI", 10, FontStyle.Bold), Location = new Point(15, 15), AutoSize = true };
                Label detail = new Label { Text = $"ID: {item.Id} | {item.Category}", ForeColor = Color.Gray, Location = new Point(15, 40), AutoSize = true };

                // The "Mark as Repaired" Button
                Button btnRepair = new Button
                {
                    Text = "REPAIRED",
                    Size = new Size(100, 35),
                    Location = new Point(340, 20),
                    BackColor = Color.Teal,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat,
                    Font = new Font("Segoe UI", 9, FontStyle.Bold)
                };

                btnRepair.Click += (s, e) => {
                    item.Condition = "Good"; // Reset condition
                    _inventoryService.UpdateItem(item); // Save to DB
                    card.Visible = false; // Hide from current view
                    _onRefresh?.Invoke(); // Trigger dashboard update
                };

                card.Controls.Add(name);
                card.Controls.Add(detail);
                card.Controls.Add(btnRepair);
                flow.Controls.Add(card);
            }

            Button btnClose = new Button { Text = "CLOSE", Dock = DockStyle.Bottom, Height = 50, BackColor = Color.FromArgb(13, 71, 161), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(lblHeader);
            this.Controls.Add(flow);
            this.Controls.Add(btnClose);
        }
    }
    public class InventoryPopup : Form
    {
        private readonly InventoryService _inventoryService;
        private readonly BorrowService _borrowService;
        private int? _editId;
        private TextBox txtName = new TextBox();
        private ComboBox cmbCategory = new ComboBox(), cmbStatus = new ComboBox(), cmbCondition = new ComboBox();

        public InventoryPopup(InventoryService invService, BorrowService borService, int? id = null)
        {
            _inventoryService = invService; _borrowService = borService; _editId = id;
            SetupCustomUI();
            if (_editId.HasValue) LoadData();
        }

        private void SetupCustomUI()
        {
            this.Size = new Size(400, 450); this.Text = _editId.HasValue ? "Edit Item" : "Add Item";
            this.StartPosition = FormStartPosition.CenterParent; this.FormBorderStyle = FormBorderStyle.FixedDialog;
            int y = 20;
            AddCtrl("Item Name:", txtName, ref y); 
            AddCtrl("Category:", cmbCategory, ref y);
            AddCtrl("Status:", cmbStatus, ref y); 
            AddCtrl("Condition:", cmbCondition, ref y);

            cmbCategory.Items.AddRange(new[] { "Hardware", "Device", "Accessory" });
            cmbStatus.Items.AddRange(new[] { "Available", "Borrowed", "Maintenance" });
            cmbCondition.Items.AddRange(new[] { "New", "Good", "Fair", "Damaged" });
            cmbCategory.SelectedIndex = 0; cmbStatus.SelectedIndex = 0; cmbCondition.SelectedIndex = 0;

            Button btn = new Button { Text = "SAVE", Dock = DockStyle.Bottom, Height = 50, BackColor = Color.FromArgb(13, 71, 161), ForeColor = Color.White };
            btn.Click += (s, e) => {
                var item = _editId.HasValue ? _inventoryService.GetItemById(_editId.Value) : new InventoryItem { DateAdded = DateTime.Now };
                item.Name = txtName.Text; item.Category = cmbCategory.Text; item.Status = cmbStatus.Text; item.Condition = cmbCondition.Text;
                if (_editId.HasValue) _inventoryService.UpdateItem(item); else _inventoryService.AddItem(item);
                this.DialogResult = DialogResult.OK;
            };
            this.Controls.Add(btn);
        }

        private void AddCtrl(string l, Control c, ref int y)
        {
            this.Controls.Add(new Label { Text = l, Location = new Point(20, y), AutoSize = true });
            c.Location = new Point(20, y + 20); c.Width = 340; this.Controls.Add(c); y += 60;
        }

        private void LoadData()
        {
            var i = _inventoryService.GetItemById(_editId.Value);
            txtName.Text = i.Name; cmbCategory.Text = i.Category; cmbStatus.Text = i.Status; cmbCondition.Text = i.Condition;
        }
    }

    public class BorrowPopup : Form
    {
        private readonly BorrowService _borrowService;
        private int _itemId; string _itemName;
        private TextBox txtBorrower = new TextBox(), txtPurpose = new TextBox();
        private ComboBox cmbGrade = new ComboBox();

        public BorrowPopup(BorrowService service, int id, string name)
        {
            _borrowService = service; _itemId = id; _itemName = name;
            SetupCustomUI();
        }

        private void SetupCustomUI()
        {
            Size = new Size(400, 420); this.Text = $"Borrowing: {_itemName}";
            StartPosition = FormStartPosition.CenterParent;
            int y = 30;
            AddCtrl("Borrower ID:", txtBorrower, ref y); AddCtrl("Grade:", cmbGrade, ref y); AddCtrl("Purpose:", txtPurpose, ref y);
            cmbGrade.Items.AddRange(new[] { "Grade 7", "Grade 8", "Grade 9", "Grade 10", "Grade 11", "Grade 12", "College" });

            Button btn = new Button { Text = "CONFIRM", Dock = DockStyle.Bottom, Height = 50, BackColor = Color.Teal, ForeColor = Color.White };
            btn.Click += (s, e) => {
                _borrowService.ProcessBorrow(new BorrowRecord
                {
                    BorrowerId = txtBorrower.Text,
                    ItemName = _itemName,
                    Purpose = txtPurpose.Text,
                    GradeLevel = cmbGrade.Text,
                    BorrowDate = DateTime.Now,
                    Status = "Active"
                }, _itemId);
                this.DialogResult = DialogResult.OK;
            };
            this.Controls.Add(btn);
        }

        private void AddCtrl(string l, Control c, ref int y)
        {
            this.Controls.Add(new Label { Text = l, Location = new Point(20, y), AutoSize = true });
            c.Location = new Point(20, y + 20); c.Width = 340; this.Controls.Add(c); y += 70;
        }
    }
}
