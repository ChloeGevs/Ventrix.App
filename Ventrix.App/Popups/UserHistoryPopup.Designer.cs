namespace Ventrix.App.Popups
{
    partial class UserHistoryPopup
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.DataGridView dgvUserHistory;
        private Guna.UI2.WinForms.Guna2Button btnClose;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private System.Windows.Forms.Panel panelContent;

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
            this.components = new System.ComponentModel.Container();
            this.lblTitle = new System.Windows.Forms.Label();
            this.dgvUserHistory = new System.Windows.Forms.DataGridView();
            this.btnClose = new Guna.UI2.WinForms.Guna2Button();
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.panelContent = new System.Windows.Forms.Panel();

            ((System.ComponentModel.ISupportInitialize)(this.dgvUserHistory)).BeginInit();
            this.panelContent.SuspendLayout();
            this.SuspendLayout();

            // guna2BorderlessForm1
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.BorderRadius = 16;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;

            // panelContent
            this.panelContent.BackColor = System.Drawing.Color.FromArgb(250, 252, 253);
            this.panelContent.Controls.Add(this.lblTitle);
            this.panelContent.Controls.Add(this.dgvUserHistory);
            this.panelContent.Controls.Add(this.btnClose);
            this.panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContent.Location = new System.Drawing.Point(0, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new System.Drawing.Size(760, 310); // Reduced overall height
            this.panelContent.TabIndex = 0;

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            this.lblTitle.Location = new System.Drawing.Point(20, 18);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(0, 25);
            this.lblTitle.Text = "";

            // btnClose
            this.btnClose.Animated = true;
            this.btnClose.BorderRadius = 17;
            this.btnClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnClose.FillColor = System.Drawing.Color.FromArgb(37, 99, 235);
            this.btnClose.HoverState.FillColor = System.Drawing.Color.FromArgb(29, 78, 216);
            this.btnClose.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnClose.ForeColor = System.Drawing.Color.White;
            this.btnClose.Location = new System.Drawing.Point(595, 262); // Moved up closer to the grid
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(140, 36);
            this.btnClose.TabIndex = 0;
            this.btnClose.Text = "Close Window";
            this.btnClose.Click += new System.EventHandler(this.BtnClose_Click);

            // UserHistoryPopup
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(760, 310); // Tightened window size
            this.Controls.Add(this.panelContent);
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "UserHistoryPopup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "UserHistoryPopup";

            ((System.ComponentModel.ISupportInitialize)(this.dgvUserHistory)).EndInit();
            this.panelContent.ResumeLayout(false);
            this.panelContent.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}