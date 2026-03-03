using System;
using System.Linq;
using System.Windows.Forms;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
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
            btnAdminToggle.Click += (s, e) => ToggleMode("Admin");
            btnStudentToggle.Click += (s, e) => ToggleMode("Student");

            // Actions - Wired cleanly now!
            btnLogin.Click += BtnLogin_Click;
            btnBorrow.Click += BtnBorrow_Click;
            lblCreateAccount.Click += LblCreateAccount_Click;
            txtPassword.IconRightClick += TxtPassword_IconRightClick;
            txtPassword.MouseMove += txtPassword_MouseMove;
            cmbGradeLevel.SelectedIndexChanged += CmbGradeLevel_SelectedIndexChanged;


            ToggleMode("Student");

            Load += async (s, e) =>
            {
                await _userService.InitializeDefaultAdminAsync();
                await LoadEquipmentListAsync();
            };
        }

        private void TxtPassword_IconRightClick(object sender, EventArgs e)
        {
            if (txtPassword.UseSystemPasswordChar)
            {
                // Show the password
                txtPassword.UseSystemPasswordChar = false;
                txtPassword.PasswordChar = '\0';
                txtPassword.IconRight = Properties.Resources.hide; // Ensure you have these images in Resources
            }
            else
            {
                // Hide the password
                txtPassword.UseSystemPasswordChar = true;
                txtPassword.PasswordChar = '●';
                txtPassword.IconRight = Properties.Resources.eye;
            }
            txtPassword.Cursor = Cursors.Hand;
        }

        private void txtPassword_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X > txtPassword.Width - 40)
            {
                txtPassword.Cursor = Cursors.Hand;
            }
            else
            {
                txtPassword.Cursor = Cursors.IBeam;
            }
        }

        private async Task LoadEquipmentListAsync()
        {
            cmbListEquipments.Items.Clear();
            var items = await _inventoryService.GetFilteredInventoryAsync("Available", "");
            var names = items.Select(i => i.Name).Distinct().ToArray();
            cmbListEquipments.Items.AddRange(names);
        }

        private void ToggleMode(string mode)
        {
            txtStudentId.Clear();
            txtPassword.Clear();
                
            if (mode == "Admin")
            {
                // UI Changes for Admin
                lblLoginHeader.Text = "ADMIN LOGIN";
                txtPassword.Visible = true;
                btnLogin.Visible = true;
                txtStudentId.PlaceholderText = "Username / Admin ID";

                // Hide Borrower UI
                cmbListEquipments.Visible = false;
                numQuantity.Visible = false;
                txtSubject.Visible = false;
                cmbGradeLevel.Visible = false;
                btnBorrow.Visible = false;
                btnReturn.Visible = false;
                lblQuantity.Visible = false;
                lblSubject.Visible = false;
                lblCreateAccount.Visible = false;
                lblEquipmentList.Visible = false;

                // Button Styling
                btnAdminToggle.FillColor = System.Drawing.Color.FromArgb(13, 71, 161);
                btnStudentToggle.FillColor = System.Drawing.Color.Gray;

                numQuantity.Maximum = 10;
            }
            else
            {
                // UI Changes for Borrower
                lblLoginHeader.Text = "BORROWING PORTAL";
                txtPassword.Visible = false;
                btnLogin.Visible = false;
                txtStudentId.PlaceholderText = "Student/Faculty ID Number";

                // Show Borrower UI
                cmbListEquipments.Visible = true;
                numQuantity.Visible = true;
                txtSubject.Visible = true;
                cmbGradeLevel.Visible = true;
                btnBorrow.Visible = true;
                btnReturn.Visible = true;
                lblCreateAccount.Visible = true;
                lblQuantity.Visible = true;
                lblSubject.Visible = true;
                lblEquipmentList.Visible = true;

                // Button Styling
                btnStudentToggle.FillColor = System.Drawing.Color.FromArgb(13, 71, 161);
                btnAdminToggle.FillColor = System.Drawing.Color.Gray;

                numQuantity.Maximum = 2;
            }
        }

        // Changed to async void for WinForms event handling
        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            try
            {
                var loginDto = new Ventrix.Application.DTOs.LoginDto
                {
                    UserId = txtStudentId.Text,
                    Password = txtPassword.Text
                };

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
                    MessageBox.Show("Invalid Credentials. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Login error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Changed to async void for WinForms event handling
        private async void BtnBorrow_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtStudentId.Text) || cmbListEquipments.SelectedIndex == -1)
            {
                MessageBox.Show("Please enter Student ID and select an item.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var record = new BorrowRecord
            {
                BorrowerId = txtStudentId.Text,
                ItemName = cmbListEquipments.Text,
                Quantity = (int)numQuantity.Value,
                Purpose = txtSubject.Text,
                GradeLevel = cmbGradeLevel.Text,
                Status = BorrowStatus.Active
            };

            try
            {
                var items = await _inventoryService.GetFilteredInventoryAsync("Available", record.ItemName);
                var itemToBorrow = items.FirstOrDefault();

                if (itemToBorrow != null)
                {
                    await _borrowService.ProcessBorrowAsync(record, itemToBorrow.Id);
                    MessageBox.Show("Item Borrowed Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await LoadEquipmentListAsync(); // Refresh the dropdown list!
                }
                else
                {
                    MessageBox.Show("Item is no longer available.", "Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error borrowing item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void LblCreateAccount_Click(object sender, EventArgs e)
        {
            BorrowerRegistration registrationForm = new BorrowerRegistration(_inventoryService, _borrowService, _userService);
            registrationForm.Show();
            this.Hide();
        }

        private void CmbGradeLevel_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbGradeLevel.SelectedItem != null && cmbGradeLevel.SelectedItem.ToString() == "Faculty")
            {
                numQuantity.Maximum = 10;
            }
            else
            {
                numQuantity.Maximum = 2;
                if (numQuantity.Value > 2)
                {
                    numQuantity.Value = 2;
                }
            }
        }
    }
}