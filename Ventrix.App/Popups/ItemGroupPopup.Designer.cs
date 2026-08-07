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

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(this.components);
            this.gridItems = new Guna.UI2.WinForms.Guna2DataGridView();
            this.gridMenu = new Guna.UI2.WinForms.Guna2ContextMenuStrip();
            this.markGoodItem = new System.Windows.Forms.ToolStripMenuItem();
            this.markDamagedItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.editItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panelContainer = new System.Windows.Forms.Panel();
            this.pnlTopBar = new System.Windows.Forms.Panel();
            this.lblTitle = new System.Windows.Forms.Label();
            this.btnClose = new Guna.UI2.WinForms.Guna2ControlBox();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.txtSearch = new Guna.UI2.WinForms.Guna2TextBox();
            this.btnAddUnit = new Guna.UI2.WinForms.Guna2Button();

            this.pnlStatTotal = new System.Windows.Forms.Panel();
            this.lblTotalLabel = new System.Windows.Forms.Label();
            this.lblTotalVal = new System.Windows.Forms.Label();
            this.pnlStatAvail = new System.Windows.Forms.Panel();
            this.lblAvailLabel = new System.Windows.Forms.Label();
            this.lblAvailVal = new System.Windows.Forms.Label();
            this.pnlStatRepair = new System.Windows.Forms.Panel();
            this.lblRepairLabel = new System.Windows.Forms.Label();
            this.lblRepairVal = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).BeginInit();
            this.gridMenu.SuspendLayout();
            this.panelContainer.SuspendLayout();
            this.pnlTopBar.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlStatTotal.SuspendLayout();
            this.pnlStatAvail.SuspendLayout();
            this.pnlStatRepair.SuspendLayout();
            this.SuspendLayout();
            // 
            // guna2BorderlessForm1
            // 
            this.guna2BorderlessForm1.BorderRadius = 16;
            this.guna2BorderlessForm1.ContainerControl = this;
            this.guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            this.guna2BorderlessForm1.HasFormShadow = false;
            this.guna2BorderlessForm1.ShadowColor = System.Drawing.Color.Transparent;
            this.guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // pnlTopBar
            // 
            this.pnlTopBar.BackColor = System.Drawing.Color.White;
            this.pnlTopBar.Controls.Add(this.lblTitle);
            this.pnlTopBar.Controls.Add(this.btnClose);
            this.pnlTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopBar.Location = new System.Drawing.Point(0, 0);
            this.pnlTopBar.Name = "pnlTopBar";
            this.pnlTopBar.Padding = new System.Windows.Forms.Padding(0, 5, 0, 0);
            this.pnlTopBar.Size = new System.Drawing.Size(820, 65);
            this.pnlTopBar.TabIndex = 0;
            // 
            // lblTitle
            // 
            this.lblTitle.AutoSize = true;
            this.lblTitle.Font = new System.Drawing.Font("Segoe UI", 15.5F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblTitle.Location = new System.Drawing.Point(20, 18);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(160, 30);
            this.lblTitle.TabIndex = 1;
            this.lblTitle.Text = "Manage Group";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.BorderRadius = 8;
            this.btnClose.FillColor = System.Drawing.Color.Transparent;
            this.btnClose.HoverState.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(226)))), ((int)(((byte)(232)))), ((int)(((byte)(240)))));
            this.btnClose.IconColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(116)))), ((int)(((byte)(139)))));
            this.btnClose.Location = new System.Drawing.Point(765, 15);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(35, 35);
            this.btnClose.TabIndex = 0;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // pnlHeader
            // 
            this.pnlHeader.BackColor = System.Drawing.Color.White;
            this.pnlHeader.Controls.Add(this.pnlStatTotal);
            this.pnlHeader.Controls.Add(this.pnlStatAvail);
            this.pnlHeader.Controls.Add(this.pnlStatRepair);
            this.pnlHeader.Controls.Add(this.txtSearch);
            this.pnlHeader.Controls.Add(this.btnAddUnit);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Location = new System.Drawing.Point(20, 65);
            this.pnlHeader.Name = "pnlHeader";
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(0, 10, 0, 15);
            this.pnlHeader.Size = new System.Drawing.Size(780, 85);
            this.pnlHeader.TabIndex = 1;
            // 
            // pnlStatTotal
            // 
            this.pnlStatTotal.BackColor = System.Drawing.Color.White;
            this.pnlStatTotal.Controls.Add(this.lblTotalLabel);
            this.pnlStatTotal.Controls.Add(this.lblTotalVal);
            this.pnlStatTotal.Location = new System.Drawing.Point(0, 14);
            this.pnlStatTotal.Name = "pnlStatTotal";
            this.pnlStatTotal.Size = new System.Drawing.Size(75, 52);
            this.pnlStatTotal.TabIndex = 3;
            // 
            // lblTotalLabel
            // 
            this.lblTotalLabel.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.lblTotalLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(163)))), ((int)(((byte)(184)))));
            this.lblTotalLabel.Location = new System.Drawing.Point(0, 6);
            this.lblTotalLabel.Name = "lblTotalLabel";
            this.lblTotalLabel.Size = new System.Drawing.Size(70, 15);
            this.lblTotalLabel.TabIndex = 1;
            this.lblTotalLabel.Text = "TOTAL";
            // 
            // lblTotalVal
            // 
            this.lblTotalVal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblTotalVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblTotalVal.Location = new System.Drawing.Point(0, 21);
            this.lblTotalVal.Name = "lblTotalVal";
            this.lblTotalVal.Size = new System.Drawing.Size(70, 24);
            this.lblTotalVal.TabIndex = 0;
            this.lblTotalVal.Text = "0";
            // 
            // pnlStatAvail
            // 
            this.pnlStatAvail.BackColor = System.Drawing.Color.White;
            this.pnlStatAvail.Controls.Add(this.lblAvailLabel);
            this.pnlStatAvail.Controls.Add(this.lblAvailVal);
            this.pnlStatAvail.Location = new System.Drawing.Point(82, 14);
            this.pnlStatAvail.Name = "pnlStatAvail";
            this.pnlStatAvail.Size = new System.Drawing.Size(90, 52);
            this.pnlStatAvail.TabIndex = 4;
            // 
            // lblAvailLabel
            // 
            this.lblAvailLabel.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.lblAvailLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(22)))), ((int)(((byte)(101)))), ((int)(((byte)(52)))));
            this.lblAvailLabel.Location = new System.Drawing.Point(0, 6);
            this.lblAvailLabel.Name = "lblAvailLabel";
            this.lblAvailLabel.Size = new System.Drawing.Size(85, 15);
            this.lblAvailLabel.TabIndex = 1;
            this.lblAvailLabel.Text = "AVAILABLE";
            // 
            // lblAvailVal
            // 
            this.lblAvailVal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblAvailVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblAvailVal.Location = new System.Drawing.Point(0, 21);
            this.lblAvailVal.Name = "lblAvailVal";
            this.lblAvailVal.Size = new System.Drawing.Size(85, 24);
            this.lblAvailVal.TabIndex = 0;
            this.lblAvailVal.Text = "0";
            // 
            // pnlStatRepair
            // 
            this.pnlStatRepair.BackColor = System.Drawing.Color.White;
            this.pnlStatRepair.Controls.Add(this.lblRepairLabel);
            this.pnlStatRepair.Controls.Add(this.lblRepairVal);
            this.pnlStatRepair.Location = new System.Drawing.Point(179, 14);
            this.pnlStatRepair.Name = "pnlStatRepair";
            this.pnlStatRepair.Size = new System.Drawing.Size(90, 52);
            this.pnlStatRepair.TabIndex = 5;
            // 
            // lblRepairLabel
            // 
            this.lblRepairLabel.Font = new System.Drawing.Font("Segoe UI", 7.5F, System.Drawing.FontStyle.Bold);
            this.lblRepairLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(159)))), ((int)(((byte)(18)))), ((int)(((byte)(57)))));
            this.lblRepairLabel.Location = new System.Drawing.Point(0, 6);
            this.lblRepairLabel.Name = "lblRepairLabel";
            this.lblRepairLabel.Size = new System.Drawing.Size(85, 15);
            this.lblRepairLabel.TabIndex = 1;
            this.lblRepairLabel.Text = "REPAIR";
            // 
            // lblRepairVal
            // 
            this.lblRepairVal.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold);
            this.lblRepairVal.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(15)))), ((int)(((byte)(23)))), ((int)(((byte)(42)))));
            this.lblRepairVal.Location = new System.Drawing.Point(0, 21);
            this.lblRepairVal.Name = "lblRepairVal";
            this.lblRepairVal.Size = new System.Drawing.Size(85, 24);
            this.lblRepairVal.TabIndex = 0;
            this.lblRepairVal.Text = "0";
            // 
            // txtSearch
            // 
            this.txtSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.txtSearch.BorderRadius = 18;
            this.txtSearch.Cursor = System.Windows.Forms.Cursors.IBeam;
            this.txtSearch.DefaultText = "";
            this.txtSearch.Font = new System.Drawing.Font("Segoe UI", 9.5F);
            this.txtSearch.Location = new System.Drawing.Point(400, 20);
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.PlaceholderText = "Search units...";
            this.txtSearch.Size = new System.Drawing.Size(190, 38);
            this.txtSearch.TabIndex = 0;
            this.txtSearch.TextOffset = new System.Drawing.Point(5, 0);
            this.txtSearch.TextChanged += new System.EventHandler(this.txtSearch_TextChanged);
            // 
            // btnAddUnit
            // 
            this.btnAddUnit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddUnit.BorderRadius = 18;
            this.btnAddUnit.Cursor = System.Windows.Forms.Cursors.Hand;
            this.btnAddUnit.FillColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(70)))), ((int)(((byte)(229)))));
            this.btnAddUnit.Font = new System.Drawing.Font("Segoe UI Semibold", 9.5F, System.Drawing.FontStyle.Bold);
            this.btnAddUnit.ForeColor = System.Drawing.Color.White;
            this.btnAddUnit.Location = new System.Drawing.Point(605, 20);
            this.btnAddUnit.Name = "btnAddUnit";
            this.btnAddUnit.Size = new System.Drawing.Size(170, 38); // Made much smaller and balanced
            this.btnAddUnit.TabIndex = 1;
            this.btnAddUnit.Text = "+ Add Units";
            this.btnAddUnit.Click += new System.EventHandler(this.btnAddUnit_Click);
            // 
            // gridItems
            // 
            this.gridItems.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.White;
            this.gridItems.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(71)))), ((int)(((byte)(85)))), ((int)(((byte)(105)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gridItems.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.gridItems.ColumnHeadersHeight = 50;
            this.gridItems.ContextMenuStrip = this.gridMenu;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 10.5F);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(51)))), ((int)(((byte)(65)))), ((int)(((byte)(85)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(242)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(56)))), ((int)(((byte)(202)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gridItems.DefaultCellStyle = dataGridViewCellStyle3;
            this.gridItems.Dock = System.Windows.Forms.DockStyle.Fill;
            this.gridItems.Font = new System.Drawing.Font("Segoe UI", 10F);
            this.gridItems.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(241)))), ((int)(((byte)(245)))), ((int)(((byte)(249)))));
            this.gridItems.Location = new System.Drawing.Point(20, 150);
            this.gridItems.Name = "gridItems";
            this.gridItems.ReadOnly = true;
            this.gridItems.RowHeadersVisible = false;
            this.gridItems.RowTemplate.Height = 55;
            this.gridItems.Size = new System.Drawing.Size(780, 380);
            this.gridItems.TabIndex = 0;
            this.gridItems.ThemeStyle.AlternatingRowsStyle.BackColor = System.Drawing.Color.White;
            this.gridItems.ThemeStyle.BackColor = System.Drawing.Color.White;
            this.gridItems.ThemeStyle.HeaderStyle.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(248)))), ((int)(((byte)(250)))), ((int)(((byte)(252)))));
            this.gridItems.ThemeStyle.HeaderStyle.Height = 50;
            // 
            // gridMenu
            // 
            this.gridMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.markGoodItem,
            this.markDamagedItem,
            this.toolStripSeparator1,
            this.editItem,
            this.deleteItem});
            this.gridMenu.Name = "gridMenu";
            this.gridMenu.RenderStyle.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(238)))), ((int)(((byte)(242)))), ((int)(((byte)(255)))));
            this.gridMenu.RenderStyle.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(67)))), ((int)(((byte)(56)))), ((int)(((byte)(202)))));
            this.gridMenu.Size = new System.Drawing.Size(175, 120);
            // 
            // markGoodItem
            // 
            this.markGoodItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(16)))), ((int)(((byte)(185)))), ((int)(((byte)(129)))));
            this.markGoodItem.Name = "markGoodItem";
            this.markGoodItem.Size = new System.Drawing.Size(174, 24);
            this.markGoodItem.Text = "Mark as Good";
            this.markGoodItem.Click += new System.EventHandler(this.markGoodItem_Click);
            // 
            // markDamagedItem
            // 
            this.markDamagedItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.markDamagedItem.Name = "markDamagedItem";
            this.markDamagedItem.Size = new System.Drawing.Size(174, 24);
            this.markDamagedItem.Text = "Mark as Damaged";
            this.markDamagedItem.Click += new System.EventHandler(this.markDamagedItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(171, 6);
            // 
            // editItem
            // 
            this.editItem.Name = "editItem";
            this.editItem.Size = new System.Drawing.Size(174, 24);
            this.editItem.Text = "Full Edit";
            this.editItem.Click += new System.EventHandler(this.editItem_Click);
            // 
            // deleteItem
            // 
            this.deleteItem.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(239)))), ((int)(((byte)(68)))), ((int)(((byte)(68)))));
            this.deleteItem.Name = "deleteItem";
            this.deleteItem.Size = new System.Drawing.Size(174, 24);
            this.deleteItem.Text = "Delete Item";
            this.deleteItem.Click += new System.EventHandler(this.deleteItem_Click);
            // 
            // panelContainer
            // 
            this.panelContainer.BackColor = System.Drawing.Color.White;
            this.panelContainer.Controls.Add(this.gridItems);
            this.panelContainer.Controls.Add(this.pnlHeader);
            this.panelContainer.Controls.Add(this.pnlTopBar);
            this.panelContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panelContainer.Location = new System.Drawing.Point(0, 0);
            this.panelContainer.Name = "panelContainer";
            this.panelContainer.Padding = new System.Windows.Forms.Padding(20, 0, 20, 20);
            this.panelContainer.Size = new System.Drawing.Size(820, 550);
            this.panelContainer.TabIndex = 1;
            // 
            // ItemGroupPopup
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.White;
            this.ClientSize = new System.Drawing.Size(820, 550);
            this.Controls.Add(this.panelContainer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Name = "ItemGroupPopup";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Manage Item Group";
            this.Load += new System.EventHandler(this.ItemGroupPopup_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gridItems)).EndInit();
            this.gridMenu.ResumeLayout(false);
            this.panelContainer.ResumeLayout(false);
            this.pnlTopBar.ResumeLayout(false);
            this.pnlTopBar.PerformLayout();
            this.pnlHeader.ResumeLayout(false);
            this.pnlStatTotal.ResumeLayout(false);
            this.pnlStatAvail.ResumeLayout(false);
            this.pnlStatRepair.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
        private System.Windows.Forms.Panel panelContainer;
        private System.Windows.Forms.Panel pnlTopBar;
        private System.Windows.Forms.Label lblTitle;
        private Guna.UI2.WinForms.Guna2ControlBox btnClose;
        private System.Windows.Forms.Panel pnlHeader;

        private System.Windows.Forms.Panel pnlStatTotal;
        private System.Windows.Forms.Label lblTotalLabel;
        private System.Windows.Forms.Label lblTotalVal;
        private System.Windows.Forms.Panel pnlStatAvail;
        private System.Windows.Forms.Label lblAvailLabel;
        private System.Windows.Forms.Label lblAvailVal;
        private System.Windows.Forms.Panel pnlStatRepair;
        private System.Windows.Forms.Label lblRepairLabel;
        private System.Windows.Forms.Label lblRepairVal;

        private Guna.UI2.WinForms.Guna2TextBox txtSearch;
        private Guna.UI2.WinForms.Guna2Button btnAddUnit;
        private Guna.UI2.WinForms.Guna2DataGridView gridItems;
        private Guna.UI2.WinForms.Guna2ContextMenuStrip gridMenu;
        private System.Windows.Forms.ToolStripMenuItem markGoodItem;
        private System.Windows.Forms.ToolStripMenuItem markDamagedItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem editItem;
        private System.Windows.Forms.ToolStripMenuItem deleteItem;
    }
}