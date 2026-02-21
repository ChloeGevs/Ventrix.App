namespace Ventrix.App
{
    partial class BorrowerRegistration
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
            lblHeader = new Guna.UI2.WinForms.Guna2HtmlLabel();
            cmbRole = new Guna.UI2.WinForms.Guna2ComboBox();
            txtFirstName = new Guna.UI2.WinForms.Guna2TextBox();
            txtLastName = new Guna.UI2.WinForms.Guna2TextBox();
            txtMiddleName = new Guna.UI2.WinForms.Guna2TextBox();
            btnRegister = new Guna.UI2.WinForms.Guna2Button();
            lblLoginLink = new Guna.UI2.WinForms.Guna2HtmlLabel();
            txtSuffix = new Guna.UI2.WinForms.Guna2TextBox();
            chkNoSuffix = new Guna.UI2.WinForms.Guna2CheckBox();
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
            pnlRegCard.Controls.Add(btnRegister);
            pnlRegCard.Controls.Add(lblLoginLink);
            pnlRegCard.Controls.Add(txtSuffix);
            pnlRegCard.Controls.Add(chkNoSuffix);
            pnlRegCard.CustomizableEdges = customizableEdges13;
            pnlRegCard.FillColor = Color.White;
            pnlRegCard.Location = new Point(350, 19);
            pnlRegCard.Name = "pnlRegCard";
            pnlRegCard.ShadowDecoration.BorderRadius = 15;
            pnlRegCard.ShadowDecoration.CustomizableEdges = customizableEdges14;
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
            txtFirstName.Font = new Font("Segoe UI Variable Text", 10F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtFirstName.Location = new Point(57, 197);
            txtFirstName.Margin = new Padding(3, 4, 3, 4);
            txtFirstName.Name = "txtFirstName";
            txtFirstName.PlaceholderText = "First Name";
            txtFirstName.SelectedText = "";
            txtFirstName.ShadowDecoration.CustomizableEdges = customizableEdges4;
            txtFirstName.Size = new Size(380, 51);
            txtFirstName.TabIndex = 2;
            // 
            // txtLastName
            // 
            txtLastName.BorderRadius = 10;
            txtLastName.CustomizableEdges = customizableEdges5;
            txtLastName.DefaultText = "";
            txtLastName.Font = new Font("Segoe UI Variable Text", 10F);
            txtLastName.Location = new Point(57, 273);
            txtLastName.Margin = new Padding(3, 4, 3, 4);
            txtLastName.Name = "txtLastName";
            txtLastName.PlaceholderText = "Last Name";
            txtLastName.SelectedText = "";
            txtLastName.ShadowDecoration.CustomizableEdges = customizableEdges6;
            txtLastName.Size = new Size(380, 52);
            txtLastName.TabIndex = 3;
            // 
            // txtMiddleName
            // 
            txtMiddleName.BorderRadius = 10;
            txtMiddleName.CustomizableEdges = customizableEdges7;
            txtMiddleName.DefaultText = "";
            txtMiddleName.Font = new Font("Segoe UI Variable Text", 10F);
            txtMiddleName.Location = new Point(57, 350);
            txtMiddleName.Margin = new Padding(3, 4, 3, 4);
            txtMiddleName.Name = "txtMiddleName";
            txtMiddleName.PlaceholderText = "Middle Name";
            txtMiddleName.SelectedText = "";
            txtMiddleName.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtMiddleName.Size = new Size(380, 55);
            txtMiddleName.TabIndex = 4;
            // 
            // btnRegister
            // 
            btnRegister.BorderRadius = 10;
            btnRegister.CustomizableEdges = customizableEdges9;
            btnRegister.FillColor = Color.FromArgb(13, 71, 161);
            btnRegister.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            btnRegister.ForeColor = Color.White;
            btnRegister.Location = new Point(57, 544);
            btnRegister.Name = "btnRegister";
            btnRegister.ShadowDecoration.CustomizableEdges = customizableEdges10;
            btnRegister.Size = new Size(380, 55);
            btnRegister.TabIndex = 7;
            btnRegister.Text = "REGISTER";
            // 
            // lblLoginLink
            // 
            lblLoginLink.BackColor = Color.Transparent;
            lblLoginLink.Location = new Point(130, 605);
            lblLoginLink.Name = "lblLoginLink";
            lblLoginLink.Size = new Size(240, 22);
            lblLoginLink.TabIndex = 8;
            lblLoginLink.Text = "Already registered? <font color='#1565C0'><u>Borrower Portal</u></font>";
            // 
            // txtSuffix
            // 
            txtSuffix.BorderRadius = 10;
            txtSuffix.CustomizableEdges = customizableEdges11;
            txtSuffix.DefaultText = "";
            txtSuffix.Font = new Font("Segoe UI Variable Text", 10F);
            txtSuffix.Location = new Point(57, 412);
            txtSuffix.Margin = new Padding(3, 4, 3, 4);
            txtSuffix.Name = "txtSuffix";
            txtSuffix.PlaceholderText = "Suffix (e.g. Jr., III)";
            txtSuffix.SelectedText = "";
            txtSuffix.ShadowDecoration.CustomizableEdges = customizableEdges12;
            txtSuffix.Size = new Size(200, 54);
            txtSuffix.TabIndex = 5;
            // 
            // chkNoSuffix
            // 
            chkNoSuffix.CheckedState.BorderColor = Color.FromArgb(13, 71, 161);
            chkNoSuffix.CheckedState.BorderRadius = 2;
            chkNoSuffix.CheckedState.BorderThickness = 0;
            chkNoSuffix.CheckedState.FillColor = Color.FromArgb(13, 71, 161);
            chkNoSuffix.Font = new Font("Segoe UI Variable Text", 10F);
            chkNoSuffix.Location = new Point(273, 424);
            chkNoSuffix.Name = "chkNoSuffix";
            chkNoSuffix.Size = new Size(121, 28);
            chkNoSuffix.TabIndex = 6;
            chkNoSuffix.Text = "None";
            chkNoSuffix.UncheckedState.BorderRadius = 0;
            chkNoSuffix.UncheckedState.BorderThickness = 0;
            // 
            // BorrowerRegistration
            // 
            ClientSize = new Size(1200, 850);
            Controls.Add(regTableLayout);
            Name = "BorrowerRegistration";
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
        private Guna.UI2.WinForms.Guna2Button btnRegister;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblLoginLink;
        private Guna.UI2.WinForms.Guna2TextBox txtSuffix;
        private Guna.UI2.WinForms.Guna2CheckBox chkNoSuffix;
    }
}