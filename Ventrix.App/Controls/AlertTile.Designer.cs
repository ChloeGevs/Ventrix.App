namespace Ventrix.App.Controls
{
    partial class AlertTile
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlAlert = new System.Windows.Forms.Panel();
            this.lblAlertMsg = new System.Windows.Forms.Label();
            this.SuspendLayout();

            this.pnlAlert.BackColor = System.Drawing.Color.White;
            this.pnlAlert.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlAlert.Cursor = System.Windows.Forms.Cursors.Hand;

            this.lblAlertMsg.Font = new System.Drawing.Font("Segoe UI", 12, System.Drawing.FontStyle.Bold);
            this.lblAlertMsg.Location = new System.Drawing.Point(10, 15);
            this.lblAlertMsg.AutoSize = true;

            this.Controls.Add(this.lblAlertMsg);
            this.Controls.Add(this.pnlAlert);
            this.Size = new System.Drawing.Size(400, 60);
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlAlert;
        private System.Windows.Forms.Label lblAlertMsg;
    }
}