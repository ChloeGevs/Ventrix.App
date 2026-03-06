using System;
using System.Linq;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;
using MaterialSkin.Controls;
using Ventrix.Application.Services;
using Ventrix.Domain.Enums;
using Ventrix.App.Controls;

namespace Ventrix.App.Popups
{
    public partial class ItemGroupPopup : MaterialForm
    {
        private readonly InventoryService _inventoryService;
        private readonly BorrowService _borrowService;
        private readonly string _itemName;

        public ItemGroupPopup(InventoryService inventoryService, BorrowService borrowService, string itemName)
        {
            InitializeComponent();
            ThemeManager.ApplyMaterialTheme(this); // Applies sleek colors

            _inventoryService = inventoryService;
            _borrowService = borrowService;
            _itemName = itemName;

            this.Text = $"Manage Group: {itemName.ToUpper()}";

            StyleGrid(); // Applies advanced styling to the datagrid

            gridItems.CellFormatting += GridItems_CellFormatting;
            gridItems.CellDoubleClick += GridItems_CellDoubleClick;
        }

        private void StyleGrid()
        {
            gridItems.BackgroundColor = Color.White;
            gridItems.BorderStyle = BorderStyle.None;
            gridItems.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            gridItems.GridColor = Color.FromArgb(230, 235, 240);

            gridItems.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(13, 71, 161);
            gridItems.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            gridItems.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI Semibold", 10F);

            gridItems.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(240, 245, 255);
            gridItems.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(13, 71, 161);

            gridItems.ThemeStyle.AlternatingRowsStyle.BackColor = Color.FromArgb(250, 252, 255);

            // Cursor hover effect
            gridItems.CellMouseEnter += (s, e) => { if (e.RowIndex >= 0) gridItems.Cursor = Cursors.Hand; };
            gridItems.CellMouseLeave += (s, e) => { if (e.RowIndex >= 0) gridItems.Cursor = Cursors.Default; };
        }

        private async void ItemGroupPopup_Load(object sender, EventArgs e)
        {
            await LoadData();
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

            // Adjusted logic to match your Enums
            int damaged = specificItems.Count(x => x.Condition == Condition.Damaged || x.Status.ToString() == "Lost");

            lblStats.Text = $"Total Units: {total}   |   Available: {available}   |   Needs Repair: {damaged}";
            if (damaged > 0) lblStats.ForeColor = Color.IndianRed;
            else lblStats.ForeColor = Color.FromArgb(64, 64, 64);

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

                e.CellStyle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);

                if (colName == "Status" || colName == "Condition")
                {
                    if (value == "Available" || value == "Good") e.CellStyle.ForeColor = Color.MediumSeaGreen;
                    else if (value == "Borrowed") e.CellStyle.ForeColor = Color.DarkOrange;
                    else if (value == "Damaged" || value == "Missing" || value == "Lost") e.CellStyle.ForeColor = Color.IndianRed;
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
                    MessageBox.Show($"All units for '{_itemName}' have been deleted.", "Ventrix System", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.DialogResult = DialogResult.OK;
                    this.Close();
                }
            }
        }
    }
}