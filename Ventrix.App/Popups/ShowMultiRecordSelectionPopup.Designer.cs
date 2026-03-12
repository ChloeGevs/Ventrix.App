namespace Ventrix.App.Popups
{
    partial class MultiRecordSelectionPopup
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            borderlessForm = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            lblTitle = new Label();
            lblInstruction = new Label();
            pnlRecords = new FlowLayoutPanel();
            btnConfirm = new Guna.UI2.WinForms.Guna2Button();
            btnCancel = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // borderlessForm
            // 
            borderlessForm.BorderRadius = 12;
            borderlessForm.ContainerControl = this;
            borderlessForm.DockIndicatorTransparencyValue = 0.6D;
            borderlessForm.ShadowColor = Color.FromArgb(20, 0, 0, 0);
            borderlessForm.TransparentWhileDrag = true;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(31, 41, 55);
            lblTitle.Location = new Point(23, 27);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(64, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Title";
            // 
            // lblInstruction
            // 
            lblInstruction.AutoSize = true;
            lblInstruction.Font = new Font("Segoe UI", 9.5F);
            lblInstruction.ForeColor = Color.FromArgb(107, 114, 128);
            lblInstruction.Location = new Point(23, 73);
            lblInstruction.Name = "lblInstruction";
            lblInstruction.Size = new Size(84, 21);
            lblInstruction.TabIndex = 1;
            lblInstruction.Text = "Instruction";
            // 
            // pnlRecords
            // 
            pnlRecords.AutoScroll = true;
            pnlRecords.BackColor = Color.FromArgb(249, 250, 251);
            pnlRecords.FlowDirection = FlowDirection.TopDown;
            pnlRecords.Location = new Point(23, 120);
            pnlRecords.Margin = new Padding(3, 4, 3, 4);
            pnlRecords.Name = "pnlRecords";
            pnlRecords.Padding = new Padding(6, 7, 6, 7);
            pnlRecords.Size = new Size(434, 200);
            pnlRecords.TabIndex = 2;
            pnlRecords.WrapContents = false;
            // 
            // btnConfirm
            // 
            btnConfirm.BorderRadius = 8;
            btnConfirm.Cursor = Cursors.Hand;
            btnConfirm.CustomizableEdges = customizableEdges3;
            btnConfirm.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnConfirm.ForeColor = Color.White;
            btnConfirm.Location = new Point(171, 328);
            btnConfirm.Margin = new Padding(3, 4, 3, 4);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnConfirm.Size = new Size(229, 56);
            btnConfirm.TabIndex = 3;
            btnConfirm.Text = "Confirm";
            btnConfirm.Click += btnConfirm_Click;
            // 
            // btnCancel
            // 
            btnCancel.BorderColor = Color.FromArgb(229, 231, 235);
            btnCancel.BorderRadius = 8;
            btnCancel.BorderThickness = 1;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.CustomizableEdges = customizableEdges1;
            btnCancel.FillColor = Color.White;
            btnCancel.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            btnCancel.ForeColor = Color.FromArgb(75, 85, 99);
            btnCancel.HoverState.FillColor = Color.FromArgb(243, 244, 246);
            btnCancel.Location = new Point(51, 328);
            btnCancel.Margin = new Padding(3, 4, 3, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnCancel.Size = new Size(114, 56);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // MultiRecordSelectionPopup
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(480, 427);
            Controls.Add(btnCancel);
            Controls.Add(btnConfirm);
            Controls.Add(pnlRecords);
            Controls.Add(lblInstruction);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.None;
            Margin = new Padding(3, 4, 3, 4);
            Name = "MultiRecordSelectionPopup";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Select Records";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm borderlessForm;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblInstruction;
        private System.Windows.Forms.FlowLayoutPanel pnlRecords;
        private Guna.UI2.WinForms.Guna2Button btnConfirm;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
    }
}