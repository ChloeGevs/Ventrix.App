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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges19 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges20 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
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
            mainTableLayout = new TableLayoutPanel();
            pnlLoginCard = new Guna.UI2.WinForms.Guna2Panel();
            lblCreateAccount = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnStaffToggle = new Guna.UI2.WinForms.Guna2Button();
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
            mainTableLayout.SuspendLayout();
            pnlLoginCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)numQuantity).BeginInit();
            SuspendLayout();
            // 
            // mainTableLayout
            // 
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
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 620F));
            mainTableLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            mainTableLayout.Size = new Size(1194, 733);
            mainTableLayout.TabIndex = 0;
            // 
            // pnlLoginCard
            // 
            pnlLoginCard.BackColor = Color.Transparent;
            pnlLoginCard.BorderRadius = 20;
            pnlLoginCard.Controls.Add(lblCreateAccount);
            pnlLoginCard.Controls.Add(btnStaffToggle);
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
            pnlLoginCard.CustomizableEdges = customizableEdges19;
            pnlLoginCard.Dock = DockStyle.Fill;
            pnlLoginCard.FillColor = Color.White;
            pnlLoginCard.Location = new Point(375, 59);
            pnlLoginCard.Name = "pnlLoginCard";
            pnlLoginCard.ShadowDecoration.CustomizableEdges = customizableEdges20;
            pnlLoginCard.ShadowDecoration.Enabled = true;
            pnlLoginCard.Size = new Size(444, 614);
            pnlLoginCard.TabIndex = 0;
            // 
            // lblCreateAccount
            // 
            lblCreateAccount.BackColor = Color.Transparent;
            lblCreateAccount.Font = new Font("Sitka Banner", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblCreateAccount.Location = new Point(141, 504);
            lblCreateAccount.Name = "lblCreateAccount";
            lblCreateAccount.Size = new Size(180, 28);
            lblCreateAccount.TabIndex = 12;
            lblCreateAccount.Text = "No account? <font color=\"#1565C0\"><u>Register here</u></font>";
            // 
            // btnStaffToggle
            // 
            btnStaffToggle.BorderRadius = 10;
            btnStaffToggle.CustomizableEdges = customizableEdges1;
            btnStaffToggle.Font = new Font("Sitka Banner", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnStaffToggle.ForeColor = Color.White;
            btnStaffToggle.Location = new Point(50, 80);
            btnStaffToggle.Name = "btnStaffToggle";
            btnStaffToggle.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnStaffToggle.Size = new Size(170, 45);
            btnStaffToggle.TabIndex = 0;
            btnStaffToggle.Text = "ADMIN LOGIN";
            // 
            // btnStudentToggle
            // 
            btnStudentToggle.BorderRadius = 10;
            btnStudentToggle.CustomizableEdges = customizableEdges3;
            btnStudentToggle.Font = new Font("Sitka Banner", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnStudentToggle.ForeColor = Color.White;
            btnStudentToggle.Location = new Point(230, 80);
            btnStudentToggle.Name = "btnStudentToggle";
            btnStudentToggle.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnStudentToggle.Size = new Size(170, 45);
            btnStudentToggle.TabIndex = 1;
            btnStudentToggle.Text = "BORROWER";
            // 
            // lblLoginHeader
            // 
            lblLoginHeader.AutoSize = false;
            lblLoginHeader.BackColor = Color.Transparent;
            lblLoginHeader.Font = new Font("Sitka Heading", 22F, FontStyle.Bold);
            lblLoginHeader.ForeColor = Color.FromArgb(13, 71, 161);
            lblLoginHeader.Location = new Point(0, 15);
            lblLoginHeader.Name = "lblLoginHeader";
            lblLoginHeader.Size = new Size(444, 60);
            lblLoginHeader.TabIndex = 13;
            lblLoginHeader.Text = "BORROWING PORTAL";
            lblLoginHeader.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // txtStudentId
            // 
            txtStudentId.BorderRadius = 10;
            txtStudentId.CustomizableEdges = customizableEdges5;
            txtStudentId.DefaultText = "";
            txtStudentId.Font = new Font("Sitka Text", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtStudentId.Location = new Point(50, 156);
            txtStudentId.Margin = new Padding(3, 4, 3, 4);
            txtStudentId.Name = "txtStudentId";
            txtStudentId.PlaceholderText = "";
            txtStudentId.SelectedText = "";
            txtStudentId.ShadowDecoration.CustomizableEdges = customizableEdges6;
            txtStudentId.Size = new Size(350, 50);
            txtStudentId.TabIndex = 3;
            // 
            // cmbListEquipments
            // 
            cmbListEquipments.BackColor = Color.Transparent;
            cmbListEquipments.BorderRadius = 10;
            cmbListEquipments.CustomizableEdges = customizableEdges7;
            cmbListEquipments.DrawMode = DrawMode.OwnerDrawFixed;
            cmbListEquipments.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbListEquipments.FocusedColor = Color.Empty;
            cmbListEquipments.Font = new Font("Sitka Banner", 11F, FontStyle.Regular, GraphicsUnit.Point, 0);
            cmbListEquipments.ForeColor = Color.FromArgb(68, 88, 112);
            cmbListEquipments.ItemHeight = 30;
            cmbListEquipments.Location = new Point(50, 228);
            cmbListEquipments.Name = "cmbListEquipments";
            cmbListEquipments.ShadowDecoration.CustomizableEdges = customizableEdges8;
            cmbListEquipments.Size = new Size(350, 36);
            cmbListEquipments.TabIndex = 4;
            // 
            // lblQuantity
            // 
            lblQuantity.BackColor = Color.Transparent;
            lblQuantity.Font = new Font("Sitka Text", 11F);
            lblQuantity.Location = new Point(50, 305);
            lblQuantity.Name = "lblQuantity";
            lblQuantity.Size = new Size(38, 28);
            lblQuantity.TabIndex = 14;
            lblQuantity.Text = "QTY";
            // 
            // numQuantity
            // 
            numQuantity.BackColor = Color.Transparent;
            numQuantity.BorderRadius = 10;
            numQuantity.CustomizableEdges = customizableEdges9;
            numQuantity.Font = new Font("Sitka Text", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            numQuantity.Location = new Point(50, 332);
            numQuantity.Margin = new Padding(3, 4, 3, 4);
            numQuantity.Minimum = new decimal(new int[] { 1, 0, 0, 0 });
            numQuantity.Name = "numQuantity";
            numQuantity.ShadowDecoration.CustomizableEdges = customizableEdges10;
            numQuantity.Size = new Size(100, 45);
            numQuantity.TabIndex = 6;
            numQuantity.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // lblSubject
            // 
            lblSubject.BackColor = Color.Transparent;
            lblSubject.Font = new Font("Sitka Text", 11F);
            lblSubject.Location = new Point(165, 305);
            lblSubject.Name = "lblSubject";
            lblSubject.Size = new Size(174, 28);
            lblSubject.TabIndex = 15;
            lblSubject.Text = "SUBJECT / PURPOSE";
            // 
            // txtSubject
            // 
            txtSubject.BorderRadius = 10;
            txtSubject.CustomizableEdges = customizableEdges11;
            txtSubject.DefaultText = "";
            txtSubject.Font = new Font("Sitka Text", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            txtSubject.Location = new Point(165, 332);
            txtSubject.Margin = new Padding(3, 4, 3, 4);
            txtSubject.Name = "txtSubject";
            txtSubject.PlaceholderText = "e.g., IT 211";
            txtSubject.SelectedText = "";
            txtSubject.ShadowDecoration.CustomizableEdges = customizableEdges12;
            txtSubject.Size = new Size(235, 45);
            txtSubject.TabIndex = 8;
            // 
            // btnReturn
            // 
            btnReturn.BorderRadius = 12;
            btnReturn.CustomizableEdges = customizableEdges13;
            btnReturn.Font = new Font("Sitka Display", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnReturn.ForeColor = Color.White;
            btnReturn.Location = new Point(50, 448);
            btnReturn.Name = "btnReturn";
            btnReturn.ShadowDecoration.CustomizableEdges = customizableEdges14;
            btnReturn.Size = new Size(165, 50);
            btnReturn.TabIndex = 9;
            btnReturn.Text = "RETURN";
            // 
            // btnBorrow
            // 
            btnBorrow.BorderRadius = 12;
            btnBorrow.CustomizableEdges = customizableEdges15;
            btnBorrow.FillColor = Color.Teal;
            btnBorrow.Font = new Font("Sitka Display", 10.8F, FontStyle.Regular, GraphicsUnit.Point, 0);
            btnBorrow.ForeColor = Color.White;
            btnBorrow.Location = new Point(235, 448);
            btnBorrow.Name = "btnBorrow";
            btnBorrow.ShadowDecoration.CustomizableEdges = customizableEdges16;
            btnBorrow.Size = new Size(165, 50);
            btnBorrow.TabIndex = 10;
            btnBorrow.Text = "BORROW";
            // 
            // btnLogin
            // 
            btnLogin.BorderRadius = 12;
            btnLogin.CustomizableEdges = customizableEdges17;
            btnLogin.FillColor = Color.FromArgb(13, 71, 161);
            btnLogin.Font = new Font("Sitka Display", 12F, FontStyle.Bold);
            btnLogin.ForeColor = Color.White;
            btnLogin.Location = new Point(50, 410);
            btnLogin.Name = "btnLogin";
            btnLogin.ShadowDecoration.CustomizableEdges = customizableEdges18;
            btnLogin.Size = new Size(350, 55);
            btnLogin.TabIndex = 13;
            btnLogin.Text = "LOGIN";
            btnLogin.Visible = false;
            // 
            // Form1
            // 
            ClientSize = new Size(1200, 800);
            Controls.Add(mainTableLayout);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            mainTableLayout.ResumeLayout(false);
            pnlLoginCard.ResumeLayout(false);
            pnlLoginCard.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)numQuantity).EndInit();
            ResumeLayout(false);
        }

        private System.Windows.Forms.TableLayoutPanel mainTableLayout;
        private Guna.UI2.WinForms.Guna2Panel pnlLoginCard;
        private Guna.UI2.WinForms.Guna2Button btnStaffToggle;
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
    }
}