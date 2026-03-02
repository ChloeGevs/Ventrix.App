using System;
using System.Linq; 
using System.Windows.Forms;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using Ventrix.Infrastructure;
using System.Threading.Tasks;

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
            btnLogin.Click += async (s, e) => await BtnLogin_Click(s, e);
            btnBorrow.Click += async (s, e) => await BtnBorrow_Click(s, e);
            lblCreateAccount.Click += LblCreateAccount_Click;

            // Initial State
            ToggleMode("Student");

            // FIX: Seed the admin account into the database right when the app loads
            Load += async (s, e) =>
            {
                await _userService.InitializeDefaultAdminAsync(); // This creates "admin" / "admin123"
                await LoadEquipmentListAsync();
            };
        }

        private async Task LoadEquipmentListAsync()
        {
            cmbListEquipments.Items.Clear();
            // FIX: Corrected parameter order (Status first, then search term)
            var items = await _inventoryService.GetFilteredInventoryAsync("Available", ""); 
            var names = items.Select(i => i.Name).Distinct().ToArray();
            cmbListEquipments.Items.AddRange(names);
        }

        private void ToggleMode(string mode)
        {
            if (mode == "Staff")
            {
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

                btnStaffToggle.FillColor = System.Drawing.Color.FromArgb(13, 71, 161);
                btnStudentToggle.FillColor = System.Drawing.Color.Gray;
            }
            else
            {
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

                btnStudentToggle.FillColor = System.Drawing.Color.FromArgb(13, 71, 161);
                btnStaffToggle.FillColor = System.Drawing.Color.Gray;
            }
        }

        private async Task BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Package the UI text into the DTO
                var loginDto = new Ventrix.Application.DTOs.LoginDto
                {
                    UserId = txtStudentId.Text,
                    Password = txtPassword.Text
                };

                // 2. Pass the DTO to the service
                var user = await _userService.LoginAsync(loginDto);

                if (user != null)
                {
                    if (user.Role == UserRole.Admin || user.Role == UserRole.Faculty)
                    {
                        AdminDashboard dashboard = new AdminDashboard(_inventoryService, _borrowService);
                        dashboard.Show();
                        Hide();
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

        private async Task BtnBorrow_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtStudentId.Text) || cmbListEquipments.SelectedIndex == -1)
            {
                MessageBox.Show("Please enter Student ID and select an item.");
                return;
            }

            var record = new BorrowRecord
            {
                BorrowerId = txtStudentId.Text,
                ItemName = cmbListEquipments.Text,
                Quantity = (int)numQuantity.Value,
                Purpose = txtSubject.Text,
                GradeLevel = cmbGradeLevel.Text,
                Status = BorrowStatus.Active // FIX: Uses Enum instead of string
            };

            try
            {
                // FIX: Corrected parameter order to pass Status "Available" first, then the search term
                var items = await _inventoryService.GetFilteredInventoryAsync("Available", record.ItemName);
                var itemToBorrow = items.FirstOrDefault();

                if (itemToBorrow != null)
                {
                    await _borrowService.ProcessBorrowAsync(record, itemToBorrow.Id);
                    MessageBox.Show("Item Borrowed Successfully!");
                    await LoadEquipmentListAsync(); 
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