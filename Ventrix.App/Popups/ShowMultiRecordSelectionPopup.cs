using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Ventrix.Domain.Models;

namespace Ventrix.App.Popups
{
    public class MultiRecordSelectionPopup : Form
    {
        public List<int> SelectedIds { get; private set; } = new List<int>();

        private CheckedListBox chkListItems;
        private Button btnConfirm;
        private Button btnCancel;
        private Label lblTitle;
        private Label lblInstruction;
        private List<BorrowRecord> _records;

        public MultiRecordSelectionPopup(string title, string instruction, List<BorrowRecord> records, string btnText, Color btnColor)
        {
            _records = records;

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

            // Checked List Box for Records
            chkListItems = new CheckedListBox
            {
                Location = new Point(20, 100),
                Size = new Size(390, 280),
                Font = new Font("Segoe UI", 10F),
                CheckOnClick = true,
                BorderStyle = BorderStyle.FixedSingle
            };

            // Populate the checklist
            foreach (var record in _records)
            {
                // Display the item name and its current status
                string displayText = $"[{record.Status}] {record.ItemName ?? "Unknown Item"}";
                chkListItems.Items.Add(new RecordItemWrapper { DisplayText = displayText, RecordId = record.Id });
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

            foreach (RecordItemWrapper checkedItem in chkListItems.CheckedItems)
            {
                SelectedIds.Add(checkedItem.RecordId);
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // Helper class to store the ID behind the scenes while displaying text
        private class RecordItemWrapper
        {
            public string DisplayText { get; set; }
            public int RecordId { get; set; }
            public override string ToString() => DisplayText;
        }
    }
}