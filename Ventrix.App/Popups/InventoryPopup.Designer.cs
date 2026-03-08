namespace Ventrix.App.Popups
{
    partial class InventoryPopup
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            panelContent = new System.Windows.Forms.Panel();
            btnSave = new Guna.UI2.WinForms.Guna2Button();
            cmbCondition = new Guna.UI2.WinForms.Guna2ComboBox();
            cmbStatus = new Guna.UI2.WinForms.Guna2ComboBox();
            cmbCategory = new Guna.UI2.WinForms.Guna2ComboBox();
            txtName = new Guna.UI2.WinForms.Guna2TextBox();

            panelContent.SuspendLayout();
            SuspendLayout();

            // panelContent
            panelContent.BackColor = System.Drawing.Color.White;
            panelContent.Controls.Add(btnSave);
            panelContent.Controls.Add(cmbCondition);
            panelContent.Controls.Add(cmbStatus);
            panelContent.Controls.Add(cmbCategory);
            panelContent.Controls.Add(txtName);
            panelContent.Dock = System.Windows.Forms.DockStyle.Fill;
            panelContent.Location = new System.Drawing.Point(0, 64);
            panelContent.Name = "panelContent";
            panelContent.Size = new System.Drawing.Size(350, 360);

            // txtName
            txtName.BorderRadius = 6;
            txtName.Cursor = System.Windows.Forms.Cursors.IBeam;
            txtName.DefaultText = "";
            txtName.Font = new System.Drawing.Font("Segoe UI", 10F);
            txtName.PlaceholderText = "Item";
            txtName.Location = new System.Drawing.Point(30, 30);
            txtName.Name = "txtName";
            txtName.Size = new System.Drawing.Size(290, 45);

            // cmbCategory
            cmbCategory.BackColor = System.Drawing.Color.Transparent;
            cmbCategory.BorderRadius = 6;
            cmbCategory.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cmbCategory.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbCategory.Font = new System.Drawing.Font("Segoe UI", 10F);
            cmbCategory.ForeColor = System.Drawing.Color.FromArgb(68, 88, 112);
            cmbCategory.Location = new System.Drawing.Point(30, 90);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.Size = new System.Drawing.Size(290, 36);

            // cmbStatus
            cmbStatus.BackColor = System.Drawing.Color.Transparent;
            cmbStatus.BorderRadius = 6;
            cmbStatus.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbStatus.Font = new System.Drawing.Font("Segoe UI", 10F);
            cmbStatus.ForeColor = System.Drawing.Color.FromArgb(68, 88, 112);
            cmbStatus.Location = new System.Drawing.Point(30, 140);
            cmbStatus.Name = "cmbStatus";
            cmbStatus.Size = new System.Drawing.Size(290, 36);

            // cmbCondition
            cmbCondition.BackColor = System.Drawing.Color.Transparent;
            cmbCondition.BorderRadius = 6;
            cmbCondition.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            cmbCondition.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            cmbCondition.Font = new System.Drawing.Font("Segoe UI", 10F);
            cmbCondition.ForeColor = System.Drawing.Color.FromArgb(68, 88, 112);
            cmbCondition.Location = new System.Drawing.Point(30, 190);
            cmbCondition.Name = "cmbCondition";
            cmbCondition.Size = new System.Drawing.Size(290, 36);

            // btnSave
            btnSave.BorderRadius = 6;
            btnSave.FillColor = System.Drawing.Color.FromArgb(13, 71, 161);
            btnSave.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Bold);
            btnSave.ForeColor = System.Drawing.Color.White;
            btnSave.Location = new System.Drawing.Point(30, 250);
            btnSave.Name = "btnSave";
            btnSave.Size = new System.Drawing.Size(290, 45);
            btnSave.Text = "SAVE ITEM RECORD";
            btnSave.Click += new System.EventHandler(btnSave_Click);

            // InventoryPopup
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(350, 380);
            Controls.Add(panelContent);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InventoryPopup";
            Sizable = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            Text = "Inventory Item";

            panelContent.ResumeLayout(false);
            ResumeLayout(false);
        }

        private System.Windows.Forms.Panel panelContent;
        private Guna.UI2.WinForms.Guna2TextBox txtName;
        private Guna.UI2.WinForms.Guna2ComboBox cmbCategory;
        private Guna.UI2.WinForms.Guna2ComboBox cmbStatus;
        private Guna.UI2.WinForms.Guna2ComboBox cmbCondition;
        private Guna.UI2.WinForms.Guna2Button btnSave;
    }
}