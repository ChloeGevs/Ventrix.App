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

            // Attach clicks to the entire tile
            this.Click += (s, e) => AlertClicked?.Invoke(this, e);
            lblAlertMsg.Click += (s, e) => AlertClicked?.Invoke(this, e);
            pnlAlert.Click += (s, e) => AlertClicked?.Invoke(this, e);
        }
    }
}