using System;
using System.Windows.Forms;
using System.Threading.Tasks;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using Ventrix.Application.DTOs; // Added for RegisterDto

namespace Ventrix.App
{
    public partial class BorrowerRegistration : Form
    {
        private readonly UserService _userService;
        private readonly InventoryService _inventoryService;
        private readonly BorrowService _borrowService;

        public BorrowerRegistration(InventoryService inventoryService, BorrowService borrowService, UserService userService)
        {
            _inventoryService = inventoryService;
            _borrowService = borrowService;
            _userService = userService;
            InitializeComponent();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            // Event wire-up works perfectly with the new async void method
            btnRegister.Click += btnRegister_Click;

            lblLoginLink.Click += (s, e) =>
            {
                var portal = System.Windows.Forms.Application.OpenForms["BorrowerPortal"];

                if (portal != null)
                {
                    portal.Show();
                    Close();
                }
                else
                {
                    var newPortal = new BorrowerPortal(_inventoryService, _borrowService, _userService);
                    newPortal.Show();
                    Close();
                }
            };
        }

        // Changed to 'async void' so we can await the EF Core service
        private async void btnRegister_Click(object sender, EventArgs e)
        {
            try
            {
                // 1. Package ALL the form data into your DTO
                var registrationData = new Ventrix.Application.DTOs.RegisterDto
                {
                    FirstName = txtFirstName.Text,

                    MiddleName = txtMiddleName.Text,

                    LastName = txtLastName.Text,
                    Suffix = chkNoSuffix.Checked ? "" : txtSuffix.Text,

                    // Grab the role from your dropdown (assuming it's named cmbRole)
                    Role = cmbRole.SelectedItem?.ToString() ?? "Student",

                    // WE MUST PROVIDE A PASSWORD! Let's default it to their last name lowercase
                    Password = txtLastName.Text.ToLower() + "123"
                };

                // 2. Send the DTO to your EF Core service AND get the generated user back
                var registeredUser = await _userService.RegisterNewBorrowerAsync(registrationData);

                // 3. Success! Show them their new automatically generated ID
                string successMessage = $"Registration successful!\n\nYour Student ID is: {registeredUser.UserId}\nYour Password is: {registrationData.Password}\n\nPlease write this down.";
                MessageBox.Show(successMessage, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // 4. Open the portal
                BorrowerPortal portal = new BorrowerPortal(_inventoryService, _borrowService, _userService);
                portal.Show();
                this.Hide();
            }
            catch (Exception ex)
            {
                // THIS WILL SHOW YOU THE EXACT DATABASE ERROR!
                string errorMessage = ex.Message;
                if (ex.InnerException != null)
                {
                    errorMessage += "\n\nInner Details: " + ex.InnerException.Message;
                }

                MessageBox.Show(errorMessage, "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void lblLoginLink_MouseEnter(object sender, EventArgs e)
        {
            lblLoginLink.Cursor = Cursors.Hand;
        }

        private void chkNoSuffix_CheckedChanged(object sender, EventArgs e)
        {
            // If the "None" box is checked
            if (chkNoSuffix.Checked)
            {
                txtSuffix.Enabled = false;
                txtSuffix.Clear();
            }
            else
            {
                txtSuffix.Enabled = true;
            }
        }
    }
}