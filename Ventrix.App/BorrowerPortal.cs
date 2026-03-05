using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using System.Threading.Tasks;
using Ventrix.Domain.Enums;

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
            FormClosed += (s, e) => System.Windows.Forms.Application.Exit();

            // Toggles
            btnAdminToggle.Click += (s, e) => ToggleMode("Admin");
            btnStudentToggle.Click += (s, e) => ToggleMode("Student");

            // Actions
            btnLogin.Click += BtnLogin_Click;
            btnBorrow.Click += BtnBorrow_Click;
            lblCreateAccount.Click += LblCreateAccount_Click;
            txtPassword.IconRightClick += TxtPassword_IconRightClick;
            txtPassword.MouseMove += txtPassword_MouseMove;
            cmbGradeLevel.SelectedIndexChanged += CmbGradeLevel_SelectedIndexChanged;

            // Enter Key Support
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnLogin.PerformClick(); };
            txtStudentId.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter && btnBorrow.Visible) btnBorrow.PerformClick(); else if (e.KeyCode == Keys.Enter && btnLogin.Visible) txtPassword.Focus(); };
            txtSubject.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnBorrow.PerformClick(); };

            ToggleMode("Student");

            Load += async (s, e) =>
            {
                await _userService.InitializeDefaultAdminAsync();
                await LoadEquipmentListAsync();
            };

        }

        private void TxtPassword_IconRightClick(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;
            txtPassword.PasswordChar = txtPassword.UseSystemPasswordChar ? '●' : '\0';
            txtPassword.IconRight = txtPassword.UseSystemPasswordChar ? Properties.Resources.eye : Properties.Resources.hide;
            txtPassword.Cursor = Cursors.Hand;
        }

        private void txtPassword_MouseMove(object sender, MouseEventArgs e)
        {
            txtPassword.Cursor = (e.X > txtPassword.Width - 40) ? Cursors.Hand : Cursors.IBeam;
        }

        private async Task LoadEquipmentListAsync()
        {
            cmbListEquipments.Items.Clear();
            var items = await _inventoryService.GetFilteredInventoryAsync("Available", "");
            var names = items.Select(i => i.Name).Distinct().ToArray();
            if (names.Any()) cmbListEquipments.Items.AddRange(names);
        }

        public void ToggleMode(string mode)
        {
            txtStudentId.Clear();
            txtPassword.Clear();
            txtSubject.Clear();

            bool isAdmin = mode == "Admin";

            // Header & Placeholders
            lblLoginHeader.Text = isAdmin ? "ADMIN LOGIN" : "BORROWING PORTAL";
            txtStudentId.PlaceholderText = isAdmin ? "Username / Admin ID" : "Student/Faculty ID Number";

            // Visibility
            txtPassword.Visible = isAdmin;
            btnLogin.Visible = isAdmin;

            cmbListEquipments.Visible = !isAdmin;
            numQuantity.Visible = !isAdmin;
            txtSubject.Visible = !isAdmin;
            cmbGradeLevel.Visible = !isAdmin;
            btnBorrow.Visible = !isAdmin;
            btnReturn.Visible = !isAdmin;
            lblQuantity.Visible = !isAdmin;
            lblSubject.Visible = !isAdmin;
            lblCreateAccount.Visible = !isAdmin;
            lblEquipmentList.Visible = !isAdmin;

            // Active/Inactive Button Styling
            btnAdminToggle.FillColor = isAdmin ? Color.FromArgb(13, 71, 161) : Color.FromArgb(240, 240, 240);
            btnAdminToggle.ForeColor = isAdmin ? Color.White : Color.Gray;

            btnStudentToggle.FillColor = !isAdmin ? Color.FromArgb(13, 71, 161) : Color.FromArgb(240, 240, 240);
            btnStudentToggle.ForeColor = !isAdmin ? Color.White : Color.Gray;

            numQuantity.Maximum = isAdmin ? 10 : 2;

            // Smart Focus
            txtStudentId.Focus();
        }

        private void SetLoadingState(bool isLoading)
        {
            this.Cursor = isLoading ? Cursors.WaitCursor : Cursors.Default;
            btnLogin.Enabled = !isLoading;
            btnBorrow.Enabled = !isLoading;
            btnReturn.Enabled = !isLoading;
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentId.Text) || string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                MessageBox.Show("Please enter both ID and Password.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetLoadingState(true);
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
                        MessageBox.Show($"Welcome, {user.FirstName}! Student portal features coming soon.", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            finally
            {
                SetLoadingState(false);
            }
        }

        private async void BtnBorrow_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentId.Text) ||
                cmbListEquipments.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtSubject.Text) ||
                cmbGradeLevel.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill out all fields and select a Grade Level before borrowing.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetLoadingState(true);
            try
            {
                var record = new BorrowRecord
                {
                    BorrowerId = txtStudentId.Text,
                    ItemName = cmbListEquipments.Text,
                    Quantity = (int)numQuantity.Value,
                    Purpose = txtSubject.Text,
                    GradeLevel = cmbGradeLevel.Text,
                    Status = BorrowStatus.Active
                };

                var items = await _inventoryService.GetFilteredInventoryAsync("Available", record.ItemName);
                var itemToBorrow = items.FirstOrDefault();

                if (itemToBorrow != null)
                {
                    await _borrowService.ProcessBorrowAsync(record, itemToBorrow.Id);
                    MessageBox.Show("Item Borrowed Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSubject.Clear(); // Clear subject to prep for next action
                    await LoadEquipmentListAsync();
                }
                else
                {
                    MessageBox.Show("Item is no longer available.", "Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += "\n\nInner Details: " + ex.InnerException.Message;
                }

                MessageBox.Show($"Error borrowing item: {errorMessage}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
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
                if (numQuantity.Value > 2) numQuantity.Value = 2;
            }
        }
    }
}