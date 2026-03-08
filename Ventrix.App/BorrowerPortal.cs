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
            SetupFocusHighlighting(); // Initialize visual feedback for Tab navigation
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

            // Enter Key Support for final actions
            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnLogin.PerformClick(); };
            txtSubject.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnBorrow.PerformClick(); };

            // Student ID Enter logic: Smart submission or focus jump
            txtStudentId.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtPassword.Visible)
                    {
                        txtPassword.Focus(); // Move to password in Admin mode
                    }
                    else if (btnReturn.Visible && !string.IsNullOrWhiteSpace(txtStudentId.Text))
                    {
                        btnReturn.PerformClick(); // Quick Return support if ID is already filled
                    }
                    else
                    {
                        cmbListEquipments.Focus(); // Move to equipment list in Student mode
                    }

                    e.SuppressKeyPress = true;
                }
            };

            ToggleMode("Student");

            Load += async (s, e) =>
            {
                await _userService.InitializeDefaultAdminAsync();
                await LoadEquipmentListAsync();
            };
        }

        private void SetupFocusHighlighting()
        {
            // Apply to main actionable buttons
            var actionButtons = new[] { btnLogin, btnBorrow, btnReturn, btnAdminToggle, btnStudentToggle };

            foreach (var btn in actionButtons)
            {
                if (btn == null) continue;

                // When the button receives focus (via Tab key)
                btn.GotFocus += (s, e) =>
                {
                    btn.BorderThickness = 2;
                    btn.BorderColor = Color.Cyan; 
                };

                // When focus moves to the next item
                btn.LostFocus += (s, e) =>
                {
                    btn.BorderThickness = 0;
                };
            }
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
            var availableItems = await _inventoryService.GetFilteredInventoryAsync("Available", "");

            // Extract the base name (remove " #1", " #2") and get unique names
            var distinctItemNames = availableItems
                .Select(item =>
                {
                    int hashIndex = item.Name.LastIndexOf(" #");
                    return hashIndex > 0 ? item.Name.Substring(0, hashIndex).Trim() : item.Name.Trim();
                })
                .Distinct()
                .OrderBy(name => name) // Sort alphabetically A-Z
                .ToArray();

            if (distinctItemNames.Any())
            {
                cmbListEquipments.Items.AddRange(distinctItemNames);
            }
        }

        public void ToggleMode(string mode)
        {
            txtStudentId.Clear();
            txtPassword.Clear();
            txtSubject.Clear();

            // Set mnemonic shortcuts (Alt + A for Admin, Alt + S for Student)
            btnAdminToggle.Text = "&Admin Mode";
            btnStudentToggle.Text = "&Student Mode";

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

            // Smart Focus: Return to ID field automatically
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
                var loginDto = new Ventrix.Application.DTOs.LoginDTO
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
                    ItemName = cmbListEquipments.Text, // This will just say "Laptop" or "Mouse"
                    Quantity = (int)numQuantity.Value,
                    Purpose = txtSubject.Text,
                    GradeLevel = Enum.Parse<GradeLevel>(cmbGradeLevel.Text),
                    Status = BorrowStatus.Active
                };

                // Fetch ALL available items to manually filter exactly
                var allAvailableItems = await _inventoryService.GetFilteredInventoryAsync("Available", "");

                // Find the first available item whose base name matches the dropdown exactly
                var itemToBorrow = allAvailableItems.FirstOrDefault(i =>
                {
                    int hashIndex = i.Name.LastIndexOf(" #");
                    string baseName = hashIndex > 0 ? i.Name.Substring(0, hashIndex).Trim() : i.Name.Trim();
                    return baseName.Equals(record.ItemName, StringComparison.OrdinalIgnoreCase);
                });

                if (itemToBorrow != null)
                {
                    // Update record ItemName to the exact database name (e.g., "Laptop #12") so the system tracks the specific physical item
                    record.ItemName = itemToBorrow.Name;

                    await _borrowService.ProcessBorrowAsync(record, itemToBorrow.Id);
                    MessageBox.Show("Item Borrowed Successfully!", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    txtSubject.Clear(); // Clear subject to prep for next action
                    await LoadEquipmentListAsync(); // Reload the list to update availability
                }
                else
                {
                    MessageBox.Show("This item is currently out of stock or no longer available.", "Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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