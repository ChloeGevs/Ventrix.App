using MaterialSkin.Controls;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;

namespace Ventrix.App.Popups
{
    public partial class RepairDetailsPopup : MaterialForm
    {
        private readonly InventoryService _inventoryService;
        private readonly Action _onRefresh;
        private List<InventoryItem> _damagedItems;

        public RepairDetailsPopup(List<InventoryItem> damagedItems, InventoryService inventoryService, Action onRefresh)
        {
            _inventoryService = inventoryService;
            _onRefresh = onRefresh;
            _damagedItems = damagedItems;

            InitializeComponent();

            ThemeManager.Initialize(this); // Sync colors and layout
            ApplyLocalBranding();

            LoadRepairList();
        }

        private void ApplyLocalBranding()
        {
            // Lock header font using the ThemeManager
            ThemeManager.ApplyCustomFont(lblHeader, ThemeManager.SubHeaderFont, ThemeManager.VentrixBlue);
            this.Text = "Ventrix | Maintenance Queue";
        }

        private void LoadRepairList()
        {
            flowRepairList.Controls.Clear();
            lblHeader.Text = $"Maintenance Queue ({_damagedItems.Count})";

            foreach (var item in _damagedItems)
            {
                Panel card = new Panel
                {
                    Size = new Size(flowRepairList.Width - 30, 70),
                    BackColor = Color.White,
                    Margin = new Padding(0, 0, 0, 10),
                    BorderStyle = BorderStyle.FixedSingle
                };

                Label name = new Label
                {
                    Text = $"{item.Name} ({item.Category})",
                    Font = new Font("Segoe UI", 9, FontStyle.Bold),
                    Location = new Point(10, 15),
                    AutoSize = true
                };

                Button btnRepair = new Button
                {
                    Text = "FIXED",
                    Size = new Size(80, 30),
                    Location = new Point(card.Width - 90, 15),
                    BackColor = Color.Teal,
                    ForeColor = Color.White,
                    FlatStyle = FlatStyle.Flat
                };

                btnRepair.Click += (s, e) => {
                    item.Condition = "Good";
                    item.Status = "Available";
                    _inventoryService.UpdateItem(item);

                    _onRefresh?.Invoke(); // Refresh dashboard counts

                    // Update local list
                    _damagedItems.Remove(item);
                    LoadRepairList();
                };

                card.Controls.Add(name);
                card.Controls.Add(btnRepair);
                flowRepairList.Controls.Add(card);
            }
        }

        private void btnClose_Click(object sender, EventArgs e) => this.Close();
    }
}