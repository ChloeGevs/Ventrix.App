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

            pnlCard.BorderRadius = 15;
            pnlCard.FillColor = System.Drawing.Color.White;
            pnlCard.Dock = System.Windows.Forms.DockStyle.Fill;

            lblTitle.Font = new System.Drawing.Font("Sitka", 10F);
            lblTitle.ForeColor = System.Drawing.Color.Gray;
            lblTitle.Location = new System.Drawing.Point(20, 15);

            lblCount.Font = new System.Drawing.Font("Sitka", 18F, System.Drawing.FontStyle.Bold);
            lblCount.Location = new System.Drawing.Point(20, 55);

            Controls.Add(this.lblCount);
            Controls.Add(this.lblTitle);
            Controls.Add(this.pnlCard);
            Size = new System.Drawing.Size(200, 110);
            ResumeLayout(false);
        }

        private Guna.UI2.WinForms.Guna2Panel pnlCard;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitle;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCount;
    }
}