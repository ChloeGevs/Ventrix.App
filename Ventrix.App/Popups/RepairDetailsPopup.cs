using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MaterialSkin.Controls;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using System.Threading.Tasks;
using Ventrix.Domain.Enums;

namespace Ventrix.App.Popups
{
    public partial class RepairDetailsPopup : MaterialForm
    {
        private readonly InventoryService _inventoryService;
        private readonly Func<Task> _onRefresh;
        private List<InventoryItem> _damagedItems;

        public RepairDetailsPopup(List<InventoryItem> damagedItems, InventoryService inventoryService, Func<Task> onRefresh)
        {
            _inventoryService = inventoryService;
            _onRefresh = onRefresh;
            _damagedItems = damagedItems;

            InitializeComponent();
            ApplyLocalBranding();
            LoadRepairList();
        }

        private void ApplyLocalBranding()
        {
            ThemeManager.ApplyCustomFont(lblHeader, ThemeManager.SubHeaderFont, ThemeManager.VentrixBlue);
            Text = "Ventrix | Maintenance Queue";
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

                btnRepair.Click += async (s, e) => {
                    item.Condition = Condition.Good;
                    // FIX: Use the Enum instead of a string
                    item.Status = ItemStatus.Available;

                    // FIX: Pass the specific properties to match your Service signature
                    await _inventoryService.UpdateItemAsync(item.Id, item.Name, item.Category.ToString(), item.Status.ToString(), item.Condition);

                    if (_onRefresh != null)
                    {
                        await _onRefresh.Invoke();
                    }

                    _damagedItems.Remove(item);
                    LoadRepairList();

                    if (_damagedItems.Count == 0)
                    {
                        MessageBox.Show("All items have been repaired.", "Maintenance Complete");
                        this.Close();
                    }
                };

                card.Controls.Add(name);
                card.Controls.Add(btnRepair);
                flowRepairList.Controls.Add(card);
            }
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}