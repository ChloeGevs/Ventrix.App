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
            txtName = new TextBox();
            cmbCategory = new ComboBox();
            cmbStatus = new ComboBox();
            cmbCondition = new ComboBox();
            btnSave = new Button();
            lblTitle = new Label();
            lblCategory = new Label();
            lblStatus = new Label();
            lblCondition = new Label();
            SuspendLayout();
            // 
            // txtName
            // 
            txtName.Font = new Font("Segoe UI", 10F);
            txtName.Location = new Point(48, 189);
            txtName.Margin = new Padding(3, 4, 3, 4);
            txtName.Name = "txtName";
            txtName.PlaceholderText = "Enter Item Name";
            txtName.Size = new Size(406, 30);
            txtName.TabIndex = 1;
            // 
            // cmbCategory
            // 
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.FormattingEnabled = true;
            cmbCategory.Location = new Point(48, 274);
            cmbCategory.Margin = new Padding(3, 4, 3, 4);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.Size = new Size(406, 28);
            cmbCategory.TabIndex = 2;
            // 
            // cmbStatus
            // 
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.FormattingEnabled = true;
            cmbStatus.Location = new Point(48, 344);
            cmbStatus.Margin = new Padding(3, 4, 3, 4);
            cmbStatus.Name = "cmbStatus";
            cmbStatus.Size = new Size(406, 28);
            cmbStatus.TabIndex = 3;
            // 
            // cmbCondition
            // 
            cmbCondition.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCondition.FormattingEnabled = true;
            cmbCondition.Location = new Point(48, 424);
            cmbCondition.Margin = new Padding(3, 4, 3, 4);
            cmbCondition.Name = "cmbCondition";
            cmbCondition.Size = new Size(406, 28);
            cmbCondition.TabIndex = 4;
            // 
            // btnSave
            // 
            btnSave.BackColor = Color.FromArgb(13, 71, 161);
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(25, 566);
            btnSave.Margin = new Padding(3, 4, 3, 4);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(449, 48);
            btnSave.TabIndex = 5;
            btnSave.Text = "SAVE CHANGES";
            btnSave.UseVisualStyleBackColor = false;
            btnSave.Click += btnSave_Click;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Microsoft Sans Serif", 14F, FontStyle.Bold);
            lblTitle.Location = new Point(48, 127);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(151, 29);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Item Details";
            // 
            // lblCategory
            // 
            lblCategory.AutoSize = true;
            lblCategory.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCategory.Location = new Point(48, 240);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new Size(86, 20);
            lblCategory.TabIndex = 7;
            lblCategory.Text = "CATEGORY";
            // 
            // lblStatus
            // 
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblStatus.Location = new Point(48, 320);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(167, 20);
            lblStatus.TabIndex = 6;
            lblStatus.Text = "AVAILABILITY STATUS";
            // 
            // lblCondition
            // 
            lblCondition.AutoSize = true;
            lblCondition.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblCondition.Location = new Point(48, 400);
            lblCondition.Name = "lblCondition";
            lblCondition.Size = new Size(94, 20);
            lblCondition.TabIndex = 5;
            lblCondition.Text = "CONDITION";
            // 
            // InventoryPopup
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(515, 720);
            Controls.Add(cmbCondition);
            Controls.Add(lblCondition);
            Controls.Add(cmbStatus);
            Controls.Add(lblStatus);
            Controls.Add(cmbCategory);
            Controls.Add(lblCategory);
            Controls.Add(lblTitle);
            Controls.Add(txtName);
            Controls.Add(btnSave);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Margin = new Padding(3, 4, 3, 4);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InventoryPopup";
            Padding = new Padding(3, 85, 3, 2);
            StartPosition = FormStartPosition.CenterParent;
            Text = "Ventrix | Inventory Management";
            ResumeLayout(false);
            PerformLayout();
        }

        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.ComboBox cmbCategory;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.ComboBox cmbCondition;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.Label lblCategory;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.Label lblCondition;
    }
}