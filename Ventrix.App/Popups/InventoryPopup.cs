using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventrix.Application.Services;
using Ventrix.Domain.Enums;

namespace Ventrix.App.Popups
{
    public partial class InventoryPopup : Form
    {
        private readonly InventoryService _inventoryService;
        private readonly int? _editId;
        private int _targetY;
        private bool _isAnimating = false; // Prevents interactions/glitches during transition

        // Typewriter Effect Fields
        private string _fullTitleText = "";
        private int _charIndex = 0;
        private int _typewriterCounter = 0;

        public InventoryPopup(InventoryService invService, int? id = null)
        {
            InitializeComponent();
            _inventoryService = invService;
            _editId = id;

            // Set up form for animation
            this.Opacity = 0;
            this.StartPosition = FormStartPosition.CenterParent;

            SetupDropdowns();
        }

        private async void InventoryPopup_Load(object sender, EventArgs e)
        {
            // Determine the title text based on whether we are editing or adding
            if (_editId.HasValue)
            {
                _fullTitleText = $"Edit Item #{_editId.Value}";
            }
            else
            {
                _fullTitleText = "Add Inventory Item";
            }

            // Start with an empty title for the typewriter effect
            lblTitle.Text = "";

            // Establish target position for the kinetic slide
            _targetY = this.Location.Y;

            // Start 30px lower for a tighter, snappier feel
            this.Location = new Point(this.Location.X, _targetY + 30);

            // Modern Exponential Ease-Out Entrance Animation (10ms interval for high-refresh feel)
            System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer { Interval = 10 };
            animTimer.Tick += (object s, EventArgs ev) =>
            {
                // Smooth Lerp for Opacity (decelerates as it approaches 1.0)
                this.Opacity += (1.0 - this.Opacity) * 0.2;

                int currentY = this.Location.Y;
                int distance = currentY - _targetY;

                // Smooth Lerp for Position (decelerates as it approaches target)
                if (distance > 0)
                {
                    int move = (int)Math.Ceiling(distance * 0.2); // Ceiling ensures it snaps perfectly to the end
                    this.Location = new Point(this.Location.X, currentY - move);
                }

                // Typewriter Effect Logic (Appends characters every 2 ticks)
                _typewriterCounter++;
                if (_typewriterCounter % 2 == 0 && _charIndex < _fullTitleText.Length)
                {
                    _charIndex++;
                    lblTitle.Text = _fullTitleText.Substring(0, _charIndex);
                }

                // Stop condition (when visually complete and title is fully typed out)
                if (distance <= 0 && this.Opacity >= 0.98 && _charIndex >= _fullTitleText.Length)
                {
                    this.Opacity = 1.0;
                    this.Location = new Point(this.Location.X, _targetY);
                    lblTitle.Text = _fullTitleText; // Ensure exact match at completion
                    animTimer.Stop();
                    animTimer.Dispose();
                }
            };
            animTimer.Start();

            // Load data asynchronously if editing
            if (_editId.HasValue)
            {
                await LoadItemDataAsync();
            }
        }

        private void SetupDropdowns()
        {
            cmbCategory.DataSource = Enum.GetValues(typeof(ItemCategory));
            cmbStatus.DataSource = Enum.GetValues(typeof(ItemStatus));
            cmbCondition.DataSource = Enum.GetValues(typeof(Condition));
        }

        private async Task LoadItemDataAsync()
        {
            try
            {
                var item = await _inventoryService.GetItemByIdAsync(_editId.Value);
                if (item != null)
                {
                    txtName.Text = item.Name;
                    cmbCategory.SelectedItem = item.Category;
                    cmbStatus.SelectedItem = item.Status;
                    cmbCondition.SelectedItem = item.Condition;
                }
                else
                {
                    MessageBox.Show("Item not found.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    await ClosePopupAsync();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load item details: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                await ClosePopupAsync();
            }
        }

        // Modern Exit Animation
        private async Task ClosePopupAsync(DialogResult result = DialogResult.Cancel)
        {
            if (_isAnimating) return;
            _isAnimating = true;

            System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer { Interval = 10 };
            animTimer.Tick += (object s, EventArgs ev) =>
            {
                // Quick linear fade out and slight drop down
                this.Opacity -= 0.15;
                this.Location = new Point(this.Location.X, this.Location.Y + 2);

                if (this.Opacity <= 0)
                {
                    animTimer.Stop();
                    animTimer.Dispose();
                    this.DialogResult = result;
                    this.Close(); // Actually close the form once invisible
                }
            };
            animTimer.Start();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter an item name.", "Validation Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            try
            {
                // UI Kinetic Loading State
                btnSave.Enabled = false;
                btnSave.Text = "Saving...";
                btnSave.FillColor = Color.FromArgb(100, 116, 139); // Slate grayish-blue

                var condition = (Condition)cmbCondition.SelectedItem;
                var category = cmbCategory.SelectedItem.ToString();
                var status = cmbStatus.SelectedItem.ToString();

                if (_editId.HasValue)
                {
                    await _inventoryService.UpdateItemAsync(_editId.Value, txtName.Text, category, status, condition);
                }
                else
                {
                    await _inventoryService.AddItemAsync(txtName.Text, category, status, condition);
                }

                // Trigger modern exit instead of immediate Close()
                await ClosePopupAsync(DialogResult.OK);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to save item: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                btnSave.Enabled = true;
                btnSave.Text = "Save Item";
                btnSave.FillColor = Color.FromArgb(79, 70, 229); // Back to Indigo
            }
        }

        private async void btnCancel_Click(object sender, EventArgs e)
        {
            // Trigger modern exit instead of immediate Close()
            await ClosePopupAsync(DialogResult.Cancel);
        }
    }
}