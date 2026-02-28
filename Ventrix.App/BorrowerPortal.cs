using System;
using System.Linq; // Needed for database queries
using System.Windows.Forms;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using Ventrix.Infrastructure;

namespace Ventrix.App
{
    public partial class BorrowerPortal : Form
    {
        private readonly InventoryService _inventoryService;
        private readonly BorrowService _borrowService;
        private readonly UserService _userService;

        public BorrowerPortal(InventoryService invService, BorrowService borrowService, UserService userService)
        {
            _inventoryService = invService;
            _borrowService = borrowService;
            _userService = userService;
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
            lblCreateAccount.Click += LblCreateAccount_Click;

            // Initial State
            ToggleMode("Student");
            LoadEquipmentList();
        }

        private void LoadEquipmentList()
        {
            cmbListEquipments.Items.Clear();
            var items = _inventoryService.GetFilteredInventory("", "Available")
                                         .Select(i => i.Name).Distinct().ToArray();
            cmbListEquipments.Items.AddRange(items);
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

                txtStudentId.PlaceholderText = "Student/Faculty ID Number";
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
            try
            {
                var user = _userService.Login(txtStudentId.Text, txtPassword.Text);

                if (user != null)
                {
                    if (user.Role == "Admin" || user.Role == "Faculty")
                    {
                        // Open Admin Dashboard and pass required services
                        AdminDashboard dashboard = new AdminDashboard(_inventoryService, _borrowService);
                        dashboard.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show($"Welcome, {user.FirstName}! Student portal features coming soon.");
                    }
                }
                else
                {
                    MessageBox.Show("Invalid Credentials. Please try again.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}");
            }
        }

        private void BtnBorrow_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtStudentId.Text) || cmbListEquipments.SelectedIndex == -1)
            {
                MessageBox.Show("Please enter Student ID and select an item.");
                return;
            }

            // 1. Create the Domain Model
            var record = new BorrowRecord
            {
                BorrowerId = txtStudentId.Text,
                ItemName = cmbListEquipments.Text,
                Quantity = (int)numQuantity.Value,
                Purpose = txtSubject.Text,
                GradeLevel = cmbGradeLevel.Text,
                Status = "Active"
            };

            try
            {
                // 2. Use the Service instead of direct DB context
                // You need to find the item ID first, or update your service 
                // to accept the name. Assuming a helper method or search:
                var items = _inventoryService.GetFilteredInventory(record.ItemName, "Available");
                var itemToBorrow = items.FirstOrDefault();

                if (itemToBorrow != null)
                {
                    _borrowService.ProcessBorrow(record, itemToBorrow.Id);
                    MessageBox.Show("Item Borrowed Successfully!");
                    LoadEquipmentList(); // Refresh the list
                }
                else
                {
                    MessageBox.Show("Item is no longer available.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error borrowing item: {ex.Message}");
            }
        }

        private void LblCreateAccount_Click(object sender, EventArgs e)
        {
            BorrowerRegistration registrationForm = new BorrowerRegistration(_inventoryService, _borrowService, _userService);
            registrationForm.Show();
            this.Hide();
        }
    }
   }
