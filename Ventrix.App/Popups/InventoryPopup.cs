using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventrix.Application.Services;
using Ventrix.Domain.Enums;
using Ventrix.Domain.Models;

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
            MaterialSkinManager.Instance.AddFormToManage(this);
            SetupDropdowns();

            this.Load += async (s, e) =>
            {
                if (_editId.HasValue) await LoadItemDataAsync();
            };
            ApplyPopupBranding();
        }

        private void ApplyPopupBranding()
        {
            ThemeManager.ApplyCustomFont(lblTitle, ThemeManager.SubHeaderFont, ThemeManager.VentrixBlue);
            btnSave.BackColor = ThemeManager.VentrixBlue;
            btnSave.Font = ThemeManager.ButtonFont;
        }

        private void SetupDropdowns()
        {
            cmbCategory.Items.Clear();
            cmbStatus.Items.Clear();
            cmbCondition.Items.Clear();

            cmbCategory.Items.AddRange(new[] { "Hardware", "Device", "Accessory" });
            cmbStatus.Items.AddRange(new[] { "Available", "Borrowed", "Maintenance" });
            cmbCondition.Items.AddRange(new[] { "New", "Good", "Fair", "Damaged" });

            cmbCategory.SelectedIndex = 0;
            cmbStatus.SelectedIndex = 0;
            cmbCondition.SelectedIndex = 0;
        }

        private async Task LoadItemDataAsync()
        {
            var item = await _inventoryService.GetItemByIdAsync(_editId.Value);
            if (item != null)
            {
                Text = "Ventrix | Edit Item Record";
                txtName.Text = item.Name;
                // FIX: Convert Enums to Strings to display in the UI
                cmbCategory.Text = item.Category.ToString();
                cmbStatus.Text = item.Status.ToString();
                cmbCondition.Text = item.Condition.ToString();
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter an item name.", "Validation Error");
                return;
            }

            var category = Enum.Parse<ItemCategory>(cmbCategory.Text);
            var status = Enum.Parse<ItemStatus>(cmbStatus.Text);
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