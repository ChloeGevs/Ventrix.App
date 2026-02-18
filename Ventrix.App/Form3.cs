using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Ventrix.Infrastructure; 
using Ventrix.Domain.Models;         

namespace Ventrix.App
{
    public partial class Form3 : MaterialForm
    {
        private bool isSidebarExpanded = true;
        private const int sidebarMaxWidth = 240;
        private const int sidebarMinWidth = 70;

        public Form3()
        {
            InitializeComponent();
            InitializeMaterialSkin();

            // --- WIRE UP EVENTS ---
            // 1. CRUD Buttons
            btnCreate.Click += BtnCreate_Click;
            btnEdit.Click += BtnEdit_Click;
            btnDelete.Click += BtnDelete_Click;

            // 2. Search & Filters
            txtSearch.TextChanged += (s, e) => LoadFromDatabase("All");

            // 3. Card Clicks (Filter the grid)
            cardTotal.Click += (s, e) => LoadFromDatabase("All");
            cardAvailable.Click += (s, e) => LoadFromDatabase("Available");
            cardPending.Click += (s, e) => LoadFromDatabase("Borrowed");
            cardBorrowers.Click += (s, e) => LoadFromDatabase("Borrowers");

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
        //        PART 1: CRUD LOGIC (DATABASE)
        // ==========================================

        // --- CREATE: Opens the Popup to Add a New Item ---
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

        // --- READ: Loads Data from SQLite into Grid ---
        private void LoadFromDatabase(string statusFilter)
        {
            dgvInventory.Rows.Clear();
            dgvInventory.Columns.Clear();
            dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            lblDashboardHeader.Text = $"INVENTORY: {statusFilter.ToUpper()}";

            using (var db = new AppDbContext())
            {
                var query = db.InventoryItems.AsQueryable();

                // Apply Search if text exists
                if (!string.IsNullOrEmpty(txtSearch.Text))
                {
                    string search = txtSearch.Text.ToLower();
                    query = query.Where(i => i.Name.ToLower().Contains(search) || i.Category.ToLower().Contains(search));
                }

                // Apply Status Filter
                switch (statusFilter)
                {
                    case "All":
                        SetupColumns("ID", "Name", "Category", "Status", "Condition");
                        foreach (var item in query.ToList())
                        {
                            dgvInventory.Rows.Add(item.Id, item.Name, item.Category, item.Status, item.Condition);
                        }
                        break;

                    case "Available":
                        SetupColumns("ID", "Name", "Category", "Condition");
                        foreach (var item in query.Where(i => i.Status == "Available").ToList())
                        {
                            dgvInventory.Rows.Add(item.Id, item.Name, item.Category, item.Condition);
                        }
                        break;

                    case "Borrowed":
                        SetupColumns("ID", "Name", "Borrower", "Due Date");
                        // Note: If you don't have Borrower columns in DB yet, this is just a placeholder
                        foreach (var item in query.Where(i => i.Status == "Borrowed").ToList())
                        {
                            dgvInventory.Rows.Add(item.Id, item.Name, "Student #" + item.Id, DateTime.Now.AddDays(3).ToShortDateString());
                        }
                        break;
                }
            }
        }

        // --- UPDATE: Opens the Popup to Edit Selected Item ---
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

        // --- DELETE: Removes Item from SQLite ---
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
                // Real-time counts from database
                lblTotalCount.Text = db.InventoryItems.Count().ToString("N0");
                lblAvailCount.Text = db.InventoryItems.Count(x => x.Status == "Available").ToString("N0");
                lblPendingCount.Text = db.InventoryItems.Count(x => x.Status == "Borrowed").ToString("N0");
                // Placeholder for borrowers count until you have a Users/Borrowers table
                lblBorrowersCount.Text = "...";
            }
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

            StyleNavButton(btnCreate, "ADD ITEM", Color.Teal);
            StyleNavButton(btnEdit, "EDIT RECORD", Color.FromArgb(33, 150, 243));
            StyleNavButton(btnDelete, "DELETE ITEM", Color.FromArgb(192, 0, 0));

            UpdateDashboardCounts();
        }

        // --- FIXED SETUP CARD (Prevents Text Overlap) ---
        private void SetupCard(Guna.UI2.WinForms.Guna2Panel card, Guna.UI2.WinForms.Guna2HtmlLabel title, Guna.UI2.WinForms.Guna2HtmlLabel count, string titleText, string countText, Color accentColor)
        {
            title.Parent = card;
            count.Parent = card;
            card.FillColor = Color.White;
            card.BorderRadius = 15;
            card.ShadowDecoration.Enabled = true;
            card.ShadowDecoration.Shadow = new Padding(10);
            card.Cursor = Cursors.Hand;

            title.Text = titleText;
            title.Font = new Font("Segoe UI Semibold", 9F);
            title.ForeColor = Color.Gray;
            title.BackColor = Color.Transparent;
            // FIXED LOCATION:
            title.Location = new Point(20, 15);

            count.Text = countText;
            count.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
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
            btn.TextAlign = HorizontalAlignment.Left;
            btn.TextOffset = new Point(10, 0);
            btn.BorderRadius = 10;
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            int animationSpeed = 40;
            if (isSidebarExpanded)
            {
                pnlSidebar.Width -= animationSpeed;
                if (pnlSidebar.Width <= sidebarMinWidth) { pnlSidebar.Width = sidebarMinWidth; isSidebarExpanded = false; sidebarTimer.Stop(); ToggleSidebarText(false); }
            }
            else
            {
                pnlSidebar.Width += animationSpeed;
                if (pnlSidebar.Width >= sidebarMaxWidth) { pnlSidebar.Width = sidebarMaxWidth; isSidebarExpanded = true; sidebarTimer.Stop(); ToggleSidebarText(true); }
            }
            RefreshLayout();
        }

        private void ToggleSidebarText(bool show)
        {
            btnCreate.Text = show ? "ADD ITEM" : "";
            btnEdit.Text = show ? "EDIT RECORD" : "";
            btnDelete.Text = show ? "DELETE ITEM" : "";
        }

        private void RefreshLayout()
        {
            if (pnlMainContent == null) return;
            lblDashboardHeader.Location = new Point(70, 20);
            txtSearch.Location = new Point(pnlTopBar.Width - txtSearch.Width - 30, 20);

            int spacing = 20;
            int totalSpacing = spacing * 5;
            int cardWidth = (pnlMainContent.Width - totalSpacing) / 4;
            Size cardSize = new Size(cardWidth, 130);
            cardTotal.Size = cardAvailable.Size = cardPending.Size = cardBorrowers.Size = cardSize;

            cardTotal.Location = new Point(20, 30);
            cardAvailable.Location = new Point(cardTotal.Right + spacing, 30);
            cardPending.Location = new Point(cardAvailable.Right + spacing, 30);
            cardBorrowers.Location = new Point(cardPending.Right + spacing, 30);

            pnlGridContainer.Location = new Point(20, 180);
            pnlGridContainer.Width = pnlMainContent.Width - 40;
            pnlGridContainer.Height = pnlMainContent.Height - 210;

            int btnWidth = pnlSidebar.Width - 20;
            btnCreate.Size = btnEdit.Size = btnDelete.Size = new Size(btnWidth, 50);
            btnCreate.Location = new Point(10, 120);
            btnEdit.Location = new Point(10, 180);
            btnDelete.Location = new Point(10, 240);

            picUser.Location = new Point(isSidebarExpanded ? 15 : 12, 25);
            picUser.Size = isSidebarExpanded ? new Size(45, 45) : new Size(40, 40);
            lblOwnerRole.Visible = isSidebarExpanded;
            cmbAccountActions.Visible = isSidebarExpanded;
            if (isSidebarExpanded) { lblOwnerRole.Location = new Point(70, 25); cmbAccountActions.Location = new Point(65, 42); cmbAccountActions.Size = new Size(160, 30); }
        }
    }

    // ==========================================
    //    PART 3: POPUP FORM CLASS (Add/Edit)
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
}