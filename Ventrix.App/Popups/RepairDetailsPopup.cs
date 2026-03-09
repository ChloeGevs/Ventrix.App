using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using Ventrix.Domain.Enums;

namespace Ventrix.App.Popups
{
    public partial class RepairDetailsPopup : MaterialForm
    {
        private readonly List<InventoryItem> _damagedItems;
        private readonly InventoryService _inventoryService;
        private readonly Func<Task> _onSaved;

        private DataGridView dgvDamagedItems;

        public RepairDetailsPopup(List<InventoryItem> damagedItems, InventoryService inventoryService, Func<Task> onSaved)
        {
            _damagedItems = damagedItems;
            _inventoryService = inventoryService;
            _onSaved = onSaved;

            InitializeComponent();
            this.Text = "Damaged Items Report";

            SetupGrid();
            LoadItems();
        }

        private void SetupGrid()
        {
            dgvDamagedItems = new DataGridView
            {
                Location = new Point(25, 120), 
                Size = new Size(this.Width - 50, this.Height - 220),
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                RowHeadersVisible = false,
                BackgroundColor = Color.White,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(230, 235, 240)
            };

            dgvDamagedItems.ColumnHeadersHeight = 40;
            dgvDamagedItems.EnableHeadersVisualStyles = false;
            dgvDamagedItems.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(13, 71, 161);
            dgvDamagedItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDamagedItems.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 10, FontStyle.Bold);
            dgvDamagedItems.DefaultCellStyle.Font = new Font("Segoe UI", 10);
            dgvDamagedItems.DefaultCellStyle.Padding = new Padding(5);
            dgvDamagedItems.RowTemplate.Height = 40;

            dgvDamagedItems.Columns.Add("Id", "System ID");
            dgvDamagedItems.Columns.Add("Name", "Item Name");
            dgvDamagedItems.Columns.Add("Category", "Category");

            ContextMenuStrip repairMenu = new ContextMenuStrip();
            var fixBtn = new ToolStripMenuItem("🔧 Mark Item as Repaired");
            fixBtn.Click += async (s, e) => await RepairSelectedItemAsync();
            repairMenu.Items.Add(fixBtn);
            dgvDamagedItems.ContextMenuStrip = repairMenu;

            this.Controls.Add(dgvDamagedItems);
            dgvDamagedItems.BringToFront();
        }

        private void LoadItems()
        {
            dgvDamagedItems.Rows.Clear();
            foreach (var item in _damagedItems)
            {
                dgvDamagedItems.Rows.Add(item.Id, item.Name, item.Category.ToString());
            }
        }

        private async Task RepairSelectedItemAsync()
        {
            if (dgvDamagedItems.SelectedRows.Count > 0)
            {
                int itemId = Convert.ToInt32(dgvDamagedItems.SelectedRows[0].Cells["Id"].Value);
                string itemName = dgvDamagedItems.SelectedRows[0].Cells["Name"].Value.ToString();

                if (MessageBox.Show($"Are you sure you want to mark '{itemName}' as fully repaired and available?", "Confirm Repair", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        var itemToFix = await _inventoryService.GetItemByIdAsync(itemId);
                        if (itemToFix != null)
                        {
                            itemToFix.Condition = Condition.Good;
                            itemToFix.Status = ItemStatus.Available;
                            await _inventoryService.UpdateItemAsync(
                                itemToFix.Id,
                                itemToFix.Name,
                                itemToFix.Category.ToString(),
                                itemToFix.Status.ToString(),
                                itemToFix.Condition
                            );

                            _damagedItems.RemoveAll(i => i.Id == itemId);
                            LoadItems();

                            MessageBox.Show($"{itemName} is now back in active inventory!", "Repaired", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Error updating database: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            base.OnFormClosing(e);
            _onSaved?.Invoke();
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}