namespace Ventrix.App.Popups
{
    partial class ShowMultiReturnSelectionPopup
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
            lblTitle = new System.Windows.Forms.Label();
            lblInstruction = new System.Windows.Forms.Label();
            flowRecords = new System.Windows.Forms.FlowLayoutPanel();
            btnCancel = new Guna.UI2.WinForms.Guna2Button();
            btnOk = new Guna.UI2.WinForms.Guna2Button();
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            panelContent = new System.Windows.Forms.Panel();

            panelContent.SuspendLayout();
            SuspendLayout();

            // guna2BorderlessForm1
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.BorderRadius = 16;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;

            // panelContent
            panelContent.BackColor = System.Drawing.Color.FromArgb(250, 252, 253);
            panelContent.Controls.Add(lblTitle);
            panelContent.Controls.Add(lblInstruction);
            panelContent.Controls.Add(flowRecords);
            panelContent.Controls.Add(btnCancel);
            panelContent.Controls.Add(btnOk);
            panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            panelContent.Location = new System.Drawing.Point(0, 0);
            panelContent.Name = "panelContent";
            panelContent.Size = new System.Drawing.Size(460, 360); // Compact height
            panelContent.TabIndex = 0;

            // lblTitle
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 15F, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            lblTitle.Location = new System.Drawing.Point(20, 18);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(0, 28);
            lblTitle.Text = "";

            // lblInstruction
            lblInstruction.AutoSize = true;
            lblInstruction.Font = new System.Drawing.Font("Segoe UI", 9F);
            lblInstruction.ForeColor = System.Drawing.Color.FromArgb(100, 116, 139);
            lblInstruction.Location = new System.Drawing.Point(22, 50);
            lblInstruction.Name = "lblInstruction";
            lblInstruction.Size = new System.Drawing.Size(250, 15);
            lblInstruction.TabIndex = 3;
            lblInstruction.Text = "Select the items you are returning right now.";

            // flowRecords
            flowRecords.AutoScroll = true;
            flowRecords.BackColor = System.Drawing.Color.Transparent;
            flowRecords.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            flowRecords.Location = new System.Drawing.Point(20, 75);
            flowRecords.Name = "flowRecords";
            flowRecords.Size = new System.Drawing.Size(420, 195); // Shorter list container to eliminate dead space
            flowRecords.TabIndex = 2;
            flowRecords.WrapContents = false;

            // btnCancel
            btnCancel.Animated = true;
            btnCancel.BorderRadius = 18;
            btnCancel.BorderThickness = 1;
            btnCancel.BorderColor = System.Drawing.Color.FromArgb(226, 232, 240);
            btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            btnCancel.FillColor = System.Drawing.Color.White;
            btnCancel.HoverState.FillColor = System.Drawing.Color.FromArgb(241, 245, 249);
            btnCancel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            btnCancel.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            btnCancel.Location = new System.Drawing.Point(165, 295);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(130, 38);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // btnOk
            btnOk.Animated = true;
            btnOk.BorderRadius = 18;
            btnOk.Cursor = System.Windows.Forms.Cursors.Hand;
            btnOk.FillColor = System.Drawing.Color.FromArgb(16, 185, 129);
            btnOk.HoverState.FillColor = System.Drawing.Color.FromArgb(5, 150, 105);
            btnOk.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold);
            btnOk.ForeColor = System.Drawing.Color.White;
            btnOk.Location = new System.Drawing.Point(305, 295);
            btnOk.Name = "btnOk";
            btnOk.Size = new System.Drawing.Size(135, 38);
            btnOk.TabIndex = 0;
            btnOk.Text = "Confirm Return";
            btnOk.Click += new System.EventHandler(this.btnOk_Click);

            // ShowMultiReturnSelectionPopup
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(460, 360); // Smaller overall window footprint
            Controls.Add(panelContent);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ShowMultiReturnSelectionPopup";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Return Items";
            panelContent.ResumeLayout(false);
            panelContent.PerformLayout();
            ResumeLayout(false);
        }

        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblInstruction;
        private System.Windows.Forms.FlowLayoutPanel flowRecords;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
        private Guna.UI2.WinForms.Guna2Button btnOk;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private System.Windows.Forms.Panel panelContent;
    }
}