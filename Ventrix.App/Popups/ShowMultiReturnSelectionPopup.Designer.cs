using System.Drawing;
using System.Windows.Forms;

namespace Ventrix.App.Popups
{
    partial class ShowMultiReturnSelectionPopup
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Label lblInstruction;
        private Panel listContainer;
        private CheckedListBox clbRecords;
        private Button btnCancel;
        private Button btnOk;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblTitle = new Label();
            lblInstruction = new Label();
            listContainer = new Panel();
            clbRecords = new CheckedListBox();
            btnCancel = new Button();
            btnOk = new Button();
            listContainer.SuspendLayout();
            SuspendLayout();
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lblTitle.ForeColor = Color.FromArgb(31, 41, 55);
            lblTitle.Location = new Point(25, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(161, 32);
            lblTitle.TabIndex = 4;
            lblTitle.Text = "Return Items";
            // 
            // lblInstruction
            // 
            lblInstruction.AutoSize = true;
            lblInstruction.Font = new Font("Segoe UI", 10F);
            lblInstruction.ForeColor = Color.FromArgb(107, 114, 128);
            lblInstruction.Location = new Point(25, 55);
            lblInstruction.Name = "lblInstruction";
            lblInstruction.Size = new Size(384, 23);
            lblInstruction.TabIndex = 3;
            lblInstruction.Text = "Check ALL the items you are returning right now:";
            // 
            // listContainer
            // 
            listContainer.BackColor = Color.White;
            listContainer.BorderStyle = BorderStyle.FixedSingle;
            listContainer.Controls.Add(clbRecords);
            listContainer.ForeColor = Color.FromArgb(209, 213, 219);
            listContainer.Location = new Point(25, 85);
            listContainer.Name = "listContainer";
            listContainer.Size = new Size(430, 185);
            listContainer.TabIndex = 2;
            // 
            // clbRecords
            // 
            clbRecords.BorderStyle = BorderStyle.None;
            clbRecords.CheckOnClick = true;
            clbRecords.Cursor = Cursors.Hand;
            clbRecords.Dock = DockStyle.Fill;
            clbRecords.Font = new Font("Segoe UI", 11F);
            clbRecords.IntegralHeight = false;
            clbRecords.Location = new Point(0, 0);
            clbRecords.Name = "clbRecords";
            clbRecords.Size = new Size(428, 183);
            clbRecords.TabIndex = 0;
            // 
            // btnCancel
            // 
            btnCancel.BackColor = Color.White;
            btnCancel.Cursor = Cursors.Hand;
            btnCancel.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCancel.ForeColor = Color.FromArgb(107, 114, 128);
            btnCancel.Location = new Point(215, 290);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(110, 40);
            btnCancel.TabIndex = 1;
            btnCancel.Text = "Cancel";
            btnCancel.UseVisualStyleBackColor = false;
            btnCancel.Click += BtnCancel_Click;
            // 
            // btnOk
            // 
            btnOk.BackColor = Color.FromArgb(16, 185, 129);
            btnOk.Cursor = Cursors.Hand;
            btnOk.FlatAppearance.BorderSize = 0;
            btnOk.FlatStyle = FlatStyle.Flat;
            btnOk.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnOk.ForeColor = Color.White;
            btnOk.Location = new Point(335, 290);
            btnOk.Name = "btnOk";
            btnOk.Size = new Size(120, 40);
            btnOk.TabIndex = 0;
            btnOk.Text = "Request Return";
            btnOk.UseVisualStyleBackColor = false;
            btnOk.Click += BtnOk_Click;
            // 
            // ShowMultiReturnSelectionPopup
            // 
            BackColor = Color.White;
            ClientSize = new Size(500, 390);
            Controls.Add(btnOk);
            Controls.Add(btnCancel);
            Controls.Add(listContainer);
            Controls.Add(lblInstruction);
            Controls.Add(lblTitle);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ShowMultiReturnSelectionPopup";
            StartPosition = FormStartPosition.CenterParent;
            Text = "Return Items";
            listContainer.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }
    }
}