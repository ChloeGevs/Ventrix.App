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

            cmbRole.SelectedIndexChanged += (s, e) => {
                bool isStudent = cmbRole.Text == "Student";
                lblHeader.Text = isStudent ? "STUDENT ENROLLMENT" : "STAFF REGISTRATION";
                txtSchoolID.PlaceholderText = isStudent ? "Student ID (e.g. 2024-XXXX)" : "Faculty / Staff ID";
                btnRegister.Text = isStudent ? "ENROLL STUDENT" : "CREATE ACCOUNT";
            };

            lblLoginLink.Click += (s, e) => this.Close();
        }
    }
}