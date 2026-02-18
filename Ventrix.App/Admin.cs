using MaterialSkin;
using MaterialSkin.Controls;
using Ventrix.Services.Service;

namespace Ventrix.App
{
    public partial class Admin : MaterialForm
    {
        private readonly InventoryService _inventoryService;

        public Admin(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
            InitializeComponent();
            SetupTheme();

            btnHamburger.Click += (s, e) => sidebarTimer.Start();

            cardTotal.Click += (s, e) => LoadFilteredData("All");
            cardAvailable.Click += (s, e) => LoadFilteredData("Available");
            cardPending.Click += (s, e) => LoadFilteredData("Borrowed");

            this.Load += (s, e) => LoadFilteredData("All");
        }

        private void SetupTheme()
        {
            MaterialSkinManager.Instance.AddFormToManage(this);
        }

        private void LoadFilteredData(string status)
        {
            dgvInventory.Rows.Clear();
            dgvInventory.Columns.Clear();

            // Define Grid structure
            dgvInventory.Columns.Add("ID", "ID");
            dgvInventory.Columns.Add("Name", "Material Name");
            dgvInventory.Columns.Add("Status", "Status");

            var data = _inventoryService.GetFilteredInventory(status);
            foreach (var item in data)
            {
                dgvInventory.Rows.Add(item.Id, item.Name, item.Status);
            }
            lblDashboardHeader.Text = $"INVENTORY: {status.ToUpper()}";
        }
    }
}