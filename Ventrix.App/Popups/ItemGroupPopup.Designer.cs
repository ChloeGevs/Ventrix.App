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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            gridItems = new Guna.UI2.WinForms.Guna2DataGridView();
            gridMenu = new Guna.UI2.WinForms.Guna2ContextMenuStrip();
            markGoodItem = new ToolStripMenuItem();
            markDamagedItem = new ToolStripMenuItem();
            toolStripSeparator1 = new ToolStripSeparator();
            editItem = new ToolStripMenuItem();
            deleteItem = new ToolStripMenuItem();
            panelContainer = new Panel();
            pnlHeader = new Panel();
            txtSearch = new Guna.UI2.WinForms.Guna2TextBox();
            btnAddUnit = new Guna.UI2.WinForms.Guna2Button();
            lblStats = new Label();
            ((System.ComponentModel.ISupportInitialize)gridItems).BeginInit();
            gridMenu.SuspendLayout();
            panelContainer.SuspendLayout();
            pnlHeader.SuspendLayout();
            SuspendLayout();
            // 
            // gridItems
            // 
            gridItems.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = Color.White;
            gridItems.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(13, 71, 161);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            gridItems.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            gridItems.ColumnHeadersHeight = 29;
            gridItems.ContextMenuStrip = gridMenu;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            gridItems.DefaultCellStyle = dataGridViewCellStyle3;
            gridItems.Dock = DockStyle.Fill;
            gridItems.Font = new Font("Segoe UI", 10F);
            gridItems.GridColor = Color.FromArgb(231, 229, 255);
            gridItems.Location = new Point(20, 100);
            gridItems.Margin = new Padding(4, 5, 4, 5);
            gridItems.Name = "gridItems";
            gridItems.ReadOnly = true;
            gridItems.RowHeadersVisible = false;
            gridItems.RowHeadersWidth = 51;
            gridItems.RowTemplate.Height = 45;
            gridItems.Size = new Size(952, 620);
            gridItems.TabIndex = 0;
            gridItems.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            gridItems.ThemeStyle.AlternatingRowsStyle.Font = null;
            gridItems.ThemeStyle.AlternatingRowsStyle.ForeColor = Color.Empty;
            gridItems.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = Color.Empty;
            gridItems.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = Color.Empty;
            gridItems.ThemeStyle.BackColor = Color.White;
            gridItems.ThemeStyle.GridColor = Color.FromArgb(231, 229, 255);
            gridItems.ThemeStyle.HeaderStyle.BackColor = Color.FromArgb(13, 71, 161);
            gridItems.ThemeStyle.HeaderStyle.BorderStyle = DataGridViewHeaderBorderStyle.None;
            gridItems.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 10F);
            gridItems.ThemeStyle.HeaderStyle.ForeColor = Color.White;
            gridItems.ThemeStyle.HeaderStyle.HeaightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridItems.ThemeStyle.HeaderStyle.Height = 29;
            gridItems.ThemeStyle.ReadOnly = true;
            gridItems.ThemeStyle.RowsStyle.BackColor = Color.White;
            gridItems.ThemeStyle.RowsStyle.BorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            gridItems.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 10F);
            gridItems.ThemeStyle.RowsStyle.ForeColor = Color.FromArgb(71, 69, 94);
            gridItems.ThemeStyle.RowsStyle.Height = 45;
            gridItems.ThemeStyle.RowsStyle.SelectionBackColor = Color.FromArgb(231, 229, 255);
            gridItems.ThemeStyle.RowsStyle.SelectionForeColor = Color.FromArgb(71, 69, 94);
            // 
            // gridMenu
            // 
            gridMenu.ImageScalingSize = new Size(20, 20);
            gridMenu.Items.AddRange(new ToolStripItem[] { markGoodItem, markDamagedItem, toolStripSeparator1, editItem, deleteItem });
            gridMenu.Name = "gridMenu";
            gridMenu.RenderStyle.ArrowColor = Color.FromArgb(151, 143, 255);
            gridMenu.RenderStyle.BorderColor = Color.Gainsboro;
            gridMenu.RenderStyle.ColorTable = null;
            gridMenu.RenderStyle.RoundedEdges = true;
            gridMenu.RenderStyle.SelectionArrowColor = Color.White;
            gridMenu.RenderStyle.SelectionBackColor = Color.FromArgb(100, 88, 255);
            gridMenu.RenderStyle.SelectionForeColor = Color.White;
            gridMenu.RenderStyle.SeparatorColor = Color.Gainsboro;
            gridMenu.RenderStyle.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SystemDefault;
            gridMenu.Size = new Size(200, 106);
            // 
            // markGoodItem
            // 
            markGoodItem.ForeColor = Color.MediumSeaGreen;
            markGoodItem.Name = "markGoodItem";
            markGoodItem.Size = new Size(199, 24);
            markGoodItem.Text = "Mark as Good";
            markGoodItem.Click += markGoodItem_Click;
            // 
            // markDamagedItem
            // 
            markDamagedItem.ForeColor = Color.IndianRed;
            markDamagedItem.Name = "markDamagedItem";
            markDamagedItem.Size = new Size(199, 24);
            markDamagedItem.Text = "Mark as Damaged";
            markDamagedItem.Click += markDamagedItem_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(196, 6);
            // 
            // editItem
            // 
            editItem.Name = "editItem";
            editItem.Size = new Size(199, 24);
            editItem.Text = "Full Edit";
            editItem.Click += editItem_Click;
            // 
            // deleteItem
            // 
            deleteItem.ForeColor = Color.IndianRed;
            deleteItem.Name = "deleteItem";
            deleteItem.Size = new Size(199, 24);
            deleteItem.Text = "Delete Item";
            deleteItem.Click += deleteItem_Click;
            // 
            // panelContainer
            // 
            panelContainer.BackColor = Color.White;
            panelContainer.Controls.Add(gridItems);
            panelContainer.Controls.Add(pnlHeader);
            panelContainer.Dock = DockStyle.Fill;
            panelContainer.Location = new Point(4, 98);
            panelContainer.Margin = new Padding(4, 5, 4, 5);
            panelContainer.Name = "panelContainer";
            panelContainer.Padding = new Padding(20, 23, 20, 23);
            panelContainer.Size = new Size(992, 743);
            panelContainer.TabIndex = 1;
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.White;
            pnlHeader.Controls.Add(txtSearch);
            pnlHeader.Controls.Add(btnAddUnit);
            pnlHeader.Controls.Add(lblStats);
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(20, 23);
            pnlHeader.Margin = new Padding(4, 5, 4, 5);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Padding = new Padding(0, 0, 0, 15);
            pnlHeader.Size = new Size(952, 77);
            pnlHeader.TabIndex = 1;
            // 
            // txtSearch
            // 
            txtSearch.BorderRadius = 6;
            txtSearch.Cursor = Cursors.IBeam;
            txtSearch.CustomizableEdges = customizableEdges1;
            txtSearch.DefaultText = "";
            txtSearch.Font = new Font("Segoe UI", 9F);
            txtSearch.Location = new Point(480, 0);
            txtSearch.Margin = new Padding(4, 6, 4, 6);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Search ID or Name...";
            txtSearch.SelectedText = "";
            txtSearch.ShadowDecoration.CustomizableEdges = customizableEdges2;
            txtSearch.Size = new Size(267, 62);
            txtSearch.TabIndex = 0;
            txtSearch.TextChanged += txtSearch_TextChanged;
            // 
            // btnAddUnit
            // 
            btnAddUnit.BorderRadius = 6;
            btnAddUnit.Cursor = Cursors.Hand;
            btnAddUnit.CustomizableEdges = customizableEdges3;
            btnAddUnit.Dock = DockStyle.Right;
            btnAddUnit.FillColor = Color.FromArgb(13, 71, 161);
            btnAddUnit.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnAddUnit.ForeColor = Color.White;
            btnAddUnit.Location = new Point(752, 0);
            btnAddUnit.Margin = new Padding(4, 5, 4, 5);
            btnAddUnit.Name = "btnAddUnit";
            btnAddUnit.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnAddUnit.Size = new Size(200, 62);
            btnAddUnit.TabIndex = 1;
            btnAddUnit.Text = "➕ ADD UNITS";
            btnAddUnit.Click += btnAddUnit_Click;
            // 
            // lblStats
            // 
            lblStats.AutoSize = true;
            lblStats.Font = new Font("Segoe UI Semibold", 10F, FontStyle.Bold);
            lblStats.ForeColor = Color.FromArgb(64, 64, 64);
            lblStats.Location = new Point(0, 15);
            lblStats.Margin = new Padding(4, 0, 4, 0);
            lblStats.Name = "lblStats";
            lblStats.Size = new Size(123, 23);
            lblStats.TabIndex = 2;
            lblStats.Text = "Loading stats...";
            // 
            // ItemGroupPopup
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1000, 846);
            Controls.Add(panelContainer);
            Margin = new Padding(4, 5, 4, 5);
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "ItemGroupPopup";
            Padding = new Padding(4, 98, 4, 5);
            ShowInTaskbar = false;
            Sizable = false;
            StartPosition = FormStartPosition.CenterParent;
            Load += ItemGroupPopup_Load;
            ((System.ComponentModel.ISupportInitialize)gridItems).EndInit();
            gridMenu.ResumeLayout(false);
            panelContainer.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
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