namespace Ventrix.App.Popups
{
    partial class ItemGroupPopup
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            gridItems = new Guna.UI2.WinForms.Guna2DataGridView();
            panelContainer = new System.Windows.Forms.Panel();
            pnlHeader = new System.Windows.Forms.Panel();
            txtSearch = new Guna.UI2.WinForms.Guna2TextBox();
            lblStats = new System.Windows.Forms.Label();
            btnAddUnit = new Guna.UI2.WinForms.Guna2Button();
            gridMenu = new Guna.UI2.WinForms.Guna2ContextMenuStrip();
            markGoodItem = new System.Windows.Forms.ToolStripMenuItem();
            markDamagedItem = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            editItem = new System.Windows.Forms.ToolStripMenuItem();
            deleteItem = new System.Windows.Forms.ToolStripMenuItem();

            panelContainer.SuspendLayout();
            pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(gridItems)).BeginInit();
            gridMenu.SuspendLayout();
            SuspendLayout();

            // panelContainer
            panelContainer.BackColor = System.Drawing.Color.White; // Modern White Fix
            panelContainer.Controls.Add(gridItems);
            panelContainer.Controls.Add(pnlHeader);
            panelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            panelContainer.Location = new System.Drawing.Point(0, 64); // Offset for Material Title Bar
            panelContainer.Name = "panelContainer";
            panelContainer.Padding = new System.Windows.Forms.Padding(15, 15, 15, 15);
            panelContainer.Size = new System.Drawing.Size(750, 486);

            // pnlHeader
            pnlHeader.BackColor = System.Drawing.Color.White;
            pnlHeader.Controls.Add(txtSearch);
            pnlHeader.Controls.Add(btnAddUnit);
            pnlHeader.Controls.Add(lblStats);
            pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            pnlHeader.Location = new System.Drawing.Point(15, 15);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new System.Drawing.Size(720, 50);
            pnlHeader.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);

            // lblStats
            lblStats.AutoSize = true;
            lblStats.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            lblStats.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            lblStats.Location = new System.Drawing.Point(0, 10);
            lblStats.Name = "lblStats";
            lblStats.Text = "Loading stats...";

            // txtSearch
            txtSearch.BorderRadius = 6;
            txtSearch.Cursor = System.Windows.Forms.Cursors.IBeam;
            txtSearch.DefaultText = "";
            txtSearch.PlaceholderText = "Search ID or Name...";
            txtSearch.Location = new System.Drawing.Point(360, 0);
            txtSearch.Name = "txtSearch";
            txtSearch.Size = new System.Drawing.Size(200, 40);
            txtSearch.TextChanged += new System.EventHandler(txtSearch_TextChanged);

            // btnAddUnit
            btnAddUnit.BorderRadius = 6;
            btnAddUnit.Dock = System.Windows.Forms.DockStyle.Right;
            btnAddUnit.FillColor = System.Drawing.Color.FromArgb(13, 71, 161);
            btnAddUnit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            btnAddUnit.ForeColor = System.Drawing.Color.White;
            btnAddUnit.Location = new System.Drawing.Point(570, 0);
            btnAddUnit.Name = "btnAddUnit";
            btnAddUnit.Size = new System.Drawing.Size(150, 40);
            btnAddUnit.Text = "➕ ADD UNITS";
            btnAddUnit.Click += new System.EventHandler(btnAddUnit_Click);

            // gridItems
            gridItems.AllowUserToAddRows = false;
            gridItems.BackgroundColor = System.Drawing.Color.White;
            gridItems.ContextMenuStrip = gridMenu;
            gridItems.Dock = System.Windows.Forms.DockStyle.Fill;
            gridItems.Font = new System.Drawing.Font("Segoe UI", 10F);
            gridItems.Location = new System.Drawing.Point(15, 65);
            gridItems.Name = "gridItems";
            gridItems.ReadOnly = true;
            gridItems.RowHeadersVisible = false;
            gridItems.RowTemplate.Height = 45; // Taller modern rows
            gridItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            gridItems.Size = new System.Drawing.Size(720, 406);
            gridItems.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(13, 71, 161);
            gridItems.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;

            // gridMenu
            gridMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            markGoodItem,
            markDamagedItem,
            toolStripSeparator1,
            editItem,
            deleteItem});
            gridMenu.Name = "gridMenu";
            gridMenu.Size = new System.Drawing.Size(185, 98);

            // markGoodItem
            markGoodItem.ForeColor = System.Drawing.Color.MediumSeaGreen;
            markGoodItem.Name = "markGoodItem";
            markGoodItem.Size = new System.Drawing.Size(184, 22);
            markGoodItem.Text = "Mark as Good";
            markGoodItem.Click += new System.EventHandler(markGoodItem_Click);

            // markDamagedItem
            markDamagedItem.ForeColor = System.Drawing.Color.IndianRed;
            markDamagedItem.Name = "markDamagedItem";
            markDamagedItem.Size = new System.Drawing.Size(184, 22);
            markDamagedItem.Text = "Mark as Damaged";
            markDamagedItem.Click += new System.EventHandler(markDamagedItem_Click);

            // toolStripSeparator1
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(181, 6);

            // editItem
            editItem.Name = "editItem";
            editItem.Size = new System.Drawing.Size(184, 22);
            editItem.Text = "Full Edit";
            editItem.Click += new System.EventHandler(editItem_Click);

            // deleteItem
            deleteItem.ForeColor = System.Drawing.Color.IndianRed;
            deleteItem.Name = "deleteItem";
            deleteItem.Size = new System.Drawing.Size(184, 22);
            deleteItem.Text = "Delete Item";
            deleteItem.Click += new System.EventHandler(deleteItem_Click);

            // ItemGroupPopup
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            ClientSize = new System.Drawing.Size(750, 550);
            Controls.Add(panelContainer);
            Name = "ItemGroupPopup";

            // Focus properties
            MaximizeBox = false;
            MinimizeBox = false;
            ShowInTaskbar = false;
            Sizable = false;
            StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            Load += new System.EventHandler(ItemGroupPopup_Load);

            panelContainer.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(gridItems)).EndInit();
            gridMenu.ResumeLayout(false);
            ResumeLayout(false);
        }

        private Guna.UI2.WinForms.Guna2DataGridView gridItems;
        private System.Windows.Forms.Panel panelContainer;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Label lblStats;
        private Guna.UI2.WinForms.Guna2TextBox txtSearch;
        private Guna.UI2.WinForms.Guna2Button btnAddUnit;
        private Guna.UI2.WinForms.Guna2ContextMenuStrip gridMenu;
        private System.Windows.Forms.ToolStripMenuItem markGoodItem;
        private System.Windows.Forms.ToolStripMenuItem markDamagedItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem editItem;
        private System.Windows.Forms.ToolStripMenuItem deleteItem;
    }
}