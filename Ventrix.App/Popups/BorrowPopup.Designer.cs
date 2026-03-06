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
            this.panelContent = new System.Windows.Forms.Panel();
            this.txtBorrower = new Guna.UI2.WinForms.Guna2TextBox();
            this.cmbGrade = new Guna.UI2.WinForms.Guna2ComboBox();
            this.txtPurpose = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnConfirm = new Guna.UI2.WinForms.Guna2Button();

            this.panelContent.SuspendLayout();
            this.SuspendLayout();

            // panelContent
            this.panelContent.BackColor = System.Drawing.Color.White;
            this.panelContent.Controls.Add(this.btnConfirm);
            this.panelContent.Controls.Add(this.txtPurpose);
            this.panelContent.Controls.Add(this.cmbGrade);
            this.panelContent.Controls.Add(this.txtBorrower);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 64);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(350, 340);

            // txtBorrower
            this.txtBorrower.BorderRadius = 6;
            this.txtBorrower.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtBorrower.DefaultText = "";
            this.txtBorrower.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtBorrower.PlaceholderText = "Borrower ID Number";
            this.txtBorrower.Location = new System.Drawing.Point(30, 30);
            this.txtBorrower.Name = "txtBorrower";
            this.txtBorrower.Size = new System.Drawing.Size(290, 45);

            // cmbGrade
            this.cmbGrade.BackColor = System.Drawing.Color.Transparent;
            this.cmbGrade.BorderRadius = 6;
            this.cmbGrade.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.cmbGrade.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbGrade.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.cmbGrade.ForeColor = System.Drawing.Color.FromArgb(68, 88, 112);
            this.cmbGrade.Location = new System.Drawing.Point(30, 90);
            this.cmbGrade.Name = "cmbGrade";
            this.cmbGrade.Size = new System.Drawing.Size(290, 36);

            // txtPurpose
            this.txtPurpose.BorderRadius = 6;
            this.txtPurpose.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtPurpose.DefaultText = "";
            this.txtPurpose.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.txtPurpose.PlaceholderText = "Purpose of Borrowing (Optional)";
            this.txtPurpose.Location = new System.Drawing.Point(30, 140);
            this.txtPurpose.Name = "txtPurpose";
            this.txtPurpose.Size = new System.Drawing.Size(290, 45);

            // btnConfirm
            this.btnConfirm.BorderRadius = 6;
            this.btnConfirm.FillColor = System.Drawing.Color.FromArgb(33, 150, 243);
            this.btnConfirm.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnConfirm.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.Location = new System.Drawing.Point(30, 210);
            this.btnConfirm.Name = "btnConfirm";
            this.btnConfirm.Size = new System.Drawing.Size(290, 45);
            this.btnConfirm.Text = "CONFIRM CHECKOUT";
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);

            // BorrowPopup
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(350, 340);
            this.Controls.Add(this.panelContent);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BorrowPopup";
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Borrow Item";

            this.panelContent.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelContent;
        private Guna.UI2.WinForms.Guna2TextBox txtBorrower;
        private Guna.UI2.WinForms.Guna2ComboBox cmbGrade;
        private Guna.UI2.WinForms.Guna2TextBox txtPurpose;
        private Guna.UI2.WinForms.Guna2Button btnConfirm;
    }
}