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

            // Toggle Events
            btnStaffToggle.Click += (s, e) => SetLoginMode("Staff");
            btnStudentToggle.Click += (s, e) => SetLoginMode("Student");

            // Navigation
            lblCreateAccount.Click += (s, e) => {
                Form2 regForm = new Form2();
                regForm.ShowDialog();
            };

            SetLoginMode("Staff"); // Default view
        }

        private void SetLoginMode(string mode)
        {
            bool isStaff = (mode == "Staff");

            lblLoginHeader.Text = isStaff ? "STAFF LOGIN" : "STUDENT ACCESS";
            txtUsername.PlaceholderText = isStaff ? "Username" : "Student ID Number";
            lblCreateAccount.Visible = isStaff;

            // Visual feedback for toggles
            btnStaffToggle.FillColor = isStaff ? Color.FromArgb(13, 71, 161) : Color.FromArgb(224, 224, 224);
            btnStaffToggle.ForeColor = isStaff ? Color.White : Color.DimGray;

            btnStudentToggle.FillColor = !isStaff ? Color.FromArgb(13, 71, 161) : Color.FromArgb(224, 224, 224);
            btnStudentToggle.ForeColor = !isStaff ? Color.White : Color.DimGray;
        }
    }
}