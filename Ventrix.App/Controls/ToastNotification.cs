using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ventrix.App.Controls
{
    public enum ToastType
    {
        Success,
        Error,
        Info,
        Warning
    }

    public partial class ToastNotification : Form
    {
        private Label lblMessage;
        private System.Windows.Forms.Timer actionTimer;
        private int toastLifeCounter = 0;
        private ToastAction action;
        private int targetY;

        private enum ToastAction { FadeIn, Wait, FadeOut }

        protected override bool ShowWithoutActivation => true;

        public ToastNotification(string message, ToastType type)
        {
            this.FormBorderStyle = FormBorderStyle.None;
            this.Size = new Size(320, 60);
            this.BackColor = Color.White;
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.Opacity = 0.0;

            Guna2Elipse elipse = new Guna2Elipse();
            elipse.TargetControl = this;
            elipse.BorderRadius = 12;

            lblMessage = new Label
            {
                Text = message,
                Font = new Font("Segoe UI Semibold", 10F),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(270, 60),
                Location = new Point(50, 0),
                TextAlign = ContentAlignment.MiddleLeft
            };
            this.Controls.Add(lblMessage);

            Label lblIcon = new Label
            {
                Font = new Font("Segoe UI", 14F),
                AutoSize = false,
                Size = new Size(50, 60),
                Location = new Point(0, 0),
                TextAlign = ContentAlignment.MiddleCenter
            };
            this.Controls.Add(lblIcon);

            switch (type)
            {
                case ToastType.Success:
                    this.BackColor = Color.MediumSeaGreen;
                    lblIcon.Text = "✔️";
                    break;
                case ToastType.Error:
                    this.BackColor = Color.IndianRed;
                    lblIcon.Text = "❌";
                    break;
                case ToastType.Warning:
                    this.BackColor = Color.DarkOrange;
                    lblIcon.Text = "⚠️";
                    break;
                case ToastType.Info:
                    this.BackColor = Color.FromArgb(33, 150, 243);
                    lblIcon.Text = "ℹ️";
                    break;
            }

            actionTimer = new System.Windows.Forms.Timer { Interval = 15 };
            actionTimer.Tick += ActionTimer_Tick;
        }

        public static void Show(Form owner, string message, ToastType type = ToastType.Success)
        {
            ToastNotification toast = new ToastNotification(message, type);

            int startX = owner.Location.X + owner.Width - toast.Width - 30;
            toast.targetY = owner.Location.Y + owner.Height - toast.Height - 30;

            toast.Location = new Point(startX, toast.targetY + 20);
            toast.action = ToastAction.FadeIn;

            toast.Show(owner);
            toast.actionTimer.Start();
        }

        private void ActionTimer_Tick(object sender, EventArgs e)
        {
            switch (action)
            {
                case ToastAction.FadeIn:
                    if (this.Opacity < 1.0)
                    {
                        this.Opacity += 0.1;
                        if (this.Top > targetY) this.Top -= 2;
                    }
                    else action = ToastAction.Wait;
                    break;

                case ToastAction.Wait:
                    toastLifeCounter++;
                    if (toastLifeCounter >= 160) action = ToastAction.FadeOut;
                    break;

                case ToastAction.FadeOut:
                    if (this.Opacity > 0.0)
                    {
                        this.Opacity -= 0.1;
                        this.Top += 1;
                    }
                    else
                    {
                        actionTimer.Stop();
                        actionTimer.Dispose();
                        this.Close();
                    }
                    break;
            }
        }
    }
}