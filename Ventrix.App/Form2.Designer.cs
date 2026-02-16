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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges15 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges16 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            regTableLayout = new TableLayoutPanel();
            pnlRegCard = new Guna.UI2.WinForms.Guna2Panel();
            lblHeader = new Guna.UI2.WinForms.Guna2HtmlLabel();
            cmbRole = new Guna.UI2.WinForms.Guna2ComboBox();
            txtFirstName = new Guna.UI2.WinForms.Guna2TextBox();
            txtLastName = new Guna.UI2.WinForms.Guna2TextBox();
            txtMiddleName = new Guna.UI2.WinForms.Guna2TextBox();
            txtSchoolID = new Guna.UI2.WinForms.Guna2TextBox();
            txtPassword = new Guna.UI2.WinForms.Guna2TextBox();
            btnRegister = new Guna.UI2.WinForms.Guna2Button();
            lblLoginLink = new Guna.UI2.WinForms.Guna2HtmlLabel();
            regTableLayout.SuspendLayout();
            pnlRegCard.SuspendLayout();
            SuspendLayout();
            // 
            // regTableLayout
            // 
            regTableLayout.BackgroundImage = Properties.Resources._5_imresizer__1_;
            regTableLayout.BackgroundImageLayout = ImageLayout.Stretch;
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
            regTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 750F));
            regTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            regTableLayout.Size = new Size(1194, 783);
            regTableLayout.TabIndex = 0;
            // 
            // pnlRegCard
            // 
            pnlRegCard.BackColor = Color.Transparent;
            pnlRegCard.BorderRadius = 15;
            pnlRegCard.Controls.Add(lblHeader);
            pnlRegCard.Controls.Add(cmbRole);
            pnlRegCard.Controls.Add(txtFirstName);
            pnlRegCard.Controls.Add(txtLastName);
            pnlRegCard.Controls.Add(txtMiddleName);
            pnlRegCard.Controls.Add(txtSchoolID);
            pnlRegCard.Controls.Add(txtPassword);
            pnlRegCard.Controls.Add(btnRegister);
            pnlRegCard.Controls.Add(lblLoginLink);
            pnlRegCard.CustomizableEdges = customizableEdges15;
            pnlRegCard.FillColor = Color.White;
            pnlRegCard.Location = new Point(350, 19);
            pnlRegCard.Name = "pnlRegCard";
            pnlRegCard.ShadowDecoration.CustomizableEdges = customizableEdges16;
            pnlRegCard.ShadowDecoration.Enabled = true;
            pnlRegCard.Size = new Size(494, 730);
            pnlRegCard.TabIndex = 0;
            // 
            // lblHeader
            // 
            lblHeader.AutoSize = false;
            lblHeader.BackColor = Color.Transparent;
            lblHeader.Font = new Font("Sitka Heading", 22F, FontStyle.Bold);
            lblHeader.ForeColor = Color.FromArgb(13, 71, 161);
            lblHeader.Location = new Point(0, 20);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new Size(494, 70);
            lblHeader.TabIndex = 0;
            lblHeader.Text = "STUDENT REGISTRATION";
            lblHeader.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // cmbRole
            // 
            cmbRole.BackColor = Color.Transparent;
            cmbRole.BorderRadius = 10;
            cmbRole.CustomizableEdges = customizableEdges1;
            cmbRole.DrawMode = DrawMode.OwnerDrawFixed;
            cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRole.FocusedColor = Color.Empty;
            cmbRole.Font = new Font("Segoe UI", 10F);
            cmbRole.ForeColor = Color.FromArgb(68, 88, 112);
            cmbRole.ItemHeight = 30;
            cmbRole.Items.AddRange(new object[] { "Student", "Staff" });
            cmbRole.Location = new Point(57, 126);
            cmbRole.Name = "cmbRole";
            cmbRole.ShadowDecoration.CustomizableEdges = customizableEdges2;
            cmbRole.Size = new Size(380, 36);
            cmbRole.StartIndex = 0;
            cmbRole.TabIndex = 1;
            // 
            // txtFirstName
            // 
            txtFirstName.BorderRadius = 10;
            txtFirstName.CustomizableEdges = customizableEdges3;
            txtFirstName.DefaultText = "";
            txtFirstName.Font = new Font("Segoe UI", 9F);
            txtFirstName.Location = new Point(57, 197);
            txtFirstName.Margin = new Padding(3, 4, 3, 4);
            txtFirstName.Name = "txtFirstName";
            txtFirstName.PlaceholderText = "First Name";
            txtFirstName.SelectedText = "";
            txtFirstName.ShadowDecoration.CustomizableEdges = customizableEdges4;
            txtFirstName.Size = new Size(380, 45);
            txtFirstName.TabIndex = 2;
            // 
            // txtLastName
            // 
            txtLastName.BorderRadius = 10;
            txtLastName.CustomizableEdges = customizableEdges5;
            txtLastName.DefaultText = "";
            txtLastName.Font = new Font("Segoe UI", 9F);
            txtLastName.Location = new Point(57, 257);
            txtLastName.Margin = new Padding(3, 4, 3, 4);
            txtLastName.Name = "txtLastName";
            txtLastName.PlaceholderText = "Last Name";
            txtLastName.SelectedText = "";
            txtLastName.ShadowDecoration.CustomizableEdges = customizableEdges6;
            txtLastName.Size = new Size(380, 45);
            txtLastName.TabIndex = 3;
            // 
            // txtMiddleName
            // 
            txtMiddleName.BorderRadius = 10;
            txtMiddleName.CustomizableEdges = customizableEdges7;
            txtMiddleName.DefaultText = "";
            txtMiddleName.Font = new Font("Segoe UI", 9F);
            txtMiddleName.Location = new Point(57, 317);
            txtMiddleName.Margin = new Padding(3, 4, 3, 4);
            txtMiddleName.Name = "txtMiddleName";
            txtMiddleName.PlaceholderText = "Middle Name";
            txtMiddleName.SelectedText = "";
            txtMiddleName.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtMiddleName.Size = new Size(380, 45);
            txtMiddleName.TabIndex = 4;
            // 
            // txtSchoolID
            // 
            txtSchoolID.BorderRadius = 10;
            txtSchoolID.CustomizableEdges = customizableEdges9;
            txtSchoolID.DefaultText = "";
            txtSchoolID.Font = new Font("Segoe UI", 9F);
            txtSchoolID.Location = new Point(57, 377);
            txtSchoolID.Margin = new Padding(3, 4, 3, 4);
            txtSchoolID.Name = "txtSchoolID";
            txtSchoolID.PlaceholderText = "Student ID (e.g. 2024-XXXX)";
            txtSchoolID.SelectedText = "";
            txtSchoolID.ShadowDecoration.CustomizableEdges = customizableEdges10;
            txtSchoolID.Size = new Size(380, 45);
            txtSchoolID.TabIndex = 5;
            // 
            // txtPassword
            // 
            txtPassword.BorderRadius = 10;
            txtPassword.CustomizableEdges = customizableEdges11;
            txtPassword.DefaultText = "";
            txtPassword.Font = new Font("Segoe UI", 9F);
            txtPassword.IconRight = Properties.Resources.eye;
            txtPassword.Location = new Point(57, 437);
            txtPassword.Margin = new Padding(3, 4, 3, 4);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '●';
            txtPassword.PlaceholderText = "Create Password";
            txtPassword.SelectedText = "";
            txtPassword.ShadowDecoration.CustomizableEdges = customizableEdges12;
            txtPassword.Size = new Size(380, 45);
            txtPassword.TabIndex = 6;
            // 
            // btnRegister
            // 
            btnRegister.BorderRadius = 10;
            btnRegister.CustomizableEdges = customizableEdges13;
            btnRegister.FillColor = Color.FromArgb(13, 71, 161);
            btnRegister.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnRegister.ForeColor = Color.White;
            btnRegister.Location = new Point(57, 544);
            btnRegister.Name = "btnRegister";
            btnRegister.ShadowDecoration.CustomizableEdges = customizableEdges14;
            btnRegister.Size = new Size(380, 55);
            btnRegister.TabIndex = 7;
            btnRegister.Text = "REGISTER";
            // 
            // lblLoginLink
            // 
            lblLoginLink.BackColor = Color.Transparent;
            lblLoginLink.Location = new Point(170, 605);
            lblLoginLink.Name = "lblLoginLink";
            lblLoginLink.Size = new Size(173, 22);
            lblLoginLink.TabIndex = 8;
            lblLoginLink.Text = "Already registered? <font color='#1565C0'><u>Login</u></font>";
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
        private Guna.UI2.WinForms.Guna2HtmlLabel lblHeader;
        private Guna.UI2.WinForms.Guna2ComboBox cmbRole;
        private Guna.UI2.WinForms.Guna2TextBox txtFirstName;
        private Guna.UI2.WinForms.Guna2TextBox txtLastName;
        private Guna.UI2.WinForms.Guna2TextBox txtMiddleName;
        private Guna.UI2.WinForms.Guna2TextBox txtSchoolID;
        private Guna.UI2.WinForms.Guna2TextBox txtPassword;
        private Guna.UI2.WinForms.Guna2Button btnRegister;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblLoginLink;
    }
}