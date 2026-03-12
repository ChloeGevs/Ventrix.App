using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Ventrix.Domain.Models;
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

            // Generate modern Checkboxes
            foreach (var record in records)
            {
                var cb = new Guna2CheckBox
                {
                    Text = record.ItemName ?? "Unknown Item",
                    Tag = record.Id,
                    Checked = true,
                    Font = new Font("Segoe UI", 10.5F),
                    ForeColor = Color.FromArgb(64, 64, 64),
                    Cursor = Cursors.Hand,
                    AutoSize = true,
                    MaximumSize = new Size(380, 0), // Prevents long text from bleeding
                    Margin = new Padding(5, 5, 5, 5),

                    UncheckedState = { BorderColor = Color.FromArgb(200, 200, 200), BorderRadius = 4, BorderThickness = 1, FillColor = Color.White },
                    CheckedState = { BorderColor = btnColor, BorderRadius = 4, BorderThickness = 1, FillColor = btnColor }
                };

                pnlRecords.Controls.Add(cb);
            }

            // Adjust layout dynamically
            AdjustLayout();
        }

        private void AdjustLayout()
        {
            // 1. Set a strict explicit width for the Form first
            int explicitWidth = 460;
            this.Width = explicitWidth;

            // 2. Force the instruction label to wrap properly based on the fixed width
            lblInstruction.MaximumSize = new Size(explicitWidth - 40, 0);
            this.PerformLayout();

            // 3. Position the records panel below the label
            pnlRecords.Top = lblInstruction.Bottom + 15;
            pnlRecords.Width = explicitWidth - 40;

            // 4. Calculate required height for all checkboxes
            // Start with 15px buffer to prevent the bottom item from getting cut off
            int requiredHeight = 15;
            foreach (Control c in pnlRecords.Controls)
            {
                requiredHeight += c.Height + c.Margin.Top + c.Margin.Bottom;
            }

            // Set panel height (min 60, max 250 so it scrolls if too many items)
            pnlRecords.Height = Math.Max(60, Math.Min(requiredHeight, 250));

            // 5. Position buttons perfectly below the list with extra spacing
            btnConfirm.Top = pnlRecords.Bottom + 20;
            btnCancel.Top = btnConfirm.Top;

            // 6. Center the buttons horizontally
            int spacingBetweenButtons = 10;
            int totalButtonWidth = btnCancel.Width + spacingBetweenButtons + btnConfirm.Width;
            int startX = (explicitWidth - totalButtonWidth) / 2; // Calculates the exact middle

            btnCancel.Left = startX;
            btnConfirm.Left = btnCancel.Right + spacingBetweenButtons;

            // 7. Adjust final form size (added 30px padding to the bottom to fix clipping)
            this.ClientSize = new Size(explicitWidth, btnConfirm.Bottom + 30);
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            foreach (Control c in pnlRecords.Controls)
            {
                if (c is Guna2CheckBox cb && cb.Checked)
                {
                    SelectedIds.Add((int)cb.Tag);
                }
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