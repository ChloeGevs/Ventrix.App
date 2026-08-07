using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using Ventrix.Domain.Enums;
using Ventrix.App.Controls;
using Guna.UI2.WinForms;

namespace Ventrix.App.Popups
{
    public partial class RepairDetailsPopup : MaterialForm
    {
        private readonly List<InventoryItem> _damagedItems;
        private readonly InventoryService _inventoryService;
        private readonly Func<Task> _onSaved;

        private int _targetY;
        private bool _isAnimating = false;
        private readonly List<Guna2Panel> _itemCards = new List<Guna2Panel>();

        // Typewriter Effect Variables
        private readonly string _targetHeaderText = "Requires Attention";
        private int _typewriterIndex = 0;

        public RepairDetailsPopup(List<InventoryItem> damagedItems, InventoryService inventoryService, Func<Task> onSaved)
        {
            _damagedItems = damagedItems;
            _inventoryService = inventoryService;
            _onSaved = onSaved;

            InitializeComponent();
            this.Text = "Damaged Items Report";

            this.Opacity = 0;
            this.StartPosition = FormStartPosition.CenterParent;

            // Clear header text initially for the typewriter effect
            lblHeader.Text = "";

            this.Load += RepairDetailsPopup_Load;
            LoadCards();
        }

        private void RepairDetailsPopup_Load(object sender, EventArgs e)
        {
            _targetY = this.Location.Y;
            this.Location = new Point(this.Location.X, _targetY + 40);

            // Prepare cards for staggered slide-up
            int index = 0;
            foreach (var card in _itemCards)
            {
                card.Tag = card.Top;
                card.Top += 20 + (15 * index);
                index++;
            }

            // Window and Card Animation Timer
            System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer { Interval = 15 };
            animTimer.Tick += (object s, EventArgs ev) =>
            {
                // Smooth Form Fade & Slide
                this.Opacity += (1.0 - this.Opacity) * 0.18;

                int currentY = this.Location.Y;
                int distance = currentY - _targetY;

                if (distance > 0)
                {
                    int move = (int)Math.Ceiling(distance * 0.18);
                    this.Location = new Point(this.Location.X, currentY - move);
                }

                // Smoothly drift item cards into place
                bool cardsMoving = false;
                foreach (var card in _itemCards)
                {
                    if (card.Tag is int targetTop && card.Top > targetTop)
                    {
                        int cardDist = card.Top - targetTop;
                        int cardMove = (int)Math.Ceiling(cardDist * 0.15);
                        card.Top -= cardMove;
                        cardsMoving = true;
                    }
                }

                if (distance <= 0 && !cardsMoving && this.Opacity >= 0.98)
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

            // Start Typewriter Animation Timer
            System.Windows.Forms.Timer typeTimer = new System.Windows.Forms.Timer { Interval = 40 }; // 40ms per character
            typeTimer.Tick += (object s, EventArgs ev) =>
            {
                if (_typewriterIndex < _targetHeaderText.Length)
                {
                    lblHeader.Text += _targetHeaderText[_typewriterIndex];
                    _typewriterIndex++;
                }
                else
                {
                    typeTimer.Stop();
                    typeTimer.Dispose();
                }
            };

            // Give the form a tiny moment to start appearing before typing starts
            Task.Delay(100).ContinueWith(t => this.Invoke(new Action(() => typeTimer.Start())));
        }

        private async Task ClosePopupAsync()
        {
            if (_isAnimating) return;
            _isAnimating = true;

            System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer { Interval = 10 };
            animTimer.Tick += (object s, EventArgs ev) =>
            {
                this.Opacity -= 0.15;
                this.Location = new Point(this.Location.X, this.Location.Y + 4);

                if (this.Opacity <= 0)
                {
                    animTimer.Stop();
                    animTimer.Dispose();
                    this.Close();
                }
            };
            animTimer.Start();
        }

        private void LoadCards()
        {
            flowRepairList.Controls.Clear();
            _itemCards.Clear();

            if (_damagedItems == null || _damagedItems.Count == 0)
            {
                Guna2Panel emptyPanel = new Guna2Panel
                {
                    Size = new Size(flowRepairList.Width - 10, 120),
                    FillColor = Color.FromArgb(248, 250, 252),
                    BorderRadius = 12,
                    BorderThickness = 1,
                    BorderColor = Color.FromArgb(226, 232, 240)
                };

                Label lblIcon = new Label
                {
                    Text = "✨",
                    Font = new Font("Segoe UI Emoji", 20F),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(emptyPanel.Width, 40),
                    Location = new Point(0, 20)
                };

                Label lblEmpty = new Label
                {
                    Text = "All items are fully repaired and ready to go!",
                    Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    TextAlign = ContentAlignment.MiddleCenter,
                    Size = new Size(emptyPanel.Width, 30),
                    Location = new Point(0, 65)
                };

                emptyPanel.Controls.Add(lblIcon);
                emptyPanel.Controls.Add(lblEmpty);
                flowRepairList.Controls.Add(emptyPanel);
                return;
            }

            foreach (var item in _damagedItems)
            {
                var card = CreateItemCard(item);
                flowRepairList.Controls.Add(card);
                _itemCards.Add(card);
            }
        }

        private Guna2Panel CreateItemCard(InventoryItem item)
        {
            Guna2Panel card = new Guna2Panel
            {
                Size = new Size(flowRepairList.Width - 10, 75),
                FillColor = Color.White,
                BorderRadius = 10,
                BorderThickness = 1,
                BorderColor = Color.FromArgb(226, 232, 240),
                CustomBorderThickness = new Padding(5, 0, 0, 0),
                CustomBorderColor = Color.FromArgb(244, 63, 94),
                Cursor = Cursors.Hand,
                Margin = new Padding(0, 0, 0, 10)
            };

            Label lblName = new Label
            {
                Text = item.Name,
                Font = new Font("Segoe UI", 10.5F, FontStyle.Bold),
                ForeColor = Color.FromArgb(15, 23, 42),
                Location = new Point(20, 15),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblName);

            Label lblDetails = new Label
            {
                Text = $"System ID: #{item.Id}  •  Category: {item.Category}",
                Font = new Font("Segoe UI Semibold", 8.5F),
                ForeColor = Color.FromArgb(100, 116, 139),
                Location = new Point(20, 40),
                AutoSize = true,
                BackColor = Color.Transparent
            };
            card.Controls.Add(lblDetails);

            Guna2Button btnFix = new Guna2Button
            {
                Text = "Repair",
                Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                FillColor = Color.FromArgb(99, 102, 241),
                HoverState = { FillColor = Color.FromArgb(79, 70, 229) },
                ForeColor = Color.White,
                BorderRadius = 16,
                Size = new Size(95, 34),
                Location = new Point(card.Width - 110, 20),
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand,
                Animated = true
            };
            btnFix.Click += async (s, e) => await RepairItemAsync(item.Id, item.Name);
            card.Controls.Add(btnFix);

            EventHandler hoverIn = (s, e) =>
            {
                card.FillColor = Color.FromArgb(248, 250, 252);
                card.BorderColor = Color.FromArgb(203, 213, 225);
            };
            EventHandler hoverOut = (s, e) =>
            {
                card.FillColor = Color.White;
                card.BorderColor = Color.FromArgb(226, 232, 240);
            };

            card.MouseEnter += hoverIn;
            lblName.MouseEnter += hoverIn;
            lblDetails.MouseEnter += hoverIn;

            card.MouseLeave += hoverOut;
            lblName.MouseLeave += hoverOut;
            lblDetails.MouseLeave += hoverOut;

            return card;
        }

        private async Task RepairItemAsync(int itemId, string itemName)
        {
            if (MessageBox.Show($"Are you sure you want to mark '{itemName}' as fully repaired and available?", "Confirm Repair", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var itemToFix = await _inventoryService.GetItemByIdAsync(itemId);
                    if (itemToFix != null)
                    {
                        itemToFix.Condition = Condition.Good;
                        itemToFix.Status = ItemStatus.Available;
                        await _inventoryService.UpdateItemAsync(
                            itemToFix.Id,
                            itemToFix.Name,
                            itemToFix.Category.ToString(),
                            itemToFix.Status.ToString(),
                            itemToFix.Condition
                        );

                        _damagedItems.RemoveAll(i => i.Id == itemId);

                        this.Location = new Point(this.Location.X, this.Location.Y + 5);

                        // Briefly re-trigger typewriter when reloading
                        lblHeader.Text = "";
                        _typewriterIndex = 0;

                        LoadCards();
                        RepairDetailsPopup_Load(this, EventArgs.Empty);

                        ToastNotification.Show(this, $"{itemName} is back in active inventory!", ToastType.Success);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error updating database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _onSaved?.Invoke();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _ = ClosePopupAsync();
        }
    }
}