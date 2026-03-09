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

            AutoSize = false;

            Margin = new Padding(3);

            MinimumSize = new Size(100, 60);

            Click += (s, e) => AlertClicked?.Invoke(this, e);
            lblAlertMsg.Click += (s, e) => AlertClicked?.Invoke(this, e);
            pnlAlert.Click += (s, e) => AlertClicked?.Invoke(this, e);
        }
    }
}