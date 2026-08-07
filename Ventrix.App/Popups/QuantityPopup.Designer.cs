namespace Ventrix.App.Popups
{
    partial class QuantityPopup
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            lblMessage = new System.Windows.Forms.Label();
            txtQuantity = new Guna.UI2.WinForms.Guna2TextBox();
            btnConfirm = new Guna.UI2.WinForms.Guna2Button();
            btnCancel = new Guna.UI2.WinForms.Guna2Button();
            panelContent = new System.Windows.Forms.Panel();
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            panelContent.SuspendLayout();
            SuspendLayout();

            // guna2BorderlessForm1
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.BorderRadius = 14;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;

            // panelContent
            panelContent.BackColor = System.Drawing.Color.White;
            panelContent.Controls.Add(btnCancel);
            panelContent.Controls.Add(btnConfirm);
            panelContent.Controls.Add(txtQuantity);
            panelContent.Controls.Add(lblMessage);
            panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            panelContent.Location = new System.Drawing.Point(0, 64); // Adjusted for MaterialForm header
            panelContent.Name = "panelContent";
            panelContent.Size = new System.Drawing.Size(380, 186);
            panelContent.TabIndex = 0;

            // lblMessage
            lblMessage.AutoSize = false;
            lblMessage.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            lblMessage.ForeColor = System.Drawing.Color.FromArgb(15, 23, 42);
            lblMessage.Location = new System.Drawing.Point(0, 20);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new System.Drawing.Size(380, 25);
            lblMessage.TabIndex = 3;
            lblMessage.Text = "How many identical units to add?";
            lblMessage.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;

            // txtQuantity
            txtQuantity.Animated = true; // Smooth native transitions
            txtQuantity.BorderRadius = 10;
            txtQuantity.Cursor = System.Windows.Forms.Cursors.IBeam;
            txtQuantity.CustomizableEdges = customizableEdges1;
            txtQuantity.DefaultText = "1";
            txtQuantity.Font = new System.Drawing.Font("Segoe UI", 20F, System.Drawing.FontStyle.Bold);
            txtQuantity.ForeColor = System.Drawing.Color.FromArgb(79, 70, 229);
            txtQuantity.FillColor = System.Drawing.Color.FromArgb(248, 250, 252);
            txtQuantity.Location = new System.Drawing.Point(115, 60);
            txtQuantity.Name = "txtQuantity";
            txtQuantity.ShadowDecoration.CustomizableEdges = customizableEdges2;
            txtQuantity.Size = new System.Drawing.Size(150, 55); // Scaled down slightly
            txtQuantity.TabIndex = 2;
            txtQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            txtQuantity.FocusedState.BorderColor = System.Drawing.Color.FromArgb(99, 102, 241);

            // btnConfirm
            btnConfirm.Animated = true; // Smooth hover effects
            btnConfirm.BorderRadius = 20;
            btnConfirm.Cursor = System.Windows.Forms.Cursors.Hand;
            btnConfirm.CustomizableEdges = customizableEdges3;
            btnConfirm.FillColor = System.Drawing.Color.FromArgb(79, 70, 229);
            btnConfirm.HoverState.FillColor = System.Drawing.Color.FromArgb(67, 56, 202);
            btnConfirm.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            btnConfirm.ForeColor = System.Drawing.Color.White;
            btnConfirm.Location = new System.Drawing.Point(195, 132);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnConfirm.Size = new System.Drawing.Size(140, 40); // Tighter button size
            btnConfirm.TabIndex = 1;
            btnConfirm.Text = "Add Units";
            btnConfirm.Click += new System.EventHandler(this.btnConfirm_Click);

            // btnCancel
            btnCancel.Animated = true;
            btnCancel.BorderRadius = 20;
            btnCancel.BorderThickness = 1;
            btnCancel.BorderColor = System.Drawing.Color.FromArgb(226, 232, 240);
            btnCancel.Cursor = System.Windows.Forms.Cursors.Hand;
            btnCancel.CustomizableEdges = customizableEdges5;
            btnCancel.FillColor = System.Drawing.Color.White;
            btnCancel.HoverState.FillColor = System.Drawing.Color.FromArgb(241, 245, 249);
            btnCancel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            btnCancel.ForeColor = System.Drawing.Color.FromArgb(71, 85, 105);
            btnCancel.Location = new System.Drawing.Point(45, 132);
            btnCancel.Name = "btnCancel";
            btnCancel.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnCancel.Size = new System.Drawing.Size(140, 40);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "Cancel";
            btnCancel.Click += new System.EventHandler(this.btnCancel_Click);

            // QuantityPopup
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(380, 250); // Significantly tighter overall footprint
            Controls.Add(panelContent);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "QuantityPopup";
            Sizable = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Batch Add Units";
            panelContent.ResumeLayout(false);
            ResumeLayout(false);
        }

        private System.Windows.Forms.Label lblMessage;
        private Guna.UI2.WinForms.Guna2TextBox txtQuantity;
        private Guna.UI2.WinForms.Guna2Button btnConfirm;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
        private System.Windows.Forms.Panel panelContent;
        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
    }
}