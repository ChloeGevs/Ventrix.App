using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Ventrix.Domain.Enums;
using Ventrix.Domain.Models;

namespace Ventrix.App.Popups
{
    public partial class ShowMultiReturnSelectionPopup : Form
    {
        private readonly List<BorrowRecord> _allRecords;
        public List<BorrowRecord> SelectedRecords { get; private set; } = new List<BorrowRecord>();

        public ShowMultiReturnSelectionPopup(List<BorrowRecord> records)
        {
            InitializeComponent();
            _allRecords = records;
            PopulateList();
        }

        private void PopulateList()
        {
            clbRecords.Items.Clear();
            foreach (var record in _allRecords)
            {
                string statusTag = record.Status == BorrowStatus.Overdue ? "[OVERDUE] " : "";
                clbRecords.Items.Add(new RecordComboItem
                {
                    Text = $"   {statusTag}{record.ItemName} (Borrowed: {record.BorrowDate:MMM dd})",
                    RecordId = record.Id
                });
            }
        }

        private void BtnOk_Click(object sender, EventArgs e)
        {
            if (clbRecords.CheckedItems.Count == 0)
            {
                MessageBox.Show("Please select at least one item to return.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (RecordComboItem item in clbRecords.CheckedItems)
            {
                SelectedRecords.Add(_allRecords.First(r => r.Id == item.RecordId));
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
        private class RecordComboItem
        {
            public string Text { get; set; }
            public int RecordId { get; set; }
            public override string ToString() => Text;
        }
    }
}