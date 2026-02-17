using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ventrix.App
{
    public partial class Form2 : MaterialForm
    {
        public Form2()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);

            // Apply the custom font immediately after MaterialSkin takes control
            ApplyHeaderStyle();
            ApplyCustomFonts();

            // Handle switching between Student and Staff UI
            cmbRole.SelectedIndexChanged += (s, e) =>
            {
                bool isStudent = cmbRole.Text == "Student";
                lblHeader.Text = isStudent ? "STUDENT REGISTRATION" : "STAFF REGISTRATION";
                btnRegister.Text = "REGISTER";
                lblHeader.AutoSize = false;
                lblHeader.Width = pnlRegCard.Width;
                lblHeader.TextAlignment = ContentAlignment.MiddleCenter;
                lblHeader.Font = new Font("Sitka Heading", 22F, FontStyle.Bold);

                // Re-apply style to override theme defaults
                ApplyHeaderStyle();
                ApplyCustomFonts();
            };

            // Disable/Enable Suffix box based on checkbox
            chkNoSuffix.CheckedChanged += (s, e) => {
                if (chkNoSuffix.Checked)
                {
                    txtSuffix.Text = "";
                    txtSuffix.Enabled = false;
                    txtSuffix.FillColor = Color.FromArgb(240, 240, 240); // Visual cue for disabled
                }
                else
                {
                    txtSuffix.Enabled = true;
                    txtSuffix.FillColor = Color.White;
                }
            };

            pnlRegCard.BorderRadius = 20;
            pnlRegCard.ShadowDecoration.BorderRadius = 20;
            pnlRegCard.BackColor = Color.Transparent;

            // Registration Action with Modern Popup
            btnRegister.Click += (s, e) =>
            {
                string role = cmbRole.Text;
                string suffix = chkNoSuffix.Checked ? "" : txtSuffix.Text.Trim();

                // Format full name including suffix if it exists
                string fullName = string.IsNullOrEmpty(suffix)
                    ? $"{txtFirstName.Text} {txtLastName.Text}"
                    : $"{txtFirstName.Text} {txtLastName.Text}, {suffix}";

                // Launch custom Modern Popup
                using (FormRegistrationSuccess successPopup = new FormRegistrationSuccess(role, fullName))
                {
                    if (successPopup.ShowDialog() == DialogResult.OK)
                    {
                        this.Close(); // Return to Form1
                    }
                }
            };

            pnlRegCard.Invalidate();

            // Link to return to Login
            lblLoginLink.Click += (s, e) => this.Close();
        }

        private void ApplyHeaderStyle()
        {
            // Explicitly set font and color to bypass MaterialSkin default typography
            lblHeader.Font = new Font("Sitka Heading", 22F, FontStyle.Bold);
            lblHeader.ForeColor = Color.FromArgb(13, 71, 161);
        }
        private void ApplyCustomFonts()
        {
            // Define your Sitka Fonts
            Font sitkaBanner = new Font("Sitka Banner", 12F, FontStyle.Bold);
            Font sitkaText = new Font("Sitka Text", 11F, FontStyle.Regular);

            // Force apply to the ComboBox
            cmbRole.Font = sitkaBanner;

            // Force apply to all TextBoxes
            txtFirstName.Font = new Font("Segoe UI Regular", 9F);
            txtLastName.Font = new Font("Segoe UI Regular", 9F);
            txtMiddleName.Font = new Font("Segoe UI Regular", 9F);
            txtSuffix.Font = new Font("Segoe UI Regular", 9F);
            chkNoSuffix.Font = new Font("Sitka Banner", 11F, FontStyle.Regular);

            // Force apply to the Register Button
            btnRegister.Font = sitkaBanner;

            // Force apply to the Login Link
            lblLoginLink.Font = new Font("Sitka Banner", 11F, FontStyle.Regular);
        }
    }
}