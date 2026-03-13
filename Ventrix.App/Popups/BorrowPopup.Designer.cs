namespace Ventrix.App.Popups
{
    partial class BorrowPopup
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            panelContent = new Panel();
            btnConfirm = new Guna.UI2.WinForms.Guna2Button();
            txtPurpose = new Guna.UI2.WinForms.Guna2TextBox();
            cmbGrade = new Guna.UI2.WinForms.Guna2ComboBox();
            txtBorrower = new Guna.UI2.WinForms.Guna2TextBox();
            panelContent.SuspendLayout();
            SuspendLayout();
            // 
            // panelContent
            // 
            panelContent.BackColor = Color.White;
            panelContent.Controls.Add(btnConfirm);
            panelContent.Controls.Add(txtPurpose);
            panelContent.Controls.Add(cmbGrade);
            panelContent.Controls.Add(txtBorrower);
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(4, 98);
            panelContent.Margin = new Padding(4, 5, 4, 5);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(459, 420);
            panelContent.TabIndex = 0;
            // 
            // btnConfirm
            // 
            btnConfirm.BorderRadius = 6;
            btnConfirm.Cursor = Cursors.Hand;
            btnConfirm.CustomizableEdges = customizableEdges1;
            btnConfirm.FillColor = Color.FromArgb(33, 150, 243);
            btnConfirm.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnConfirm.ForeColor = Color.White;
            btnConfirm.Location = new Point(40, 323);
            btnConfirm.Margin = new Padding(4, 5, 4, 5);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnConfirm.Size = new Size(387, 69);
            btnConfirm.TabIndex = 0;
            btnConfirm.Text = "CONFIRM CHECKOUT";
            btnConfirm.Click += btnConfirm_Click;
            // 
            // txtPurpose
            // 
            txtPurpose.BorderRadius = 6;
            txtPurpose.Cursor = Cursors.IBeam;
            txtPurpose.CustomizableEdges = customizableEdges3;
            txtPurpose.DefaultText = "";
            txtPurpose.Font = new Font("Segoe UI", 10F);
            txtPurpose.Location = new Point(40, 215);
            txtPurpose.Margin = new Padding(4, 6, 4, 6);
            txtPurpose.Name = "txtPurpose";
            txtPurpose.PlaceholderText = "Purpose of Borrowing (Optional)";
            txtPurpose.SelectedText = "";
            txtPurpose.ShadowDecoration.CustomizableEdges = customizableEdges4;
            txtPurpose.Size = new Size(387, 69);
            txtPurpose.TabIndex = 1;
            // 
            // cmbGrade
            // 
            cmbGrade.BackColor = Color.Transparent;
            cmbGrade.BorderRadius = 6;
            cmbGrade.CustomizableEdges = customizableEdges5;
            cmbGrade.DrawMode = DrawMode.OwnerDrawFixed;
            cmbGrade.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGrade.FocusedColor = Color.Empty;
            cmbGrade.Font = new Font("Segoe UI", 10F);
            cmbGrade.ForeColor = Color.FromArgb(68, 88, 112);
            cmbGrade.ItemHeight = 30;
            cmbGrade.Location = new Point(40, 138);
            cmbGrade.Margin = new Padding(4, 5, 4, 5);
            cmbGrade.Name = "cmbGrade";
            cmbGrade.ShadowDecoration.CustomizableEdges = customizableEdges6;
            cmbGrade.Size = new Size(385, 36);
            cmbGrade.TabIndex = 2;
            // 
            // txtBorrower
            // 
            txtBorrower.BorderRadius = 6;
            txtBorrower.Cursor = Cursors.IBeam;
            txtBorrower.CustomizableEdges = customizableEdges7;
            txtBorrower.DefaultText = "";
            txtBorrower.Font = new Font("Segoe UI", 10F);
            txtBorrower.Location = new Point(40, 46);
            txtBorrower.Margin = new Padding(4, 6, 4, 6);
            txtBorrower.Name = "txtBorrower";
            txtBorrower.PlaceholderText = "Borrower ID Number";
            txtBorrower.SelectedText = "";
            txtBorrower.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtBorrower.Size = new Size(387, 69);
            txtBorrower.TabIndex = 3;
            // 
            // BorrowPopup
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(467, 523);
            Controls.Add(panelContent);
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BorrowPopup";
            Padding = new Padding(4, 98, 4, 5);
            Sizable = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Borrow Item";
            panelContent.ResumeLayout(false);
            ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelContent;
        private Guna.UI2.WinForms.Guna2TextBox txtBorrower;
        private Guna.UI2.WinForms.Guna2ComboBox cmbGrade;
        private Guna.UI2.WinForms.Guna2TextBox txtPurpose;
        private Guna.UI2.WinForms.Guna2Button btnConfirm;
    }
}