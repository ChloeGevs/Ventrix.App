using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;

namespace Ventrix.App
{
    public partial class FormRegistrationSuccess : Form
    {
        public FormRegistrationSuccess()
        {
            InitializeComponent();
        }

        private void pnlBackground_Paint(object sender, PaintEventArgs e)
        {
            // Create a rectangle that covers the entire panel
            Rectangle rect = pnlBackground.ClientRectangle;

            // Create a linear gradient brush (Horizontal from Red to Blue)
            using (LinearGradientBrush brush = new LinearGradientBrush(rect,
                   Color.FromArgb(192, 255, 255), Color.Teal, LinearGradientMode.Vertical))
            {
                // Fill the panel's background with the gradient
                e.Graphics.FillRectangle(brush, rect);
            }
        }
    }
}
