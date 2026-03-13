using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Ventrix.Domain.Models;
using Ventrix.Domain.Enums;
using Guna.UI2.WinForms;

namespace Ventrix.App.Popups
{
    public partial class MultiRecordSelectionPopup : Form
    {
        public List<int> SelectedIds { get; private set; } = new List<int>();

        public MultiRecordSelectionPopup(string title, string instruction, List<BorrowRecord> records, string btnText, Color btnColor)
        {
            InitializeComponent();

            lblTitle.Text = title;
            lblInstruction.Text = instruction;

            btnConfirm.Text = btnText;
            btnConfirm.FillColor = btnColor;

            // Generate modern Checkboxes inside the FlowLayoutPanel
            foreach (var record in records)
            {
                // Add useful tags to the text just like the main dashboard
                string statusTag = record.Status == BorrowStatus.Overdue ? "[OVERDUE] " : "";
                string dateText = record.BorrowDate.ToString("MMM dd");

                var cb = new Guna2CheckBox
                {
                    Text = $"{statusTag}{record.ItemName ?? "Unknown Item"} (Borrowed: {dateText})",
                    Tag = record.Id,
                    Checked = true,
                    Font = new Font("Segoe UI", 10.5F),
                    ForeColor = Color.FromArgb(64, 64, 64),
                    Cursor = Cursors.Hand,
                    AutoSize = true,
                    MaximumSize = new Size(pnlRecords.Width - 30, 0), // Prevents long text from bleeding past the scrollbar
                    Margin = new Padding(0, 5, 0, 10), // Adds spacing between items in the FlowLayout

                    UncheckedState = { BorderColor = Color.FromArgb(200, 200, 200), BorderRadius = 4, BorderThickness = 1, FillColor = Color.White },
                    CheckedState = { BorderColor = btnColor, BorderRadius = 4, BorderThickness = 1, FillColor = btnColor }
                };

                pnlRecords.Controls.Add(cb);
            }

            // Adjust layout dynamically based on content
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            // 1. Force the instruction label to wrap properly based on form width
            lblInstruction.MaximumSize = new Size(this.Width - 40, 0);
            this.PerformLayout(); // Force UI update

            // 2. Position the records panel below the instruction label
            pnlRecords.Top = lblInstruction.Bottom + 15;

            // 3. Calculate required height for all checkboxes inside the FlowLayout
            int requiredHeight = pnlRecords.Padding.Top + pnlRecords.Padding.Bottom;
            foreach (Control c in pnlRecords.Controls)
            {
                requiredHeight += c.Height + c.Margin.Top + c.Margin.Bottom;
            }

            // 4. Set panel height (min 60, max 250 so it scrolls if too many items)
            pnlRecords.Height = Math.Max(60, Math.Min(requiredHeight, 250));

            // 5. Position buttons perfectly below the list
            btnConfirm.Top = pnlRecords.Bottom + 25;
            btnCancel.Top = btnConfirm.Top;

            // 6. Adjust final form size to wrap tightly around the buttons
            this.ClientSize = new Size(this.Width, btnConfirm.Bottom + 25);
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            SelectedIds.Clear(); // Clear just in case

            foreach (Control c in pnlRecords.Controls)
            {
                if (c is Guna2CheckBox cb && cb.Checked)
                {
                    SelectedIds.Add((int)cb.Tag);
                }
            }

            // Validation: Prevent submitting an empty selection
            if (SelectedIds.Count == 0)
            {
                MessageBox.Show("Please select at least one item.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}