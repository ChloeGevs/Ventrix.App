using System.Drawing;
using System.Windows.Forms;

namespace Ventrix.App.Popups
{
    partial class ShowMultiUnitSelectionPopup
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblTitle;
        private Label lblInstruction;
        private Panel listContainer;
        private CheckedListBox clbUnits;
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
            this.lblTitle = new Label();
            this.lblInstruction = new Label();
            this.listContainer = new Panel();
            this.clbUnits = new CheckedListBox();
            this.btnCancel = new Button();
            this.btnOk = new Button();

            this.listContainer.SuspendLayout();
            this.SuspendLayout();

            // lblTitle
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblTitle.ForeColor = Color.FromArgb(31, 41, 55);
            this.lblTitle.Location = new Point(25, 20);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Text = "Select Specific Units";

            // lblInstruction
            this.lblInstruction.AutoSize = true;
            this.lblInstruction.Font = new Font("Segoe UI", 10F);
            this.lblInstruction.ForeColor = Color.FromArgb(107, 114, 128);
            this.lblInstruction.Location = new Point(25, 55);
            this.lblInstruction.Name = "lblInstruction";
            this.lblInstruction.Text = "Please check exactly X units:";

            // listContainer
            this.listContainer.BackColor = Color.White;
            this.listContainer.BorderStyle = BorderStyle.FixedSingle;
            this.listContainer.Controls.Add(this.clbUnits);
            this.listContainer.ForeColor = Color.FromArgb(209, 213, 219);
            this.listContainer.Location = new Point(25, 85);
            this.listContainer.Name = "listContainer";
            this.listContainer.Size = new Size(430, 185);

            // clbUnits
            this.clbUnits.BorderStyle = BorderStyle.None;
            this.clbUnits.CheckOnClick = true;
            this.clbUnits.Cursor = Cursors.Hand;
            this.clbUnits.Dock = DockStyle.Fill;
            this.clbUnits.Font = new Font("Segoe UI", 11F);
            this.clbUnits.IntegralHeight = false;
            this.clbUnits.Name = "clbUnits";

            // btnCancel
            this.btnCancel.BackColor = Color.White;
            this.btnCancel.Cursor = Cursors.Hand;
            this.btnCancel.FlatAppearance.BorderColor = Color.FromArgb(209, 213, 219);
            this.btnCancel.FlatStyle = FlatStyle.Flat;
            this.btnCancel.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnCancel.ForeColor = Color.FromArgb(107, 114, 128);
            this.btnCancel.Location = new Point(215, 290);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new Size(110, 40);
            this.btnCancel.Text = "Cancel";
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);

            // btnOk
            this.btnOk.BackColor = Color.FromArgb(37, 99, 235); // Primary Blue to match Borrowing action
            this.btnOk.Cursor = Cursors.Hand;
            this.btnOk.FlatAppearance.BorderSize = 0;
            this.btnOk.FlatStyle = FlatStyle.Flat;
            this.btnOk.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnOk.ForeColor = Color.White;
            this.btnOk.Location = new Point(335, 290);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new Size(120, 40);
            this.btnOk.Text = "Confirm";
            this.btnOk.Click += new System.EventHandler(this.BtnOk_Click);

            // ShowMultiUnitSelectionPopup
            this.BackColor = Color.White;
            this.ClientSize = new Size(500, 390);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.listContainer);
            this.Controls.Add(this.lblInstruction);
            this.Controls.Add(this.lblTitle);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ShowMultiUnitSelectionPopup";
            this.StartPosition = FormStartPosition.CenterParent;
            this.Text = "Select Inventory Units";

            this.listContainer.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }
    }
}