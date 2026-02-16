using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ventrix.App
{
    public partial class FormRegistrationSuccess : Form
    {
        public FormRegistrationSuccess(string role, string name, string id)
        {
            InitializeComponent();
            lblTitle.Text = "Registration Successful!";
            lblMessage.Text = $"Welcome to Ventrix, {name}!";
            lblDetails.Text = $"Role: {role}\nID Number: {id}";

            // Auto-close or click button to return
            btnDone.Click += (s, e) => this.DialogResult = DialogResult.OK;
        }

        // Setup the UI components manually for speed or use Designer
        private void InitializeComponent()
        {
            this.pnlBackground = new Guna2Panel();
            this.lblTitle = new Label();
            this.lblMessage = new Label();
            this.lblDetails = new Label();
            this.btnDone = new Guna2Button();
            this.iconCheck = new Guna2CirclePictureBox();

            // Form Settings
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(400, 450);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;

            // Background with Shadow
            pnlBackground.Dock = DockStyle.Fill;
            pnlBackground.BorderColor = Color.FromArgb(224, 224, 224);
            pnlBackground.BorderThickness = 1;
            pnlBackground.ShadowDecoration.Enabled = true;

            // Icon
            iconCheck.FillColor = Color.FromArgb(46, 125, 50);
            iconCheck.Location = new Point(150, 40);
            iconCheck.Size = new Size(100, 100);

            // Labels
            lblTitle.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitle.Location = new Point(0, 160);
            lblTitle.Size = new Size(400, 40);
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;

            lblMessage.Font = new Font("Segoe UI", 11F);
            lblMessage.Location = new Point(0, 200);
            lblMessage.Size = new Size(400, 30);
            lblMessage.TextAlign = ContentAlignment.MiddleCenter;

            lblDetails.Font = new Font("Segoe UI Semilight", 10F);
            lblDetails.ForeColor = Color.Gray;
            lblDetails.Location = new Point(50, 240);
            lblDetails.Size = new Size(300, 60);
            lblDetails.TextAlign = ContentAlignment.MiddleCenter;

            // Button
            btnDone.BorderRadius = 10;
            btnDone.FillColor = Color.FromArgb(13, 71, 161);
            btnDone.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnDone.Location = new Point(100, 340);
            btnDone.Size = new Size(200, 45);
            btnDone.Text = "CONTINUE TO LOGIN";

            this.Controls.Add(pnlBackground);
            pnlBackground.Controls.AddRange(new Control[] { iconCheck, lblTitle, lblMessage, lblDetails, btnDone });
        }

        private Guna2Panel pnlBackground;
        private Guna2CirclePictureBox iconCheck;
        private Label lblTitle, lblMessage, lblDetails;
        private Guna2Button btnDone;
    }
}