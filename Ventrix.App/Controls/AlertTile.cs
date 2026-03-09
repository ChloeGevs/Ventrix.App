using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ventrix.App.Controls
{
    public partial class AlertTile : UserControl
    {
        public event EventHandler AlertClicked;

        public AlertTile(string message, Color alertColor)
        {
            InitializeComponent();
            lblAlertMsg.Text = message;
            lblAlertMsg.ForeColor = alertColor;

            // --- LAYOUT FIXES FOR RESPONSIVENESS ---
            // Prevent the control from autosizing based on the text length
            this.AutoSize = false;

            // Add a small margin so it doesn't hug the edges of the FlowLayoutPanel
            this.Margin = new Padding(3);

            // Set a minimum size so it never completely collapses
            this.MinimumSize = new Size(100, 60);

            // Attach clicks to the entire tile
            this.Click += (s, e) => AlertClicked?.Invoke(this, e);
            lblAlertMsg.Click += (s, e) => AlertClicked?.Invoke(this, e);
            pnlAlert.Click += (s, e) => AlertClicked?.Invoke(this, e);
        }
    }
}