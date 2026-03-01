using System;
using System.Windows.Forms;
using MaterialSkin;
using MaterialSkin.Controls;
using Ventrix.Application.Services;
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

            // 1. Add form to manager first
            ThemeManager.Initialize(this);

            // 2. Load data
            SetupDropdowns();
            if (_editId.HasValue) LoadItemData();

            // 3. APPLY POPUP BRANDING LAST 
            // This stops the manager's broadcast from hitting the Dashboard a second time
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

        private void LoadItemData()
        {
            var item = _inventoryService.GetItemById(_editId.Value);
            if (item != null)
            {
                Text = "Ventrix | Edit Item Record";
                txtName.Text = item.Name;
                cmbCategory.Text = item.Category;
                cmbStatus.Text = item.Status;
                cmbCondition.Text = item.Condition;
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                // Ensure the message box also follows your theme manager style
                MessageBox.Show("Please enter an item name.", "Validation Error");
                return;
            }

            var item = _editId.HasValue
                ? _inventoryService.GetItemById(_editId.Value)
                : new InventoryItem { DateAdded = DateTime.Now };

            item.Name = txtName.Text;
            item.Category = cmbCategory.Text;
            item.Status = cmbStatus.Text;
            item.Condition = cmbCondition.Text;

            if (_editId.HasValue)
                _inventoryService.UpdateItem(item);
            else
                _inventoryService.AddItem(item);

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}