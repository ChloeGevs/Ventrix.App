using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ventrix.App
{
    public partial class Form3 : MaterialForm
    {
        private bool isSidebarExpanded = true;
        private const int sidebarMaxWidth = 240;
        private const int sidebarMinWidth = 70;

        public Form3()
        {
            InitializeComponent();
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;

            materialSkinManager.ColorScheme = new ColorScheme(
                Color.FromArgb(13, 71, 161), Color.FromArgb(10, 50, 120),
                Color.FromArgb(33, 150, 243), Color.FromArgb(30, 136, 229),
                TextShade.WHITE
            );

            // FIX: Initialize columns before LoadFilteredData is ever called
            InitializeInventoryGrid();

            // Card Click Events
            cardTotal.Click += (s, e) => LoadFilteredData("All");
            cardAvailable.Click += (s, e) => LoadFilteredData("Available");
            cardPending.Click += (s, e) => LoadFilteredData("Borrowed");

            // Sidebar Events
            sidebarTimer.Interval = 1;
            btnHamburger.Click += (s, e) => sidebarTimer.Start();
            sidebarTimer.Tick += SidebarTimer_Tick;

            this.Load += (s, e) => { ApplyModernBranding(); RefreshLayout(); LoadFilteredData("All"); };
            this.Resize += (s, e) => RefreshLayout();
        }

        private void InitializeInventoryGrid()
        {
            // Explicitly defining columns to prevent the "No columns" exception
            dgvInventory.Columns.Clear();
            dgvInventory.Columns.Add("ID", "ID");
            dgvInventory.Columns.Add("Name", "Material Name");
            dgvInventory.Columns.Add("Status", "Status");
            dgvInventory.Columns.Add("Borrowed", "Date Borrowed");
            dgvInventory.Columns.Add("Return", "Return Date");

            dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvInventory.AllowUserToAddRows = false;
        }

        private void ApplyModernBranding()
        {
            lblDashboardHeader.Text = "INVENTORY OVERVIEW";
            lblDashboardHeader.Font = new Font("Sitka Heading", 22F, FontStyle.Bold);
            lblDashboardHeader.ForeColor = Color.FromArgb(13, 71, 161);

            SetupCard(cardTotal, lblTotalTitle, lblTotalCount, "TOTAL ITEMS", "1,240", Color.FromArgb(13, 71, 161));
            SetupCard(cardAvailable, lblAvailTitle, lblAvailCount, "AVAILABLE", "856", Color.Teal);
            SetupCard(cardPending, lblPendingTitle, lblPendingCount, "BORROWED", "384", Color.FromArgb(192, 0, 0));

            StyleNavButton(btnCreate, "ADD ITEM", Color.Teal);
            StyleNavButton(btnEdit, "EDIT RECORD", Color.FromArgb(33, 150, 243));
            StyleNavButton(btnDelete, "DELETE ITEM", Color.FromArgb(192, 0, 0));
        }

        private void SetupCard(Guna.UI2.WinForms.Guna2Panel card, Guna.UI2.WinForms.Guna2HtmlLabel title, Guna.UI2.WinForms.Guna2HtmlLabel count, string titleText, string countText, Color accentColor)
        {
            // 1. Establish Parent-Child Relationship
            // This ensures labels move and display relative to the card, not the form
            title.Parent = card;
            count.Parent = card;

            // 2. Configure Card Visuals
            card.FillColor = Color.White;
            card.BorderRadius = 15;
            card.ShadowDecoration.Enabled = true;
            card.ShadowDecoration.Shadow = new Padding(10);
            card.Cursor = Cursors.Hand;

            // 3. Style Title Label
            title.Text = titleText;
            title.Font = new Font("Segoe UI Semibold", 9F);
            title.ForeColor = Color.Gray;
            title.BackColor = Color.Transparent; // Important: Prevents a white box over the card

            // 4. Style Count Label
            count.Text = countText;
            count.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            count.ForeColor = accentColor;
            count.BackColor = Color.Transparent;

            // 5. Force Z-Order
            // This prevents the card background from covering the text
            title.BringToFront();
            count.BringToFront();

            // 6. Interactive Glow Logic (Corrected Event Handlers)
            card.MouseDown += (s, e) => {
                card.ShadowDecoration.Color = accentColor;
                card.ShadowDecoration.Shadow = new Padding(15);
                card.BorderThickness = 2;
                card.BorderColor = accentColor;
            };

            Action resetGlow = () => {
                card.ShadowDecoration.Color = Color.FromArgb(50, Color.Black);
                card.ShadowDecoration.Shadow = new Padding(8);
                card.BorderThickness = 0;
            };

            card.MouseUp += (s, e) => resetGlow();
            card.MouseLeave += (s, e) => resetGlow();
        }

        private void LoadFilteredData(string status)
        {
            // Safety Check: Do not attempt to add rows if columns haven't been created yet
            if (dgvInventory.Columns.Count == 0) return;

            dgvInventory.Rows.Clear();
            if (status == "All" || status == "Available")
            {
                dgvInventory.Rows.Add("101", "Projector A", "Available", "-", "-");
                dgvInventory.Rows.Add("102", "HDMI Cable", "Available", "-", "-");
            }
            if (status == "All" || status == "Borrowed")
            {
                dgvInventory.Rows.Add("201", "Laptop Dell #4", "Borrowed", "2026-02-14", "2026-02-17");
                dgvInventory.Rows.Add("205", "Digital Camera", "Borrowed", "2026-02-15", "2026-02-16");
            }
            lblDashboardHeader.Text = $"INVENTORY: {status.ToUpper()}";
        }

        private void StyleNavButton(Guna.UI2.WinForms.Guna2Button btn, string text, Color hover)
        {
            btn.Text = text;
            btn.Font = new Font("Sitka Banner", 11F, FontStyle.Bold);
            btn.FillColor = Color.Transparent;
            btn.HoverState.FillColor = hover;
            btn.TextAlign = HorizontalAlignment.Left;
            btn.TextOffset = new Point(10, 0);
            btn.BorderRadius = 10;
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            int animationSpeed = 40;
            if (isSidebarExpanded)
            {
                pnlSidebar.Width -= animationSpeed;
                if (pnlSidebar.Width <= sidebarMinWidth)
                {
                    pnlSidebar.Width = sidebarMinWidth;
                    isSidebarExpanded = false;
                    sidebarTimer.Stop();
                    ToggleSidebarText(false);
                }
            }
            else
            {
                pnlSidebar.Width += animationSpeed;
                if (pnlSidebar.Width >= sidebarMaxWidth)
                {
                    pnlSidebar.Width = sidebarMaxWidth;
                    isSidebarExpanded = true;
                    sidebarTimer.Stop();
                    ToggleSidebarText(true);
                }
            }
            RefreshLayout();
        }

        private void ToggleSidebarText(bool show)
        {
            btnCreate.Text = show ? "ADD ITEM" : "";
            btnEdit.Text = show ? "EDIT RECORD" : "";
            btnDelete.Text = show ? "DELETE ITEM" : "";
        }

        private void RefreshLayout()
        {
            if (pnlMainContent == null) return;

            lblDashboardHeader.Location = new Point(70, 20);
            txtSearch.Location = new Point(pnlTopBar.Width - txtSearch.Width - 30, 20);

            int spacing = 20;
            int cardWidth = (pnlMainContent.Width - 100) / 3;
            cardTotal.Size = cardAvailable.Size = cardPending.Size = new Size(cardWidth, 130);

            cardTotal.Location = new Point(30, 30);
            cardAvailable.Location = new Point(cardTotal.Right + spacing, 30);
            cardPending.Location = new Point(cardAvailable.Right + spacing, 30);

            lblTotalTitle.Location = lblAvailTitle.Location = lblPendingTitle.Location = new Point(15, 15);
            lblTotalCount.Location = lblAvailCount.Location = lblPendingCount.Location = new Point(15, 45);

            pnlGridContainer.Location = new Point(30, 180);
            pnlGridContainer.Width = pnlMainContent.Width - 60;
            pnlGridContainer.Height = pnlMainContent.Height - 210;

            int btnWidth = pnlSidebar.Width - 20;
            btnCreate.Size = btnEdit.Size = btnDelete.Size = new Size(btnWidth, 50);
            btnCreate.Location = new Point(10, 120);
            btnEdit.Location = new Point(10, 180);
            btnDelete.Location = new Point(10, 240);
        }
    }
}