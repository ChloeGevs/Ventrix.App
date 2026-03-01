using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ventrix.App.Controls
{
    public partial class ActivityCard : UserControl
    {
        public ActivityCard(string message, DateTime time, Color statusColor)
        {
            InitializeComponent();
            lblMessage.Text = message;
            lblTimestamp.Text = $"🕒 {time.ToString("hh:mm tt")} • {time.ToString("MMM dd, yyyy")}";
            pnlStatusIndicator.FillColor = statusColor;

            // Hover effects isolated to the control
            pnlContainer.MouseEnter += (s, e) => pnlContainer.FillColor = Color.FromArgb(252, 252, 252);
            pnlContainer.MouseLeave += (s, e) => pnlContainer.FillColor = Color.White;
        }
    }
}