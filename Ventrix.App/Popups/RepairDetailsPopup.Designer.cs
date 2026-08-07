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
            this.components = new System.ComponentModel.Container();
            panelContent = new System.Windows.Forms.Panel();
            lblHeader = new System.Windows.Forms.Label();
            lblSubheader = new System.Windows.Forms.Label();
            flowRepairList = new System.Windows.Forms.FlowLayoutPanel();
            btnClose = new Guna.UI2.WinForms.Guna2Button();
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);

            panelContent.SuspendLayout();
            SuspendLayout();

            // guna2BorderlessForm1 (Adds the rounded corners)
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.BorderRadius = 16;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;

            // panelContent
            panelContent.BackColor = System.Drawing.Color.FromArgb(250, 252, 253);
            panelContent.Controls.Add(lblHeader);
            panelContent.Controls.Add(lblSubheader);
            panelContent.Controls.Add(flowRepairList);
            panelContent.Controls.Add(btnClose);
            panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            panelContent.Location = new System.Drawing.Point(0, 64);
            panelContent.Name = "panelContent";
            panelContent.Size = new System.Drawing.Size(550, 386);
            panelContent.Padding = new System.Windows.Forms.Padding(20);

            // lblHeader
            lblHeader.AutoSize = true;
            lblHeader.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            lblHeader.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            lblHeader.Location = new System.Drawing.Point(20, 15);
            lblHeader.Name = "lblHeader";
            lblHeader.Size = new System.Drawing.Size(0, 30); // Empty initially for Typewriter
            lblHeader.Text = "";

            // lblSubheader
            lblSubheader.AutoSize = true;
            lblSubheader.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            lblSubheader.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            lblSubheader.Location = new System.Drawing.Point(22, 45);
            lblSubheader.Name = "lblSubheader";
            lblSubheader.Size = new System.Drawing.Size(260, 17);
            lblSubheader.Text = "Review and mark damaged items as repaired.";

            // flowRepairList
            flowRepairList.AutoScroll = true;
            flowRepairList.Location = new System.Drawing.Point(20, 80);
            flowRepairList.Name = "flowRepairList";
            flowRepairList.Size = new System.Drawing.Size(510, 240);
            flowRepairList.BackColor = System.Drawing.Color.Transparent;
            flowRepairList.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            flowRepairList.WrapContents = false;

            // btnClose
            btnClose.BorderRadius = 18;
            btnClose.FillColor = System.Drawing.Color.White;
            btnClose.BorderColor = System.Drawing.Color.FromArgb(203, 213, 225);
            btnClose.BorderThickness = 1;
            btnClose.HoverState.FillColor = System.Drawing.Color.FromArgb(241, 245, 249);
            btnClose.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            btnClose.ForeColor = System.Drawing.Color.FromArgb(51, 65, 85);
            btnClose.Location = new System.Drawing.Point(20, 330);
            btnClose.Name = "btnClose";
            btnClose.Size = new System.Drawing.Size(510, 40);
            btnClose.Text = "Done Reviewing";
            btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            btnClose.Animated = true;
            btnClose.Click += new System.EventHandler(btnClose_Click);

            // RepairDetailsPopup
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(550, 450);
            Controls.Add(panelContent);
            Name = "RepairDetailsPopup";

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
        private System.Windows.Forms.Label lblSubheader;
        private System.Windows.Forms.FlowLayoutPanel flowRepairList;
        private Guna.UI2.WinForms.Guna2Button btnClose;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
    }
}