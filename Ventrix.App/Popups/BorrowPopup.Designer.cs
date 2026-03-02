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
            this.lblItemHeader = new System.Windows.Forms.Label();
            this.txtBorrower = new System.Windows.Forms.TextBox();
            this.cmbGrade = new System.Windows.Forms.ComboBox();
            this.txtPurpose = new System.Windows.Forms.TextBox();
            this.btnConfirm = new System.Windows.Forms.Button();
            this.SuspendLayout();

            // lblItemHeader
            this.lblItemHeader.AutoSize = true;
            this.lblItemHeader.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Bold);
            this.lblItemHeader.Location = new System.Drawing.Point(20, 20);

            // txtBorrower, cmbGrade, txtPurpose auto-generated layout logic...

            // btnConfirm
            this.btnConfirm.BackColor = System.Drawing.Color.Teal;
            this.btnConfirm.ForeColor = System.Drawing.Color.White;
            this.btnConfirm.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.btnConfirm.Height = 50;
            this.btnConfirm.Text = "CONFIRM BORROW";
            this.btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);

            // Form Settings
            this.ClientSize = new System.Drawing.Size(400, 420);
            this.Controls.Add(this.lblItemHeader);
            this.Controls.Add(this.btnConfirm);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Label lblItemHeader;
        private System.Windows.Forms.TextBox txtBorrower;
        private System.Windows.Forms.ComboBox cmbGrade;
        private System.Windows.Forms.TextBox txtPurpose;
        private System.Windows.Forms.Button btnConfirm;
    }
}