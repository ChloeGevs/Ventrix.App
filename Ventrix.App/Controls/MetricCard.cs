using System;
using System.Windows.Forms;

namespace Ventrix.App.Controls
{
    public partial class MetricCard : UserControl
    {
        // 1. Define the event so the Dashboard can "see" it
        public event EventHandler CardClicked;

        public MetricCard()
        {
            InitializeComponent();

            BindClickEvents(this);
        }

        private void BindClickEvents(Control container)
        {
            foreach (Control ctrl in container.Controls)
            {
                ctrl.Click += (s, e) => CardClicked?.Invoke(this, e);
                if (ctrl.HasChildren) BindClickEvents(ctrl);
            }
            container.Click += (s, e) => CardClicked?.Invoke(this, e);
        }

        public void UpdateMetrics(string title, string count, System.Drawing.Color accentColor)
        {
            lblTitle.Text = title;
            lblCount.Text = count;
            lblCount.ForeColor = accentColor;

            lblTitle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            lblCount.Font = new System.Drawing.Font("Segoe UI", 18F, System.Drawing.FontStyle.Bold);
        }
    }
}