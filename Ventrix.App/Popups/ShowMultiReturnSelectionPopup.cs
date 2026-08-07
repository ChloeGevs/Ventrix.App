using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Ventrix.Domain.Enums;
using Ventrix.Domain.Models;
using Guna.UI2.WinForms;
using System.Threading.Tasks;

namespace Ventrix.App.Popups
{
    public partial class ShowMultiReturnSelectionPopup : Form
    {
        private readonly List<BorrowRecord> _allRecords;
        public List<BorrowRecord> SelectedRecords { get; private set; } = new List<BorrowRecord>();

        private int _targetY;
        private bool _isAnimating = false;
        private DialogResult _closingResult = DialogResult.Cancel;
        private double _animStep = 0.0;
        private readonly List<Guna2Panel> _itemCards = new List<Guna2Panel>();
        private Form _dimOverlay; // Background dim overlay form

        // Typewriter Effect Variables
        private readonly string _targetTitleText = "Return Items";
        private int _typewriterIndex = 0;

        public ShowMultiReturnSelectionPopup(List<BorrowRecord> records)
        {
            InitializeComponent();
            _allRecords = records;

            this.Opacity = 0;
            this.StartPosition = FormStartPosition.CenterParent;

            lblTitle.Text = "";

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
            this.Location = new Point(this.Location.X, _targetY + 30);

            int index = 0;
            foreach (var card in _itemCards)
            {
                card.Tag = card.Top;
                card.Top += 20 + (10 * index);
                index++;
            }

            System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer { Interval = 15 };
            animTimer.Tick += (s, ev) =>
            {
                _animStep += 0.12;

                if (this.Opacity < 1.0) this.Opacity += 0.15;

                double progress = Math.Min(1.0, _animStep);
                double easeOutBack = 1 + (--progress) * progress * (2.7 * progress + 1.7);

                int currentY = _targetY + 30 - (int)(30 * easeOutBack);
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
            flowRecords.Controls.Clear();
            _itemCards.Clear();

            foreach (var record in _allRecords)
            {
                var card = CreateRecordCard(record);
                flowRecords.Controls.Add(card);
                _itemCards.Add(card);
            }
        }

        private Guna2Panel CreateRecordCard(BorrowRecord record)
        {
            Guna2Panel card = new Guna2Panel
            {
                Size = new Size(flowRecords.Width - 10, 50),
                FillColor = Color.White,
                BorderRadius = 10,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(226, 232, 240),
                Margin = new Padding(0, 0, 0, 8),
                Cursor = Cursors.Hand
            };

            Guna2CheckBox chk = new Guna2CheckBox
            {
                Text = record.ItemName,
                Font = new Font("Segoe UI Semibold", 10F),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(15, 14),
                AutoSize = true,
                Cursor = Cursors.Hand,
                Tag = record,
                CheckedState = {
                    BorderRadius = 4,
                    FillColor = Color.FromArgb(16, 185, 129),
                    BorderColor = Color.FromArgb(16, 185, 129)
                },
                UncheckedState = {
                    BorderRadius = 4,
                    BorderColor = Color.FromArgb(203, 213, 225)
                }
            };

            Label lblDate = new Label
            {
                Text = $"Borrowed: {record.BorrowDate:MMM dd}",
                Font = new Font("Segoe UI", 8.5F),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(180, 16),
                AutoSize = true,
                Cursor = Cursors.Hand
            };

            card.Controls.Add(chk);
            card.Controls.Add(lblDate);

            if (record.Status == BorrowStatus.Overdue)
            {
                Label lblOverdue = new Label
                {
                    Text = "OVERDUE",
                    Font = new Font("Segoe UI", 7.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(239, 68, 68),
                    Location = new Point(card.Width - 75, 17),
                    AutoSize = true,
                    Cursor = Cursors.Hand
                };
                card.Controls.Add(lblOverdue);
                card.BorderColor = Color.FromArgb(254, 202, 202);

                lblOverdue.Click += (s, e) => chk.Checked = !chk.Checked;
            }

            EventHandler hoverIn = (s, e) =>
            {
                if (record.Status != BorrowStatus.Overdue) card.BorderColor = Color.FromArgb(203, 213, 225);
                card.FillColor = Color.FromArgb(248, 250, 252);
            };
            EventHandler hoverOut = (s, e) =>
            {
                if (record.Status != BorrowStatus.Overdue) card.BorderColor = Color.FromArgb(226, 232, 240);
                card.FillColor = Color.White;
            };

            card.MouseEnter += hoverIn;
            chk.MouseEnter += hoverIn;
            lblDate.MouseEnter += hoverIn;

            card.MouseLeave += hoverOut;
            chk.MouseLeave += hoverOut;
            lblDate.MouseLeave += hoverOut;

            card.Click += (s, e) => chk.Checked = !chk.Checked;
            lblDate.Click += (s, e) => chk.Checked = !chk.Checked;

            return card;
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
            int originalX = flowRecords.Location.X;
            int[] shakeOffsets = { 5, -5, 4, -4, 3, -3, 2, -2, 1, -1, 0 };
            int step = 0;

            System.Windows.Forms.Timer shakeTimer = new System.Windows.Forms.Timer { Interval = 20 };
            shakeTimer.Tick += (s, args) =>
            {
                if (step < shakeOffsets.Length)
                {
                    flowRecords.Location = new Point(originalX + shakeOffsets[step], flowRecords.Location.Y);
                    step++;
                }
                else
                {
                    shakeTimer.Stop();
                    shakeTimer.Dispose();
                    flowRecords.Location = new Point(originalX, flowRecords.Location.Y);
                }
            };
            shakeTimer.Start();
        }

        private void btnOk_Click(object sender, EventArgs e)
        {
            SelectedRecords.Clear();

            foreach (Control control in flowRecords.Controls)
            {
                if (control is Guna2Panel card)
                {
                    var chk = card.Controls.OfType<Guna2CheckBox>().FirstOrDefault();
                    if (chk != null && chk.Checked && chk.Tag is BorrowRecord record)
                    {
                        SelectedRecords.Add(record);
                    }
                }
            }

            if (SelectedRecords.Count == 0)
            {
                TriggerShakeAnimation();
                return;
            }

            CloseAnimated(DialogResult.OK);
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            CloseAnimated(DialogResult.Cancel);
        }
    }
}