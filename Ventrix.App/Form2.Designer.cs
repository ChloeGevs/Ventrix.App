namespace Ventrix.App
{
    partial class Form2
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            regTableLayout = new TableLayoutPanel();
            pnlRegCard = new Guna.UI2.WinForms.Guna2Panel();
            lblHeader = new Label();
            txtFullName = new Guna.UI2.WinForms.Guna2TextBox();
            txtSchoolID = new Guna.UI2.WinForms.Guna2TextBox();
            txtUsername = new Guna.UI2.WinForms.Guna2TextBox();
            txtPassword = new Guna.UI2.WinForms.Guna2TextBox();
            cmbRole = new Guna.UI2.WinForms.Guna2ComboBox();
            btnRegister = new Guna.UI2.WinForms.Guna2Button();
            lblLoginLink = new Guna.UI2.WinForms.Guna2HtmlLabel();
            regTableLayout.SuspendLayout();
            pnlRegCard.SuspendLayout();
            SuspendLayout();
            // 
            // regTableLayout
            // 
            regTableLayout.ColumnCount = 3;
            regTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            regTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 500F));
            regTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            regTableLayout.Controls.Add(pnlRegCard, 1, 1);
            regTableLayout.Dock = DockStyle.Fill;
            regTableLayout.Location = new Point(3, 64);
            regTableLayout.Name = "regTableLayout";
            regTableLayout.RowCount = 3;
            regTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            regTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 680F));
            regTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            regTableLayout.Size = new Size(1194, 783);
            regTableLayout.TabIndex = 0;
            // 
            // pnlRegCard
            // 
            pnlRegCard.BackColor = Color.Transparent;
            pnlRegCard.BorderRadius = 15;
            pnlRegCard.Controls.Add(lblHeader);
            pnlRegCard.Controls.Add(txtFullName);
            pnlRegCard.Controls.Add(txtSchoolID);
            pnlRegCard.Controls.Add(txtUsername);
            pnlRegCard.Controls.Add(txtPassword);
            pnlRegCard.Controls.Add(cmbRole);
            pnlRegCard.Controls.Add(btnRegister);
            pnlRegCard.Controls.Add(lblLoginLink);
            pnlRegCard.CustomizableEdges = customizableEdges13;
            pnlRegCard.Dock = DockStyle.Fill;
            pnlRegCard.Font = new Font("Sitka Banner", 10.1999989F, FontStyle.Regular, GraphicsUnit.Point, 0);
            pnlRegCard.Location = new Point(350, 54);
            pnlRegCard.Name = "pnlRegCard";
            pnlRegCard.ShadowDecoration.CustomizableEdges = customizableEdges14;
            pnlRegCard.ShadowDecoration.Enabled = true;
            pnlRegCard.Size = new Size(494, 674);
            pnlRegCard.TabIndex = 0;
            // 
            // lblHeader
            // 
            lblHeader.Font = new Font("Sitka Heading", 22.2F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblHeader.ForeColor = Color.FromArgb(13, 71, 161);
            lblHeader.Location = new Point(0, 0);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new Size(500, 80);
            lblHeader.TabIndex = 0;
            lblHeader.Text = "STAFF REGISTRATION";
            lblHeader.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // txtFullName
            // 
            txtFullName.CustomizableEdges = customizableEdges1;
            txtFullName.DefaultText = "";
            txtFullName.Font = new Font("Sitka Banner", 10.1999989F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtFullName.Location = new Point(60, 110);
            txtFullName.Margin = new Padding(3, 4, 3, 4);
            txtFullName.Name = "txtFullName";
            txtFullName.PlaceholderText = "Full Name";
            txtFullName.SelectedText = "";
            txtFullName.ShadowDecoration.CustomizableEdges = customizableEdges2;
            txtFullName.Size = new Size(380, 45);
            txtFullName.TabIndex = 1;
            // 
            // txtSchoolID
            // 
            txtSchoolID.CustomizableEdges = customizableEdges3;
            txtSchoolID.DefaultText = "";
            txtSchoolID.Font = new Font("Sitka Banner", 10.1999989F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtSchoolID.Location = new Point(60, 175);
            txtSchoolID.Margin = new Padding(3, 4, 3, 4);
            txtSchoolID.Name = "txtSchoolID";
            txtSchoolID.PlaceholderText = "School ID";
            txtSchoolID.SelectedText = "";
            txtSchoolID.ShadowDecoration.CustomizableEdges = customizableEdges4;
            txtSchoolID.Size = new Size(380, 45);
            txtSchoolID.TabIndex = 2;
            // 
            // txtUsername
            // 
            txtUsername.CustomizableEdges = customizableEdges5;
            txtUsername.DefaultText = "";
            txtUsername.Font = new Font("Sitka Banner", 10.1999989F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtUsername.Location = new Point(60, 240);
            txtUsername.Margin = new Padding(3, 4, 3, 4);
            txtUsername.Name = "txtUsername";
            txtUsername.PlaceholderText = "Username";
            txtUsername.SelectedText = "";
            txtUsername.ShadowDecoration.CustomizableEdges = customizableEdges6;
            txtUsername.Size = new Size(380, 45);
            txtUsername.TabIndex = 3;
            // 
            // txtPassword
            // 
            txtPassword.CustomizableEdges = customizableEdges7;
            txtPassword.DefaultText = "";
            txtPassword.Font = new Font("Sitka Banner", 10.1999989F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtPassword.Location = new Point(60, 305);
            txtPassword.Margin = new Padding(3, 4, 3, 4);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '●';
            txtPassword.PlaceholderText = "Password";
            txtPassword.SelectedText = "";
            txtPassword.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtPassword.Size = new Size(380, 45);
            txtPassword.TabIndex = 4;
            // 
            // cmbRole
            // 
            cmbRole.BackColor = Color.Transparent;
            cmbRole.CustomizableEdges = customizableEdges9;
            cmbRole.DrawMode = DrawMode.OwnerDrawFixed;
            cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRole.FocusedColor = Color.Empty;
            cmbRole.Font = new Font("Sitka Banner", 10.1999989F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbRole.ForeColor = Color.FromArgb(68, 88, 112);
            cmbRole.ItemHeight = 30;
            cmbRole.Items.AddRange(new object[] { "Lab Administrator", "Lab Assistant", "Student" });
            cmbRole.Location = new Point(60, 370);
            cmbRole.Name = "cmbRole";
            cmbRole.ShadowDecoration.CustomizableEdges = customizableEdges10;
            cmbRole.Size = new Size(380, 36);
            cmbRole.StartIndex = 0;
            cmbRole.TabIndex = 5;
            // 
            // btnRegister
            // 
            btnRegister.CustomizableEdges = customizableEdges11;
            btnRegister.FillColor = Color.FromArgb(13, 71, 161);
            btnRegister.Font = new Font("Sitka Banner", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnRegister.ForeColor = Color.White;
            btnRegister.Location = new Point(60, 450);
            btnRegister.Name = "btnRegister";
            btnRegister.ShadowDecoration.CustomizableEdges = customizableEdges12;
            btnRegister.Size = new Size(380, 55);
            btnRegister.TabIndex = 6;
            btnRegister.Text = "CREATE ACCOUNT";
            // 
            // lblLoginLink
            // 
            lblLoginLink.BackColor = Color.Transparent;
            lblLoginLink.Font = new Font("Sitka Banner", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblLoginLink.Location = new Point(169, 511);
            lblLoginLink.Name = "lblLoginLink";
            lblLoginLink.Size = new Size(178, 28);
            lblLoginLink.TabIndex = 7;
            lblLoginLink.Text = "Already registered? <font color=\"#1565C0\"><u>Login</u></font>";
            // 
            // Form2
            // 
            ClientSize = new Size(1200, 850);
            Controls.Add(regTableLayout);
            Name = "Form2";
            Text = "Ventrix | Registration Portal";
            regTableLayout.ResumeLayout(false);
            pnlRegCard.ResumeLayout(false);
            pnlRegCard.PerformLayout();
            ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel regTableLayout;
        private Guna.UI2.WinForms.Guna2Panel pnlRegCard;
        private System.Windows.Forms.Label lblHeader;
        private Guna.UI2.WinForms.Guna2TextBox txtFullName;
        private Guna.UI2.WinForms.Guna2TextBox txtSchoolID;
        private Guna.UI2.WinForms.Guna2TextBox txtUsername;
        private Guna.UI2.WinForms.Guna2TextBox txtPassword;
        private Guna.UI2.WinForms.Guna2ComboBox cmbRole;
        private Guna.UI2.WinForms.Guna2Button btnRegister;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblLoginLink;
    }
}