using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using MaterialSkin.Controls;
using Ventrix.Infrastructure;

namespace Ventrix.App
{
    public partial class Form3 : MaterialForm
    {
        public Form3()
        {
            // Connects to the Designer partial class
            InitializeComponent();

<<<<<<< Updated upstream
            // Ensures the lab database file is created safely
            using (var db = new AppDbContext()) { db.Database.EnsureCreated(); }

            this.Load += (s, e) => {
                RefreshLayout();
                UpdateDashboardStats();
=======
            // Ensures the lab database file exists
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
            }

            // Wait for the UI to be ready before running math
            this.Load += (s, e) => {
                RefreshLayout();
                UpdateDashboardStats();
                LoadFilteredData("All");
>>>>>>> Stashed changes
            };
            this.Resize += (s, e) => RefreshLayout();
        }

        private void UpdateDashboardStats()
        {
            using (var db = new AppDbContext())
            {
<<<<<<< Updated upstream
                // Update the dashboard with real laboratory stats
                int total = db.InventoryItems.Count();
                lblTotalCount.Text = total.ToString("N0");
                lblTotalCount.ForeColor = Color.FromArgb(13, 71, 161);
=======
                // Pulls real hardware stats
                lblTotalCount.Text = db.InventoryItems.Count().ToString("N0");
                lblAvailCount.Text = db.InventoryItems.Count(i => i.Status == "Available").ToString("N0");
                lblPendingCount.Text = db.BorrowRecords.Count(r => r.ReturnDate == null).ToString("N0");
                lblBorrowersCount.Text = db.BorrowRecords.Where(r => r.ReturnDate == null)
                                           .Select(r => r.BorrowerId).Distinct().Count().ToString("N0");
>>>>>>> Stashed changes
            }
        }

        public void RefreshLayout()
        {
            if (pnlMainContent == null) return;

<<<<<<< Updated upstream
            // Positioning the Sidebar Button (Matches image_5c7a5e.png)
            btnCreate.Size = new Size(pnlSidebar.Width - 30, 45);
            btnCreate.Location = new Point(15, 180);
            btnCreate.Text = "ADD ITEM";

            // Positioning the Card and Labels
            cardTotal.Size = new Size(250, 150);
            cardTotal.Location = new Point(30, 30);

            lblTotalTitle.Text = "TOTAL ITEMS";
            lblTotalTitle.Location = new Point(15, 15);

            lblTotalCount.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblTotalCount.Location = new Point(15, 50);

            // Positioning the DataGrid
            dgvInventory.Location = new Point(30, 200);
            dgvInventory.Size = new Size(pnlMainContent.Width - 60, pnlMainContent.Height - 250);
=======
            // 1. Sidebar Buttons Positioning (Fixes image_5c1828.png)
            int btnWidth = pnlSidebar.Width - 40;
            int startY = 150;
            btnCreate.Size = new Size(btnWidth, 45);
            btnCreate.Location = new Point(20, startY);
            btnEdit.Size = new Size(btnWidth, 45);
            btnEdit.Location = new Point(20, btnCreate.Bottom + 10);
            btnDelete.Size = new Size(btnWidth, 45);
            btnDelete.Location = new Point(20, btnEdit.Bottom + 10);

            // 2. Card Math Floor (Prevents 'slivers' in image_5c0582.png)
            int currentWidth = Math.Max(pnlMainContent.Width, 1000);
            int spacing = 20;
            int cardWidth = (currentWidth - (spacing * 5)) / 4;
            Size cardSize = new Size(cardWidth, 130);

            cardTotal.Size = cardAvailable.Size = cardPending.Size = cardBorrowers.Size = cardSize;
            cardTotal.Location = new Point(spacing, 30);
            cardAvailable.Location = new Point(cardTotal.Right + spacing, 30);
            cardPending.Location = new Point(cardAvailable.Right + spacing, 30);
            cardBorrowers.Location = new Point(cardPending.Right + spacing, 30);

            // 3. Label Parenting (Relative to the card)
            Point labelPos = new Point(15, 45);
            lblTotalCount.Location = lblAvailCount.Location = labelPos;

            pnlGridContainer.Location = new Point(spacing, 180);
            pnlGridContainer.Width = pnlMainContent.Width - (spacing * 2);
            pnlGridContainer.Height = pnlMainContent.Height - 210;
        }

        private void LoadFilteredData(string status)
        {
            dgvInventory.Rows.Clear();
            dgvInventory.Columns.Clear();
            dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;

            using (var db = new AppDbContext())
            {
                if (status == "All")
                {
                    dgvInventory.Columns.Add("Id", "ID");
                    dgvInventory.Columns.Add("Name", "Item Name");
                    dgvInventory.Columns.Add("Status", "Status");
                    foreach (var item in db.InventoryItems.ToList())
                        dgvInventory.Rows.Add(item.Id, item.Name, item.Status);
                }
            }
>>>>>>> Stashed changes
        }
    }
}