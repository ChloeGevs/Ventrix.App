using System;
using System.Windows.Forms;

namespace Ventrix.App
{
    public partial class FormRegistrationSuccess : Form
    {
        // This is the ONLY place this constructor should exist
        public FormRegistrationSuccess(string role, string name)
        {
            InitializeComponent(); // This calls the code in the Designer file

            lblTitle.Text = "Registration Successful!";
            lblMessage.Text = $"Welcome to Ventrix, {name}!";

            // Wire up the button result
            btnDone.Click += (s, e) => this.DialogResult = DialogResult.OK;
        }

        private void pnlBackground_Paint(object sender, PaintEventArgs e)
        {
            // Your gradient logic here...
        }
    }
}