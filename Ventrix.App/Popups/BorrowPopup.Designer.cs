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
            panelContent = new System.Windows.Forms.Panel();
            txtBorrower = new Guna.UI2.WinForms.Guna2TextBox();
            cmbGrade = new Guna.UI2.WinForms.Guna2ComboBox();
            txtPurpose = new Guna.UI2.WinForms.Guna2TextBox();
            btnConfirm = new Guna.UI2.WinForms.Guna2Button();

            panelContent.SuspendLayout();
            SuspendLayout();

            // panelContent
            panelContent.BackColor = System.Drawing.Color.White;
            panelContent.Controls.Add(btnConfirm);
            panelContent.Controls.Add(txtPurpose);
            panelContent.Controls.Add(cmbGrade);
            panelContent.Controls.Add(txtBorrower);
            panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            panelContent.Location = new System.Drawing.Point(0, 64);
            panelContent.Name = "panelContent";
            panelContent.Size = new System.Drawing.Size(350, 340);

            // txtBorrower
            txtBorrower.BorderRadius = 6;
            txtBorrower.Cursor = System.Windows.Forms.Cursors.IBeam;
            txtBorrower.DefaultText = "";
            txtBorrower.Font = new System.Drawing.Font("Segoe UI", 10F);
            txtBorrower.PlaceholderText = "Borrower ID Number";
            txtBorrower.Location = new System.Drawing.Point(30, 30);
            txtBorrower.Name = "txtBorrower";
            txtBorrower.Size = new System.Drawing.Size(290, 45);

            // cmbGrade
            cmbGrade.BackColor = System.Drawing.Color.Transparent;
            cmbGrade.BorderRadius = 6;
            cmbGrade.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cmbGrade.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbGrade.Font = new System.Drawing.Font("Segoe UI", 10F);
            cmbGrade.ForeColor = System.Drawing.Color.FromArgb(68, 88, 112);
            cmbGrade.Location = new System.Drawing.Point(30, 90);
            cmbGrade.Name = "cmbGrade";
            cmbGrade.Size = new System.Drawing.Size(290, 36);

            // txtPurpose
            txtPurpose.BorderRadius = 6;
            txtPurpose.Cursor = System.Windows.Forms.Cursors.IBeam;
            txtPurpose.DefaultText = "";
            txtPurpose.Font = new System.Drawing.Font("Segoe UI", 10F);
            txtPurpose.PlaceholderText = "Purpose of Borrowing (Optional)";
            txtPurpose.Location = new System.Drawing.Point(30, 140);
            txtPurpose.Name = "txtPurpose";
            txtPurpose.Size = new System.Drawing.Size(290, 45);

            // btnConfirm
            btnConfirm.BorderRadius = 6;
            btnConfirm.FillColor = System.Drawing.Color.FromArgb(33, 150, 243);
            btnConfirm.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnConfirm.ForeColor = System.Drawing.Color.White;
            btnConfirm.Location = new System.Drawing.Point(30, 210);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new System.Drawing.Size(290, 45);
            btnConfirm.Text = "CONFIRM CHECKOUT";
            btnConfirm.Click += new System.EventHandler(btnConfirm_Click);

            // BorrowPopup
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(350, 340);
            Controls.Add(panelContent);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "BorrowPopup";
            Sizable = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
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