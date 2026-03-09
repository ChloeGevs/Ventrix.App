namespace Ventrix.App.Controls
{
    partial class ActivityCard
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
            pnlContainer = new Guna.UI2.WinForms.Guna2Panel();
            pnlStatusIndicator = new Guna.UI2.WinForms.Guna2Panel();
            lblMessage = new Label();
            lblTimestamp = new Label();
            SuspendLayout();
            // 
            // pnlContainer
            // 
            pnlContainer.BackColor = SystemColors.Control;
            pnlContainer.BorderRadius = 12;
            pnlContainer.CustomizableEdges = customizableEdges1;
            pnlContainer.Dock = DockStyle.Fill;
            pnlContainer.FillColor = Color.White;
            pnlContainer.Location = new Point(0, 0);
            pnlContainer.Name = "pnlContainer";
            pnlContainer.ShadowDecoration.CustomizableEdges = customizableEdges2;
            pnlContainer.Size = new Size(400, 85);
            pnlContainer.TabIndex = 3;
            // 
            // pnlStatusIndicator
            // 
            pnlStatusIndicator.BackColor = SystemColors.Control;
            pnlStatusIndicator.CustomizableEdges = customizableEdges3;
            pnlStatusIndicator.Dock = DockStyle.Left;
            pnlStatusIndicator.Location = new Point(0, 0);
            pnlStatusIndicator.Name = "pnlStatusIndicator";
            pnlStatusIndicator.ShadowDecoration.CustomizableEdges = customizableEdges4;
            pnlStatusIndicator.Size = new Size(6, 85);
            pnlStatusIndicator.TabIndex = 2;
            // 
            // lblMessage
            // 
            lblMessage.BackColor = Color.White;
            lblMessage.AutoSize = true;
            lblMessage.Font = new Font("Segoe UI Semibold", 11F, FontStyle.Bold);
            lblMessage.Location = new Point(22, 18);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 25);
            lblMessage.TabIndex = 1;
            // 
            // lblTimestamp
            // 
            lblTimestamp.BackColor = Color.White;
            lblTimestamp.AutoSize = true;
            lblTimestamp.Font = new Font("Segoe UI", 8.5F);
            lblTimestamp.ForeColor = Color.DarkGray;
            lblTimestamp.Location = new Point(22, 45);
            lblTimestamp.Name = "lblTimestamp";
            lblTimestamp.Size = new Size(0, 20);
            lblTimestamp.TabIndex = 0;
            // 
            // ActivityCard
            // 
            BackColor =  SystemColors.Control;
            Controls.Add(lblTimestamp);
            Controls.Add(lblMessage);
            Controls.Add(pnlStatusIndicator);
            Controls.Add(pnlContainer);
            Name = "ActivityCard";
            Size = new Size(400, 85);
            ResumeLayout(false);
            PerformLayout();
        }

        private Guna.UI2.WinForms.Guna2Panel pnlContainer;
        private Guna.UI2.WinForms.Guna2Panel pnlStatusIndicator;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.Label lblTimestamp;
    }
}