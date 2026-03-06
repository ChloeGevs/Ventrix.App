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
            this.components = new System.ComponentModel.Container();
            this.gridItems = new Guna.UI2.WinForms.Guna2DataGridView();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.txtSearch = new Guna.UI2.WinForms.Guna2TextBox();
            this.lblStats = new System.Windows.Forms.Label();
            this.btnAddUnit = new Guna.UI2.WinForms.Guna2Button();
            this.gridMenu = new Guna.UI2.WinForms.Guna2ContextMenuStrip();
            this.markGoodItem = new System.Windows.Forms.ToolStripMenuItem();
            this.markDamagedItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteItem = new System.Windows.Forms.ToolStripMenuItem();

            this.panelContainer.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).BeginInit();
            this.gridMenu.SuspendLayout();
            this.SuspendLayout();

            // panelContainer
            this.panelContainer.BackColor = System.Drawing.Color.White; // Modern White Fix
            this.panelContainer.Controls.Add(this.gridItems);
            this.panelContainer.Controls.Add(this.pnlHeader);
            this.panelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContainer.Location = new System.Drawing.Point(0, 64); // Offset for Material Title Bar
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Padding = new System.Windows.Forms.Padding(15, 15, 15, 15);
            this.panelContainer.Size = new System.Drawing.Size(750, 486);

            // pnlHeader
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.txtSearch);
            this.pnlHeader.Controls.Add(this.btnAddUnit);
            this.pnlHeader.Controls.Add(this.lblStats);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(15, 15);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Size = new System.Drawing.Size(720, 50);
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(0, 0, 0, 10);

            // lblStats
            this.lblStats.AutoSize = true;
            this.lblStats.Font = new System.Drawing.Font("Segoe UI Semibold", 10F, System.Drawing.FontStyle.Bold);
            this.lblStats.ForeColor = System.Drawing.Color.FromArgb(64, 64, 64);
            this.lblStats.Location = new System.Drawing.Point(0, 10);
            this.lblStats.Name = "lblStats";
            this.lblStats.Text = "Loading stats...";

            // txtSearch
            this.txtSearch.BorderRadius = 6;
            this.txtSearch.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSearch.DefaultText = "";
            this.txtSearch.PlaceholderText = "Search ID or Name...";
            this.txtSearch.Location = new System.Drawing.Point(360, 0);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(200, 40);
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);

            // btnAddUnit
            this.btnAddUnit.BorderRadius = 6;
            this.btnAddUnit.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnAddUnit.FillColor = System.Drawing.Color.FromArgb(13, 71, 161);
            this.btnAddUnit.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.btnAddUnit.ForeColor = System.Drawing.Color.White;
            this.btnAddUnit.Location = new System.Drawing.Point(570, 0);
            this.btnAddUnit.Name = "btnAddUnit";
            this.btnAddUnit.Size = new System.Drawing.Size(150, 40);
            this.btnAddUnit.Text = "➕ ADD UNITS";
            this.btnAddUnit.Click += new System.EventHandler(this.btnAddUnit_Click);

            // gridItems
            this.gridItems.AllowUserToAddRows = false;
            this.gridItems.BackgroundColor = System.Drawing.Color.White;
            this.gridItems.ContextMenuStrip = this.gridMenu;
            this.gridItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridItems.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gridItems.Location = new System.Drawing.Point(15, 65);
            this.gridItems.Name = "gridItems";
            this.gridItems.ReadOnly = true;
            this.gridItems.RowHeadersVisible = false;
            this.gridItems.RowTemplate.Height = 45; // Taller modern rows
            this.gridItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.gridItems.Size = new System.Drawing.Size(720, 406);
            this.gridItems.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(13, 71, 161);
            this.gridItems.ThemeStyle.HeaderStyle.ForeColor = System.Drawing.Color.White;

            // gridMenu
            this.gridMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.markGoodItem,
            this.markDamagedItem,
            this.toolStripSeparator1,
            this.editItem,
            this.deleteItem});
            this.gridMenu.Name = "gridMenu";
            this.gridMenu.Size = new System.Drawing.Size(185, 98);

            // markGoodItem
            this.markGoodItem.ForeColor = System.Drawing.Color.MediumSeaGreen;
            this.markGoodItem.Name = "markGoodItem";
            this.markGoodItem.Size = new System.Drawing.Size(184, 22);
            this.markGoodItem.Text = "✅ Mark as Good";
            this.markGoodItem.Click += new System.EventHandler(this.markGoodItem_Click);

            // markDamagedItem
            this.markDamagedItem.ForeColor = System.Drawing.Color.IndianRed;
            this.markDamagedItem.Name = "markDamagedItem";
            this.markDamagedItem.Size = new System.Drawing.Size(184, 22);
            this.markDamagedItem.Text = "🚩 Flag as Damaged";
            this.markDamagedItem.Click += new System.EventHandler(this.markDamagedItem_Click);

            // toolStripSeparator1
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(181, 6);

            // editItem
            this.editItem.Name = "editItem";
            this.editItem.Size = new System.Drawing.Size(184, 22);
            this.editItem.Text = "✏️ Full Edit";
            this.editItem.Click += new System.EventHandler(this.editItem_Click);

            // deleteItem
            this.deleteItem.ForeColor = System.Drawing.Color.IndianRed;
            this.deleteItem.Name = "deleteItem";
            this.deleteItem.Size = new System.Drawing.Size(184, 22);
            this.deleteItem.Text = "🗑️ Delete Item";
            this.deleteItem.Click += new System.EventHandler(this.deleteItem_Click);

            // ItemGroupPopup
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 550);
            this.Controls.Add(this.panelContainer);
            this.Name = "ItemGroupPopup";

            // Focus properties
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.ShowInTaskbar = false;
            this.Sizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;

            this.Load += new System.EventHandler(this.ItemGroupPopup_Load);

            this.panelContainer.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).EndInit();
            this.gridMenu.ResumeLayout(false);
            this.ResumeLayout(false);
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