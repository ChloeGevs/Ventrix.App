namespace Ventrix.App.Popups
{
    partial class RepairDetailsPopup
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
            this.lblHeader = new System.Windows.Forms.Label();
            this.flowRepairList = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new Guna.UI2.WinForms.Guna2Button();

            this.panelContent.SuspendLayout();
            this.SuspendLayout();

            // panelContent
            this.panelContent.BackColor = System.Drawing.Color.White; // Ensures modern white look
            this.panelContent.Controls.Add(this.lblHeader);
            this.panelContent.Controls.Add(this.flowRepairList);
            this.panelContent.Controls.Add(this.btnClose);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 64); // Offset for Title Bar
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(600, 586);

            // lblHeader
            this.lblHeader.AutoSize = true;
            this.lblHeader.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblHeader.Location = new System.Drawing.Point(30, 25);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(262, 25);
            this.lblHeader.Text = "Items Requiring Attention";

            // flowRepairList
            this.flowRepairList.AutoScroll = true;
            this.flowRepairList.Location = new System.Drawing.Point(30, 70);
            this.flowRepairList.Name = "flowRepairList";
            this.flowRepairList.Size = new System.Drawing.Size(540, 420);
            this.flowRepairList.BackColor = System.Drawing.Color.White;

            // btnClose
            this.btnClose.BorderRadius = 6;
            this.btnClose.BorderThickness = 1;
            this.btnClose.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
            this.btnClose.FillColor = System.Drawing.Color.White;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this.btnClose.Location = new System.Drawing.Point(30, 510);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(540, 45);
            this.btnClose.Text = "CLOSE WINDOW";
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);

            // RepairDetailsPopup
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 650);
            this.Controls.Add(this.panelContent);
            this.Name = "RepairDetailsPopup";

            // Prevent maximizing and toolbars for standard popup behavior
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            this.panelContent.ResumeLayout(false);
            this.panelContent.PerformLayout();
            this.ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.FlowLayoutPanel flowRepairList;
        private Guna.UI2.WinForms.Guna2Button btnClose;
    }
}