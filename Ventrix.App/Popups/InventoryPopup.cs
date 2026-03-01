using System;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using System.Threading.Tasks;

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

            // 1. Add form to manager first
            MaterialSkinManager.Instance.AddFormToManage(this);

            // 2. Load data
            SetupDropdowns();
            this.Load += async (s, e) =>
            {
                if (_editId.HasValue) await LoadItemDataAsync();
            };
            ApplyPopupBranding();
        }

        private void ApplyPopupBranding()
        {
            // Use the manager to lock fonts so they don't revert to Roboto
            ThemeManager.ApplyCustomFont(lblTitle, ThemeManager.SubHeaderFont, ThemeManager.VentrixBlue);

            // Apply to the Save button
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
                cmbCategory.Text = item.Category;
                cmbStatus.Text = item.Status;
                cmbCondition.Text = item.Condition;
            }
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                // Ensure the message box also follows your theme manager style
                MessageBox.Show("Please enter an item name.", "Validation Error");
                return;
            }

            var item = _editId.HasValue
                ? await _inventoryService.GetItemByIdAsync(_editId.Value)
                : new InventoryItem { DateAdded = DateTime.Now };

            item.Name = txtName.Text;
            item.Category = cmbCategory.Text;
            item.Status = cmbStatus.Text;
            item.Condition = cmbCondition.Text;

            if (_editId.HasValue)
                await _inventoryService.UpdateItemAsync(item);
            else
                await _inventoryService.AddItemAsync(item);

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}