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
            lblTitle = new System.Windows.Forms.Label();
            lblInstruction = new System.Windows.Forms.Label();
            pnlRecords = new System.Windows.Forms.FlowLayoutPanel();
            btnConfirm = new Guna.UI2.WinForms.Guna2Button();
            btnCancel = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // borderlessForm
            // 
            borderlessForm.BorderRadius = 12;
            borderlessForm.ContainerControl = this;
            borderlessForm.DockIndicatorTransparencyValue = 0.6D;
            borderlessForm.ShadowColor = System.Drawing.Color.FromArgb(20, 0, 0, 0);
            borderlessForm.TransparentWhileDrag = true;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            lblTitle.ForeColor = System.Drawing.Color.FromArgb(31, 41, 55);
            lblTitle.Location = new System.Drawing.Point(23, 27);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new System.Drawing.Size(64, 32);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Title";
            // 
            // lblInstruction
            // 
            lblInstruction.AutoSize = true;
            lblInstruction.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            lblInstruction.ForeColor = System.Drawing.Color.FromArgb(107, 114, 128);
            lblInstruction.Location = new System.Drawing.Point(23, 73);
            lblInstruction.Name = "lblInstruction";
            lblInstruction.Size = new System.Drawing.Size(84, 21);
            lblInstruction.TabIndex = 1;
            lblInstruction.Text = "Instruction";
            // 
            // pnlRecords
            // 
            pnlRecords.AutoScroll = true;
            pnlRecords.BackColor = System.Drawing.Color.FromArgb(249, 250, 251);
            pnlRecords.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            pnlRecords.Location = new System.Drawing.Point(23, 120);
            pnlRecords.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            pnlRecords.Name = "pnlRecords";
            pnlRecords.Padding = new System.Windows.Forms.Padding(6, 7, 6, 7);
            pnlRecords.Size = new System.Drawing.Size(434, 200);
            pnlRecords.TabIndex = 2;
            pnlRecords.WrapContents = false;
            // 
            // btnConfirm
            // 
            btnConfirm.BorderRadius = 8;
            btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            btnConfirm.CustomizableEdges = customizableEdges3;
            btnConfirm.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            btnConfirm.ForeColor = System.Drawing.Color.White;
            btnConfirm.Location = new System.Drawing.Point(171, 328);
            btnConfirm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnConfirm.Size = new System.Drawing.Size(229, 56);
            btnConfirm.TabIndex = 3;
            btnConfirm.Text = "Confirm";
            btnConfirm.Click += btnConfirm_Click;
            // 
            // btnCancel
            // 
            btnCancel.BorderColor = System.Drawing.Color.FromArgb(229, 231, 235);
            btnCancel.BorderRadius = 8;
            btnCancel.BorderThickness = 1;
            btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            btnCancel.CustomizableEdges = customizableEdges1;
            btnCancel.FillColor = System.Drawing.Color.White;
            btnCancel.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            btnCancel.ForeColor = System.Drawing.Color.FromArgb(75, 85, 99);
            btnCancel.HoverState.FillColor = System.Drawing.Color.FromArgb(243, 244, 246);
            btnCancel.Location = new System.Drawing.Point(51, 328);
            btnCancel.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            btnCancel.Name = "btnCancel";
            btnCancel.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnCancel.Size = new System.Drawing.Size(114, 56);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "Cancel";
            btnCancel.Click += btnCancel_Click;
            // 
            // MultiRecordSelectionPopup
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            BackColor = System.Drawing.Color.White;
            ClientSize = new System.Drawing.Size(480, 427);
            Controls.Add(btnCancel);
            Controls.Add(btnConfirm);
            Controls.Add(pnlRecords);
            Controls.Add(lblInstruction);
            Controls.Add(lblTitle);
            FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            Name = "MultiRecordSelectionPopup";
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
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