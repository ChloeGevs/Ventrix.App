namespace Ventrix.App.Popups
{
    partial class ShowMultiUnitSelectionPopup
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Label lblInstruction;
        private FlowLayoutPanel flowUnits;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
        private Guna.UI2.WinForms.Guna2Button btnOk;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private Panel panelContent;

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
            this.lblTitle = new Label();
            this.lblInstruction = new Label();
            this.flowUnits = new FlowLayoutPanel();
            this.btnCancel = new Guna.UI2.WinForms.Guna2Button();
            this.btnOk = new Guna.UI2.WinForms.Guna2Button();
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.panelContent = new Panel();

            this.panelContent.SuspendLayout();
            this.SuspendLayout();

            // guna2BorderlessForm1
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.BorderRadius = 16;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;

            // panelContent
            this.panelContent.BackColor = Color.FromArgb(250, 252, 253);
            this.panelContent.Controls.Add(this.lblTitle);
            this.panelContent.Controls.Add(this.lblInstruction);
            this.panelContent.Controls.Add(this.flowUnits);
            this.panelContent.Controls.Add(this.btnCancel);
            this.panelContent.Controls.Add(this.btnOk);
            this.panelContent.Dock = DockStyle.Fill;
            this.panelContent.Location = new Point(0, 0);
            this.panelContent.Name = "panelContent";
            this.panelContent.Size = new Size(520, 335); // Expanded width to 520px

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 14.5F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(15, 23, 42);
            this.lblTitle.Location = new Point(20, 16);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new Size(0, 26);
            this.lblTitle.Text = "";

            // lblInstruction
            this.lblInstruction.AutoSize = true;
            this.lblInstruction.Font = new Font("Segoe UI", 9F);
            this.lblInstruction.ForeColor = Color.FromArgb(100, 116, 139);
            this.lblInstruction.Location = new Point(22, 45);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Size = new Size(250, 15);
            this.lblInstruction.Text = "Please check exactly X units:";

            // flowUnits
            this.flowUnits.AutoScroll = true;
            this.flowUnits.BackColor = Color.Transparent;
            this.flowUnits.FlowDirection = FlowDirection.TopDown;
            this.flowUnits.Location = new Point(20, 70);
            this.flowUnits.Name = "flowUnits";
            this.flowUnits.Size = new Size(480, 175); // Wider list container
            this.flowUnits.WrapContents = false;

            // btnCancel
            this.btnCancel.Animated = true;
            this.btnCancel.BorderRadius = 17;
            this.btnCancel.BorderThickness = 1;
            this.btnCancel.BorderColor = Color.FromArgb(226, 232, 240);
            this.btnCancel.Cursor = Cursors.Hand;
            this.btnCancel.FillColor = Color.White;
            this.btnCancel.HoverState.FillColor = Color.FromArgb(241, 245, 249);
            this.btnCancel.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            this.btnCancel.ForeColor = Color.FromArgb(71, 85, 105);
            this.btnCancel.Location = new Point(230, 275);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(125, 38);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);

            // btnOk
            this.btnOk.Animated = true;
            this.btnOk.BorderRadius = 17;
            this.btnOk.Cursor = Cursors.Hand;
            this.btnOk.FillColor = Color.FromArgb(37, 99, 235);
            this.btnOk.HoverState.FillColor = Color.FromArgb(29, 78, 216);
            this.btnOk.Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold);
            this.btnOk.ForeColor = Color.White;
            this.btnOk.Location = new Point(365, 275);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new Size(135, 38);
            this.btnOk.Text = "Confirm";
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);

            // ShowMultiUnitSelectionPopup
            this.AutoScaleDimensions = new SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(520, 335); // Expanded form size
            this.Controls.Add(this.panelContent);
            this.FormBorderStyle = FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ShowMultiUnitSelectionPopup";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Select Inventory Units";

            this.panelContent.ResumeLayout(false);
            this.panelContent.PerformLayout();
            this.ResumeLayout(false);
        }
    }
}