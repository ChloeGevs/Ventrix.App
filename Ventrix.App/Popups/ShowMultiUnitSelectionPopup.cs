using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Ventrix.Domain.Models;
using Guna.UI2.WinForms;
using System.Threading.Tasks;

namespace Ventrix.App.Popups
{
    public partial class ShowMultiUnitSelectionPopup : Form
    {
        private readonly List<InventoryItem> _availableUnits;
        private readonly int _requiredQuantity;
        public List<InventoryItem> SelectedUnits { get; private set; } = new List<InventoryItem>();

        private int _targetY;
        private bool _isAnimating = false;
        private DialogResult _closingResult = DialogResult.Cancel;
        private double _animStep = 0.0;
        private readonly List<Guna2Panel> _itemCards = new List<Guna2Panel>();
        private Form _dimOverlay;

        // Typewriter Effect Variables
        private readonly string _targetTitleText = "Select Specific Units";
        private int _typewriterIndex = 0;

        public ShowMultiUnitSelectionPopup(List<InventoryItem> units, string baseName, int requiredQuantity)
        {
            InitializeComponent();
            _availableUnits = units;
            _requiredQuantity = requiredQuantity;

            this.Opacity = 0;
            this.StartPosition = FormStartPosition.CenterParent;

            lblTitle.Text = "";
            lblInstruction.Text = $"Please check exactly {requiredQuantity} {baseName}(s):";
            UpdateSelectionCounter();

            this.Load += Popup_Load;
            this.FormClosed += Popup_FormClosed;
            PopulateList();
        }

        private void ShowDimOverlay(Form parentForm)
        {
            if (parentForm == null) return;

            _dimOverlay = new Form
            {
                FormBorderStyle = FormBorderStyle.None,
                StartPosition = FormStartPosition.Manual,
                Bounds = parentForm.Bounds,
                BackColor = Color.Black,
                Opacity = 0.0,
                ShowInTaskbar = false,
                Owner = parentForm
            };

            System.Windows.Forms.Timer fadeTimer = new System.Windows.Forms.Timer { Interval = 10 };
            fadeTimer.Tick += (s, e) =>
            {
                if (_dimOverlay.Opacity < 0.45)
                {
                    _dimOverlay.Opacity += 0.05;
                }
                else
                {
                    fadeTimer.Stop();
                    fadeTimer.Dispose();
                }
            };

            _dimOverlay.Show(parentForm);
            fadeTimer.Start();
        }

        private void Popup_Load(object sender, EventArgs e)
        {
            if (this.Owner != null)
            {
                ShowDimOverlay(this.Owner);
            }

            _targetY = this.Location.Y;
            this.Location = new Point(this.Location.X, _targetY + 25);

            int index = 0;
            foreach (var card in _itemCards)
            {
                card.Tag = card.Top;
                card.Top += 15 + (8 * index);
                index++;
            }

            System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer { Interval = 15 };
            animTimer.Tick += (s, ev) =>
            {
                _animStep += 0.12;

                if (this.Opacity < 1.0) this.Opacity += 0.15;

                double progress = Math.Min(1.0, _animStep);
                double easeOutBack = 1 + (--progress) * progress * (2.7 * progress + 1.7);

                int currentY = _targetY + 25 - (int)(25 * easeOutBack);
                this.Location = new Point(this.Location.X, currentY);

                bool cardsMoving = false;
                foreach (var card in _itemCards)
                {
                    if (card.Tag is int targetTop && card.Top > targetTop)
                    {
                        int cardDist = card.Top - targetTop;
                        card.Top -= (int)Math.Ceiling(cardDist * 0.18);
                        cardsMoving = true;
                    }
                }

                if (_animStep >= 1.0 && !cardsMoving && this.Opacity >= 0.98)
                {
                    this.Opacity = 1.0;
                    this.Location = new Point(this.Location.X, _targetY);

                    foreach (var card in _itemCards)
                    {
                        if (card.Tag is int targetTop) card.Top = targetTop;
                    }

                    animTimer.Stop();
                    animTimer.Dispose();
                }
            };
            animTimer.Start();

            System.Windows.Forms.Timer typeTimer = new System.Windows.Forms.Timer { Interval = 40 };
            typeTimer.Tick += (s, ev) =>
            {
                if (_typewriterIndex < _targetTitleText.Length)
                {
                    lblTitle.Text += _targetTitleText[_typewriterIndex];
                    _typewriterIndex++;
                }
                else
                {
                    typeTimer.Stop();
                    typeTimer.Dispose();
                }
            };

            Task.Delay(150).ContinueWith(t => this.Invoke(new Action(() => typeTimer.Start())));
        }

        private void Popup_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (_dimOverlay != null && !_dimOverlay.IsDisposed)
            {
                _dimOverlay.Close();
                _dimOverlay.Dispose();
            }
        }

        private void PopulateList()
        {
            flowUnits.Controls.Clear();
            _itemCards.Clear();

            foreach (var unit in _availableUnits)
            {
                var card = CreateUnitCard(unit);
                flowUnits.Controls.Add(card);
                _itemCards.Add(card);
            }
        }

        private Guna2Panel CreateUnitCard(InventoryItem unit)
        {
            Guna2Panel card = new Guna2Panel
            {
                Size = new Size(flowUnits.Width - 10, 46),
                FillColor = Color.White,
                BorderRadius = 8,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(226, 232, 240),
                Margin = new Padding(0, 0, 0, 6),
                Cursor = Cursors.Hand
            };

            Guna2CheckBox chk = new Guna2CheckBox
            {
                Text = unit.Name,
                Font = new Font("Segoe UI Semibold", 9.5F),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(12, 12),
                AutoSize = true,
                Cursor = Cursors.Hand,
                Tag = unit,
                CheckedState = {
                    BorderRadius = 4,
                    FillColor = Color.FromArgb(37, 99, 235),
                    BorderColor = Color.FromArgb(37, 99, 235)
                },
                UncheckedState = {
                    BorderRadius = 4,
                    BorderColor = Color.FromArgb(203, 213, 225)
                }
            };

            chk.CheckedChanged += (s, e) => {
                UpdateSelectionCounter();
                card.FillColor = chk.Checked ? Color.FromArgb(239, 246, 255) : Color.White;
                card.BorderColor = chk.Checked ? Color.FromArgb(147, 197, 253) : Color.FromArgb(226, 232, 240);
            };

            // Moved further right to safely accommodate long names without clipping
            Label lblCondition = new Label
            {
                Text = $"Condition: {unit.Condition}",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(310, 14),
                AutoSize = true,
                Cursor = Cursors.Hand
            };

            card.Controls.Add(chk);
            card.Controls.Add(lblCondition);

            EventHandler hoverIn = (s, e) =>
            {
                if (!chk.Checked)
                {
                    card.BorderColor = Color.FromArgb(203, 213, 225);
                    card.FillColor = Color.FromArgb(248, 250, 252);
                }
            };
            EventHandler hoverOut = (s, e) =>
            {
                if (!chk.Checked)
                {
                    card.BorderColor = Color.FromArgb(226, 232, 240);
                    card.FillColor = Color.White;
                }
            };

            card.MouseEnter += hoverIn;
            chk.MouseEnter += hoverIn;
            lblCondition.MouseEnter += hoverIn;

            card.MouseLeave += hoverOut;
            chk.MouseLeave += hoverOut;
            lblCondition.MouseLeave += hoverOut;

            card.Click += (s, e) => chk.Checked = !chk.Checked;
            lblCondition.Click += (s, e) => chk.Checked = !chk.Checked;

            return card;
        }

        private void UpdateSelectionCounter()
        {
            int currentChecked = 0;
            foreach (Control control in flowUnits.Controls)
            {
                if (control is Guna2Panel card)
                {
                    var chk = card.Controls.OfType<Guna2CheckBox>().FirstOrDefault();
                    if (chk != null && chk.Checked) currentChecked++;
                }
            }
            btnOk.Text = $"Confirm ({currentChecked}/{_requiredQuantity})";
        }

        private void CloseAnimated(DialogResult result)
        {
            if (_isAnimating) return;
            _isAnimating = true;
            _closingResult = result;

            if (_dimOverlay != null && !_dimOverlay.IsDisposed)
            {
                System.Windows.Forms.Timer overlayFade = new System.Windows.Forms.Timer { Interval = 10 };
                overlayFade.Tick += (s, ev) =>
                {
                    if (_dimOverlay.Opacity > 0) _dimOverlay.Opacity -= 0.05;
                    else { overlayFade.Stop(); overlayFade.Dispose(); }
                };
                overlayFade.Start();
            }

            System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer { Interval = 10 };
            animTimer.Tick += (s, ev) =>
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
            int originalX = flowUnits.Location.X;
            int[] shakeOffsets = { 5, -5, 4, -4, 3, -3, 2, -2, 1, -1, 0 };
            int step = 0;

            System.Windows.Forms.Timer shakeTimer = new System.Windows.Forms.Timer { Interval = 20 };
            shakeTimer.Tick += (s, args) =>
            {
                if (step < shakeOffsets.Length)
                {
                    flowUnits.Location = new Point(originalX + shakeOffsets[step], flowUnits.Location.Y);
                    step++;
                }
                else
                {
                    shakeTimer.Stop();
                    shakeTimer.Dispose();
                    flowUnits.Location = new Point(originalX, flowUnits.Location.Y);
                }
            };
            shakeTimer.Start();
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            SelectedUnits.Clear();

            foreach (Control control in flowUnits.Controls)
            {
                if (control is Guna2Panel card)
                {
                    var chk = card.Controls.OfType<Guna2CheckBox>().FirstOrDefault();
                    if (chk != null && chk.Checked && chk.Tag is InventoryItem unit)
                    {
                        SelectedUnits.Add(unit);
                    }
                }
            }

            if (SelectedUnits.Count != _requiredQuantity)
            {
                MessageBox.Show($"You requested {_requiredQuantity} item(s). Please check exactly {_requiredQuantity} box(es).", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                TriggerShakeAnimation();
                return;
            }

            CloseAnimated(DialogResult.OK);
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            CloseAnimated(DialogResult.Cancel);
        }
    }
}