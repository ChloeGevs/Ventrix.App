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
            ThemeManager.ApplyMaterialTheme(this);

            this.Text = "Ventrix | Maintenance Queue";
            LoadRepairList();
        }

        private void LoadRepairList()
        {
            flowRepairList.Controls.Clear();
            lblHeader.Text = $"Items Requiring Attention ({_damagedItems.Count})";
            lblHeader.ForeColor = Color.IndianRed;

            flowRepairList.Padding = new Padding(5);

            foreach (var item in _damagedItems)
            {
                // Create a sleek, rounded background card
                Guna.UI2.WinForms.Guna2Panel card = new Guna.UI2.WinForms.Guna2Panel
                {
                    Size = new Size(flowRepairList.Width - 30, 80),
                    BackColor = Color.Transparent,
                    FillColor = Color.White,
                    BorderRadius = 8,
                    BorderThickness = 1,
                    BorderColor = Color.FromArgb(230, 235, 240),
                    CustomBorderThickness = new Padding(6, 0, 0, 0),
                    CustomBorderColor = Color.IndianRed, // Red accent on the left
                    Margin = new Padding(5, 5, 5, 10)
                };

                // Item Name Label
                Label name = new Label
                {
                    Text = $"{item.Name} (Unit #{item.Id})",
                    Font = new Font("Segoe UI Semibold", 11.5F, FontStyle.Bold),
                    ForeColor = Color.FromArgb(64, 64, 64),
                    Location = new Point(20, 15),
                    AutoSize = true
                };

                // Item Category/Status Label
                Label details = new Label
                {
                    Text = $"Category: {item.Category}  |  Current Status: {item.Condition}",
                    Font = new Font("Segoe UI", 9F, FontStyle.Regular),
                    ForeColor = Color.Gray,
                    Location = new Point(20, 42),
                    AutoSize = true
                };

                // Modern Action Button
                Guna.UI2.WinForms.Guna2Button btnRepair = new Guna.UI2.WinForms.Guna2Button
                {
                    Text = "MARK FIXED",
                    Size = new Size(120, 36),
                    Location = new Point(card.Width - 140, 22),
                    BorderRadius = 6,
                    FillColor = Color.MediumSeaGreen,
                    Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                    ForeColor = Color.White,
                    Cursor = Cursors.Hand
                };

                btnRepair.Click += async (s, e) => {
                    item.Condition = Condition.Good;
                    item.Status = ItemStatus.Available;

                    await _inventoryService.UpdateItemAsync(item.Id, item.Name, item.Category.ToString(), item.Status.ToString(), item.Condition);

                    if (_onRefresh != null)
                    {
                        await _onRefresh.Invoke();
                    }

                    _damagedItems.Remove(item);
                    LoadRepairList();

                    if (_damagedItems.Count == 0)
                    {
                        // Add "Ventrix.App." to the front so it knows it is a namespace!
                        Ventrix.App.Controls.ToastNotification.Show(this, "All items repaired!", Ventrix.App.Controls.ToastType.Success);
                        await Task.Delay(1000);
                        this.DialogResult = DialogResult.OK;
                        this.Close();
                    }
                };

                card.Controls.Add(name);
                card.Controls.Add(details);
                card.Controls.Add(btnRepair);
                flowRepairList.Controls.Add(card);
            }
        }

        private void btnClose_Click(object sender, EventArgs e) => Close();
    }
}