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

            // 2. Make every internal label/panel trigger the CardClicked event
            BindClickEvents(this);
        }

        // This helper method loops through all labels inside the card
        // and tells them: "If you are clicked, tell the parent CardClicked fired."
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
        }
    }
}