using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventrix.Application.Services;
using Ventrix.Domain.Enums;
using Ventrix.Application.Services;

namespace Ventrix.App.Popups
{
    public partial class EditUserPopup : Form
    {
        private readonly UserService _userService;
        private readonly string _userId;

        // Property to let the parent dashboard know if a refresh is needed
        public bool WasUpdated { get; private set; } = false;

        public EditUserPopup(string userId, UserService userService)
        {
            InitializeComponent();
            _userId = userId;
            _userService = userService;

            // Setup Role dropdown
            cmbRole.DataSource = Enum.GetValues(typeof(UserRole));
        }

        private async void EditUserPopup_Load(object sender, EventArgs e)
        {
            try
            {
                // Fetch the existing user details to populate the form
                var user = await _userService.GetUserByIdAsync(_userId);

                if (user != null)
                {
                    txtSchoolId.Text = user.UserId;
                    txtFirstName.Text = user.FirstName;
                    txtLastName.Text = user.LastName;
                    txtSuffix.Text = user.Suffix ?? "";
                    cmbRole.DataSource = new UserRole[] {UserRole.Student, UserRole.Faculty
            };
                }
                else
                {
                    MessageBox.Show("User not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load user details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                this.Close();
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("First Name and Last Name are required.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Disable the button to prevent double-clicks
                btnSave.Enabled = false;
                btnSave.Text = "Saving...";

                // Create a temporary user object to hold the new details
                var updatedData = new Ventrix.Domain.Models.User
                {
                    FirstName = txtFirstName.Text.Trim(),
                    LastName = txtLastName.Text.Trim(),
                    Suffix = txtSuffix.Text.Trim(),
                    Role = (UserRole)cmbRole.SelectedItem
                };

                // Extract the (potentially new) ID from the textbox
                string newId = txtSchoolId.Text.Trim();

                // CORRECT CALL: Passing 1. Old ID, 2. Data object, 3. New ID
                await _userService.UpdateUserWithIdChangeAsync(_userId, updatedData, newId);

                WasUpdated = true;
                this.Close();
            
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                btnSave.Enabled = true;
                btnSave.Text = "Save Changes";
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}