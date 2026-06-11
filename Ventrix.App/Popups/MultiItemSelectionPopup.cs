using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Ventrix.Domain.Models;

namespace Ventrix.App.Popups
{
    public class MultiItemSelectionPopup : Form
    {
        public List<InventoryItem> SelectedItems { get; private set; } = new List<InventoryItem>();

        private CheckedListBox chkListItems;
        private Button btnConfirm;
        private Button btnCancel;
        private Label lblTitle;
        private Label lblInstruction;
        private List<InventoryItem> _items;

        public MultiItemSelectionPopup(string title, string instruction, List<InventoryItem> items, string btnText, Color btnColor)
        {
            _items = items;

            // Form Setup
            this.Text = title;
            this.Size = new Size(450, 500);
            this.StartPosition = FormStartPosition.CenterParent;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.BackColor = Color.White;
            this.ShowInTaskbar = false;

            // Title Label
            lblTitle = new Label
            {
                Text = title,
                Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                ForeColor = Color.FromArgb(31, 41, 55),
                Location = new Point(20, 20),
                AutoSize = true
            };

            // Instruction Label
            lblInstruction = new Label
            {
                Text = instruction,
                Font = new Font("Segoe UI", 10F),
                ForeColor = Color.FromArgb(107, 114, 128),
                Location = new Point(20, 55),
                Size = new Size(390, 40)
            };

            // Checked List Box for Inventory Items
            chkListItems = new CheckedListBox
            {
                Location = new Point(20, 100),
                Size = new Size(390, 280),
                Font = new Font("Segoe UI", 10F),
                CheckOnClick = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Populate the checklist
            foreach (var item in _items)
            {
                // Display the ID and the Item Name clearly
                string displayText = $"[ID: {item.Id}] {item.Name}";
                chkListItems.Items.Add(new ItemWrapper { DisplayText = displayText, Item = item });
            }

            // Confirm Button
            btnConfirm = new Button
            {
                Text = btnText,
                BackColor = btnColor,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F, FontStyle.Bold),
                Location = new Point(230, 400),
                Size = new Size(180, 40),
                Cursor = Cursors.Hand
            };
            btnConfirm.FlatAppearance.BorderSize = 0;
            btnConfirm.Click += BtnConfirm_Click;

            // Cancel Button
            btnCancel = new Button
            {
                Text = "Cancel",
                BackColor = Color.FromArgb(243, 244, 246),
                ForeColor = Color.FromArgb(31, 41, 55),
                FlatStyle = FlatStyle.Flat,
                Font = new Font("Segoe UI", 10F),
                Location = new Point(20, 400),
                Size = new Size(100, 40),
                Cursor = Cursors.Hand
            };
            btnCancel.FlatAppearance.BorderSize = 0;
            btnCancel.Click += (s, e) => { this.DialogResult = DialogResult.Cancel; this.Close(); };

            // Add controls
            this.Controls.Add(lblTitle);
            this.Controls.Add(lblInstruction);
            this.Controls.Add(chkListItems);
            this.Controls.Add(btnConfirm);
            this.Controls.Add(btnCancel);
        }

        private void BtnConfirm_Click(object sender, EventArgs e)
        {
            if (chkListItems.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one item to proceed.", "No Items Selected", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (ItemWrapper checkedItem in chkListItems.CheckedItems)
            {
                SelectedItems.Add(checkedItem.Item);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Helper class to store the actual item behind the scenes
        private class ItemWrapper
        {
            public string DisplayText { get; set; }
            public InventoryItem Item { get; set; }
            public override string ToString() => DisplayText;
        }
    }
}