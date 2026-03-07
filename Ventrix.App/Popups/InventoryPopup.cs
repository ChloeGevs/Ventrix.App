using MaterialSkin.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventrix.Application.Services;
using Ventrix.Domain.Enums;

namespace Ventrix.App.Popups
{
    public partial class InventoryPopup : MaterialForm
    {
        private readonly InventoryService _inventoryService;
        private readonly int? _editId;

        public InventoryPopup(InventoryService invService, int? id = null)
        {
            _inventoryService = invService;
            _editId = id;

            InitializeComponent();
            ThemeManager.ApplyMaterialTheme(this);
            AcceptButton = btnSave;

            SetupDropdowns();

            this.Load += async (s, e) =>
            {
                if (_editId.HasValue) await LoadItemDataAsync();
                else this.Text = "Add New Inventory Item";
            };
        }

        private void SetupDropdowns()
        {
            cmbCategory.Items.Clear();
            cmbStatus.Items.Clear();
            cmbCondition.Items.Clear();

            cmbCategory.Items.AddRange(Enum.GetNames(typeof(ItemCategory)));
            cmbStatus.Items.AddRange(Enum.GetNames(typeof(ItemStatus)));
            cmbCondition.Items.AddRange(Enum.GetNames(typeof(Condition)));

            if (cmbCategory.Items.Count > 0) cmbCategory.SelectedIndex = 0;
            if (cmbStatus.Items.Count > 0) cmbStatus.SelectedIndex = 0;
            if (cmbCondition.Items.Count > 0) cmbCondition.SelectedIndex = 0;
        }

        private async Task LoadItemDataAsync()
        {
            var item = await _inventoryService.GetItemByIdAsync(_editId.Value);
            if (item != null)
            {
                this.Text = $"Edit Item: #{item.Id}";
                txtName.Text = item.Name;
                cmbCategory.Text = item.Category.ToString();
                cmbStatus.Text = item.Status.ToString();
                cmbCondition.Text = item.Condition.ToString();
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter an item name.", "Validation Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtName.Focus();
                return;
            }

            var condition = Enum.Parse<Condition>(cmbCondition.Text);

            if (_editId.HasValue)
            {
                await _inventoryService.UpdateItemAsync(_editId.Value, txtName.Text, cmbCategory.Text, cmbStatus.Text, condition);
            }
            else
            {
                await _inventoryService.AddItemAsync(txtName.Text, cmbCategory.Text, cmbStatus.Text, condition);
            }

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}