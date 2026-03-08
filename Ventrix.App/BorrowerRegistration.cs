using System;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using Ventrix.Application.DTOs;

namespace Ventrix.App
{
    public partial class BorrowerRegistration : Form
    {
        private readonly UserService _userService;
        private readonly InventoryService _inventoryService;
        private readonly BorrowService _borrowService;

        // Store default border color for resets
        private readonly Color DefaultBorderColor = Color.FromArgb(213, 218, 223);
        private readonly Color ErrorBorderColor = Color.Red;

        public BorrowerRegistration(InventoryService inventoryService, BorrowService borrowService, UserService userService)
        {
            _inventoryService = inventoryService;
            _borrowService = borrowService;
            _userService = userService;

            InitializeComponent();
            SetupEvents();

            // Fade in effect
            this.Opacity = 0;
            this.Load += BorrowerRegistration_Load;
        }

        private async void BorrowerRegistration_Load(object sender, EventArgs e)
        {
            // Subtle fade-in animation
            for (double i = 0; i <= 1; i += 0.1)
            {
                this.Opacity = i;
                await Task.Delay(15);
            }
        }

        private void SetupEvents()
        {
            FormClosed += (s, e) => System.Windows.Forms.Application.Exit();

            // Button and Checkbox Actions
            btnRegister.Click += BtnRegister_Click;
            chkNoSuffix.CheckedChanged += ChkNoSuffix_CheckedChanged;

            // Link Actions
            lblLoginLink.Click += LblLoginLink_Click;
            lblLoginLink.MouseEnter += (s, e) => lblLoginLink.Cursor = Cursors.Hand;

            // Enter Key Support 
            KeyEventHandler enterKeyHandler = (s, e) => { if (e.KeyCode == Keys.Enter) btnRegister.PerformClick(); };
            txtFirstName.KeyDown += enterKeyHandler;
            txtLastName.KeyDown += enterKeyHandler;
            txtMiddleName.KeyDown += enterKeyHandler;
            txtSuffix.KeyDown += enterKeyHandler;

            // Reset validation colors when typing
            txtFirstName.TextChanged += (s, e) => ResetValidation(txtFirstName);
            txtLastName.TextChanged += (s, e) => ResetValidation(txtLastName);

            // Defaults
            cmbRole.SelectedIndex = 0;
            txtFirstName.Focus();

        }

        private void ResetValidation(Guna.UI2.WinForms.Guna2TextBox textBox)
        {
            textBox.BorderColor = DefaultBorderColor;
            textBox.HoverState.BorderColor = Color.FromArgb(13, 71, 161);
        }

        private bool ValidateInputs()
        {
            bool isValid = true;

            if (string.IsNullOrWhiteSpace(txtFirstName.Text))
            {
                txtFirstName.BorderColor = ErrorBorderColor;
                txtFirstName.HoverState.BorderColor = ErrorBorderColor;
                isValid = false;
            }

            if (string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                txtLastName.BorderColor = ErrorBorderColor;
                txtLastName.HoverState.BorderColor = ErrorBorderColor;
                isValid = false;
            }

            return isValid;
        }

        private void SetLoadingState(bool isLoading)
        {
            this.Cursor = isLoading ? Cursors.WaitCursor : Cursors.Default;
            btnRegister.Enabled = !isLoading;
            pnlRegCard.Enabled = !isLoading;
        }

        private async void BtnRegister_Click(object sender, EventArgs e)
        {
            if (!ValidateInputs())
            {
                MessageBox.Show("Please fill in the highlighted required fields.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetLoadingState(true);

            try
            {
                var registrationData = new RegisterDTO
                {
                    FirstName = txtFirstName.Text.Trim(),
                    MiddleName = txtMiddleName.Text.Trim(),
                    LastName = txtLastName.Text.Trim(),
                    Suffix = chkNoSuffix.Checked ? "" : txtSuffix.Text.Trim(),
                    Role = Enum.Parse<Ventrix.Domain.Enums.UserRole>(cmbRole.SelectedItem?.ToString() ?? "Student"),
                };

                var registeredUser = await _userService.RegisterNewBorrowerAsync(registrationData);

                string successMessage = $"Registration successful!\n\nYour Student ID is: {registeredUser.UserId}\n\nPlease write this down and use it to log in.";
                MessageBox.Show(successMessage, "Account Created", MessageBoxButtons.OK, MessageBoxIcon.Information);

                NavigateToPortal();
            }
            catch (Exception ex)
            {
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += "\n\nInner Details: " + ex.InnerException.Message;
                }

                MessageBox.Show(errorMessage, "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private void ChkNoSuffix_CheckedChanged(object sender, EventArgs e)
        {
            if (chkNoSuffix.Checked)
            {
                txtSuffix.Enabled = false;
                txtSuffix.Clear();
                txtSuffix.FillColor = Color.FromArgb(245, 245, 245); // Visual cue that it's disabled
            }
            else
            {
                txtSuffix.Enabled = true;
                txtSuffix.FillColor = Color.White;
                txtSuffix.Focus();
            }
        }

        private void LblLoginLink_Click(object sender, EventArgs e)
        {
            NavigateToPortal();
        }

        private void NavigateToPortal()
        {
            // Find the hidden portal
            var portal = System.Windows.Forms.Application.OpenForms.OfType<BorrowerPortal>().FirstOrDefault();

            if (portal != null)
            {
                portal.ToggleMode("Student");
                portal.Show();
            }
            else
            {
                var newPortal = new BorrowerPortal(_inventoryService, _borrowService, _userService);
                newPortal.Show();
            }

            this.Hide();
        }

        private void lblHeader_Click(object sender, EventArgs e)
        {

        }
    }
}