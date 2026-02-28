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
                SwitchView("Home");
            };
            this.Resize += (s, e) => RefreshLayout();
        }
        
        #region Navigation Logic

        private void SwitchView(string viewName, string filter = "All")
        {
            pnlHomeSummary.Visible = false;
            pnlGridContainer.Visible = false;
            pnlHistory.Visible = false;

            btnCreate.Visible = btnEdit.Visible = btnDelete.Visible = (viewName == "Inventory");

            switch (viewName)
            {
                case "Home":
                    pnlHomeSummary.Visible = true;
                    pnlHomeSummary.BringToFront();
                    lblDashboardHeader.Text = "SYSTEM EXECUTIVE SUMMARY";
                    LoadHomeContent();
                    break;
                case "Inventory":
                    pnlGridContainer.Visible = true;
                    lblDashboardHeader.Text = $"INVENTORY: {filter.ToUpper()}";
                    LoadFromDatabase(filter);
                    break;
                case "History":
                    pnlHistory.Visible = true;
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
                case "Home":
                    pnlHomeSummary.Visible = true;
                    pnlHomeSummary.BringToFront();
                    lblDashboardHeader.Text = "SYSTEM EXECUTIVE SUMMARY";
                    LoadHomeContent();
                    LoadRecentActivity(); // <--- ADD THIS LINE
                    break;
                case "Available":
                    // Only show location-relevant info
                    SetupColumns("ID", "Item Name", "Category", "Condition");
                    foreach (var i in items.Where(x => x.Status == "Available"))
                        dgvInventory.Rows.Add(i.Id, i.Name, i.Category, i.Condition);
                    break;

                case "Borrowed":
                    // Show WHO has the item and WHEN they took it
                    SetupColumns("ID", "Item Name", "Borrower", "Status", "Date Borrowed");
                    var active = _borrowService.GetAllBorrowRecords().Where(b => b.Status == "Active");
                    foreach (var r in active)
                        dgvInventory.Rows.Add(r.Id, r.ItemName, r.BorrowerId, r.Status, r.BorrowDate.ToShortDateString());
                    break;

                case "Borrower List":
                    // Focus on student tracking rather than item tracking
                    SetupColumns("Borrower ID", "Grade Level", "Items Held", "Date Borrowed", "Return Date");
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

            var damaged = _inventoryService.GetAllItems().Where(i => i.Condition == "Damaged").ToList();
            if (!damaged.Any())
                AddDashboardAlert("All systems operational.", Color.Teal);
            else
                foreach (var i in damaged) AddDashboardAlert($"REPAIR NEEDED: {i.Name} (#{i.Id})", Color.DarkRed);

            AddSectionHeader("RECENT ACTIVE LOANS");
            var activeLoans = _borrowService.GetAllBorrowRecords()
                .Where(b => b.Status == "Active")
                .OrderByDescending(b => b.BorrowDate).Take(5);

            foreach (var loan in activeLoans)
                AddDashboardAlert($"{loan.ItemName} borrowed by {loan.BorrowerId}", Color.DimGray, loan.Id);
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

        private void ProcessReturn(int recordId)
        {
            if (MessageBox.Show("Confirm return of this item?", "Return Process", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if (_borrowService.ReturnItem(recordId))
                {
                    LoadHomeContent();
                    UpdateDashboardCounts();
                }
            }
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

        private void AddDashboardAlert(string message, Color color, int? recordId = null)
        {
            Panel tile = new Panel { Size = new Size(flowRecentActivity.Width - 40, 60), BackColor = Color.White, Margin = new Padding(0, 0, 0, 10) };
            Label msg = new Label { Text = message, ForeColor = color, Location = new Point(15, 20), AutoSize = true, Font = new Font("Segoe UI", 10) };
            tile.Controls.Add(msg);

            if (recordId.HasValue)
            {
                Button btn = new Button { Text = "RETURN", Location = new Point(tile.Width - 100, 15), Size = new Size(80, 30), BackColor = Color.FromArgb(13, 71, 161), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                btn.Click += (s, e) => ProcessReturn(recordId.Value);
                tile.Controls.Add(btn);
            }
            flowRecentActivity.Controls.Add(tile);
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
            Label msg = new Label { Text = message, ForeColor = color, AutoSize = true, Margin = new Padding(10) };
            flowRecentActivity.Controls.Add(msg);
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

            // CRITICAL: Only update the items INSIDE the sidebar during the timer
            // Do NOT call RefreshLayout() here to prevent header/background glitching
            UpdateSidebarInternalUI();
        }

        private void RefreshLayout()
        {
            if (pnlMainContent == null || pnlSidebar == null || lblDashboardHeader == null) return;

            int rightMargin = 70;
            int contentStartX = 110;
            int contentWidth = pnlMainContent.Width;

            btnClearActivity.Size = new Size(110, 30);
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

            pnlHomeSummary.Bounds = pnlGridContainer.Bounds = pnlHistory.Bounds = new Rectangle(shiftedLocation, shiftedSize); ;
            dgvInventory.Anchor = dgvHistory.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;
            dgvInventory.Size = dgvHistory.Size = new Size(pnlGridContainer.Width - 10, pnlGridContainer.Height - 10);
            dgvInventory.Location = dgvHistory.Location = new Point(5, 5);

            // 3. INNER PANELS: Ensure grids and summaries are centered in the content area
            Size contentSize = new Size(pnlMainContent.Width - 40, pnlMainContent.Height - 120);
            pnlHomeSummary.Location = pnlGridContainer.Location = pnlHistory.Location = new Point(20, 80);
            pnlHomeSummary.Size = pnlGridContainer.Size = pnlHistory.Size = contentSize;

            // 4. FLOATING SIDEBAR: Positioned on top (Z-order)
            pnlSidebar.Location = new Point(0, 64);
            pnlSidebar.Height = this.Height - 64;

            // 5. CRUD BUTTONS: Fixed to the top-right of the form
            int btnWidth = 150;

            btnDelete.Location = new Point(this.Width - btnWidth - rightMargin, 30);
            btnEdit.Location = new Point(btnDelete.Left - btnWidth - 40, 30);
            btnCreate.Location = new Point(btnEdit.Left - btnWidth - 40, 30);

            // Add this near the bottom of RefreshLayout()
            // Inside RefreshLayout()
            if (pnlHomeSummary.Visible)
            {
                lblUrgentHeader.Location = new Point(20, 20);
                // Position the FlowPanel
                flowRecentActivity.Size = new Size(pnlHomeSummary.Width / 3, pnlHomeSummary.Height - 100);
                flowRecentActivity.Location = new Point(pnlHomeSummary.Width - flowRecentActivity.Width - 10, 80);

                // Position the Clear Button directly above it
                btnClearActivity.Size = new Size(100, 30);
                btnClearActivity.Location = new Point(flowRecentActivity.Right - btnClearActivity.Width, 45);
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
        }

        private void AddActivityCard(string message, DateTime time, Color statusColor)
        {
            // Create a small 'Activity Tile'
            Panel card = new Panel
            {
                Size = new Size(flowRecentActivity.Width - 25, 70),
                BackColor = Color.White,
                Margin = new Padding(5, 0, 5, 10)
            };

            // Colored indicator bar on the left
            Panel indicator = new Panel { Size = new Size(5, 70), BackColor = statusColor, Dock = DockStyle.Left };

            Label lblMsg = new Label
            {
                Text = message,
                Location = new Point(15, 12),
                AutoSize = true,
                Font = new Font("Segoe UI", 9F, FontStyle.Bold)
            };

            Label lblTime = new Label
            {
                Text = time.ToString("hh:mm tt | MMM dd"),
                Location = new Point(15, 35),
                AutoSize = true,
                ForeColor = Color.Gray,
                Font = new Font("Segoe UI", 8F)
            };

            card.Controls.Add(indicator);
            card.Controls.Add(lblMsg);
            card.Controls.Add(lblTime);
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

                // 2. Optional: If you want to delete them from the database/service:
                // _borrowService.ClearAllLogs(); 

                // 3. Refresh to show the "All systems operational" state
                LoadRecentActivity();
            }
        }
        #endregion
    }

    // ==========================================
    //  POPUP CLASSES (Moved Outside AdminDashboard)
    // ==========================================

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