using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using Ventrix.Application.Services;
using Ventrix.Domain.Enums;
using Ventrix.App.Controls;

namespace Ventrix.App.Popups
{
    public partial class ItemGroupPopup : Form
    {
        private readonly InventoryService _inventoryService;
        private readonly BorrowService _borrowService;
        private readonly string _itemName;

        private int _targetY;
        private bool _isAnimating = false;

        // Typewriter Effect Fields
        private string _fullTitleText = "";
        private int _charIndex = 0;
        private int _typewriterCounter = 0;

        public ItemGroupPopup(InventoryService inventoryService, BorrowService borrowService, string itemName)
        {
            InitializeComponent();

            _inventoryService = inventoryService;
            _borrowService = borrowService;
            _itemName = itemName;

            // Prepare the full text, but start the label empty for the typewriter effect
            _fullTitleText = $"Manage Group: {itemName.ToUpper()}";
            lblTitle.Text = "";

            this.Opacity = 0;
            this.StartPosition = FormStartPosition.CenterParent;

            StyleGrid();

            gridItems.CellFormatting += GridItems_CellFormatting;
            gridItems.CellDoubleClick += GridItems_CellDoubleClick;
            gridItems.KeyDown += GridItems_KeyDown;
        }

        private async void ItemGroupPopup_Load(object sender, EventArgs e)
        {
            _targetY = this.Location.Y;
            this.Location = new Point(this.Location.X, _targetY + 40);

            System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer { Interval = 10 };
            animTimer.Tick += (object s, EventArgs ev) =>
            {
                // Form Fade & Slide Animation
                this.Opacity += (1.0 - this.Opacity) * 0.2;

                int currentY = this.Location.Y;
                int distance = currentY - _targetY;

                if (distance > 0)
                {
                    int move = (int)Math.Ceiling(distance * 0.2);
                    this.Location = new Point(this.Location.X, currentY - move);
                }

                // Typewriter Effect Logic (Appends characters smoothly every 2 ticks)
                _typewriterCounter++;
                if (_typewriterCounter % 2 == 0 && _charIndex < _fullTitleText.Length)
                {
                    _charIndex++;
                    lblTitle.Text = _fullTitleText.Substring(0, _charIndex);
                }

                // Completion check for both form and title
                if (distance <= 0 && this.Opacity >= 0.98 && _charIndex >= _fullTitleText.Length)
                {
                    this.Opacity = 1.0;
                    this.Location = new Point(this.Location.X, _targetY);
                    lblTitle.Text = _fullTitleText; // Ensure full text is set accurately

                    animTimer.Stop();
                    animTimer.Dispose();
                }
            };
            animTimer.Start();

            await LoadData();
        }

        private async Task ClosePopupAsync(DialogResult result = DialogResult.Cancel)
        {
            if (_isAnimating) return;
            _isAnimating = true;

            System.Windows.Forms.Timer animTimer = new System.Windows.Forms.Timer { Interval = 10 };
            animTimer.Tick += (object s, EventArgs ev) =>
            {
                this.Opacity -= 0.15;
                this.Location = new Point(this.Location.X, this.Location.Y + 3);

                if (this.Opacity <= 0)
                {
                    animTimer.Stop();
                    animTimer.Dispose();
                    this.DialogResult = result;
                    this.Close();
                }
            };
            animTimer.Start();
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                _ = ClosePopupAsync();
                return true;
            }

            if (keyData == (Keys.Control | Keys.F))
            {
                if (txtSearch != null)
                {
                    txtSearch.Focus();
                    txtSearch.SelectAll();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            _ = ClosePopupAsync();
        }

        private void GridItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (gridItems.SelectedRows.Count > 0)
            {
                if (e.KeyCode == Keys.Delete)
                {
                    deleteItem_Click(sender, e);
                    e.Handled = true;
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    editItem_Click(sender, e);
                    e.Handled = true;
                }
            }
        }

        private void StyleGrid()
        {
            gridItems.BackgroundColor = Color.White;
            gridItems.BorderStyle = BorderStyle.None;
            gridItems.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            gridItems.GridColor = Color.FromArgb(241, 245, 249);

            gridItems.RowTemplate.Height = 55;
            gridItems.ColumnHeadersHeight = 50;

            gridItems.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(248, 250, 252);
            gridItems.ThemeStyle.HeaderStyle.ForeColor = Color.FromArgb(71, 85, 105);
            gridItems.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);

            gridItems.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(238, 242, 255);
            gridItems.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(67, 56, 202);

            gridItems.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(252, 253, 255);

            gridItems.CellMouseEnter += (s, e) => { if (e.RowIndex >= 0) gridItems.Cursor = Cursors.Hand; };
            gridItems.CellMouseLeave += (s, e) => { if (e.RowIndex >= 0) gridItems.Cursor = Cursors.Default; };
        }

        private string GetBaseItemName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Unknown Item";
            int index = name.IndexOf(" #");
            return index > 0 ? name.Substring(0, index).Trim() : name.Trim();
        }

        private async Task LoadData(string searchTerm = "")
        {
            gridItems.Rows.Clear();
            gridItems.Columns.Clear();

            gridItems.Columns.Add("ID", "System ID");
            gridItems.Columns.Add("ExactName", "Exact Name");
            gridItems.Columns.Add("Condition", "Condition");
            gridItems.Columns.Add("Status", "Status");

            gridItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridItems.Columns["ID"].FillWeight = 40;

            var allItems = await _inventoryService.GetAllItemsAsync();

            var specificItems = allItems
                .Where(i => GetBaseItemName(i.Name) == _itemName)
                .OrderBy(i => i.Id)
                .ToList();

            int total = specificItems.Count;
            int available = specificItems.Count(x => x.Status == ItemStatus.Available);
            int damaged = specificItems.Count(x => x.Condition == Condition.Damaged || x.Status == ItemStatus.Lost);

            lblTotalVal.Text = total.ToString();
            lblAvailVal.Text = available.ToString();
            lblRepairVal.Text = damaged.ToString();
            lblRepairVal.ForeColor = damaged > 0 ? Color.FromArgb(225, 29, 72) : Color.FromArgb(15, 23, 42);

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                specificItems = specificItems.Where(i =>
                    i.Id.ToString().Contains(searchTerm) ||
                    i.Name.ToLower().Contains(searchTerm)).ToList();
            }

            foreach (var item in specificItems)
            {
                gridItems.Rows.Add(item.Id, item.Name, item.Condition, item.Status);
            }
        }

        private async void txtSearch_TextChanged(object sender, EventArgs e)
        {
            await LoadData(txtSearch.Text);
        }

        private async void btnAddUnit_Click(object sender, EventArgs e)
        {
            var allItems = await _inventoryService.GetAllItemsAsync();
            var templateItem = allItems.FirstOrDefault(i => GetBaseItemName(i.Name) == _itemName);

            if (templateItem != null)
            {
                using (var popup = new QuantityPopup())
                {
                    if (popup.ShowDialog() == DialogResult.OK)
                    {
                        int quantity = popup.SelectedQuantity;

                        for (int i = 0; i < quantity; i++)
                        {
                            await _inventoryService.AddItemAsync(
                                _itemName,
                                templateItem.Category.ToString(),
                                ItemStatus.Available.ToString(),
                                Condition.Good
                            );
                        }

                        await LoadData();
                        ToastNotification.Show(this, $"Successfully added {quantity} units!", ToastType.Success);
                    }
                }
            }
        }

        private async void markDamagedItem_Click(object sender, EventArgs e)
        {
            await ToggleItemCondition(Condition.Damaged);
        }

        private async void markGoodItem_Click(object sender, EventArgs e)
        {
            await ToggleItemCondition(Condition.Good);
        }

        private async Task ToggleItemCondition(Condition newCondition)
        {
            if (gridItems.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(gridItems.SelectedRows[0].Cells["ID"].Value);

            var item = await _inventoryService.GetItemByIdAsync(id);
            if (item != null)
            {
                await _inventoryService.UpdateItemAsync(id, item.Name, item.Category.ToString(), item.Status.ToString(), newCondition);
                await LoadData();
                ToastNotification.Show(this, $"Unit #{id} marked as {newCondition}.", newCondition == Condition.Good ? ToastType.Success : ToastType.Warning);
            }
        }

        private void GridItems_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.Value != null)
            {
                string colName = gridItems.Columns[e.ColumnIndex].Name;
                string value = e.Value.ToString();

                e.CellStyle.Font = new Font("Segoe UI Semibold", 10F);
                e.CellStyle.ForeColor = Color.FromArgb(51, 65, 85);

                if (colName == "Status" || colName == "Condition")
                {
                    if (value == "Available" || value == "Good")
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(16, 185, 129);
                    }
                    else if (value == "Borrowed")
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(217, 119, 6);
                    }
                    else if (value == "Damaged" || value == "Missing" || value == "Lost")
                    {
                        e.CellStyle.ForeColor = Color.FromArgb(239, 68, 68);
                    }
                }
            }
        }

        private void GridItems_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            editItem_Click(sender, e);
        }

        private async void editItem_Click(object sender, EventArgs e)
        {
            if (gridItems.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(gridItems.SelectedRows[0].Cells["ID"].Value);

            using (var popup = new InventoryPopup(_inventoryService, id))
            {
                if (popup.ShowDialog() == DialogResult.OK) await LoadData();
            }
        }

        private async void deleteItem_Click(object sender, EventArgs e)
        {
            if (gridItems.SelectedRows.Count == 0) return;

            int id = Convert.ToInt32(gridItems.SelectedRows[0].Cells["ID"].Value);
            string exactName = gridItems.SelectedRows[0].Cells["ExactName"].Value.ToString();

            if (MessageBox.Show($"Are you sure you want to permanently delete {exactName} (Unit #{id})?", "Confirm Deletion", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                await _inventoryService.DeleteItemAsync(id);
                await LoadData();

                if (gridItems.Rows.Count == 0)
                {
                    MessageBox.Show($"All units for '{_itemName}' have been deleted.", "System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await ClosePopupAsync(DialogResult.OK);
                }
            }
        }
    }
}