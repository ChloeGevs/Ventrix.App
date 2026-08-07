namespace Ventrix.App.Popups
{
    partial class BorrowerItemsPopup
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Panel pnlMain;
        private System.Windows.Forms.Panel pnlHeaderContainer;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblSubtitle;
        private System.Windows.Forms.Label lblBadge;
        private System.Windows.Forms.FlowLayoutPanel flowPanelItems;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.Button btnX;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.pnlMain = new System.Windows.Forms.Panel();
            this.btnX = new System.Windows.Forms.Button();
            this.pnlHeaderContainer = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.lblSubtitle = new System.Windows.Forms.Label();
            this.flowPanelItems = new System.Windows.Forms.FlowLayoutPanel();
            this.btnClose = new System.Windows.Forms.Button();
            this.lblBadge = new System.Windows.Forms.Label();
            this.pnlMain.SuspendLayout();
            this.pnlHeaderContainer.SuspendLayout();
            this.SuspendLayout();
            // 
            // pnlMain
            // 
            this.pnlMain.BackColor = System.Drawing.Color.White;
            this.pnlMain.Controls.Add(this.btnX);
            this.pnlMain.Controls.Add(this.pnlHeaderContainer);
            this.pnlMain.Controls.Add(this.flowPanelItems);
            this.pnlMain.Controls.Add(this.btnClose);
            this.pnlMain.Controls.Add(this.lblBadge);
            this.pnlMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMain.Location = new System.Drawing.Point(0, 0);
            this.pnlMain.Name = "pnlMain";
            this.pnlMain.Padding = new System.Windows.Forms.Padding(24);
            this.pnlMain.Size = new System.Drawing.Size(400, 420);
            this.pnlMain.TabIndex = 0;
            // 
            // btnX
            // 
            this.btnX.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnX.FlatAppearance.BorderSize = 0;
            this.btnX.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnX.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.btnX.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.btnX.Location = new System.Drawing.Point(340, 20);
            this.btnX.Name = "btnX";
            this.btnX.Size = new System.Drawing.Size(36, 36);
            this.btnX.TabIndex = 5;
            this.btnX.Text = "✕";
            this.btnX.UseVisualStyleBackColor = true;
            this.btnX.Click += new System.EventHandler(this.btnX_Click);
            // 
            // pnlHeaderContainer
            // 
            this.pnlHeaderContainer.Controls.Add(this.lblTitle);
            this.pnlHeaderContainer.Controls.Add(this.lblSubtitle);
            this.pnlHeaderContainer.Location = new System.Drawing.Point(22, 20);
            this.pnlHeaderContainer.Name = "pnlHeaderContainer";
            this.pnlHeaderContainer.Size = new System.Drawing.Size(300, 56);
            this.pnlHeaderContainer.TabIndex = 6;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 16F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblTitle.Location = new System.Drawing.Point(0, 0);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(111, 30);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Borrower";
            // 
            // lblSubtitle
            // 
            this.lblSubtitle.AutoSize = true;
            this.lblSubtitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.lblSubtitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.lblSubtitle.Location = new System.Drawing.Point(2, 32);
            this.lblSubtitle.Name = "lblSubtitle";
            this.lblSubtitle.Size = new System.Drawing.Size(130, 15);
            this.lblSubtitle.TabIndex = 0;
            this.lblSubtitle.Text = "Requested items overview";
            // 
            // flowPanelItems
            // 
            this.flowPanelItems.AutoScroll = true;
            this.flowPanelItems.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.flowPanelItems.Location = new System.Drawing.Point(22, 145);
            this.flowPanelItems.Name = "flowPanelItems";
            this.flowPanelItems.Size = new System.Drawing.Size(356, 160);
            this.flowPanelItems.TabIndex = 4;
            this.flowPanelItems.WrapContents = false;
            // 
            // btnClose
            // 
            this.btnClose.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FlatAppearance.BorderSize = 0;
            this.btnClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClose.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(22, 332);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(356, 44);
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "Got it, Close";
            this.btnClose.UseVisualStyleBackColor = false;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // lblBadge
            // 
            this.lblBadge.AutoSize = true;
            this.lblBadge.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(254)))), ((int)(((byte)(243)))), ((int)(((byte)(199)))));
            this.lblBadge.Font = new System.Drawing.Font("Segoe UI", 8.5F, System.Drawing.FontStyle.Bold);
            this.lblBadge.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(83)))), ((int)(((byte)(9)))));
            this.lblBadge.Location = new System.Drawing.Point(22, 94);
            this.lblBadge.Name = "lblBadge";
            this.lblBadge.Padding = new System.Windows.Forms.Padding(8, 4, 8, 4);
            this.lblBadge.Size = new System.Drawing.Size(61, 23);
            this.lblBadge.TabIndex = 2;
            this.lblBadge.Text = "STATUS";
            // 
            // BorrowerItemsPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(400, 420);
            this.Controls.Add(this.pnlMain);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BorrowerItemsPopup";
            this.ShowIcon = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "BorrowerItemsPopup";
            this.pnlMain.ResumeLayout(false);
            this.pnlMain.PerformLayout();
            this.pnlHeaderContainer.ResumeLayout(false);
            this.pnlHeaderContainer.PerformLayout();
            this.ResumeLayout(false);

        }
    }
}