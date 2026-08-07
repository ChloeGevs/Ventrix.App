using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ventrix.App.Popups
{
    public partial class BorrowerItemsPopup : Form
    {
        private System.Windows.Forms.Timer transitionTimer;
        private System.Windows.Forms.Timer marqueeTimer;
        private System.Windows.Forms.Timer activeTimer;

        private int targetY;
        private bool isClosing = false;
        private double activePulseAngle = 0;
        private double redLedPulseAngle = 0;
        private float marqueeOffset = -200f;

        public BorrowerItemsPopup(string borrowerName, string status, string itemsString)
        {
            InitializeComponent();

            this.StartPosition = FormStartPosition.CenterParent;
            this.Opacity = 0;
            this.FormBorderStyle = FormBorderStyle.None;
            this.BackColor = Color.White;

            this.Load += (s, e) => {
                this.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, this.Width, this.Height, 20, 20));

                targetY = this.Top;
                this.Top += 30;

                transitionTimer.Start();
                if (marqueeTimer != null) marqueeTimer.Start();
                if (activeTimer != null) activeTimer.Start();
            };

            // Physics-based Easing Animation Timer 
            transitionTimer = new System.Windows.Forms.Timer();
            transitionTimer.Interval = 12;
            transitionTimer.Tick += (s, e) => {
                if (!isClosing)
                {
                    this.Opacity += (1.0 - this.Opacity) * 0.15;

                    int distanceY = this.Top - targetY;
                    if (distanceY > 0)
                    {
                        this.Top -= Math.Max(1, distanceY / 4);
                    }
                    else if (this.Opacity >= 0.99)
                    {
                        this.Opacity = 1.0;
                        transitionTimer.Stop();
                    }
                }
                else
                {
                    this.Opacity -= 0.12;
                    this.Top += 3;
                    if (this.Opacity <= 0.05)
                    {
                        transitionTimer.Stop();
                        transitionTimer.Dispose();
                        this.Close();
                    }
                }
            };

            lblTitle.Text = borrowerName;

            // =================================================================
            // BADGE LOGIC & ANIMATIONS
            // =================================================================

            bool isPending = status == "Pending" || status == "PendingReturn";
            bool isActive = status.Equals("Active", StringComparison.OrdinalIgnoreCase);

            int panelWidth = this.Width - (lblBadge.Location.X * 2);

            Panel badgeContainer = new Panel
            {
                Location = new Point(lblBadge.Location.X, lblBadge.Location.Y - 4),
                Size = new Size(panelWidth, 38)
            };
            badgeContainer.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, badgeContainer.Width, badgeContainer.Height, 18, 18));

            Control badgeParent = lblBadge.Parent;
            badgeParent.Controls.Remove(lblBadge);
            badgeParent.Controls.Add(badgeContainer); // Fixed control nesting typo

            if (isPending)
            {
                badgeContainer.BackColor = Color.FromArgb(254, 242, 242);
                string tickerPhrase = status == "Pending" ? "⏳  Pending Approval            •            " : "⏳  Pending Return            •            ";
                string fullTickerText = tickerPhrase + tickerPhrase + tickerPhrase + tickerPhrase + tickerPhrase + tickerPhrase;

                typeof(Panel).InvokeMember("DoubleBuffered",
                    System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                    null, badgeContainer, new object[] { true });

                badgeContainer.Paint += (s, pe) => {
                    pe.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    redLedPulseAngle += 0.15;
                    int redIntensity = (int)(200 + 55 * Math.Sin(redLedPulseAngle));

                    using (Brush textBrush = new SolidBrush(Color.FromArgb(255, redIntensity > 255 ? 255 : redIntensity, 0, 0)))
                    {
                        using (Font tickerFont = new Font("Segoe UI", 9.5F, FontStyle.Bold))
                        {
                            SizeF textSize = pe.Graphics.MeasureString(fullTickerText, tickerFont);
                            float blockWidth = textSize.Width;

                            float xCoord = (marqueeOffset % blockWidth) - blockWidth;
                            while (xCoord < badgeContainer.Width)
                            {
                                pe.Graphics.DrawString(fullTickerText, tickerFont, textBrush, xCoord, 10);
                                xCoord += blockWidth;
                            }
                        }
                    }
                };

                marqueeTimer = new System.Windows.Forms.Timer();
                marqueeTimer.Interval = 16;
                marqueeTimer.Tick += (sender, args) => {
                    marqueeOffset += 1f;
                    badgeContainer.Invalidate();
                };
            }
            else if (isActive)
            {
                badgeContainer.BackColor = Color.FromArgb(209, 234, 219);

                badgeContainer.Controls.Add(lblBadge);
                lblBadge.BackColor = Color.Transparent;
                lblBadge.AutoSize = true;
                lblBadge.Text = "✨  ACTIVE";
                lblBadge.ForeColor = Color.FromArgb(6, 95, 70);
                lblBadge.Left = (badgeContainer.Width - lblBadge.PreferredWidth) / 2;
                lblBadge.Top = 9;

                int baseTop = 9;
                activeTimer = new System.Windows.Forms.Timer();
                activeTimer.Interval = 30;
                activeTimer.Tick += (sender, args) => {
                    activePulseAngle += 0.12;
                    lblBadge.Top = baseTop + (int)(2 * Math.Sin(activePulseAngle));
                    int pulseGreen = (int)(110 + 40 * Math.Sin(activePulseAngle));
                    try
                    {
                        lblBadge.ForeColor = Color.FromArgb(6, pulseGreen, 70);
                    }
                    catch { }
                };
            }
            else
            {
                badgeContainer.BackColor = Color.FromArgb(241, 245, 249);
                badgeContainer.Controls.Add(lblBadge);
                lblBadge.BackColor = Color.Transparent;
                lblBadge.AutoSize = true;
                lblBadge.Text = $"📌  {status.ToUpper()}";
                lblBadge.ForeColor = Color.FromArgb(71, 85, 105);
                lblBadge.Left = (badgeContainer.Width - lblBadge.PreferredWidth) / 2;
                lblBadge.Top = 9;
            }
            // =================================================================

            // Populate items
            var itemsList = itemsString.Split(new[] { ", " }, StringSplitOptions.RemoveEmptyEntries);
            lblSubtitle.Text = $"Requested items overview • {itemsList.Length} item{(itemsList.Length > 1 ? "s" : "")} total";

            foreach (var item in itemsList)
            {
                var itemCard = new Panel
                {
                    Size = new Size(panelWidth, 60),
                    BackColor = Color.FromArgb(248, 250, 252),
                    Margin = new Padding(0, 0, 0, 8),
                    Cursor = Cursors.Hand
                };

                var accentStrip = new Panel
                {
                    Size = new Size(4, 60),
                    BackColor = Color.FromArgb(99, 102, 241),
                    Dock = DockStyle.Left
                };

                var lblItemText = new Label
                {
                    Text = item,
                    Font = new Font("Segoe UI", 11F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(30, 41, 59),
                    Location = new Point(16, 9),
                    Size = new Size(panelWidth - 32, 22),
                    AutoEllipsis = true
                };

                var lblItemMeta = new Label
                {
                    Text = "Asset Verified • Ready for allocation",
                    Font = new Font("Segoe UI", 8.5F, FontStyle.Regular),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    Location = new Point(16, 32),
                    Size = new Size(panelWidth - 32, 18)
                };

                itemCard.Controls.Add(lblItemText);
                itemCard.Controls.Add(lblItemMeta);
                itemCard.Controls.Add(accentStrip);

                EventHandler onEnter = (s, e) => {
                    itemCard.BackColor = Color.FromArgb(238, 242, 255);
                    accentStrip.BackColor = Color.FromArgb(79, 70, 229);
                    accentStrip.Width = 8;
                    lblItemText.Location = new Point(20, 9);
                    lblItemMeta.Location = new Point(20, 32);
                };

                EventHandler onLeave = (s, e) => {
                    itemCard.BackColor = Color.FromArgb(248, 250, 252);
                    accentStrip.BackColor = Color.FromArgb(99, 102, 241);
                    accentStrip.Width = 4;
                    lblItemText.Location = new Point(16, 9);
                    lblItemMeta.Location = new Point(16, 32);
                };

                itemCard.MouseEnter += onEnter;
                lblItemText.MouseEnter += onEnter;
                lblItemMeta.MouseEnter += onEnter;
                accentStrip.MouseEnter += onEnter;
                itemCard.MouseLeave += onLeave;

                flowPanelItems.Controls.Add(itemCard);
            }

            btnClose.MouseEnter += (s, e) => btnClose.BackColor = Color.FromArgb(67, 56, 202);
            btnClose.MouseLeave += (s, e) => btnClose.BackColor = Color.FromArgb(79, 70, 229);
            btnClose.MouseDown += (s, e) => btnClose.Location = new Point(btnClose.Location.X, btnClose.Location.Y + 2);
            btnClose.MouseUp += (s, e) => btnClose.Location = new Point(btnClose.Location.X, btnClose.Location.Y - 2);

            btnX.MouseEnter += (s, e) => btnX.ForeColor = Color.FromArgb(30, 41, 59);
            btnX.MouseLeave += (s, e) => btnX.ForeColor = Color.FromArgb(148, 163, 184);
        }

        [System.Runtime.InteropServices.DllImport("Gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectRgn(
            int nLeftRect, int nTopRect, int nRightRect, int nBottomRect, int nWidthEllipse, int nHeightEllipse
        );

        private void btnClose_Click(object sender, EventArgs e)
        {
            if (isClosing) return;

            isClosing = true;
            btnClose.Enabled = false;

            if (marqueeTimer != null) { marqueeTimer.Stop(); marqueeTimer.Dispose(); }
            if (activeTimer != null) { activeTimer.Stop(); activeTimer.Dispose(); }

            transitionTimer.Start();
        }

        private void btnX_Click(object sender, EventArgs e)
        {
            btnClose_Click(sender, e);
        }
    }
}