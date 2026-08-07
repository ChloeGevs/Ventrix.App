using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventrix.Application.Services;
using Ventrix.Domain.Enums;

namespace Ventrix.App.Popups
{
    public partial class EditUserPopup : Form
    {
        private readonly UserService _userService;
        private readonly string _userId;

        // Form Animation State Fields
        private int _targetY;
        private bool _isAnimating = false;

        // Typewriter Effect Fields
        private string _fullTitleText = "👤  Edit User";
        private int _charIndex = 0;
        private int _typewriterCounter = 0;

        // Property to let the parent dashboard know if a refresh is needed
        public bool WasUpdated { get; private set; } = false;

        public EditUserPopup(string userId, UserService userService)
        {
            InitializeComponent();
            _userId = userId;
            _userService = userService;

            // Setup Role dropdown
            cmbRole.DataSource = Enum.GetValues(typeof(UserRole));

            // Set up form for animation
            this.Opacity = 0;
            this.StartPosition = FormStartPosition.CenterParent;
        }

        private async void EditUserPopup_Load(object sender, EventArgs e)
        {
            // Start with an empty title for the typewriter effect
            lblTitle.Text = "";

            // Establish target position for the kinetic slide
            _targetY = this.Location.Y;

            // Start 30px lower for a tighter, snappier feel
            this.Location = new Point(this.Location.X, _targetY + 30);

            // Modern Exponential Ease-Out Entrance Animation & Typewriter Timer (10ms interval)
            System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer { Interval = 10 };
            animTimer.Tick += (object s, EventArgs ev) =>
            {
                // Smooth Lerp for Opacity (decelerates as it approaches 1.0)
                this.Opacity += (1.0 - this.Opacity) * 0.2;

                int currentY = this.Location.Y;
                int distance = currentY - _targetY;

                // Smooth Lerp for Position (decelerates as it approaches target)
                if (distance > 0)
                {
                    int move = (int)Math.Ceiling(distance * 0.2);
                    this.Location = new Point(this.Location.X, currentY - move);
                }

                // Typewriter Effect Logic (Appends characters every 2 ticks)
                _typewriterCounter++;
                if (_typewriterCounter % 2 == 0 && _charIndex < _fullTitleText.Length)
                {
                    _charIndex++;
                    lblTitle.Text = _fullTitleText.Substring(0, _charIndex);
                }

                // Stop condition (when visually complete and title is fully typed out)
                if (distance <= 0 && this.Opacity >= 0.98 && _charIndex >= _fullTitleText.Length)
                {
                    this.Opacity = 1.0;
                    this.Location = new Point(this.Location.X, _targetY);
                    lblTitle.Text = _fullTitleText; // Ensure exact match at completion
                    animTimer.Stop();
                    animTimer.Dispose();
                }
            };
            animTimer.Start();

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
                    cmbRole.DataSource = new UserRole[] { UserRole.Student, UserRole.Faculty };
                }
                else
                {
                    MessageBox.Show("User not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await ClosePopupAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load user details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await ClosePopupAsync();
            }
        }

        // Modern Exit Animation
        private async Task ClosePopupAsync(DialogResult result = DialogResult.Cancel)
        {
            if (_isAnimating) return;
            _isAnimating = true;

            System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer { Interval = 10 };
            animTimer.Tick += (object s, EventArgs ev) =>
            {
                // Quick linear fade out and slight drop down
                this.Opacity -= 0.15;
                this.Location = new Point(this.Location.X, this.Location.Y + 2);

                if (this.Opacity <= 0)
                {
                    animTimer.Stop();
                    animTimer.Dispose();
                    this.DialogResult = result;
                    this.Close();
                }
            };
            animTimer.Start();
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
                await ClosePopupAsync(DialogResult.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to update user: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = true;
                btnSave.Text = "Save Changes";
            }
        }

        private async void btnCancel_Click(object sender, EventArgs e)
        {
            await ClosePopupAsync(DialogResult.Cancel);
        }
    }
}