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
            panelContent = new System.Windows.Forms.Panel();
            lblHeader = new System.Windows.Forms.Label();
            flowRepairList = new System.Windows.Forms.FlowLayoutPanel();
            btnClose = new Guna.UI2.WinForms.Guna2Button();

            panelContent.SuspendLayout();
            SuspendLayout();

            // panelContent
            panelContent.BackColor = System.Drawing.Color.White; // Ensures modern white look
            panelContent.Controls.Add(lblHeader);
            panelContent.Controls.Add(flowRepairList);
            panelContent.Controls.Add(btnClose);
            panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            panelContent.Location = new System.Drawing.Point(0, 64); // Offset for Title Bar
            panelContent.Name = "panelContent";
            panelContent.Size = new System.Drawing.Size(600, 586);

            // lblHeader
            lblHeader.AutoSize = true;
            lblHeader.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblHeader.Location = new System.Drawing.Point(30, 25);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new System.Drawing.Size(262, 25);
            lblHeader.Text = "Items Requiring Attention";

            // flowRepairList
            flowRepairList.AutoScroll = true;
            flowRepairList.Location = new System.Drawing.Point(30, 70);
            flowRepairList.Name = "flowRepairList";
            flowRepairList.Size = new System.Drawing.Size(540, 420);
            flowRepairList.BackColor = System.Drawing.Color.White;

            // btnClose
            btnClose.BorderRadius = 6;
            btnClose.BorderThickness = 1;
            btnClose.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
            btnClose.FillColor = System.Drawing.Color.White;
            btnClose.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnClose.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            btnClose.Location = new System.Drawing.Point(30, 510);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(540, 45);
            btnClose.Text = "CLOSE WINDOW";
            btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            btnClose.Click += new System.EventHandler(btnClose_Click);

            // RepairDetailsPopup
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(600, 650);
            Controls.Add(panelContent);
            Name = "RepairDetailsPopup";

            // Prevent maximizing and toolbars for standard popup behavior
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Sizable = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            panelContent.ResumeLayout(false);
            panelContent.PerformLayout();
            ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelContent;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.FlowLayoutPanel flowRepairList;
        private Guna.UI2.WinForms.Guna2Button btnClose;
    }
}