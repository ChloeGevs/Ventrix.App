using System;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace Ventrix.App.Popups
{
    public partial class QuantityPopup : MaterialForm
    {
        public int SelectedQuantity { get; private set; }

        private int _targetY;
        private bool _isAnimating = false;
        private DialogResult _closingResult = DialogResult.Cancel;
        private double _animStep = 0.0;

        public QuantityPopup()
        {
            InitializeComponent();
            ThemeManager.ApplyMaterialTheme(this);

            this.Opacity = 0;
            this.StartPosition = FormStartPosition.CenterParent;

            this.AcceptButton = btnConfirm;
            this.CancelButton = btnCancel;

            this.Load += QuantityPopup_Load;

            txtQuantity.KeyPress += (s, e) => {
                // Only allow digits and control characters
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true;
                }
            };
        }

        private void QuantityPopup_Load(object sender, EventArgs e)
        {
            _targetY = this.Location.Y;
            this.Location = new Point(this.Location.X, _targetY + 40);

            System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer { Interval = 15 };
            animTimer.Tick += (object s, EventArgs ev) =>
            {
                _animStep += 0.12;

                // Fade in quickly
                if (this.Opacity < 1.0) this.Opacity += 0.15;

                // Elastic/Bouncy ease-out equation
                double progress = Math.Min(1.0, _animStep);
                double easeOutBack = 1 + (--progress) * progress * (2.7 * progress + 1.7);

                int currentY = _targetY + 40 - (int)(40 * easeOutBack);
                this.Location = new Point(this.Location.X, currentY);

                if (_animStep >= 1.0 && this.Opacity >= 0.98)
                {
                    this.Opacity = 1.0;
                    this.Location = new Point(this.Location.X, _targetY);
                    animTimer.Stop();
                    animTimer.Dispose();

                    txtQuantity.Focus();
                    txtQuantity.SelectAll();
                }
            };
            animTimer.Start();
        }

        private void CloseAnimated(DialogResult result)
        {
            if (_isAnimating) return;
            _isAnimating = true;
            _closingResult = result;

            System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer { Interval = 10 };
            animTimer.Tick += (object s, EventArgs ev) =>
            {
                this.Opacity -= 0.15;
                this.Location = new Point(this.Location.X, this.Location.Y + 4);

                if (this.Opacity <= 0)
                {
                    animTimer.Stop();
                    animTimer.Dispose();
                    this.DialogResult = _closingResult;
                    this.Close();
                }
            };
            animTimer.Start();
        }

        private void TriggerShakeAnimation()
        {
            int originalX = txtQuantity.Location.X;
            int[] shakeOffsets = { 5, -5, 4, -4, 3, -3, 2, -2, 1, -1, 0 };
            int step = 0;

            System.Windows.Forms.Timer shakeTimer = new System.Windows.Forms.Timer { Interval = 20 };
            shakeTimer.Tick += (s, args) =>
            {
                if (step < shakeOffsets.Length)
                {
                    txtQuantity.Location = new Point(originalX + shakeOffsets[step], txtQuantity.Location.Y);
                    step++;
                }
                else
                {
                    shakeTimer.Stop();
                    shakeTimer.Dispose();
                    txtQuantity.Location = new Point(originalX, txtQuantity.Location.Y);
                    txtQuantity.Focus();
                    txtQuantity.SelectAll();
                }
            };
            shakeTimer.Start();

            // Flash border red briefly
            txtQuantity.BorderColor = Color.FromArgb(239, 68, 68);
            System.Windows.Forms.Timer colorTimer = new System.Windows.Forms.Timer { Interval = 400 };
            colorTimer.Tick += (s, args) => {
                txtQuantity.BorderColor = Color.FromArgb(213, 218, 223); // Back to default
                colorTimer.Stop();
                colorTimer.Dispose();
            };
            colorTimer.Start();
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtQuantity.Text, out int quantity) && quantity > 0)
            {
                SelectedQuantity = quantity;
                CloseAnimated(DialogResult.OK);
            }
            else
            {
                TriggerShakeAnimation();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CloseAnimated(DialogResult.Cancel);
        }
    }
}