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
            pnlAlert = new System.Windows.Forms.Panel();
            lblAlertMsg = new System.Windows.Forms.Label();
            pnlAlert.SuspendLayout();
            SuspendLayout();

            // 
            // pnlAlert
            // 
            pnlAlert.BackColor = System.Drawing.Color.White;
            pnlAlert.Cursor = System.Windows.Forms.Cursors.Hand;
            pnlAlert.Dock = System.Windows.Forms.DockStyle.Fill;
            pnlAlert.Controls.Add(lblAlertMsg); 

            // 
            // lblAlertMsg
            // 
            lblAlertMsg.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            lblAlertMsg.Location = new System.Drawing.Point(10, 15);

            lblAlertMsg.AutoSize = false;
            lblAlertMsg.Size = new System.Drawing.Size(380, 30);
            lblAlertMsg.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right; // Make it stretch with the tile
            lblAlertMsg.AutoEllipsis = true;

            // 
            // AlertTile
            // 
            Controls.Add(pnlAlert);
            Size = new System.Drawing.Size(400, 60);

            pnlAlert.ResumeLayout(false);
            ResumeLayout(false);
        }

        private System.Windows.Forms.Panel pnlAlert;
        private System.Windows.Forms.Label lblAlertMsg;
    }
}