using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ventrix.Domain.Models;

namespace Ventrix.App.Popups
{
    public partial class ShowMultiUnitSelectionPopup : Form
    {
        private readonly List<InventoryItem> _availableUnits;
        private readonly int _requiredQuantity;

        public List<InventoryItem> SelectedUnits { get; private set; } = new List<InventoryItem>();

        public ShowMultiUnitSelectionPopup(List<InventoryItem> units, string baseName, int requiredQuantity)
        {
            InitializeComponent();
            _availableUnits = units;
            _requiredQuantity = requiredQuantity;

            lblTitle.Text = $"Select Specific Units";
            lblInstruction.Text = $"Please check exactly {requiredQuantity} {baseName}(s):";

            PopulateList();
        }

        private void PopulateList()
        {
            clbUnits.Items.Clear();
            foreach (var unit in _availableUnits)
            {
                clbUnits.Items.Add(new UnitComboItem
                {
                    Text = $"   {unit.Name} (Condition: {unit.Condition})",
                    Unit = unit
                });
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (clbUnits.CheckedItems.Count != _requiredQuantity)
            {
                MessageBox.Show($"You requested {_requiredQuantity} item(s). Please check exactly {_requiredQuantity} box(es).", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (UnitComboItem item in clbUnits.CheckedItems)
            {
                SelectedUnits.Add(item.Unit);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        // Helper class for the CheckedListBox
        private class UnitComboItem
        {
            public string Text { get; set; }
            public InventoryItem Unit { get; set; }
            public override string ToString() => Text;
        }
    }
}