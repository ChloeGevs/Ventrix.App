namespace Ventrix.App.Controls
{
    partial class MetricCard
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
            pnlCard = new Guna.UI2.WinForms.Guna2Panel();
            lblCount = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            pnlCard.SuspendLayout();
            SuspendLayout();
            // 
            // pnlCard
            // 
            pnlCard.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlCard.BorderRadius = 15;
            pnlCard.Controls.Add(lblCount);
            pnlCard.Controls.Add(lblTitle);
            pnlCard.CustomizableEdges = customizableEdges1;
            pnlCard.FillColor = Color.White;
            pnlCard.Location = new Point(0, 0);
            pnlCard.Name = "pnlCard";
            pnlCard.ShadowDecoration.CustomizableEdges = customizableEdges2;
            pnlCard.Size = new Size(200, 110);
            pnlCard.TabIndex = 0;
            // 
            // lblCount
            // 
            lblCount.BackColor = Color.Transparent;
            lblCount.Font = new Font("Microsoft Sans Serif", 18F, FontStyle.Bold);
            lblCount.Location = new Point(20, 55);
            lblCount.Name = "lblCount";
            lblCount.Size = new Size(3, 2);
            lblCount.TabIndex = 0;
            lblCount.Text = null;
            // 
            // lblTitle
            // 
            lblTitle.BackColor = Color.Transparent;
            lblTitle.Font = new Font("Microsoft Sans Serif", 10F);
            lblTitle.ForeColor = Color.Gray;
            lblTitle.Location = new Point(20, 15);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(3, 2);
            lblTitle.TabIndex = 1;
            lblTitle.Text = null;
            // 
            // MetricCard
            // 
            Controls.Add(pnlCard);
            Name = "MetricCard";
            Size = new Size(200, 110);
            pnlCard.ResumeLayout(false);
            pnlCard.PerformLayout();
            ResumeLayout(false);
        }

        private Guna.UI2.WinForms.Guna2Panel pnlCard;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCount;
    }
}