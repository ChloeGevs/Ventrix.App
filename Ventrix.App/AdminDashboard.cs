using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Ventrix.App.Controls;
using Ventrix.App.Popups;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using Ventrix.Domain.Enums;
using System.Threading.Tasks;

namespace Ventrix.App
{
    public partial class AdminDashboard : MaterialForm
    {
        private readonly InventoryService _inventoryService;
        private readonly BorrowService _borrowService;

        private bool isSidebarExpanded = true;
        private const int sidebarMaxWidth = 240;
        private const int sidebarMinWidth = 70;

        public AdminDashboard(InventoryService inventoryService, BorrowService borrowService)
        {
            _inventoryService = inventoryService;
            _borrowService = borrowService;

            InitializeComponent();

            ThemeManager.Initialize(this);
            InitializeMaterialSkin();
            ConfigureRuntimeUI();

            Shown += async (s, e) => {
                RefreshLayout();
               // await _inventoryService.SeedInitialInventoryAsync();
                await SwitchView("Home");
            };
        }

        private void ConfigureRuntimeUI()
        {
            // Smooth Graphics
            typeof(Panel).InvokeMember("DoubleBuffered",
                System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                null, pnlMainContent, new object[] { true });

            btnCreate.Click += async (s, e) => await BtnCreate_Click(s, e);
            btnEdit.Click += async (s, e) => await BtnEdit_Click(s, e);
            btnDelete.Click += async (s, e) => await BtnDelete_Click(s, e);
            btnHome.Click += async (s, e) => await SwitchView("Home");
            btnHistoryNav.Click += async (s, e) => await SwitchView("History");
            txtSearch.TextChanged += async (s, e) => await LoadFromDatabase("All");

            sidebarTimer.Interval = 1;
            btnHamburger.Click += (s, e) => sidebarTimer.Start();
            sidebarTimer.Tick += SidebarTimer_Tick;

            this.Resize += (s, e) => { if (this.WindowState != FormWindowState.Minimized) RefreshLayout(); };
        }

        #region THE LAYOUT FIX
        private void RefreshLayout()
        {
            if (pnlMainContent == null || pnlSidebar == null) return;

            // 1. NUKE the Designer's Docking settings so they stop fighting our code
            pnlSidebar.Dock = DockStyle.None;
            pnlMainContent.Dock = DockStyle.None;

            // 2. Lock the panels in place side-by-side
            pnlSidebar.Location = new Point(0, 64);
            pnlSidebar.Height = this.Height - 64;
            pnlSidebar.BringToFront();

            pnlMainContent.Location = new Point(pnlSidebar.Width, 64);
            pnlMainContent.Size = new Size(this.Width - pnlSidebar.Width, this.Height - 64);

            // 3. Define the internal container bounds
            int margin = 20;
            Rectangle contentArea = new Rectangle(margin, 40, pnlMainContent.Width - (margin * 2), pnlMainContent.Height - 100);

            if (pnlHomeSummary != null) pnlHomeSummary.Bounds = contentArea;
            if (pnlGridContainer != null) pnlGridContainer.Bounds = contentArea;
            if (pnlHistory != null) pnlHistory.Bounds = contentArea;

            // 4. Force Grids to fill their areas properly
            if (dgvInventory != null) dgvInventory.Dock = DockStyle.Fill;
            if (dgvHistory != null) dgvHistory.Dock = DockStyle.Fill;

            UpdateSidebarInternalUI();
        }

        private void UpdateSidebarInternalUI()
        {
            // This spaces out the 4 metric cards so they don't stack on top of each other!
            int cardY = 200;
            var cards = new[] { cardTotal, cardAvailable, cardPending, cardBorrowers };

            foreach (var card in cards)
            {
                if (card != null)
                {
                    card.SetBounds(10, cardY, pnlSidebar.Width - 20, 110);
                    cardY += 120;
                }
            }
        }
        #endregion

        #region Navigation & Data Logic
        private async Task SwitchView(string viewName, string filter = "All")
        {
            if (pnlHomeSummary != null) pnlHomeSummary.Visible = (viewName == "Home");
            if (pnlGridContainer != null) pnlGridContainer.Visible = (viewName == "Inventory");
            if (pnlHistory != null) pnlHistory.Visible = (viewName == "History");

            if (viewName == "Home") await LoadHomeContent();
            else if (viewName == "Inventory") await LoadFromDatabase(filter);

            await UpdateDashboardCounts();
        }

        private async Task LoadFromDatabase(string statusFilter)
        {
            if (dgvInventory == null) return;

            dgvInventory.Rows.Clear();
            dgvInventory.Columns.Clear();
            var items = await _inventoryService.GetAllItemsAsync();

            SetupColumns("ID", "Item Name", "Category", "Status", "Condition");

            foreach (var i in items)
            {
                dgvInventory.Rows.Add(i.Id, i.Name, i.Category.ToString(), i.Status.ToString(), i.Condition.ToString());
            }
        }

        private async Task LoadHomeContent()
        {
            var items = await _inventoryService.GetAllItemsAsync();
            int damagedCount = items.Count(x => x.Condition == Condition.Damaged);
            if (lblUrgentHeader != null)
            {
                lblUrgentHeader.Text = damagedCount > 0 ? $"URGENT ({damagedCount})" : "STABLE";
            }
        }

        private async Task UpdateDashboardCounts()
        {
            var items = (await _inventoryService.GetAllItemsAsync()).ToList();
            var records = (await _borrowService.GetAllBorrowRecordsAsync()).ToList();

            if (cardTotal != null) cardTotal.UpdateMetrics("TOTAL", items.Count.ToString(), Color.FromArgb(13, 71, 161));
            if (cardAvailable != null) cardAvailable.UpdateMetrics("AVAILABLE", items.Count(x => x.Status == ItemStatus.Available).ToString(), Color.Teal);
            if (cardPending != null) cardPending.UpdateMetrics("BORROWED", items.Count(x => x.Status == ItemStatus.Borrowed).ToString(), Color.Maroon);
            if (cardBorrowers != null) cardBorrowers.UpdateMetrics("RECORDS", records.Count.ToString(), Color.Orange);
        }
        #endregion

        #region CRUD Actions
        private async Task BtnCreate_Click(object sender, EventArgs e)
        {
            using (var p = new InventoryPopup(_inventoryService))
                if (p.ShowDialog() == DialogResult.OK) await LoadFromDatabase("All");
        }

        private async Task BtnEdit_Click(object sender, EventArgs e)
        {
            if (dgvInventory.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgvInventory.SelectedRows[0].Cells[0].Value);
            using (var p = new InventoryPopup(_inventoryService, id))
                if (p.ShowDialog() == DialogResult.OK) await LoadFromDatabase("All");
        }

        private async Task BtnDelete_Click(object sender, EventArgs e)
        {
            if (dgvInventory.SelectedRows.Count == 0) return;
            int id = Convert.ToInt32(dgvInventory.SelectedRows[0].Cells[0].Value);
            await _inventoryService.DeleteItemAsync(id);
            await LoadFromDatabase("All");
        }
        #endregion

        #region Setup Helpers
        private void InitializeMaterialSkin()
        {
            var manager = MaterialSkinManager.Instance;
            manager.AddFormToManage(this);
            manager.Theme = MaterialSkinManager.Themes.LIGHT;
            manager.ColorScheme = new ColorScheme(Color.FromArgb(13, 71, 161), Color.FromArgb(10, 50, 120), Color.FromArgb(33, 150, 243), Color.FromArgb(30, 136, 229), TextShade.WHITE);
        }

        private void SetupColumns(params string[] names)
        {
            if (dgvInventory == null) return;
            dgvInventory.Columns.Clear();
            foreach (var n in names) dgvInventory.Columns.Add(n.Replace(" ", ""), n);
            dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            if (isSidebarExpanded) { pnlSidebar.Width -= 40; if (pnlSidebar.Width <= sidebarMinWidth) { pnlSidebar.Width = sidebarMinWidth; isSidebarExpanded = false; sidebarTimer.Stop(); } }
            else { pnlSidebar.Width += 40; if (pnlSidebar.Width >= sidebarMaxWidth) { pnlSidebar.Width = sidebarMaxWidth; isSidebarExpanded = true; sidebarTimer.Stop(); } }
            RefreshLayout();
        }
        #endregion
    }
}