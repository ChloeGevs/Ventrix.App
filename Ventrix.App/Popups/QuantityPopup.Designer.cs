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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            lblMessage = new Label();
            txtQuantity = new Guna.UI2.WinForms.Guna2TextBox();
            btnConfirm = new Guna.UI2.WinForms.Guna2Button();
            btnCancel = new Guna.UI2.WinForms.Guna2Button();
            panelContent = new Panel();
            panelContent.SuspendLayout();
            SuspendLayout();
            // 
            // lblMessage
            // 
            lblMessage.AutoSize = true;
            lblMessage.Font = new Font("Segoe UI Semibold", 10.5F, FontStyle.Bold);
            lblMessage.ForeColor = Color.FromArgb(64, 64, 64);
            lblMessage.Location = new Point(31, 31);
            lblMessage.Margin = new Padding(4, 0, 4, 0);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(290, 25);
            lblMessage.TabIndex = 3;
            lblMessage.Text = "How many identical units to add?";
            // 
            // txtQuantity
            // 
            txtQuantity.BorderRadius = 6;
            txtQuantity.Cursor = Cursors.IBeam;
            txtQuantity.CustomizableEdges = customizableEdges1;
            txtQuantity.DefaultText = "1";
            txtQuantity.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            txtQuantity.ForeColor = Color.FromArgb(13, 71, 161);
            txtQuantity.Location = new Point(36, 77);
            txtQuantity.Margin = new Padding(4, 6, 4, 6);
            txtQuantity.Name = "txtQuantity";
            txtQuantity.PlaceholderText = "";
            txtQuantity.SelectedText = "";
            txtQuantity.ShadowDecoration.CustomizableEdges = customizableEdges2;
            txtQuantity.Size = new Size(395, 69);
            txtQuantity.TabIndex = 2;
            txtQuantity.TextAlign = HorizontalAlignment.Center;
            // 
            // btnConfirm
            // 
            btnConfirm.BorderRadius = 6;
            btnConfirm.Cursor = Cursors.Hand;
            btnConfirm.CustomizableEdges = customizableEdges3;
            btnConfirm.FillColor = Color.FromArgb(13, 71, 161);
            btnConfirm.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnConfirm.ForeColor = Color.White;
            btnConfirm.Location = new Point(237, 169);
            btnConfirm.Margin = new Padding(4, 5, 4, 5);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnConfirm.Size = new Size(193, 62);
            btnConfirm.TabIndex = 1;
            btnConfirm.Text = "ADD UNITS";
            btnConfirm.Click += btnConfirm_Click;
            // 
            // btnCancel
            // 
            btnCancel.BorderColor = Color.FromArgb(200, 200, 200);
            btnCancel.BorderRadius = 6;
            btnCancel.BorderThickness = 1;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.CustomizableEdges = customizableEdges5;
            btnCancel.FillColor = Color.White;
            btnCancel.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnCancel.ForeColor = Color.FromArgb(64, 64, 64);
            btnCancel.Location = new Point(36, 169);
            btnCancel.Margin = new Padding(4, 5, 4, 5);
            btnCancel.Name = "btnCancel";
            btnCancel.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnCancel.Size = new Size(193, 62);
            btnCancel.TabIndex = 0;
            btnCancel.Text = "CANCEL";
            btnCancel.Click += btnCancel_Click;
            // 
            // panelContent
            // 
            panelContent.BackColor = Color.White;
            panelContent.Controls.Add(btnCancel);
            panelContent.Controls.Add(btnConfirm);
            panelContent.Controls.Add(txtQuantity);
            panelContent.Controls.Add(lblMessage);
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(4, 98);
            panelContent.Margin = new Padding(4, 5, 4, 5);
            panelContent.Name = "panelContent";
            panelContent.Padding = new Padding(27, 31, 27, 31);
            panelContent.Size = new Size(459, 251);
            panelContent.TabIndex = 0;
            // 
            // QuantityPopup
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(467, 354);
            Controls.Add(panelContent);
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "QuantityPopup";
            Padding = new Padding(4, 98, 4, 5);
            Sizable = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Batch Add Units";
            panelContent.ResumeLayout(false);
            panelContent.PerformLayout();
            ResumeLayout(false);
        }

        private System.Windows.Forms.Label lblMessage;
        private Guna.UI2.WinForms.Guna2TextBox txtQuantity;
        private Guna.UI2.WinForms.Guna2Button btnConfirm;
        private Guna.UI2.WinForms.Guna2Button btnCancel;
        private System.Windows.Forms.Panel panelContent;
    }
}