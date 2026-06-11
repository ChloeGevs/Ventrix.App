using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Windows.Forms;

namespace Ventrix.App.Popups
{
    // Enum to tell the dashboard which button the admin clicked
    public enum QrExportOption { None, ExportSpecific, ExportAll }

    public class QrManagerPopup : Form
    {
        private Bitmap _qrImage;
        public QrExportOption SelectedOption { get; private set; } = QrExportOption.None;

        public QrManagerPopup(Bitmap qrImage, string targetName, string targetType)
        {
            _qrImage = qrImage;
            this.Text = $"Manage QR Tags - {targetName}";
            this.Size = new Size(570, 480);
            this.StartPosition = FormStartPosition.CenterParent;
            this.BackColor = Color.White;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;

            // Header Texts
            Label lblTitle = new Label
            {
                Text = $"QR Tag Manager",
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                Location = new Point(30, 25),
                AutoSize = true
            };
            Label lblSub = new Label
            {
                Text = targetName,
                Font = new Font("Segoe UI", 11F, FontStyle.Regular),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(32, 55),
                AutoSize = true
            };

            // PictureBox (Preview)
            PictureBox picQr = new PictureBox
            {
                Image = _qrImage,
                SizeMode = PictureBoxSizeMode.Zoom,
                Size = new Size(220, 260),
                Location = new Point(30, 95),
                BorderStyle = BorderStyle.FixedSingle
            };

            // Print Current Button (Directly under the PictureBox)
            Button btnPrintCurrent = new Button
            {
                Text = "🖨️ Print Previewed Tag",
                Size = new Size(220, 40),
                Location = new Point(30, 365),
                BackColor = Color.FromArgb(243, 244, 246),
                ForeColor = Color.Black,
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnPrintCurrent.FlatAppearance.BorderSize = 0;
            btnPrintCurrent.Click += BtnPrintCurrent_Click;

            // Description Label
            Label lblInfo = new Label
            {
                Text = $"How would you like to export QR tags for the {targetType}?",
                Font = new Font("Segoe UI Semibold", 10F),
                Location = new Point(280, 95),
                Size = new Size(250, 45)
            };

            // Export Specific Button
            Button btnExportSpecific = new Button
            {
                Text = "📝 Select Specific Tags...",
                Size = new Size(250, 50),
                Location = new Point(280, 145),
                BackColor = Color.FromArgb(245, 158, 11), // Warning Amber
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExportSpecific.FlatAppearance.BorderSize = 0;
            btnExportSpecific.Click += (s, e) => { SelectedOption = QrExportOption.ExportSpecific; this.DialogResult = DialogResult.OK; this.Close(); };

            // Export All Button
            Button btnExportAll = new Button
            {
                Text = "📦 Export ALL in Database",
                Size = new Size(250, 50),
                Location = new Point(280, 205),
                BackColor = Color.FromArgb(13, 71, 161), // Ventrix Blue
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnExportAll.FlatAppearance.BorderSize = 0;
            btnExportAll.Click += (s, e) => { SelectedOption = QrExportOption.ExportAll; this.DialogResult = DialogResult.OK; this.Close(); };

            // Cancel Button
            Button btnClose = new Button
            {
                Text = "Cancel",
                Size = new Size(100, 40),
                Location = new Point(430, 365),
                BackColor = Color.FromArgb(243, 244, 246),
                FlatStyle = FlatStyle.Flat,
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.Click += (s, e) => this.Close();

            // Add Controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblSub);
            this.Controls.Add(picQr);
            this.Controls.Add(btnPrintCurrent);
            this.Controls.Add(lblInfo);
            this.Controls.Add(btnExportSpecific);
            this.Controls.Add(btnExportAll);
            this.Controls.Add(btnClose);
        }

        private void BtnPrintCurrent_Click(object sender, EventArgs e)
        {
            // Internal printing logic so they can print the single tag without leaving the manager!
            PrintDocument pd = new PrintDocument();
            pd.PrintPage += (s, ev) => ev.Graphics!.DrawImage(_qrImage, new Point(100, 100));
            PrintDialog printDialog = new PrintDialog { Document = pd };
            if (printDialog.ShowDialog() == DialogResult.OK) pd.Print();
        }
    }
}