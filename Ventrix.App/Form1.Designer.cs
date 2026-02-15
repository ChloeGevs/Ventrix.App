namespace Ventrix.App
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            mainTableLayout = new TableLayoutPanel();
            pnlLoginCard = new Guna.UI2.WinForms.Guna2Panel();
            lblLoginHeader = new Label();
            btnStaffToggle = new Guna.UI2.WinForms.Guna2Button();
            btnStudentToggle = new Guna.UI2.WinForms.Guna2Button();
            txtUsername = new Guna.UI2.WinForms.Guna2TextBox();
            txtPassword = new Guna.UI2.WinForms.Guna2TextBox();
            btnLogin = new Guna.UI2.WinForms.Guna2Button();
            lblCreateAccount = new Guna.UI2.WinForms.Guna2HtmlLabel();
            mainTableLayout.SuspendLayout();
            pnlLoginCard.SuspendLayout();
            SuspendLayout();
            // 
            // mainTableLayout
            // 
            mainTableLayout.BackColor = Color.Transparent;
            mainTableLayout.ColumnCount = 3;
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 450F));
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainTableLayout.Controls.Add(pnlLoginCard, 1, 1);
            mainTableLayout.Dock = DockStyle.Fill;
            mainTableLayout.Location = new Point(3, 64);
            mainTableLayout.Name = "mainTableLayout";
            mainTableLayout.RowCount = 3;
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 550F));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainTableLayout.Size = new Size(1194, 733);
            mainTableLayout.TabIndex = 0;
            // 
            // pnlLoginCard
            // 
            pnlLoginCard.BackColor = Color.Transparent;
            pnlLoginCard.BorderRadius = 20;
            pnlLoginCard.Controls.Add(lblLoginHeader);
            pnlLoginCard.Controls.Add(btnStaffToggle);
            pnlLoginCard.Controls.Add(btnStudentToggle);
            pnlLoginCard.Controls.Add(txtUsername);
            pnlLoginCard.Controls.Add(txtPassword);
            pnlLoginCard.Controls.Add(btnLogin);
            pnlLoginCard.Controls.Add(lblCreateAccount);
            pnlLoginCard.CustomizableEdges = customizableEdges11;
            pnlLoginCard.Dock = DockStyle.Fill;
            pnlLoginCard.Location = new Point(375, 94);
            pnlLoginCard.Name = "pnlLoginCard";
            pnlLoginCard.ShadowDecoration.CustomizableEdges = customizableEdges12;
            pnlLoginCard.ShadowDecoration.Depth = 20;
            pnlLoginCard.ShadowDecoration.Enabled = true;
            pnlLoginCard.Size = new Size(444, 544);
            pnlLoginCard.TabIndex = 0;
            // 
            // lblLoginHeader
            // 
            lblLoginHeader.Font = new Font("Sitka Heading", 22.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblLoginHeader.ForeColor = Color.FromArgb(13, 71, 161);
            lblLoginHeader.Location = new Point(1, 9);
            lblLoginHeader.Name = "lblLoginHeader";
            lblLoginHeader.Size = new Size(450, 60);
            lblLoginHeader.TabIndex = 0;
            lblLoginHeader.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // btnStaffToggle
            // 
            btnStaffToggle.BorderRadius = 15;
            btnStaffToggle.CustomizableEdges = customizableEdges1;
            btnStaffToggle.Font = new Font("Sitka Banner", 10.1999989F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnStaffToggle.ForeColor = Color.White;
            btnStaffToggle.Location = new Point(95, 120);
            btnStaffToggle.Name = "btnStaffToggle";
            btnStaffToggle.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnStaffToggle.Size = new Size(125, 40);
            btnStaffToggle.TabIndex = 1;
            btnStaffToggle.Text = "STAFF";
            // 
            // btnStudentToggle
            // 
            btnStudentToggle.BorderRadius = 15;
            btnStudentToggle.CustomizableEdges = customizableEdges3;
            btnStudentToggle.Font = new Font("Sitka Banner", 10.1999989F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnStudentToggle.ForeColor = Color.White;
            btnStudentToggle.Location = new Point(230, 120);
            btnStudentToggle.Name = "btnStudentToggle";
            btnStudentToggle.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnStudentToggle.Size = new Size(125, 40);
            btnStudentToggle.TabIndex = 2;
            btnStudentToggle.Text = "STUDENT";
            // 
            // txtUsername
            // 
            txtUsername.BorderRadius = 10;
            txtUsername.CustomizableEdges = customizableEdges5;
            txtUsername.DefaultText = "";
            txtUsername.Font = new Font("Segoe UI", 9F);
            txtUsername.Location = new Point(50, 200);
            txtUsername.Margin = new Padding(3, 4, 3, 4);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "";
            txtUsername.SelectedText = "";
            txtUsername.ShadowDecoration.CustomizableEdges = customizableEdges6;
            txtUsername.Size = new Size(350, 50);
            txtUsername.TabIndex = 3;
            // 
            // txtPassword
            // 
            txtPassword.BorderRadius = 10;
            txtPassword.CustomizableEdges = customizableEdges7;
            txtPassword.DefaultText = "";
            txtPassword.Font = new Font("Segoe UI", 9F);
            txtPassword.Location = new Point(50, 270);
            txtPassword.Margin = new Padding(3, 4, 3, 4);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '●';
            txtPassword.PlaceholderText = "Password";
            txtPassword.SelectedText = "";
            txtPassword.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtPassword.Size = new Size(350, 50);
            txtPassword.TabIndex = 4;
            // 
            // btnLogin
            // 
            btnLogin.BorderRadius = 12;
            btnLogin.CustomizableEdges = customizableEdges9;
            btnLogin.FillColor = Color.FromArgb(13, 71, 161);
            btnLogin.Font = new Font("Sitka Banner", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(50, 360);
            btnLogin.Name = "btnLogin";
            btnLogin.ShadowDecoration.CustomizableEdges = customizableEdges10;
            btnLogin.Size = new Size(350, 55);
            btnLogin.TabIndex = 5;
            btnLogin.Text = "SIGN IN";
            // 
            // lblCreateAccount
            // 
            lblCreateAccount.BackColor = Color.Transparent;
            lblCreateAccount.Font = new Font("Sitka Banner", 10.1999989F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCreateAccount.Location = new Point(143, 421);
            lblCreateAccount.Name = "lblCreateAccount";
            lblCreateAccount.Size = new Size(176, 26);
            lblCreateAccount.TabIndex = 6;
            lblCreateAccount.Text = "New Staff? <font color=\"#1565C0\"><b>Register Here</b></font>";
            // 
            // Form1
            // 
            ClientSize = new Size(1200, 800);
            Controls.Add(mainTableLayout);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Ventrix | System Login";
            mainTableLayout.ResumeLayout(false);
            pnlLoginCard.ResumeLayout(false);
            pnlLoginCard.PerformLayout();
            ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel mainTableLayout;
        private Guna.UI2.WinForms.Guna2Panel pnlLoginCard;
        private System.Windows.Forms.Label lblLoginHeader;
        private Guna.UI2.WinForms.Guna2Button btnStaffToggle;
        private Guna.UI2.WinForms.Guna2Button btnStudentToggle;
        private Guna.UI2.WinForms.Guna2TextBox txtUsername;
        private Guna.UI2.WinForms.Guna2TextBox txtPassword;
        private Guna.UI2.WinForms.Guna2Button btnLogin;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCreateAccount;
    }
}