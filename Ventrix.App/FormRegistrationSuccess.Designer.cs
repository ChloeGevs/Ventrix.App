using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ventrix.App
{
    public partial class FormRegistrationSuccess : Form
    {
        public FormRegistrationSuccess(string role, string name)
        {
            InitializeComponent();
            lblTitle.Text = "Registration Successful!";
            lblMessage.Text = $"Welcome to Ventrix, {name}!";

            // Auto-close or click button to return
            btnDone.Click += (s, e) => this.DialogResult = DialogResult.OK;
        }

        // Setup the UI components manually for speed or use Designer
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            pnlBackground = new Guna2GradientPanel();
            iconCheck = new Guna2CirclePictureBox();
            lblTitle = new Label();
            lblMessage = new Label();
            lblDetails = new Label();
            btnDone = new Guna2Button();
            pnlBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)iconCheck).BeginInit();
            SuspendLayout();
            // 
            // pnlBackground
            // 
            pnlBackground.BackColor = Color.Transparent;
            pnlBackground.BorderColor = Color.Transparent;
            pnlBackground.BorderThickness = 1;
            pnlBackground.Controls.Add(iconCheck);
            pnlBackground.Controls.Add(lblTitle);
            pnlBackground.Controls.Add(lblMessage);
            pnlBackground.Controls.Add(lblDetails);
            pnlBackground.Controls.Add(btnDone);
            pnlBackground.CustomizableEdges = customizableEdges4;
            pnlBackground.Dock = DockStyle.Fill;
            pnlBackground.FillColor = Color.FromArgb(192, 255, 255);
            pnlBackground.FillColor2 = Color.Teal;
            pnlBackground.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            pnlBackground.Location = new Point(0, 0);
            pnlBackground.Name = "pnlBackground";
            pnlBackground.ShadowDecoration.CustomizableEdges = customizableEdges5;
            pnlBackground.ShadowDecoration.Enabled = true;
            pnlBackground.Size = new Size(400, 450);
            pnlBackground.TabIndex = 0;
            pnlBackground.Paint += pnlBackground_Paint;
            // 
            // iconCheck
            // 
            iconCheck.BackColor = Color.Transparent;
            iconCheck.BackgroundImage = Properties.Resources.Logo;
            iconCheck.BackgroundImageLayout = ImageLayout.Zoom;
            iconCheck.FillColor = Color.Transparent;
            iconCheck.ImageRotate = 0F;
            iconCheck.Location = new Point(118, 27);
            iconCheck.Name = "iconCheck";
            iconCheck.ShadowDecoration.CustomizableEdges = customizableEdges1;
            iconCheck.Size = new Size(159, 118);
            iconCheck.TabIndex = 0;
            iconCheck.TabStop = false;
            // 
            // lblTitle
            // 
            lblTitle.Font = new Font("Sitka Heading", 18F, FontStyle.Bold);
            lblTitle.Location = new Point(0, 160);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(400, 40);
            lblTitle.TabIndex = 1;
            lblTitle.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblMessage
            // 
            lblMessage.Font = new Font("Sitka Display", 11F);
            lblMessage.Location = new Point(0, 200);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(400, 30);
            lblMessage.TabIndex = 2;
            lblMessage.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblDetails
            // 
            lblDetails.Font = new Font("Sitka Banner", 10F);
            lblDetails.ForeColor = Color.Gray;
            lblDetails.Location = new Point(50, 240);
            lblDetails.Name = "lblDetails";
            lblDetails.Size = new Size(300, 60);
            lblDetails.TabIndex = 3;
            lblDetails.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnDone
            // 
            btnDone.BorderRadius = 10;
            btnDone.CustomizableEdges = customizableEdges2;
            btnDone.FillColor = Color.FromArgb(13, 71, 161);
            btnDone.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnDone.ForeColor = Color.White;
            btnDone.Location = new Point(100, 340);
            btnDone.Name = "btnDone";
            btnDone.ShadowDecoration.CustomizableEdges = customizableEdges3;
            btnDone.Size = new Size(200, 45);
            btnDone.TabIndex = 4;
            btnDone.Text = "CONTINUE TO LOGIN";
            // 
            // FormRegistrationSuccess
            // 
            BackColor = Color.White;
            ClientSize = new Size(400, 450);
            Controls.Add(pnlBackground);
            DoubleBuffered = true;
            FormBorderStyle = FormBorderStyle.None;
            Name = "FormRegistrationSuccess";
            StartPosition = FormStartPosition.CenterParent;
            pnlBackground.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)iconCheck).EndInit();
            ResumeLayout(false);
        }

        private Guna2GradientPanel pnlBackground;
        private Guna2CirclePictureBox iconCheck;
        private Label lblTitle, lblMessage, lblDetails;
        private Guna2Button btnDone;
    }
}