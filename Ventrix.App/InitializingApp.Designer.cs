namespace Ventrix.App
{
    partial class InitializingApp
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            pnlBackground = new Guna.UI2.WinForms.Guna2GradientPanel();
            guna2HtmlLabel2 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            picLogo = new Guna.UI2.WinForms.Guna2PictureBox();
            lblStatus = new Label();
            progBar = new Guna.UI2.WinForms.Guna2ProgressBar();
            fadeTimer = new System.Windows.Forms.Timer(components);
            guna2ShadowForm1 = new Guna.UI2.WinForms.Guna2ShadowForm(components);
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            pnlBackground.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();
            // 
            // pnlBackground
            // 
            pnlBackground.BackgroundImage = Properties.Resources._5;
            pnlBackground.BorderRadius = 30;
            pnlBackground.Controls.Add(guna2HtmlLabel2);
            pnlBackground.Controls.Add(guna2HtmlLabel1);
            pnlBackground.Controls.Add(picLogo);
            pnlBackground.Controls.Add(lblStatus);
            pnlBackground.Controls.Add(progBar);
            pnlBackground.CustomizableEdges = customizableEdges5;
            pnlBackground.Dock = DockStyle.Fill;
            pnlBackground.FillColor = Color.Transparent;
            pnlBackground.FillColor2 = Color.Transparent;
            pnlBackground.GradientMode = System.Drawing.Drawing2D.LinearGradientMode.Vertical;
            pnlBackground.Location = new Point(0, 0);
            pnlBackground.Name = "pnlBackground";
            pnlBackground.ShadowDecoration.CustomizableEdges = customizableEdges6;
            pnlBackground.Size = new Size(708, 438);
            pnlBackground.TabIndex = 0;
            // 
            // guna2HtmlLabel2
            // 
            guna2HtmlLabel2.BackColor = Color.Transparent;
            guna2HtmlLabel2.Font = new Font("Sitka Banner", 12F);
            guna2HtmlLabel2.ForeColor = Color.White;
            guna2HtmlLabel2.Location = new Point(182, 284);
            guna2HtmlLabel2.Name = "guna2HtmlLabel2";
            guna2HtmlLabel2.Size = new Size(332, 31);
            guna2HtmlLabel2.TabIndex = 4;
            guna2HtmlLabel2.Text = "Computer Laboratory Management System";
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("Sitka Heading", 25.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            guna2HtmlLabel1.ForeColor = Color.White;
            guna2HtmlLabel1.Location = new Point(263, 227);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(170, 64);
            guna2HtmlLabel1.TabIndex = 3;
            guna2HtmlLabel1.Text = "VENTRIX";
            // 
            // picLogo
            // 
            picLogo.BackColor = Color.Transparent;
            picLogo.CustomizableEdges = customizableEdges1;
            picLogo.Image = Properties.Resources.Logo;
            picLogo.ImageRotate = 0F;
            picLogo.Location = new Point(216, 30);
            picLogo.Name = "picLogo";
            picLogo.ShadowDecoration.CustomizableEdges = customizableEdges2;
            picLogo.Size = new Size(264, 191);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 0;
            picLogo.TabStop = false;
            // 
            // lblStatus
            // 
            lblStatus.BackColor = Color.Transparent;
            lblStatus.Font = new Font("Sitka Text", 11F);
            lblStatus.ForeColor = Color.White;
            lblStatus.Location = new Point(48, 379);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(600, 38);
            lblStatus.TabIndex = 1;
            lblStatus.Text = "Initializing Ventrix...";
            lblStatus.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // progBar
            // 
            progBar.CustomizableEdges = customizableEdges3;
            progBar.Dock = DockStyle.Bottom;
            progBar.FillColor = Color.Transparent;
            progBar.Location = new Point(0, 433);
            progBar.Name = "progBar";
            progBar.ProgressColor = Color.FromArgb(33, 150, 243);
            progBar.ProgressColor2 = Color.Cyan;
            progBar.ShadowDecoration.CustomizableEdges = customizableEdges4;
            progBar.Size = new Size(708, 5);
            progBar.Style = ProgressBarStyle.Marquee;
            progBar.TabIndex = 2;
            progBar.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            // 
            // fadeTimer
            // 
            fadeTimer.Enabled = true;
            fadeTimer.Interval = 10;
            fadeTimer.Tick += fadeTimer_Tick;
            // 
            // guna2ShadowForm1
            // 
            guna2ShadowForm1.BorderRadius = 30;
            guna2ShadowForm1.TargetForm = this;
            // 
            // guna2BorderlessForm1
            // 
            guna2BorderlessForm1.BorderRadius = 30;
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // InitializingApp
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(708, 438);
            Controls.Add(pnlBackground);
            FormBorderStyle = FormBorderStyle.None;
            Name = "InitializingApp";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "FormSplash";
            pnlBackground.ResumeLayout(false);
            pnlBackground.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
        }

        private Guna.UI2.WinForms.Guna2GradientPanel pnlBackground;
        private Guna.UI2.WinForms.Guna2PictureBox picLogo;
        private System.Windows.Forms.Label lblStatus;
        private Guna.UI2.WinForms.Guna2ProgressBar progBar;
        private System.Windows.Forms.Timer fadeTimer;
        private Guna.UI2.WinForms.Guna2ShadowForm guna2ShadowForm1;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1; // Added
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel2;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
    }
}