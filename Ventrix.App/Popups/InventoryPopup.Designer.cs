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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            panelContent = new Panel();
            btnSave = new Guna.UI2.WinForms.Guna2Button();
            cmbCondition = new Guna.UI2.WinForms.Guna2ComboBox();
            cmbStatus = new Guna.UI2.WinForms.Guna2ComboBox();
            cmbCategory = new Guna.UI2.WinForms.Guna2ComboBox();
            txtName = new Guna.UI2.WinForms.Guna2TextBox();
            panelContent.SuspendLayout();
            SuspendLayout();
            // 
            // panelContent
            // 
            panelContent.BackColor = Color.White;
            panelContent.Controls.Add(btnSave);
            panelContent.Controls.Add(cmbCondition);
            panelContent.Controls.Add(cmbStatus);
            panelContent.Controls.Add(cmbCategory);
            panelContent.Controls.Add(txtName);
            panelContent.Dock = DockStyle.Fill;
            panelContent.Location = new Point(4, 98);
            panelContent.Margin = new Padding(4, 5, 4, 5);
            panelContent.Name = "panelContent";
            panelContent.Size = new Size(459, 482);
            panelContent.TabIndex = 0;
            // 
            // btnSave
            // 
            btnSave.BorderRadius = 6;
            btnSave.Cursor = Cursors.Hand;
            btnSave.CustomizableEdges = customizableEdges1;
            btnSave.FillColor = Color.FromArgb(13, 71, 161);
            btnSave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnSave.ForeColor = Color.White;
            btnSave.Location = new Point(40, 385);
            btnSave.Margin = new Padding(4, 5, 4, 5);
            btnSave.Name = "btnSave";
            btnSave.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnSave.Size = new Size(387, 69);
            btnSave.TabIndex = 0;
            btnSave.Text = "SAVE ITEM RECORD";
            btnSave.Click += btnSave_Click;
            // 
            // cmbCondition
            // 
            cmbCondition.BackColor = Color.Transparent;
            cmbCondition.BorderRadius = 6;
            cmbCondition.CustomizableEdges = customizableEdges3;
            cmbCondition.DrawMode = DrawMode.OwnerDrawFixed;
            cmbCondition.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCondition.FocusedColor = Color.Empty;
            cmbCondition.Font = new Font("Segoe UI", 10F);
            cmbCondition.ForeColor = Color.FromArgb(68, 88, 112);
            cmbCondition.ItemHeight = 30;
            cmbCondition.Location = new Point(40, 292);
            cmbCondition.Margin = new Padding(4, 5, 4, 5);
            cmbCondition.Name = "cmbCondition";
            cmbCondition.ShadowDecoration.CustomizableEdges = customizableEdges4;
            cmbCondition.Size = new Size(385, 36);
            cmbCondition.TabIndex = 1;
            // 
            // cmbStatus
            // 
            cmbStatus.BackColor = Color.Transparent;
            cmbStatus.BorderRadius = 6;
            cmbStatus.CustomizableEdges = customizableEdges5;
            cmbStatus.DrawMode = DrawMode.OwnerDrawFixed;
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.FocusedColor = Color.Empty;
            cmbStatus.Font = new Font("Segoe UI", 10F);
            cmbStatus.ForeColor = Color.FromArgb(68, 88, 112);
            cmbStatus.ItemHeight = 30;
            cmbStatus.Location = new Point(40, 215);
            cmbStatus.Margin = new Padding(4, 5, 4, 5);
            cmbStatus.Name = "cmbStatus";
            cmbStatus.ShadowDecoration.CustomizableEdges = customizableEdges6;
            cmbStatus.Size = new Size(385, 36);
            cmbStatus.TabIndex = 2;
            // 
            // cmbCategory
            // 
            cmbCategory.BackColor = Color.Transparent;
            cmbCategory.BorderRadius = 6;
            cmbCategory.CustomizableEdges = customizableEdges7;
            cmbCategory.DrawMode = DrawMode.OwnerDrawFixed;
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.FocusedColor = Color.Empty;
            cmbCategory.Font = new Font("Segoe UI", 10F);
            cmbCategory.ForeColor = Color.FromArgb(68, 88, 112);
            cmbCategory.ItemHeight = 30;
            cmbCategory.Location = new Point(40, 138);
            cmbCategory.Margin = new Padding(4, 5, 4, 5);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.ShadowDecoration.CustomizableEdges = customizableEdges8;
            cmbCategory.Size = new Size(385, 36);
            cmbCategory.TabIndex = 3;
            // 
            // txtName
            // 
            txtName.BorderRadius = 6;
            txtName.Cursor = Cursors.IBeam;
            txtName.CustomizableEdges = customizableEdges9;
            txtName.DefaultText = "";
            txtName.Font = new Font("Segoe UI", 10F);
            txtName.Location = new Point(40, 46);
            txtName.Margin = new Padding(4, 6, 4, 6);
            txtName.Name = "txtName";
            txtName.PlaceholderText = "Item";
            txtName.SelectedText = "";
            txtName.ShadowDecoration.CustomizableEdges = customizableEdges10;
            txtName.Size = new Size(387, 69);
            txtName.TabIndex = 4;
            // 
            // InventoryPopup
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(467, 585);
            Controls.Add(panelContent);
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "InventoryPopup";
            Padding = new Padding(4, 98, 4, 5);
            Sizable = false;
            StartPosition = FormStartPosition.CenterParent;
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