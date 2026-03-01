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
            lblHeader = new Label();
            flowRepairList = new FlowLayoutPanel();
            btnClose = new Button();
            SuspendLayout();
            // 
            // lblHeader
            // 
            lblHeader.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblHeader.Location = new Point(41, 110);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new Size(500, 30);
            lblHeader.TabIndex = 0;
            // 
            // flowRepairList
            // 
            flowRepairList.AutoScroll = true;
            flowRepairList.Location = new Point(41, 150);
            flowRepairList.Name = "flowRepairList";
            flowRepairList.Size = new Size(500, 420);
            flowRepairList.TabIndex = 1;
            // 
            // btnClose
            // 
            btnClose.Dock = DockStyle.Bottom;
            btnClose.Location = new Point(3, 641);
            btnClose.Name = "btnClose";
            btnClose.Size = new Size(580, 50);
            btnClose.TabIndex = 2;
            btnClose.Text = "CLOSE";
            btnClose.Click += btnClose_Click;
            // 
            // RepairDetailsPopup
            // 
            ClientSize = new Size(586, 694);
            Controls.Add(lblHeader);
            Controls.Add(flowRepairList);
            Controls.Add(btnClose);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "RepairDetailsPopup";
            StartPosition = FormStartPosition.CenterParent;
            ResumeLayout(false);
        }

        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.FlowLayoutPanel flowRepairList;
        private System.Windows.Forms.Button btnClose;
    }
}