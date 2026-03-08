using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;

namespace Ventrix.App.Popups
{
    public partial class RepairDetailsPopup : MaterialForm
    {
        private readonly List<InventoryItem> _damagedItems;
        private readonly InventoryService _inventoryService;
        private readonly Func<Task> _onSaved;

        public RepairDetailsPopup(List<InventoryItem> damagedItems, InventoryService inventoryService, Func<Task> onSaved)
        {
            _damagedItems = damagedItems;
            _inventoryService = inventoryService;
            _onSaved = onSaved;

            InitializeComponent();
            this.Text = "Damaged Items Report";
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }

    }
}