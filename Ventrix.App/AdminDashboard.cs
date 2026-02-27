using MaterialSkin;
using MaterialSkin.Controls;
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
        // 1. Add private fields to store the services
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

            typeof(Panel).InvokeMember("DoubleBuffered",
        System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
        null, pnlMainContent, new object[] { true });


            // --- WIRE UP EVENTS ---
            // 1. CRUD Buttons
            btnCreate.Click += BtnCreate_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;

            // 2. Search & Filters
            txtSearch.TextChanged += (s, e) => LoadFromDatabase("All");

            // Home Button Logic
            btnHome.Click += (s, e) => SwitchView("Home");

            dgvInventory.CellDoubleClick += (s, e) =>
            {
                if (e.RowIndex < 0) return;

                // Get the ID and Name from the selected row
                int id = Convert.ToInt32(dgvInventory.Rows[e.RowIndex].Cells[0].Value);
                string name = dgvInventory.Rows[e.RowIndex].Cells[1].Value.ToString();
                string status = dgvInventory.Rows[e.RowIndex].Cells[3].Value.ToString();

                if (status == "Available")
                {
                    using (var popup = new BorrowPopup(id, name))
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
                    MessageBox.Show("This item is already borrowed or unavailable.");
                }
            };

            btnHistoryNav.Click += (s, e) => SwitchView("History");

            // Card Clicks - Now they hide the Summary and show the Grid
            cardTotal.Click += (s, e) => SwitchView("Inventory","All");
            cardAvailable.Click += (s, e) => SwitchView("Inventory", "Available");
            cardPending.Click += (s, e) => SwitchView("Inventory", "Borrowed");
            cardBorrowers.Click += (s, e) => SwitchView("Inventory", "Borrowers");
   
            // 4. Sidebar Animation
            sidebarTimer.Interval = 1;
            btnHamburger.Click += (s, e) => sidebarTimer.Start();
            sidebarTimer.Tick += SidebarTimer_Tick;

            // 5. Initial Load
            this.Load += (s, e) => {
                ApplyModernBranding();
                RefreshLayout();
                LoadFromDatabase("All");
            };
            this.Resize += (s, e) => RefreshLayout();
        }

        // ==========================================
        //         PART 1: NAVIGATION & VIEW LOGIC
        // ==========================================

        private void SwitchToGridView(string filter)
        {
            SwitchView("Inventory", filter);
        }

        private void SwitchView(string viewName, string filter = "All")
        {
            // Reset visibility
            pnlHomeSummary.Visible = false;
            pnlGridContainer.Visible = false;
            pnlHistory.Visible = false;

            if (viewName == "Inventory")
            {
                pnlGridContainer.Visible = true;
                pnlGridContainer.BringToFront(); // Force it to the top layer
                                                 // Update the header based on the card clicked
                lblDashboardHeader.Text = $"INVENTORY: {filter.ToUpper()}";

                LoadFromDatabase(filter);
            }
            else if (viewName == "Home")
            {
                pnlHomeSummary.Visible = true;
                ShowHomeDashboard();
            }

            // Show CRUD buttons only in Inventory view
            bool isInventoryView = (viewName == "Inventory");
            btnCreate.Visible = btnEdit.Visible = btnDelete.Visible = (viewName == "Inventory");

            switch (viewName)
            {
                case "Home":
                    pnlHomeSummary.Visible = true;
                    pnlHomeSummary.BringToFront();
                    ShowHomeDashboard();
                    break;
                case "Inventory":
                    pnlGridContainer.Visible = true;
                    LoadFromDatabase(filter);
                    break;
                case "History":
                    pnlHistory.Visible = true;
                    LoadHistoryData();
                    break;
            }
        }

        private void ShowHomeDashboard()
        {
            // Hide the standard inventory grid to make room for the dashboard
            pnlGridContainer.Visible = false;
            pnlHomeSummary.Visible = true;
            pnlHomeSummary.BringToFront();

            // Reset branding and headers
            lblDashboardHeader.Text = "SYSTEM EXECUTIVE SUMMARY";
            lblDashboardHeader.ForeColor = Color.FromArgb(13, 71, 161);

            // Clear search bar for a clean state
            txtSearch.Clear();
            LoadHomeContent();

            // Update Sidebar Metrics
            UpdateDashboardCounts();
        }

        // ==========================================
        //         PART 2: DASHBOARD & RETURN LOGIC
        // ==========================================

        private void LoadHomeContent()
        {
            flowHomeContent.Controls.Clear();
            AddSectionHeader("URGENT SYSTEM ALERTS");

            using (var db = new AppDbContext())
            {
                // Alerts
                var damaged = db.InventoryItems.Where(i => i.Condition == "Damaged").ToList();
                if (!damaged.Any()) AddDashboardAlert("All systems operational. No items need repair.", Color.Teal);
                else foreach (var item in damaged) AddDashboardAlert($"REPAIR NEEDED: {item.Name} (#{item.Id})", Color.FromArgb(192, 0, 0));

                AddSectionHeader("RECENT TRANSACTIONS");

                // Transactions
                var activeLoans = db.BorrowRecords.Where(b => b.Status == "Active").OrderByDescending(b => b.BorrowDate).Take(5).ToList();
                foreach (var loan in activeLoans)
                {
                    AddDashboardAlert($"BORROWED: {loan.ItemName} by Student {loan.BorrowerId}", Color.Gray, loan.Id);
                }
            }
        }

        private void AddDashboardAlert(string message, Color textColor, int? recordId = null)
        {
            Guna.UI2.WinForms.Guna2Panel alertTile = new Guna.UI2.WinForms.Guna2Panel
            {
                Size = new Size(flowHomeContent.Width - 50, 70),
                FillColor = Color.White,
                BorderRadius = 10,
                Margin = new Padding(0, 0, 0, 15),
                Padding = new Padding(15)
            };

            Label lbl = new Label
            {
                Text = message,
                ForeColor = textColor,
                AutoSize = true,
                Font = new Font("Segoe UI Semibold", 10.5F),
                Location = new Point(15, 22)
            };
            alertTile.Controls.Add(lbl);

            if (recordId.HasValue)
            {
                Guna.UI2.WinForms.Guna2Button btnReturn = new Guna.UI2.WinForms.Guna2Button
                {
                    Text = "RETURN",
                    Size = new Size(90, 35),
                    Location = new Point(alertTile.Width - 110, 17),
                    FillColor = Color.FromArgb(13, 71, 161),
                    BorderRadius = 5,
                    Anchor = AnchorStyles.Right
                };
                btnReturn.Click += (s, e) => ProcessReturn(recordId.Value);
                alertTile.Controls.Add(btnReturn);
            }
            flowHomeContent.Controls.Add(alertTile);
        }

        private void ProcessReturn(int recordId)
        {
            if (MessageBox.Show("Confirm return?", "Process", MessageBoxButtons.YesNo) != DialogResult.Yes) return;

            using (var db = new AppDbContext())
            {
                var record = db.BorrowRecords.Find(recordId);
                if (record == null) return;

                var item = db.InventoryItems.FirstOrDefault(i => i.Name == record.ItemName);
                if (item != null) item.Status = "Available";

                record.Status = "Returned";
                record.ReturnDate = DateTime.Now;
                db.SaveChanges();
            }
            LoadHomeContent();
            UpdateDashboardCounts();
        }

        private void LoadHistoryData()
        {
            dgvHistory.Rows.Clear();
            dgvHistory.Columns.Clear();
            dgvHistory.Columns.Add("ID", "ID");
            dgvHistory.Columns.Add("Item", "Item Name");
            dgvHistory.Columns.Add("Borrower", "Borrower");
            dgvHistory.Columns.Add("BDate", "Borrowed");
            dgvHistory.Columns.Add("RDate", "Returned");

            using (var db = new AppDbContext())
            {
                var logs = db.BorrowRecords.Where(b => b.Status == "Returned").OrderByDescending(b => b.ReturnDate).ToList();
                foreach (var log in logs) dgvHistory.Rows.Add(log.Id, log.ItemName, log.BorrowerId, log.BorrowDate.ToShortDateString(), log.ReturnDate?.ToShortDateString());
            }
            lblDashboardHeader.Text = "TRANSACTION AUDIT HISTORY";
        }
        // ==========================================
        //         PART 3: EXISTING CRUD & UI
        // ==========================================

        private void LoadFromDatabase(string statusFilter)
        {
            dgvInventory.Rows.Clear();
            dgvInventory.Columns.Clear(); // Clear old columns first

            using (var db = new AppDbContext())
            {
                var query = db.InventoryItems.AsQueryable();

                // Apply Search
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    string search = txtSearch.Text.ToLower();
                    query = query.Where(i => i.Name.ToLower().Contains(search));
                }

                switch (statusFilter)
                {
                    case "Available":
                        // Specific columns for Available items
                        SetupColumns("ID", "Item Name", "Category", "Condition");
                        foreach (var item in query.Where(i => i.Status == "Available").ToList())
                        {
                            dgvInventory.Rows.Add(item.Id, item.Name, item.Category, item.Condition);
                        }
                        break;

                    case "Borrowed":
                        // Specific columns for Borrowed items (Showing who has it)
                        SetupColumns("ID", "Item Name", "Quantity", "Borrower", "Due Date");

                        // We join with BorrowRecords to get the BorrowerId
                        var borrowedList = db.BorrowRecords
                            .Where(b => b.Status == "Active")
                            .ToList();

                        foreach (var record in borrowedList)
                        {
                            dgvInventory.Rows.Add(record.Id, record.ItemName, record.BorrowerId, record.BorrowDate.ToShortDateString());
                        }
                        break;
                    case "Borrower List":
                        // Show a list of borrowers and their borrowed items
                        SetupColumns("Borrower", "Item Name", "Quantity","Borrow Date", "Grade Level", "Subject/Purpose");
                        var borrowerList = db.BorrowRecords
                            .Where(b => b.Status == "Active")
                            .ToList();
                        foreach (var record in borrowerList)
                        {
                            dgvInventory.Rows.Add(record.BorrowerId, record.ItemName, record.BorrowDate.ToShortDateString(), record.GradeLevel);
                        }
                        break;

                    default: // "All"
                        SetupColumns("ID", "Item Name", "Category", "Status", "Condition");
                        foreach (var item in query.ToList())
                        {
                            dgvInventory.Rows.Add(item.Id, item.Name, item.Category, item.Status, item.Condition);
                        }
                        break;
                }
            }
        }
        private void BtnCreate_Click(object sender, EventArgs e)
        {
            using (var popup = new InventoryPopup()) // No ID passed = Add Mode
            {
                if (popup.ShowDialog() == DialogResult.OK)
                {
                    LoadFromDatabase("All");
                    UpdateDashboardCounts();
                    MessageBox.Show("Item added successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }
        private void BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvInventory.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select an item to edit.");
                return;
            }

            if (int.TryParse(dgvInventory.SelectedRows[0].Cells[0].Value.ToString(), out int selectedId))
            {
                using (var popup = new InventoryPopup(selectedId)) // ID passed = Edit Mode
                {
                    if (popup.ShowDialog() == DialogResult.OK)
                    {
                        LoadFromDatabase("All");
                        UpdateDashboardCounts();
                    }
                }
            }
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvInventory.SelectedRows.Count == 0) return;

            if (int.TryParse(dgvInventory.SelectedRows[0].Cells[0].Value.ToString(), out int selectedId))
            {
                var confirm = MessageBox.Show($"Are you sure you want to delete Item #{selectedId}?", "Confirm Delete", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (confirm == DialogResult.Yes)
                {
                    using (var db = new AppDbContext())
                    {
                        var item = db.InventoryItems.FirstOrDefault(i => i.Id == selectedId);
                        if (item != null)
                        {
                            db.InventoryItems.Remove(item);
                            db.SaveChanges();
                            LoadFromDatabase("All");
                            UpdateDashboardCounts();
                        }
                    }
                }
            }
        }
        private void UpdateDashboardCounts()
        {
            using (var db = new AppDbContext())
            {
                int total = db.InventoryItems.Count();
                int damaged = db.InventoryItems.Count(x => x.Condition == "Damaged");
                int borrowed = db.InventoryItems.Count(x => x.Status == "Borrowed");

                lblTotalCount.Text = total.ToString("N0");
                lblAvailCount.Text = db.InventoryItems.Count(x => x.Status == "Available").ToString("N0");
                lblPendingCount.Text = borrowed.ToString("N0");

                // PROACTIVE ALERT: Change color if something is damaged or overdue
                if (damaged > 0)
                {
                    lblDashboardHeader.Text = $"SYSTEM ALERT: {damaged} ITEMS NEED REPAIR";
                    lblDashboardHeader.ForeColor = Color.FromArgb(192, 0, 0); // Warning Red
                }
            }
        }

        private void AddSectionHeader(string title)
        {
            Label lblHeader = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 12F, FontStyle.Bold),
                ForeColor = Color.FromArgb(13, 71, 161),
                AutoSize = true,
                Margin = new Padding(0, 20, 0, 10)
            };
            flowHomeContent.Controls.Add(lblHeader);
        }

        // ==========================================
        //        PART 2: UI & STYLING LOGIC
        // ==========================================

        private void SetupColumns(params string[] columnNames)
        {
            foreach (var name in columnNames) dgvInventory.Columns.Add(name.Replace(" ", ""), name);
        }

        private void InitializeMaterialSkin()
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(Color.FromArgb(13, 71, 161), Color.FromArgb(10, 50, 120), Color.FromArgb(33, 150, 243), Color.FromArgb(30, 136, 229), TextShade.WHITE);
        }

        private void ApplyModernBranding()
        {
            lblDashboardHeader.Text = "INVENTORY OVERVIEW";
            lblDashboardHeader.Font = new Font("Sitka Heading", 22F, FontStyle.Bold);
            lblDashboardHeader.ForeColor = Color.FromArgb(13, 71, 161);

            // Setup Cards with Placeholders (UpdateDashboardCounts will fill numbers)
            SetupCard(cardTotal, lblTotalTitle, lblTotalCount, "TOTAL ITEMS", "0", Color.FromArgb(13, 71, 161));
            SetupCard(cardAvailable, lblAvailTitle, lblAvailCount, "AVAILABLE", "0", Color.Teal);
            SetupCard(cardPending, lblPendingTitle, lblPendingCount, "BORROWED", "0", Color.FromArgb(192, 0, 0));
            SetupCard(cardBorrowers, lblBorrowersTitle, lblBorrowersCount, "BORROWERS LIST", "0", Color.Orange);

            StyleNavButton(btnHistoryNav, "HISTORY", Color.Orange);
            StyleNavButton(btnHome, "HOME PAGE", Color.FromArgb(33, 150, 243));
            StyleNavButton(btnCreate, "ADD ITEM", Color.Teal);
            StyleNavButton(btnEdit, "EDIT RECORD ", Color.FromArgb(33, 150, 243));
            StyleNavButton(btnDelete, "DELETE ITEM ", Color.FromArgb(192, 0, 0));

            btnCreate.Visible = btnEdit.Visible = btnDelete.Visible = true;

            UpdateDashboardCounts();
        }

        // --- FIXED SETUP CARD (Prevents Text Overlap) ---
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

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            pnlMainContent.SuspendLayout();
            pnlSidebar.SuspendLayout();

            int animationSpeed = 40;
            if (isSidebarExpanded)
            {
                pnlSidebar.Width -= animationSpeed;
                if (pnlSidebar.Width <= sidebarMinWidth)
                {
                    pnlSidebar.Width = sidebarMinWidth;
                    isSidebarExpanded = false;
                    sidebarTimer.Stop();
                    ToggleSidebarText(false);
                }
            }
            else
            {
                pnlSidebar.Width += animationSpeed;
                if (pnlSidebar.Width >= sidebarMaxWidth)
                {
                    pnlSidebar.Width = sidebarMaxWidth;
                    isSidebarExpanded = true;
                    sidebarTimer.Stop();
                    ToggleSidebarText(true);
                }
            }
            RefreshLayout(); // This will maintain the buttons on the right of pnlMainContent

            pnlSidebar.ResumeLayout();
            pnlMainContent.ResumeLayout();
        }

        private void ToggleSidebarText(bool show)
        {
        }

        private void RefreshLayout()
        {
            if (pnlMainContent == null) return;

            // 1. Header and Search Positioning
            lblDashboardHeader.Location = new Point(70, 20); //
            txtSearch.Location = new Point(pnlTopBar.Width - txtSearch.Width - 30, 20);
            // Positioning Panels
            pnlHomeSummary.Location = pnlGridContainer.Location = pnlHistory.Location = new Point(20, 100);
            Size contentSize = new Size(pnlMainContent.Width - 40, pnlMainContent.Height - 130);
            pnlHomeSummary.Size = pnlGridContainer.Size = pnlHistory.Size = contentSize;

            // 2. Button Positioning (Aligned to the Right Side of Main Content)
            int btnWidth = 160;
            int btnHeight = 45;
            int btnSpacing = 15;
            int rightMargin = pnlMainContent.Width - 20;

            btnCreate.Parent = btnEdit.Parent = btnDelete.Parent = pnlMainContent;
            btnCreate.Size = btnEdit.Size = btnDelete.Size = new Size(btnWidth, btnHeight);

            // Positioned from right to left
            btnDelete.Location = new Point(rightMargin - btnWidth, 30);
            btnEdit.Location = new Point(btnDelete.Left - btnWidth - btnSpacing, 30);
            btnCreate.Location = new Point(btnEdit.Left - btnWidth - btnSpacing, 30);

            //Card Positioning (Moved to the Sidebar)
            int sidebarContentWidth = pnlSidebar.Width - 20;

            // Position Home Button above cards
            btnHome.Parent = pnlSidebar;
            btnHome.Size = new Size(sidebarContentWidth, 45);
            btnHome.Location = new Point(10, 90); // Positioned between profile and first card
            btnHome.Visible = isSidebarExpanded;

            // POSITION HISTORY BUTTON UNDER HOME PAGE
            btnHistoryNav.Parent = pnlSidebar;
            btnHistoryNav.Size = new Size(sidebarContentWidth, 45);
            btnHistoryNav.Location = new Point(10, 140); // 50 pixels below btnHome (45 height + 5 gap)
            btnHistoryNav.Visible = isSidebarExpanded;
            dgvHistory.Dock = DockStyle.Fill;

            Size cardSidebarSize = new Size(sidebarContentWidth, 110);
            cardTotal.Parent = cardAvailable.Parent = cardPending.Parent = cardBorrowers.Parent = pnlSidebar;

            // Offset cards further down to make room for btnHome

            cardTotal.Location = new Point(10, 150);
            cardAvailable.Location = new Point(10, 270);
            cardPending.Location = new Point(10, 390);
            cardBorrowers.Location = new Point(10, 510);

            cardTotal.Size = cardAvailable.Size = cardPending.Size = cardBorrowers.Size = cardSidebarSize;

            // Transition the Metric Cards (cardTotal, cardAvailable, etc.)
            int cardYOffset = 200; // Start cards below btnHome
            cardTotal.Location = new Point(10, cardYOffset);
            cardAvailable.Location = new Point(10, cardYOffset + 120);
            cardPending.Location = new Point(10, cardYOffset + 240);
            cardBorrowers.Location = new Point(10, cardYOffset + 360);

            // Sync labels visibility with the sidebar animation state
            bool showLabels = isSidebarExpanded;
            lblTotalTitle.Visible = lblTotalCount.Visible = showLabels;
            lblAvailTitle.Visible = lblAvailCount.Visible = showLabels;
            lblPendingTitle.Visible = lblPendingCount.Visible = showLabels;
            lblBorrowersTitle.Visible = lblBorrowersCount.Visible = showLabels;

            pnlHomeSummary.Width = pnlMainContent.Width - 40;
            pnlHomeSummary.Height = pnlMainContent.Height - 130;

            //User Profile Section
            picUser.Location = new Point(isSidebarExpanded ? 15 : 12, 25); //
            picUser.Size = isSidebarExpanded ? new Size(45, 45) : new Size(40, 40); //
            lblOwnerRole.Visible = isSidebarExpanded; //
            cmbAccountActions.Visible = isSidebarExpanded; //

            if (isSidebarExpanded)
            {
                lblOwnerRole.Location = new Point(70, 25); //
                cmbAccountActions.Location = new Point(65, 42); //
                cmbAccountActions.Size = new Size(160, 30); //
            }
        }
    }

    // ==========================================
    //    PART 4: POPUP FORM CLASS (Add/Edit)
    // ==========================================
    public class InventoryPopup : Form
    {
        private TextBox txtName = new TextBox();
        private ComboBox cmbCategory = new ComboBox();
        private ComboBox cmbStatus = new ComboBox();
        private ComboBox cmbCondition = new ComboBox();
        private Button btnSave = new Button();
        private int? _editId = null;

        public InventoryPopup(int? idToEdit = null)
        {
            _editId = idToEdit;
            InitializePopup();
            if (_editId.HasValue) LoadDataForEdit();
        }

        private void InitializePopup()
        {
            this.Size = new Size(400, 450);
            this.Text = _editId.HasValue ? "Edit Item" : "Add New Item";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            int y = 20;
            AddControl("Item Name:", txtName, ref y);
            AddControl("Category:", cmbCategory, ref y);
            AddControl("Status:", cmbStatus, ref y);
            AddControl("Condition:", cmbCondition, ref y);

            // Populate Dropdowns
            cmbCategory.Items.AddRange(new object[] { "Hardware", "Device", "Accessory", "Consumable" });
            cmbStatus.Items.AddRange(new object[] { "Available", "Borrowed", "Maintenance", "Lost" });
            cmbCondition.Items.AddRange(new object[] { "New", "Good", "Fair", "Damaged" });
            cmbCategory.SelectedIndex = 0; cmbStatus.SelectedIndex = 0; cmbCondition.SelectedIndex = 0;

            btnSave.Text = "SAVE ITEM";
            btnSave.BackColor = Color.FromArgb(13, 71, 161);
            btnSave.ForeColor = Color.White;
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Size = new Size(340, 45);
            btnSave.Location = new Point(20, y + 20);
            btnSave.Click += BtnSave_Click;
            this.Controls.Add(btnSave);
        }

        private void AddControl(string labelText, Control input, ref int y)
        {
            Label lbl = new Label { Text = labelText, Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            this.Controls.Add(lbl);
            input.Location = new Point(20, y + 25);
            input.Size = new Size(340, 30);
            input.Font = new Font("Segoe UI", 10);
            this.Controls.Add(input);
            y += 70;
        }

        private void LoadDataForEdit()
        {
            using (var db = new AppDbContext())
            {
                var item = db.InventoryItems.Find(_editId.Value);
                if (item != null)
                {
                    txtName.Text = item.Name;
                    cmbCategory.Text = item.Category;
                    cmbStatus.Text = item.Status;
                    cmbCondition.Text = item.Condition;
                }
            }
        }

        private void LoadRecentActivity()
        {
            using (var db = new AppDbContext())
            {
                // Pull the 5 most recent borrow transactions from the database
                var recentActions = db.BorrowRecords
                    .OrderByDescending(b => b.BorrowDate)
                    .Take(5)
                    .ToList();

                if (recentActions.Any())
                {
                    // In a real app, you would populate a small ListBox or FlowLayoutPanel
                    // showing "Student A borrowed Laptop X".
                    Console.WriteLine("Recent Activity Loaded");
                }
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text)) { MessageBox.Show("Name is required!"); return; }

            using (var db = new AppDbContext())
            {
                if (_editId.HasValue) // UPDATE
                {
                    var item = db.InventoryItems.Find(_editId.Value);
                    if (item != null)
                    {
                        item.Name = txtName.Text;
                        item.Category = cmbCategory.Text;
                        item.Status = cmbStatus.Text;
                        item.Condition = cmbCondition.Text;
                    }
                }
                else // CREATE
                {
                    var newItem = new InventoryItem
                    {
                        Name = txtName.Text,
                        Category = cmbCategory.Text,
                        Status = cmbStatus.Text,
                        Condition = cmbCondition.Text,
                        DateAdded = DateTime.Now
                    };
                    db.InventoryItems.Add(newItem);
                }
                db.SaveChanges();
            }
            this.DialogResult = DialogResult.OK;
        }
    }

    public class BorrowPopup : Form
    {
        private Label lblItemName = new Label();
        private TextBox txtBorrowerId = new TextBox();
        private TextBox txtPurpose = new TextBox();
        private ComboBox cmbGrade = new ComboBox();
        private Button btnConfirm = new Button();
        private int _itemId;
        private string _itemName;

        public BorrowPopup(int itemId, string itemName)
        {
            _itemId = itemId;
            _itemName = itemName;
            InitializeBorrowPopup();
        }

        private void InitializeBorrowPopup()
        {
            this.Size = new Size(400, 400);
            this.Text = "Borrow Item Transaction";
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;

            int y = 20;
            lblItemName.Text = $"Item: {_itemName}";
            lblItemName.Font = new Font("Segoe UI", 12, FontStyle.Bold);
            lblItemName.Location = new Point(20, y);
            lblItemName.AutoSize = true;
            this.Controls.Add(lblItemName);

            y += 50;
            AddField("Student ID / Borrower ID:", txtBorrowerId, ref y);
            AddField("Grade/Section:", cmbGrade, ref y);
            AddField("Purpose (Subject/Project):", txtPurpose, ref y);

            cmbGrade.Items.AddRange(new object[] { "Grade 7", "Grade 8", "Grade 9", "Grade 10", "Grade 11", "Grade 12", "College" });
            cmbGrade.SelectedIndex = 0;

            btnConfirm.Text = "CONFIRM BORROW";
            btnConfirm.BackColor = Color.FromArgb(13, 71, 161);
            btnConfirm.ForeColor = Color.White;
            btnConfirm.FlatStyle = FlatStyle.Flat;
            btnConfirm.Size = new Size(340, 45);
            btnConfirm.Location = new Point(20, y + 20);
            btnConfirm.Click += BtnConfirm_Click;
            this.Controls.Add(btnConfirm);
        }

        private void AddField(string label, Control input, ref int y)
        {
            Label l = new Label { Text = label, Location = new Point(20, y), AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            this.Controls.Add(l);
            input.Location = new Point(20, y + 25);
            input.Size = new Size(340, 30);
            this.Controls.Add(input);
            y += 70;
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBorrowerId.Text)) { MessageBox.Show("Borrower ID is required!"); return; }

            using (var db = new AppDbContext())
            {
                // 1. Update the Inventory Item Status
                var item = db.InventoryItems.Find(_itemId);
                if (item != null)
                {
                    item.Status = "Borrowed";
                }

                // 2. Create the Borrow Record
                var record = new BorrowRecord
                {
                    BorrowerId = txtBorrowerId.Text,
                    ItemName = _itemName,
                    Purpose = txtPurpose.Text,
                    GradeLevel = cmbGrade.Text,
                    BorrowDate = DateTime.Now,
                    Status = "Active"
                };

                db.BorrowRecords.Add(record);
                db.SaveChanges();
            }

            this.DialogResult = DialogResult.OK;
        }
    }
}