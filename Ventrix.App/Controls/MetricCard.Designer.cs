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
            pnlCard = new Guna.UI2.WinForms.Guna2Panel();
            lblTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblCount = new Guna.UI2.WinForms.Guna2HtmlLabel();
            SuspendLayout();

            // 1. Setup the Panel first but DO NOT use Dock.Fill
            pnlCard.BorderRadius = 15;
            pnlCard.FillColor = System.Drawing.Color.White;
            pnlCard.Location = new System.Drawing.Point(0, 0);
            pnlCard.Size = new System.Drawing.Size(200, 110);
            // Use Anchor instead of Dock to prevent the panel from fighting the Dashboard layout
            pnlCard.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom;

            lblTitle.Font = new System.Drawing.Font("Sitka", 10F);
            lblTitle.ForeColor = System.Drawing.Color.Gray;
            lblTitle.Location = new System.Drawing.Point(20, 15);

            lblCount.Font = new System.Drawing.Font("Sitka", 18F, System.Drawing.FontStyle.Bold);
            lblCount.Location = new System.Drawing.Point(20, 55);

            // 2. CRITICAL: Add the Panel FIRST so it is at the bottom (Z-order)
            Controls.Add(this.pnlCard);

            // 3. Add labels TO THE PANEL, not the UserControl directly
            // This ensures they move WITH the card and aren't hidden behind it
            this.pnlCard.Controls.Add(this.lblCount);
            this.pnlCard.Controls.Add(this.lblTitle);

            Size = new System.Drawing.Size(200, 110);
            ResumeLayout(false);
        }

        private Guna.UI2.WinForms.Guna2Panel pnlCard;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCount;
    }
}