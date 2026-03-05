using System.Drawing;
using System.Windows.Forms;

namespace Ventrix.App
{
    partial class BorrowerPortal
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges23 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges24 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges15 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges16 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges17 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges18 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges19 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges20 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges21 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges22 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            mainTableLayout = new TableLayoutPanel();
            pnlLoginCard = new Guna.UI2.WinForms.Guna2Panel();
            lblEquipmentList = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblCreateAccount = new Guna.UI2.WinForms.Guna2HtmlLabel();
            txtPassword = new Guna.UI2.WinForms.Guna2TextBox();
            btnAdminToggle = new Guna.UI2.WinForms.Guna2Button();
            btnStudentToggle = new Guna.UI2.WinForms.Guna2Button();
            lblLoginHeader = new Guna.UI2.WinForms.Guna2HtmlLabel();
            txtStudentId = new Guna.UI2.WinForms.Guna2TextBox();
            cmbListEquipments = new Guna.UI2.WinForms.Guna2ComboBox();
            lblQuantity = new Guna.UI2.WinForms.Guna2HtmlLabel();
            numQuantity = new Guna.UI2.WinForms.Guna2NumericUpDown();
            lblSubject = new Guna.UI2.WinForms.Guna2HtmlLabel();
            txtSubject = new Guna.UI2.WinForms.Guna2TextBox();
            btnReturn = new Guna.UI2.WinForms.Guna2Button();
            btnBorrow = new Guna.UI2.WinForms.Guna2Button();
            btnLogin = new Guna.UI2.WinForms.Guna2Button();
            cmbGradeLevel = new Guna.UI2.WinForms.Guna2ComboBox();
            mainTableLayout.SuspendLayout();
            pnlLoginCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numQuantity).BeginInit();
            SuspendLayout();
            // 
            // mainTableLayout
            // 
            mainTableLayout.BackColor = Color.Transparent;
            mainTableLayout.BackgroundImage = Properties.Resources._5_imresizer__1_;
            mainTableLayout.BackgroundImageLayout = ImageLayout.Stretch;
            mainTableLayout.ColumnCount = 3;
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 600F));
            mainTableLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            mainTableLayout.Controls.Add(pnlLoginCard, 1, 1);
            mainTableLayout.Dock = DockStyle.Fill;
            mainTableLayout.Location = new Point(0, 0);
            mainTableLayout.Name = "mainTableLayout";
            mainTableLayout.RowCount = 3;
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 750F));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainTableLayout.Size = new Size(1200, 800);
            mainTableLayout.TabIndex = 0;
            // 
            // pnlLoginCard
            // 
            pnlLoginCard.BackColor = Color.Transparent;
            pnlLoginCard.BorderRadius = 20;
            pnlLoginCard.Controls.Add(lblEquipmentList);
            pnlLoginCard.Controls.Add(lblCreateAccount);
            pnlLoginCard.Controls.Add(txtPassword);
            pnlLoginCard.Controls.Add(btnAdminToggle);
            pnlLoginCard.Controls.Add(btnStudentToggle);
            pnlLoginCard.Controls.Add(lblLoginHeader);
            pnlLoginCard.Controls.Add(txtStudentId);
            pnlLoginCard.Controls.Add(cmbListEquipments);
            pnlLoginCard.Controls.Add(lblQuantity);
            pnlLoginCard.Controls.Add(numQuantity);
            pnlLoginCard.Controls.Add(lblSubject);
            pnlLoginCard.Controls.Add(txtSubject);
            pnlLoginCard.Controls.Add(btnReturn);
            pnlLoginCard.Controls.Add(btnBorrow);
            pnlLoginCard.Controls.Add(btnLogin);
            pnlLoginCard.Controls.Add(cmbGradeLevel);
            pnlLoginCard.CustomizableEdges = customizableEdges23;
            pnlLoginCard.Dock = DockStyle.Fill;
            pnlLoginCard.FillColor = Color.White;
            pnlLoginCard.Location = new Point(303, 28);
            pnlLoginCard.Name = "pnlLoginCard";
            pnlLoginCard.ShadowDecoration.BorderRadius = 24;
            pnlLoginCard.ShadowDecoration.Color = Color.FromArgb(40, 0, 0, 0);
            pnlLoginCard.ShadowDecoration.CustomizableEdges = customizableEdges24;
            pnlLoginCard.ShadowDecoration.Enabled = true;
            pnlLoginCard.ShadowDecoration.Shadow = new Padding(0, 0, 10, 10);
            pnlLoginCard.Size = new Size(594, 744);
            pnlLoginCard.TabIndex = 0;
            // 
            // lblEquipmentList
            // 
            lblEquipmentList.BackColor = Color.Transparent;
            lblEquipmentList.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblEquipmentList.ForeColor = Color.DimGray;
            lblEquipmentList.Location = new Point(60, 275);
            lblEquipmentList.Name = "lblEquipmentList";
            lblEquipmentList.Size = new Size(144, 25);
            lblEquipmentList.TabIndex = 16;
            lblEquipmentList.Text = "List of Equipments";
            // 
            // lblCreateAccount
            // 
            lblCreateAccount.BackColor = Color.Transparent;
            lblCreateAccount.Cursor = Cursors.Hand;
            lblCreateAccount.Font = new Font("Segoe UI", 10.5F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCreateAccount.Location = new Point(205, 630);
            lblCreateAccount.Name = "lblCreateAccount";
            lblCreateAccount.Size = new Size(205, 25);
            lblCreateAccount.TabIndex = 12;
            lblCreateAccount.Text = "No account? <a href=\"#\" style=\"color: #1565C0; text-decoration: underline;\">Register here</a>";
            // 
            // txtPassword
            // 
            txtPassword.Cursor = Cursors.IBeam;
            txtPassword.CustomizableEdges = customizableEdges1;
            txtPassword.DefaultText = "";
            txtPassword.Font = new Font("Segoe UI", 11F);
            txtPassword.IconLeft = Properties.Resources.locked;
            txtPassword.IconRight = Properties.Resources.eye;
            txtPassword.Location = new Point(60, 290);
            txtPassword.Margin = new Padding(3, 4, 3, 4);
            txtPassword.Name = "txtPassword";
            txtPassword.PasswordChar = '●';
            txtPassword.PlaceholderText = "Password";
            txtPassword.SelectedText = "";
            txtPassword.ShadowDecoration.CustomizableEdges = customizableEdges2;
            txtPassword.Size = new Size(480, 50);
            txtPassword.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            txtPassword.TabIndex = 4;
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Visible = false;
            // 
            // btnAdminToggle
            // 
            btnAdminToggle.BorderRadius = 12;
            btnAdminToggle.Cursor = Cursors.Hand;
            btnAdminToggle.CustomizableEdges = customizableEdges3;
            btnAdminToggle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            btnAdminToggle.ForeColor = Color.White;
            btnAdminToggle.Location = new Point(60, 116);
            btnAdminToggle.Name = "btnAdminToggle";
            btnAdminToggle.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnAdminToggle.Size = new Size(230, 55);
            btnAdminToggle.TabIndex = 0;
            btnAdminToggle.Text = "ADMIN";
            // 
            // btnStudentToggle
            // 
            btnStudentToggle.BorderRadius = 12;
            btnStudentToggle.Cursor = Cursors.Hand;
            btnStudentToggle.CustomizableEdges = customizableEdges5;
            btnStudentToggle.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            btnStudentToggle.ForeColor = Color.White;
            btnStudentToggle.Location = new Point(310, 116);
            btnStudentToggle.Name = "btnStudentToggle";
            btnStudentToggle.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnStudentToggle.Size = new Size(230, 55);
            btnStudentToggle.TabIndex = 1;
            btnStudentToggle.Text = "BORROWER";
            // 
            // lblLoginHeader
            // 
            lblLoginHeader.AutoSize = false;
            lblLoginHeader.BackColor = Color.Transparent;
            lblLoginHeader.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblLoginHeader.ForeColor = Color.FromArgb(13, 71, 161);
            lblLoginHeader.Location = new Point(0, 25);
            lblLoginHeader.Name = "lblLoginHeader";
            lblLoginHeader.Size = new Size(594, 90);
            lblLoginHeader.TabIndex = 13;
            lblLoginHeader.Text = "BORROWING PORTAL";
            lblLoginHeader.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // txtStudentId
            // 
            txtStudentId.CustomizableEdges = customizableEdges7;
            txtStudentId.DefaultText = "";
            txtStudentId.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtStudentId.IconLeft = Properties.Resources.user;
            txtStudentId.Location = new Point(60, 205);
            txtStudentId.Margin = new Padding(3, 4, 3, 4);
            txtStudentId.Name = "txtStudentId";
            txtStudentId.PlaceholderText = "";
            txtStudentId.SelectedText = "";
            txtStudentId.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtStudentId.Size = new Size(480, 50);
            txtStudentId.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            txtStudentId.TabIndex = 3;
            // 
            // cmbListEquipments
            // 
            cmbListEquipments.BackColor = Color.Transparent;
            cmbListEquipments.BorderRadius = 8;
            cmbListEquipments.CustomizableEdges = customizableEdges9;
            cmbListEquipments.DrawMode = DrawMode.OwnerDrawFixed;
            cmbListEquipments.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbListEquipments.FocusedColor = Color.Empty;
            cmbListEquipments.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbListEquipments.ForeColor = Color.FromArgb(68, 88, 112);
            cmbListEquipments.ItemHeight = 30;
            cmbListEquipments.Location = new Point(60, 305);
            cmbListEquipments.Name = "cmbListEquipments";
            cmbListEquipments.ShadowDecoration.CustomizableEdges = customizableEdges10;
            cmbListEquipments.Size = new Size(480, 36);
            cmbListEquipments.TabIndex = 4;
            // 
            // lblQuantity
            // 
            lblQuantity.BackColor = Color.Transparent;
            lblQuantity.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblQuantity.ForeColor = Color.DimGray;
            lblQuantity.Location = new Point(60, 360);
            lblQuantity.Name = "lblQuantity";
            lblQuantity.Size = new Size(35, 25);
            lblQuantity.TabIndex = 14;
            lblQuantity.Text = "QTY";
            // 
            // numQuantity
            // 
            numQuantity.BackColor = Color.Transparent;
            numQuantity.BorderRadius = 8;
            numQuantity.CustomizableEdges = customizableEdges11;
            numQuantity.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            numQuantity.Location = new Point(60, 390);
            numQuantity.Margin = new Padding(3, 4, 3, 4);
            numQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numQuantity.Name = "numQuantity";
            numQuantity.ShadowDecoration.CustomizableEdges = customizableEdges12;
            numQuantity.Size = new Size(117, 50);
            numQuantity.TabIndex = 6;
            numQuantity.UpDownButtonFillColor = Color.FromArgb(240, 240, 240);
            numQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblSubject
            // 
            lblSubject.BackColor = Color.Transparent;
            lblSubject.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblSubject.ForeColor = Color.DimGray;
            lblSubject.Location = new Point(223, 360);
            lblSubject.Name = "lblSubject";
            lblSubject.Size = new Size(161, 25);
            lblSubject.TabIndex = 15;
            lblSubject.Text = "SUBJECT / PURPOSE";
            // 
            // txtSubject
            // 
            txtSubject.CustomizableEdges = customizableEdges13;
            txtSubject.DefaultText = "";
            txtSubject.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtSubject.Location = new Point(223, 390);
            txtSubject.Margin = new Padding(3, 4, 3, 4);
            txtSubject.Name = "txtSubject";
            txtSubject.PlaceholderText = "e.g., IT 211";
            txtSubject.SelectedText = "";
            txtSubject.ShadowDecoration.CustomizableEdges = customizableEdges14;
            txtSubject.Size = new Size(317, 50);
            txtSubject.Style = Guna.UI2.WinForms.Enums.TextBoxStyle.Material;
            txtSubject.TabIndex = 8;
            // 
            // btnReturn
            // 
            btnReturn.BorderRadius = 12;
            btnReturn.Cursor = Cursors.Hand;
            btnReturn.CustomizableEdges = customizableEdges15;
            btnReturn.FillColor = Color.FromArgb(230, 230, 230);
            btnReturn.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            btnReturn.ForeColor = Color.FromArgb(64, 64, 64);
            btnReturn.HoverState.FillColor = Color.FromArgb(210, 210, 210);
            btnReturn.Location = new Point(60, 555);
            btnReturn.Name = "btnReturn";
            btnReturn.ShadowDecoration.CustomizableEdges = customizableEdges16;
            btnReturn.Size = new Size(230, 55);
            btnReturn.TabIndex = 9;
            btnReturn.Text = "RETURN";
            // 
            // btnBorrow
            // 
            btnBorrow.BorderRadius = 12;
            btnBorrow.Cursor = Cursors.Hand;
            btnBorrow.CustomizableEdges = customizableEdges17;
            btnBorrow.FillColor = Color.FromArgb(13, 71, 161);
            btnBorrow.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            btnBorrow.ForeColor = Color.White;
            btnBorrow.HoverState.FillColor = Color.FromArgb(9, 50, 115);
            btnBorrow.Location = new Point(310, 555);
            btnBorrow.Name = "btnBorrow";
            btnBorrow.ShadowDecoration.CustomizableEdges = customizableEdges18;
            btnBorrow.Size = new Size(230, 55);
            btnBorrow.TabIndex = 10;
            btnBorrow.Text = "BORROW";
            // 
            // btnLogin
            // 
            btnLogin.BorderRadius = 12;
            btnLogin.CustomizableEdges = customizableEdges19;
            btnLogin.FillColor = Color.FromArgb(13, 71, 161);
            btnLogin.Font = new Font("Segoe UI Semibold", 12F, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.HoverState.FillColor = Color.FromArgb(9, 50, 115);
            btnLogin.Location = new Point(60, 555);
            btnLogin.Name = "btnLogin";
            btnLogin.ShadowDecoration.CustomizableEdges = customizableEdges20;
            btnLogin.Size = new Size(480, 55);
            btnLogin.TabIndex = 13;
            btnLogin.Text = "LOGIN";
            btnLogin.Visible = false;
            // 
            // cmbGradeLevel
            // 
            cmbGradeLevel.BackColor = Color.Transparent;
            cmbGradeLevel.BorderRadius = 8;
            cmbGradeLevel.CustomizableEdges = customizableEdges21;
            cmbGradeLevel.DrawMode = DrawMode.OwnerDrawFixed;
            cmbGradeLevel.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGradeLevel.FocusedColor = Color.Empty;
            cmbGradeLevel.Font = new Font("Segoe UI", 11F);
            cmbGradeLevel.ForeColor = Color.FromArgb(68, 88, 112);
            cmbGradeLevel.ItemHeight = 30;
            cmbGradeLevel.Items.AddRange(new object[] { "Grade 7", "Grade 8", "Grade 9", "Grade 10", "Grade 11", "Grade 12", "Faculty" });
            cmbGradeLevel.Location = new Point(223, 460);
            cmbGradeLevel.Name = "cmbGradeLevel";
            cmbGradeLevel.ShadowDecoration.CustomizableEdges = customizableEdges22;
            cmbGradeLevel.Size = new Size(317, 36);
            cmbGradeLevel.StartIndex = 0;
            cmbGradeLevel.TabIndex = 17;
            // 
            // BorrowerPortal
            // 
            ClientSize = new Size(1200, 800);
            Controls.Add(mainTableLayout);
            Name = "BorrowerPortal";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Ventrix | Borrower Portal";
            mainTableLayout.ResumeLayout(false);
            pnlLoginCard.ResumeLayout(false);
            pnlLoginCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numQuantity).EndInit();
            ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel mainTableLayout;
        private Guna.UI2.WinForms.Guna2Panel pnlLoginCard;
        private Guna.UI2.WinForms.Guna2Button btnAdminToggle;
        private Guna.UI2.WinForms.Guna2Button btnStudentToggle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblLoginHeader;
        private Guna.UI2.WinForms.Guna2TextBox txtStudentId;
        private Guna.UI2.WinForms.Guna2ComboBox cmbListEquipments;
        private Guna.UI2.WinForms.Guna2NumericUpDown numQuantity;
        private Guna.UI2.WinForms.Guna2TextBox txtSubject;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblQuantity;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblSubject;
        private Guna.UI2.WinForms.Guna2Button btnReturn;
        private Guna.UI2.WinForms.Guna2Button btnBorrow;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCreateAccount;
        private Guna.UI2.WinForms.Guna2Button btnLogin;
        private Guna.UI2.WinForms.Guna2TextBox txtPassword;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblEquipmentList;
        private Guna.UI2.WinForms.Guna2ComboBox cmbGradeLevel;
    }
}