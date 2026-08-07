using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Ventrix.App.Controls
{
    public class ActivityCard : UserControl
    {
        private Color _statusColor;
        private string _message;
        private DateTime _timestamp;
        private string _badgeLabel;

        private System.Windows.Forms.Timer _animationTimer;
        private float _hoverProgress = 0.0f;
        private bool _isHovered = false;

        public event EventHandler CardClicked;

        public ActivityCard(string message, DateTime time, Color statusColor, string badgeLabel = "")
        {
            _message = message;
            _timestamp = time;
            _statusColor = statusColor;
            _badgeLabel = badgeLabel;

            InitializeComponentCustom();
        }

        private void InitializeComponentCustom()
        {
            // Reduced height from 68 to 60 for a sleeker single-line look
            this.Size = new Size(700, 60);
            this.BackColor = Color.Transparent;
            this.Cursor = Cursors.Hand;
            this.Margin = new Padding(0, 0, 0, 8);

            this.SetStyle(ControlStyles.AllPaintingInWmPaint | ControlStyles.UserPaint | ControlStyles.DoubleBuffer | ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);
            this.UpdateStyles();

            _animationTimer = new System.Windows.Forms.Timer { Interval = 12 };
            _animationTimer.Tick += (s, e) =>
            {
                bool needsRedraw = false;
                if (_isHovered)
                {
                    if (_hoverProgress < 1.0f)
                    {
                        _hoverProgress += 0.15f;
                        if (_hoverProgress > 1.0f) _hoverProgress = 1.0f;
                        needsRedraw = true;
                    }
                }
                else
                {
                    if (_hoverProgress > 0.0f)
                    {
                        _hoverProgress -= 0.15f;
                        if (_hoverProgress < 0.0f) _hoverProgress = 0.0f;
                        needsRedraw = true;
                    }
                }

                if (needsRedraw) this.Invalidate();
                else if (_hoverProgress == 0.0f || _hoverProgress == 1.0f) _animationTimer.Stop();
            };

            this.MouseEnter += (s, e) => { _isHovered = true; _animationTimer.Start(); };
            this.MouseLeave += (s, e) => { _isHovered = false; _animationTimer.Start(); };
            this.Click += (s, e) => { CardClicked?.Invoke(this, EventArgs.Empty); };
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            float t = _hoverProgress;
            float ease = t * t * (3f - 2f * t);

            // 1. Refined, Softer Hover Shadow
            if (ease > 0.01f)
            {
                for (int i = 2; i >= 1; i--)
                {
                    int shadowAlpha = (int)((4 + (i * 2)) * ease);
                    using (GraphicsPath shadowPath = GetRoundedRect(new RectangleF(2, 2 + i, this.Width - 4, this.Height - 4), 10))
                    using (Pen shadowPen = new Pen(Color.FromArgb(shadowAlpha, 15, 23, 42), i * 1.5f))
                    {
                        g.DrawPath(shadowPen, shadowPath);
                    }
                }
            }

            // 2. Crisp White Card Surface
            RectangleF cardRect = new RectangleF(2, 2, this.Width - 4, this.Height - 4);
            using (GraphicsPath cardPath = GetRoundedRect(cardRect, 10))
            {
                using (SolidBrush bgBrush = new SolidBrush(Color.White))
                {
                    g.FillPath(bgBrush, cardPath);
                }

                // Subtle border that slightly highlights on hover
                int borderR = (int)(226 - (15 * ease));
                int borderG = (int)(232 - (15 * ease));
                int borderB = (int)(240 - (10 * ease));
                using (Pen borderPen = new Pen(Color.FromArgb(borderR, borderG, borderB), 1))
                {
                    g.DrawPath(borderPen, cardPath);
                }
            }

            // 3. Slim Left Accent Bar
            float barWidth = 3f; // Slimmer, elegant width
            RectangleF accentRect = new RectangleF(2, 12, barWidth, this.Height - 24);
            using (GraphicsPath accentPath = GetRoundedRect(accentRect, 2))
            using (SolidBrush accentBrush = new SolidBrush(_statusColor))
            {
                g.FillPath(accentBrush, accentPath);
            }

            // 4. Parse the HTML-style <b> tags passed from AdminDashboard
            string boldName = "";
            string actionText = _message;

            if (_message.StartsWith("<b>") && _message.Contains("</b>"))
            {
                int endIdx = _message.IndexOf("</b>");
                boldName = _message.Substring(3, endIdx - 3);
                actionText = _message.Substring(endIdx + 4);
            }

            // 5. Modern Flat Avatar Node
            float avatarSize = 32; // Slightly smaller and cleaner
            float avatarX = 18;
            float avatarY = (this.Height - avatarSize) / 2;
            RectangleF avatarRect = new RectangleF(avatarX, avatarY, avatarSize, avatarSize);

            using (GraphicsPath avatarPath = new GraphicsPath())
            {
                avatarPath.AddEllipse(avatarRect);
                // Soft slate background
                using (SolidBrush avatarBgBrush = new SolidBrush(Color.FromArgb(241, 245, 249)))
                {
                    g.FillPath(avatarBgBrush, avatarPath);
                }
            }

            string initial = !string.IsNullOrEmpty(boldName) ? boldName.Substring(0, 1).ToUpper() : "•";
            using (Font initialFont = new Font("Segoe UI", 10F, FontStyle.Bold))
            using (SolidBrush initialBrush = new SolidBrush(Color.FromArgb(71, 85, 105))) // Dark Slate text
            {
                StringFormat centerFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                // Offset Y slightly for Segoe UI visual centering
                RectangleF adjustedAvatarRect = new RectangleF(avatarRect.X, avatarRect.Y + 1, avatarRect.Width, avatarRect.Height);
                g.DrawString(initial, initialFont, initialBrush, adjustedAvatarRect, centerFormat);
            }

            // 6. Clean Typography (Vertically Centered)
            float textStartX = 62;
            using (Font boldFont = new Font("Segoe UI", 10F, FontStyle.Bold))
            using (Font regularFont = new Font("Segoe UI", 10F, FontStyle.Regular))
            using (SolidBrush textBrush = new SolidBrush(Color.FromArgb(30, 41, 59))) // Dark slate (Not pure black)
            {
                // Measure heights to perfectly center vertically
                SizeF dummySize = g.MeasureString("Ag", regularFont);
                float textY = (this.Height - dummySize.Height) / 2;

                if (!string.IsNullOrEmpty(boldName))
                {
                    g.DrawString(boldName, boldFont, textBrush, textStartX, textY);
                    SizeF boldSize = g.MeasureString(boldName, boldFont);

                    // GDI+ MeasureString adds padding, so we subtract a bit (~4px) to keep the text snug
                    g.DrawString(actionText, regularFont, textBrush, textStartX + boldSize.Width - 6, textY);
                }
                else
                {
                    g.DrawString(_message, regularFont, textBrush, textStartX, textY);
                }
            }

            // 7. Relative Timestamp & Hover Action Arrow
            string timeStr = GetRelativeTime(_timestamp);
            using (Font timeFont = new Font("Segoe UI", 9F, FontStyle.Regular))
            using (SolidBrush timeBrush = new SolidBrush(Color.FromArgb(148, 163, 184))) // Light slate
            {
                SizeF timeSize = g.MeasureString(timeStr, timeFont);
                float timeX = this.Width - timeSize.Width - 20 - (ease * 25);
                float timeY = (this.Height - timeSize.Height) / 2;
                g.DrawString(timeStr, timeFont, timeBrush, timeX, timeY);

                if (ease > 0.05f)
                {
                    g.SmoothingMode = SmoothingMode.AntiAlias;
                    float btnX = this.Width - 36 + (20 * (1f - ease));
                    float btnY = (this.Height - 24) / 2;
                    RectangleF actionBtnRect = new RectangleF(btnX, btnY, 24, 24);

                    using (GraphicsPath btnPath = GetRoundedRect(actionBtnRect, 6))
                    using (SolidBrush btnBg = new SolidBrush(Color.FromArgb((int)(255 * ease), 241, 245, 249)))
                    using (Pen btnBorder = new Pen(Color.FromArgb((int)(255 * ease), 226, 232, 240), 1))
                    using (SolidBrush arrowBrush = new SolidBrush(Color.FromArgb((int)(255 * ease), 100, 116, 139)))
                    {
                        g.FillPath(btnBg, btnPath);
                        g.DrawPath(btnBorder, btnPath);

                        StringFormat arrowFormat = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
                        g.DrawString("›", new Font("Segoe UI", 12F, FontStyle.Regular), arrowBrush, new RectangleF(actionBtnRect.X, actionBtnRect.Y - 1, actionBtnRect.Width, actionBtnRect.Height), arrowFormat);
                    }
                }
            }
        }

        private GraphicsPath GetRoundedRect(RectangleF rect, float radius)
        {
            GraphicsPath path = new GraphicsPath();
            float diameter = radius * 2;
            path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
            path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
            path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
            path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
            path.CloseFigure();
            return path;
        }

        private string GetRelativeTime(DateTime time)
        {
            var span = DateTime.Now - time;
            if (span.TotalMinutes < 1) return "Just now";
            if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
            if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
            return time.ToString("MMM dd");
        }
    }
}