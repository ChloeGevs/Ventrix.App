using System;
using System.Drawing;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace Ventrix.App.Controls
{
    public enum ToastType { Success, Warning, Error, Info }

    public partial class ToastNotification : Form
    {
        private System.Windows.Forms.Timer animTimer;
        private System.Windows.Forms.Timer dismissTimer;
        private System.Windows.Forms.Timer fadeOutTimer;

        public ToastNotification(string message, ToastType type)
        {
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.Manual;
            ShowInTaskbar = false;
            Width = 380;
            Height = 84;
            BackColor = Color.White;

            // Modern app notification card style with soft shadow elevation
            Guna2BorderlessForm borderless = new Guna2BorderlessForm
            {
                BorderRadius = 12,
                ContainerControl = this,
                ShadowColor = Color.FromArgb(40, 0, 0, 0)
            };

            // Status accent color mapping
            Color accentColor = type switch
            {
                ToastType.Success => Color.FromArgb(16, 185, 129),  // Emerald Green
                ToastType.Warning => Color.FromArgb(217, 119, 6),   // Amber Gold
                ToastType.Error => Color.FromArgb(225, 29, 72),     // Rose Red
                _ => Color.FromArgb(79, 70, 229)                    // Indigo Accent
            };

            string titleText = type switch
            {
                ToastType.Success => "Success",
                ToastType.Warning => "System Warning",
                ToastType.Error => "Action Failed",
                _ => "Notification"
            };

            // Left vertical status strip (native OS notification style)
            Panel pnlStrip = new Panel
            {
                Width = 5,
                Dock = DockStyle.Left,
                BackColor = accentColor
            };
            Controls.Add(pnlStrip);

            // App Source & Timestamp Header
            Label lblHeader = new Label
            {
                Text = "VENTRIX  •  Just now",
                Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                Location = new Point(16, 12),
                AutoSize = true
            };
            Controls.Add(lblHeader);

            // Manual Close Button (✕)
            Button btnClose = new Button
            {
                Text = "✕",
                Font = new Font("Segoe UI", 8.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(148, 163, 184),
                BackColor = Color.Transparent,
                FlatStyle = FlatStyle.Flat,
                Size = new Size(22, 22),
                Location = new Point(346, 10),
                Cursor = Cursors.Hand
            };
            btnClose.FlatAppearance.BorderSize = 0;
            btnClose.FlatAppearance.MouseOverBackColor = Color.FromArgb(241, 245, 249);
            btnClose.Click += (s, e) => CloseToast();
            Controls.Add(btnClose);

            // Title Label (Bold header)
            Label lblTitle = new Label
            {
                Text = titleText,
                Font = new Font("Segoe UI", 9.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(16, 30),
                AutoSize = true
            };
            Controls.Add(lblTitle);

            // Message Body Label
            Label lblMessage = new Label
            {
                Text = message,
                Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                ForeColor = Color.FromArgb(71, 85, 105),
                Location = new Point(16, 50),
                Size = new Size(335, 24),
                TextAlign = ContentAlignment.MiddleLeft
            };
            Controls.Add(lblMessage);

            // Pause auto-dismiss timer when user hovers over the notification card
            this.MouseEnter += (s, e) => dismissTimer?.Stop();
            this.MouseLeave += (s, e) => dismissTimer?.Start();
        }

        private void CloseToast()
        {
            animTimer?.Stop();
            dismissTimer?.Stop();
            fadeOutTimer?.Stop();
            this.Close();
            this.Dispose();
        }

        public static void Show(Form parent, string message, ToastType type)
        {
            if (parent == null || parent.IsDisposed) return;

            if (parent.InvokeRequired)
            {
                parent.Invoke(new Action(() => Show(parent, message, type)));
                return;
            }

            try
            {
                ToastNotification toast = new ToastNotification(message, type)
                {
                    TopMost = true
                };

                Rectangle screenRect = Screen.FromControl(parent).WorkingArea;
                int targetX;
                int startY;

                // Seamless positioning calculation for Normal and Maximized window states
                if (parent.WindowState == FormWindowState.Maximized)
                {
                    targetX = screenRect.Right - toast.Width - 30;
                    startY = screenRect.Top + 25;
                }
                else
                {
                    targetX = parent.Location.X + parent.Width - toast.Width - 30;
                    startY = parent.Location.Y + 80;

                    // Boundary fallback to prevent off-screen clipping on standard windows
                    if (targetX + toast.Width > screenRect.Right)
                    {
                        targetX = screenRect.Right - toast.Width - 30;
                    }
                }

                toast.Location = new Point(targetX, startY - 20);
                toast.Opacity = 0;
                toast.Show(parent);

                // Smooth slide-in entrance animation
                toast.animTimer = new System.Windows.Forms.Timer { Interval = 10 };
                int currentY = startY - 20;

                toast.animTimer.Tick += (s, e) =>
                {
                    if (toast.IsDisposed) { toast.animTimer.Stop(); toast.animTimer.Dispose(); return; }

                    if (toast.Opacity < 1.0) toast.Opacity += 0.2;
                    if (currentY < startY)
                    {
                        currentY += 4;
                        toast.Location = new Point(targetX, currentY);
                    }
                    if (toast.Opacity >= 1.0 && currentY >= startY)
                    {
                        toast.animTimer.Stop();
                        toast.animTimer.Dispose();
                    }
                };
                toast.animTimer.Start();

                // Auto-dismiss timer (3.5 seconds)
                toast.dismissTimer = new System.Windows.Forms.Timer { Interval = 3500 };
                toast.dismissTimer.Tick += (s, e) =>
                {
                    toast.dismissTimer.Stop();
                    toast.dismissTimer.Dispose();

                    if (toast.IsDisposed) return;

                    // Fade out exit animation
                    toast.fadeOutTimer = new System.Windows.Forms.Timer { Interval = 15 };
                    toast.fadeOutTimer.Tick += (s2, e2) =>
                    {
                        if (toast.IsDisposed) { toast.fadeOutTimer.Stop(); toast.fadeOutTimer.Dispose(); return; }

                        if (toast.Opacity > 0.0)
                        {
                            toast.Opacity -= 0.12;
                        }
                        else
                        {
                            toast.fadeOutTimer.Stop();
                            toast.fadeOutTimer.Dispose();
                            toast.Close();
                            toast.Dispose();
                        }
                    };
                    toast.fadeOutTimer.Start();
                };
                toast.dismissTimer.Start();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("ToastNotification Error: " + ex.Message);
            }
        }
    }
}