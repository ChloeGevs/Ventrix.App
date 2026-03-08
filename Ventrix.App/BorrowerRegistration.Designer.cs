using System.Drawing;
using System.Windows.Forms;

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
            lblLoginLink = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblHeader = new Guna.UI2.WinForms.Guna2HtmlLabel();
            cmbRole = new Guna.UI2.WinForms.Guna2ComboBox();
            txtFirstName = new Guna.UI2.WinForms.Guna2TextBox();
            txtLastName = new Guna.UI2.WinForms.Guna2TextBox();
            txtMiddleName = new Guna.UI2.WinForms.Guna2TextBox();
            btnRegister = new Guna.UI2.WinForms.Guna2Button();
            txtSuffix = new Guna.UI2.WinForms.Guna2TextBox();
            chkNoSuffix = new Guna.UI2.WinForms.Guna2CheckBox();
            regTableLayout.SuspendLayout();
            pnlRegCard.SuspendLayout();
            SuspendLayout();
            // 
            // regTableLayout
            // 
            regTableLayout.BackColor = Color.Transparent;
            regTableLayout.BackgroundImage = Properties.Resources._5;
            regTableLayout.BackgroundImageLayout = ImageLayout.Stretch;
            regTableLayout.ColumnCount = 3;
            regTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            regTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 500F));
            regTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            regTableLayout.Controls.Add(pnlRegCard, 1, 1);
            regTableLayout.Dock = DockStyle.Fill;
            regTableLayout.Location = new Point(0, 0);
            regTableLayout.Name = "regTableLayout";
            regTableLayout.RowCount = 3;
            regTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            regTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 750F));
            regTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            regTableLayout.Size = new Size(1200, 850);
            regTableLayout.TabIndex = 0;
            // 
            // pnlRegCard
            // 
            pnlRegCard.BackColor = Color.Transparent;
            pnlRegCard.BorderRadius = 24;
            pnlRegCard.Controls.Add(lblLoginLink);
            pnlRegCard.Controls.Add(lblHeader);
            pnlRegCard.Controls.Add(cmbRole);
            pnlRegCard.Controls.Add(txtFirstName);
            pnlRegCard.Controls.Add(txtLastName);
            pnlRegCard.Controls.Add(txtMiddleName);
            pnlRegCard.Controls.Add(btnRegister);
            pnlRegCard.Controls.Add(txtSuffix);
            pnlRegCard.Controls.Add(chkNoSuffix);
            pnlRegCard.CustomizableEdges = customizableEdges13;
            pnlRegCard.Dock = DockStyle.Fill;
            pnlRegCard.FillColor = Color.White;
            pnlRegCard.Location = new Point(353, 53);
            pnlRegCard.Name = "pnlRegCard";
            pnlRegCard.ShadowDecoration.BorderRadius = 24;
            pnlRegCard.ShadowDecoration.Color = Color.FromArgb(40, 0, 0, 0);
            pnlRegCard.ShadowDecoration.CustomizableEdges = customizableEdges14;
            pnlRegCard.ShadowDecoration.Enabled = true;
            pnlRegCard.ShadowDecoration.Shadow = new Padding(0, 0, 10, 10);
            pnlRegCard.Size = new Size(494, 744);
            pnlRegCard.TabIndex = 0;
            // 
            // lblLoginLink
            // 
            lblLoginLink.BackColor = Color.Transparent;
            lblLoginLink.Cursor = Cursors.Hand;
            lblLoginLink.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblLoginLink.Location = new Point(115, 620);
            lblLoginLink.Name = "lblLoginLink";
            lblLoginLink.Size = new Size(273, 25);
            lblLoginLink.TabIndex = 8;
            lblLoginLink.Text = "Already registered? <a href=\"#\" style=\"color: #1565C0; text-decoration: underline;\">Borrower Portal</a>";
            // 
            // lblHeader
            // 
            lblHeader.AutoSize = false;
            lblHeader.BackColor = Color.Transparent;
            lblHeader.Font = new Font("Segoe UI", 21F, FontStyle.Bold);
            lblHeader.ForeColor = Color.FromArgb(13, 71, 161);
            lblHeader.Location = new Point(0, 45);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new Size(494, 90);
            lblHeader.TabIndex = 0;
            lblHeader.Text = "BORROWER REGISTRATION";
            lblHeader.TextAlignment = ContentAlignment.TopCenter;
            lblHeader.Click += lblHeader_Click;
            // 
            // cmbRole
            // 
            cmbRole.BackColor = Color.Transparent;
            cmbRole.BorderRadius = 8;
            cmbRole.CustomizableEdges = customizableEdges1;
            cmbRole.DrawMode = DrawMode.OwnerDrawFixed;
            cmbRole.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbRole.FocusedColor = Color.FromArgb(13, 71, 161);
            cmbRole.FocusedState.BorderColor = Color.FromArgb(13, 71, 161);
            cmbRole.Font = new Font("Segoe UI", 11F);
            cmbRole.ForeColor = Color.FromArgb(68, 88, 112);
            cmbRole.HoverState.BorderColor = Color.FromArgb(13, 71, 161);
            cmbRole.ItemHeight = 30;
            cmbRole.Items.AddRange(new object[] { "Student", "Faculty" });
            cmbRole.Location = new Point(57, 130);
            cmbRole.Name = "cmbRole";
            cmbRole.ShadowDecoration.CustomizableEdges = customizableEdges2;
            cmbRole.Size = new Size(380, 36);
            cmbRole.StartIndex = 0;
            cmbRole.TabIndex = 1;
            // 
            // txtFirstName
            // 
            txtFirstName.CustomizableEdges = customizableEdges3;
            txtFirstName.DefaultText = "";
            txtFirstName.FocusedState.BorderColor = Color.FromArgb(13, 71, 161);
            txtFirstName.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtFirstName.HoverState.BorderColor = Color.FromArgb(13, 71, 161);
            txtFirstName.Location = new Point(57, 195);
            txtFirstName.Margin = new Padding(3, 4, 3, 4);
            txtFirstName.Name = "txtFirstName";
            txtFirstName.PlaceholderText = "First Name";
            txtFirstName.SelectedText = "";
            txtFirstName.ShadowDecoration.CustomizableEdges = customizableEdges4;
            txtFirstName.Size = new Size(380, 50);
            txtFirstName.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            txtFirstName.TabIndex = 2;
            // 
            // txtLastName
            // 
            txtLastName.CustomizableEdges = customizableEdges5;
            txtLastName.DefaultText = "";
            txtLastName.FocusedState.BorderColor = Color.FromArgb(13, 71, 161);
            txtLastName.Font = new Font("Segoe UI", 11F);
            txtLastName.HoverState.BorderColor = Color.FromArgb(13, 71, 161);
            txtLastName.Location = new Point(57, 270);
            txtLastName.Margin = new Padding(3, 4, 3, 4);
            txtLastName.Name = "txtLastName";
            txtLastName.PlaceholderText = "Last Name";
            txtLastName.SelectedText = "";
            txtLastName.ShadowDecoration.CustomizableEdges = customizableEdges6;
            txtLastName.Size = new Size(380, 50);
            txtLastName.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            txtLastName.TabIndex = 3;
            // 
            // txtMiddleName
            // 
            txtMiddleName.CustomizableEdges = customizableEdges7;
            txtMiddleName.DefaultText = "";
            txtMiddleName.FocusedState.BorderColor = Color.FromArgb(13, 71, 161);
            txtMiddleName.Font = new Font("Segoe UI", 11F);
            txtMiddleName.HoverState.BorderColor = Color.FromArgb(13, 71, 161);
            txtMiddleName.Location = new Point(57, 345);
            txtMiddleName.Margin = new Padding(3, 4, 3, 4);
            txtMiddleName.Name = "txtMiddleName";
            txtMiddleName.PlaceholderText = "Middle Name (Optional)";
            txtMiddleName.SelectedText = "";
            txtMiddleName.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtMiddleName.Size = new Size(380, 50);
            txtMiddleName.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            txtMiddleName.TabIndex = 4;
            // 
            // btnRegister
            // 
            btnRegister.BorderRadius = 12;
            btnRegister.Cursor = Cursors.Hand;
            btnRegister.CustomizableEdges = customizableEdges9;
            btnRegister.FillColor = Color.FromArgb(13, 71, 161);
            btnRegister.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            btnRegister.ForeColor = Color.White;
            btnRegister.HoverState.FillColor = Color.FromArgb(9, 50, 115);
            btnRegister.Location = new Point(57, 540);
            btnRegister.Name = "btnRegister";
            btnRegister.ShadowDecoration.CustomizableEdges = customizableEdges10;
            btnRegister.Size = new Size(380, 55);
            btnRegister.TabIndex = 7;
            btnRegister.Text = "REGISTER";
            // 
            // txtSuffix
            // 
            txtSuffix.CustomizableEdges = customizableEdges11;
            txtSuffix.DefaultText = "";
            txtSuffix.FocusedState.BorderColor = Color.FromArgb(13, 71, 161);
            txtSuffix.Font = new Font("Segoe UI", 11F);
            txtSuffix.HoverState.BorderColor = Color.FromArgb(13, 71, 161);
            txtSuffix.Location = new Point(57, 420);
            txtSuffix.Margin = new Padding(3, 4, 3, 4);
            txtSuffix.Name = "txtSuffix";
            txtSuffix.PlaceholderText = "Suffix (e.g. Jr., III)";
            txtSuffix.SelectedText = "";
            txtSuffix.ShadowDecoration.CustomizableEdges = customizableEdges12;
            txtSuffix.Size = new Size(200, 50);
            txtSuffix.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            txtSuffix.TabIndex = 5;
            // 
            // chkNoSuffix
            // 
            chkNoSuffix.CheckedState.BorderColor = Color.FromArgb(13, 71, 161);
            chkNoSuffix.CheckedState.BorderRadius = 2;
            chkNoSuffix.CheckedState.BorderThickness = 0;
            chkNoSuffix.CheckedState.FillColor = Color.FromArgb(13, 71, 161);
            chkNoSuffix.Cursor = Cursors.Hand;
            chkNoSuffix.Font = new Font("Segoe UI Semibold", 10.5F);
            chkNoSuffix.ForeColor = Color.DimGray;
            chkNoSuffix.Location = new Point(285, 435);
            chkNoSuffix.Name = "chkNoSuffix";
            chkNoSuffix.Size = new Size(121, 28);
            chkNoSuffix.TabIndex = 6;
            chkNoSuffix.Text = "No Suffix";
            chkNoSuffix.UncheckedState.BorderColor = Color.DarkGray;
            chkNoSuffix.UncheckedState.BorderRadius = 2;
            chkNoSuffix.UncheckedState.BorderThickness = 1;
            chkNoSuffix.UncheckedState.FillColor = Color.White;
            // 
            // BorrowerRegistration
            // 
            ClientSize = new Size(1200, 850);
            Controls.Add(regTableLayout);
            Name = "BorrowerRegistration";
            StartPosition = FormStartPosition.CenterScreen;
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