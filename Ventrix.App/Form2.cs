using MaterialSkin.Controls;
using System;
using System.Windows.Forms;
using Ventrix.Domain.Models ;       
using Ventrix.Infrastructure;

namespace Ventrix.App
{
    public partial class Form2 : MaterialForm
    {
        // ... (Keep your existing Constructor and UI setup code) ...

        // Find this section in your existing code and UPDATE the click event:
        private void InitializeEventHandlers()
        {
            btnRegister.Click += BtnRegister_Click;
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            // 1. Validation
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }

            // 2. Prepare Data
            var newUser = new User
            {
                // We'll generate a random ID for now, or you can add a textbox for it
                UserId = new Random().Next(20240000, 20249999).ToString(),
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                MiddleName = txtMiddleName.Text,
                Suffix = chkNoSuffix.Checked ? "" : txtSuffix.Text,
                Role = cmbRole.Text,
                Password = "123" // Default password for now
            };

            // 3. Save to Database
            try
            {
                using (var db = new AppDbContext())
                {
                    db.Users.Add(newUser);
                    db.SaveChanges();
                }

                // 4. Show Success
                using (FormRegistrationSuccess successPopup = new FormRegistrationSuccess(newUser.Role, newUser.FullName))
                {
                    if (successPopup.ShowDialog() == DialogResult.OK)
                    {
                        this.Close(); // Close registration, go back to login
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving to database: " + ex.Message);
            }
        }
    }
}