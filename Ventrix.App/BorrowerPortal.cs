using System;
using System.Linq; // Needed for database queries
using System.Windows.Forms;
using Ventrix.Domain.Models;
using Ventrix.Infrastructure;

namespace Ventrix.App
{
    public partial class BorrowerPortal : Form
    {
        public BorrowerPortal()
        {
            InitializeComponent();
            SetupEvents();
        }

        private void SetupEvents()
        {
            // Toggles
            btnStaffToggle.Click += (s, e) => ToggleMode("Staff");
            btnStudentToggle.Click += (s, e) => ToggleMode("Student");

            // Actions
            btnLogin.Click += BtnLogin_Click;
            btnBorrow.Click += BtnBorrow_Click;

            // Initial State
            ToggleMode("Student");
            LoadEquipmentList();
        }

        private void LoadEquipmentList()
        {
            // Load items from DB into ComboBox
            using (var db = new AppDbContext())
            {
                var items = db.InventoryItems
                              .Where(i => i.Status == "Available")
                              .Select(i => i.Name)
                              .Distinct() // remove duplicates
                              .ToArray();
                cmbListEquipments.Items.AddRange(items);
            }
        }

        private void ToggleMode(string mode)
        {
            if (mode == "Staff")
            {
                // Show Login UI, Hide Borrow UI
                txtPassword.Visible = true;
                btnLogin.Visible = true;

                txtStudentId.PlaceholderText = "Username / Admin ID";
                cmbListEquipments.Visible = false;
                numQuantity.Visible = false;
                txtSubject.Visible = false;
                cmbGradeLevel.Visible = false;
                btnBorrow.Visible = false;
                btnReturn.Visible = false;
                lblQuantity.Visible = false;
                lblSubject.Visible = false;
                lblCreateAccount.Visible = false;

                // Colors
                btnStaffToggle.FillColor = System.Drawing.Color.FromArgb(13, 71, 161);
                btnStudentToggle.FillColor = System.Drawing.Color.Gray;
            }
            else
            {
                // Show Borrow UI, Hide Login UI
                txtPassword.Visible = false;
                btnLogin.Visible = false;

                txtStudentId.PlaceholderText = "Student ID Number";
                cmbListEquipments.Visible = true;
                numQuantity.Visible = true;
                txtSubject.Visible = true;
                cmbGradeLevel.Visible = true;
                btnBorrow.Visible = true;
                btnReturn.Visible = true;
                lblCreateAccount.Visible = true;
                lblQuantity.Visible = true;
                lblSubject.Visible = true;

                // Colors
                btnStudentToggle.FillColor = System.Drawing.Color.FromArgb(13, 71, 161);
                btnStaffToggle.FillColor = System.Drawing.Color.Gray;
            }
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            using (var db = new AppDbContext())
            {
                var admin = db.Users.FirstOrDefault(u => u.UserId == txtStudentId.Text && u.Password == txtPassword.Text);

                if (admin != null)
                {
                    // Open Dashboard
                    AdminDashboard dashboard = new AdminDashboard();
                    dashboard.Show();
                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Invalid Admin Credentials.");
                }
            }
        }

        private void BtnBorrow_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtStudentId.Text) || cmbListEquipments.SelectedIndex == -1)
            {
                MessageBox.Show("Please enter Student ID and select an item.");
                return;
            }

            using (var db = new AppDbContext())
            {
                // 1. Create Record
                var record = new BorrowRecord
                {
                    BorrowerId = txtStudentId.Text,
                    ItemName = cmbListEquipments.Text,
                    Quantity = (int)numQuantity.Value,
                    Purpose = txtSubject.Text,
                    GradeLevel = cmbGradeLevel.Text,
                    Status = "Active"
                };

                db.BorrowRecords.Add(record);

                // 2. Update Inventory Item status (Simplified logic)
                var item = db.InventoryItems.FirstOrDefault(i => i.Name == record.ItemName && i.Status == "Available");
                if (item != null)
                {
                    item.Status = "Borrowed";
                }

                db.SaveChanges();
                MessageBox.Show("Item Borrowed Successfully!");
            }
        }

    }
}