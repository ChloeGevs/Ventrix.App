using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ventrix.App
{
    public partial class Form1 : MaterialForm
    {
        public Form1()
        {
            InitializeComponent();

            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            materialSkinManager.ColorScheme = new ColorScheme(
                Color.FromArgb(13, 71, 161),
                Color.FromArgb(10, 50, 120),
                Color.FromArgb(33, 150, 243),
                Color.FromArgb(30, 136, 229),
                TextShade.WHITE
            );

            pnlLoginCard.BorderRadius = 20;
            pnlLoginCard.ShadowDecoration.BorderRadius = 20;
            pnlLoginCard.BackColor = Color.Transparent;

            // Toggle Events
            btnStaffToggle.Click += (s, e) => SetLoginMode("Staff");
            btnStudentToggle.Click += (s, e) => SetLoginMode("Student");
            var method = typeof(Control).GetProperty("DoubleBuffered",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            method.SetValue(mainTableLayout, true, null);

            // Navigation to Registration
            lblCreateAccount.Click += (s, e) => {
                Form2 regForm = new Form2();
                regForm.ShowDialog();
            };
            
            btnLogin.Click += (s, e) => {
                // Navigate to Admin Dashboard
                Form3 dashboard = new Form3();
                dashboard.Show();
                this.Hide();
            };

            SetLoginMode("Student"); // Default to Borrower view for quick access
        }

        private void SetLoginMode(string mode)
        {
            bool isAdminLogin = (mode == "Staff");

            // Update Headers and Placeholders
            lblLoginHeader.Text = isAdminLogin ? "ADMIN LOGIN" : "BORROWING PORTAL";
            txtStudentId.PlaceholderText = isAdminLogin ? "Admin ID" : "Student or Staff ID Number";
            lblLoginHeader.AutoSize = false;
            lblLoginHeader.Width = pnlLoginCard.Width;
            lblLoginHeader.TextAlignment = ContentAlignment.MiddleCenter;
            lblLoginHeader.Font = new Font("Sitka Heading", 22F, FontStyle.Bold);

            //Define your Sitka Fonts
            Font sitkaBanner = new Font("Sitka Banner", 12F, FontStyle.Bold);
            Font sitkaText = new Font("Sitka Text", 10F, FontStyle.Regular);
            Font sitkaDisplay = new Font("Sitka Display", 10F, FontStyle.Bold);

            //Force apply to Buttons
            btnStaffToggle.Font = sitkaBanner;
            btnStudentToggle.Font = sitkaBanner;
            btnBorrow.Font = sitkaBanner;
            btnReturn.Font = sitkaBanner;
            btnLogin.Font = sitkaBanner;

            //Force apply to TextBoxes & ComboBox
            txtStudentId.Font = new Font("Segoe UI Regular", 8F);
            txtSubject.Font = new Font("Segoe UI Regular", 8F);
            cmbListEquipments.Font = sitkaBanner;

            //Force apply to Labels 
            lblQuantity.Font = sitkaDisplay;
            lblSubject.Font = sitkaDisplay;
            lblEquipmentList.Font = sitkaText;

            // Visibility Toggles
            lblCreateAccount.Visible = !isAdminLogin;
            btnLogin.Visible = isAdminLogin;
            txtPassword.Visible = isAdminLogin;

            // Toggle Visibility of Borrower-specific fields
            cmbListEquipments.Visible = !isAdminLogin;
            numQuantity.Visible = !isAdminLogin;
            txtSubject.Visible = !isAdminLogin;
            lblQuantity.Visible = !isAdminLogin;
            lblSubject.Visible = !isAdminLogin;
            btnBorrow.Visible = !isAdminLogin;
            btnReturn.Visible = !isAdminLogin;

            pnlLoginCard.Invalidate();

            if (isAdminLogin)
            {
                txtPassword.Location = new Point(50, 220);
                txtPassword.BringToFront();
                btnLogin.BringToFront(); // Ensures the button isn't hidden behind the panel
            }

            // Force the custom font to stay active
            lblLoginHeader.Font = new Font("Sitka Heading", 22F, FontStyle.Bold);

            // Visual feedback for toggles
            btnStaffToggle.FillColor = isAdminLogin ? Color.FromArgb(13, 71, 161) : Color.FromArgb(224, 224, 224);
            btnStudentToggle.FillColor = !isAdminLogin ? Color.FromArgb(13, 71, 161) : Color.FromArgb(224, 224, 224);
        }

    }
}