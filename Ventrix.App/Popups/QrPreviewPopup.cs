using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Ventrix.App.Popups
{
    public class QrPreviewPopup : Form
    {
        private Bitmap _qrImage;
        private PictureBox picQr;

        public QrPreviewPopup(Bitmap qrImage, string title)
        {
            _qrImage = qrImage;
            this.Text = title;
            this.Size = new Size(400, 550);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            picQr = new PictureBox
            {
                Image = _qrImage,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(300, 350),
                Location = new Point(40, 30),
                BorderStyle = BorderStyle.FixedSingle
            };

            Button btnPrint = new Button
            {
                Text = "🖨️ Print Now",
                Size = new Size(140, 45),
                Location = new Point(40, 410),
                BackColor = Color.FromArgb(13, 71, 161),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnPrint.FlatAppearance.BorderSize = 0;
            btnPrint.Click += BtnPrint_Click;

            Button btnClose = new Button
            {
                Text = "Close",
                Size = new Size(140, 45),
                Location = new Point(200, 410),
                BackColor = Color.FromArgb(243, 244, 246),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            this.Controls.Add(picQr);
            this.Controls.Add(btnPrint);
            this.Controls.Add(btnClose);
        }

        private void BtnPrint_Click(object sender, EventArgs e)
        {
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, ev) =>
            {
                ev.Graphics!.DrawImage(_qrImage, new Point(100, 100));
            };

            PrintDialog printDialog = new PrintDialog { Document = pd };
            if (printDialog.ShowDialog() == DialogResult.OK)
            {
                pd.Print();
            }
        }
    }
}