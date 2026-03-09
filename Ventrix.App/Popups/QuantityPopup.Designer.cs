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
            
            lblMessage = new System.Windows.Forms.Label();
            
            txtQuantity = new Guna.UI2.WinForms.Guna2TextBox();
            btnConfirm = new Guna.UI2.WinForms.Guna2Button();
            btnCancel = new Guna.UI2.WinForms.Guna2Button();
            panelContent = new System.Windows.Forms.Panel();
            panelContent.SuspendLayout();
            SuspendLayout();

            // panelContent
            panelContent.BackColor = System.Drawing.Color.White;
            panelContent.Controls.Add(btnCancel);
            panelContent.Controls.Add(btnConfirm);
            panelContent.Controls.Add(txtQuantity);
            panelContent.Controls.Add(lblMessage);
            panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            panelContent.Location = new System.Drawing.Point(0, 64); 
            panelContent.Name = "panelContent";
            panelContent.Padding = new System.Windows.Forms.Padding(20);
            panelContent.Size = new System.Drawing.Size(350, 166);

            // lblMessage
            lblMessage.AutoSize = true;
            lblMessage.Font = new System.Drawing.Font("Segoe UI Semibold", 10.5F, System.Drawing.FontStyle.Bold);
            lblMessage.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            lblMessage.Location = new System.Drawing.Point(23, 20);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new System.Drawing.Size(262, 19);
            lblMessage.Text = "How many identical units to add?";

            // txtQuantity
            txtQuantity.BorderRadius = 6;
            txtQuantity.Cursor = System.Windows.Forms.Cursors.IBeam;
            txtQuantity.DefaultText = "1";
            txtQuantity.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            txtQuantity.ForeColor = System.Drawing.Color.FromArgb(13, 71, 161);
            txtQuantity.Location = new System.Drawing.Point(27, 50);
            txtQuantity.Name = "txtQuantity";
            txtQuantity.Size = new System.Drawing.Size(296, 45);
            txtQuantity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;

            // btnConfirm
            btnConfirm.BorderRadius = 6;
            btnConfirm.FillColor = System.Drawing.Color.FromArgb(13, 71, 161);
            btnConfirm.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            btnConfirm.ForeColor = System.Drawing.Color.White;
            btnConfirm.Location = new System.Drawing.Point(178, 110);
            btnConfirm.Name = "btnConfirm";
            btnConfirm.Size = new System.Drawing.Size(145, 40);
            btnConfirm.Text = "ADD UNITS";
            btnConfirm.Click += new System.EventHandler(btnConfirm_Click);

            // btnCancel
            btnCancel.BorderRadius = 6;
            btnCancel.BorderThickness = 1;
            btnCancel.BorderColor = System.Drawing.Color.FromArgb(200, 200, 200);
            btnCancel.FillColor = System.Drawing.Color.White;
            btnCancel.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);
            btnCancel.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            btnCancel.Location = new System.Drawing.Point(27, 110);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new System.Drawing.Size(145, 40);
            btnCancel.Text = "CANCEL";
            btnCancel.Click += new System.EventHandler(btnCancel_Click);

            // QuantityPopup
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(350, 230);
            Controls.Add(panelContent);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "QuantityPopup";
            Sizable = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
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