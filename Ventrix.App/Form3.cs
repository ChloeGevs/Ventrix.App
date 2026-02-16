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

            // Event Handlers
            btnHamburger.Click += (s, e) => sidebarTimer.Start();
            sidebarTimer.Tick += SidebarTimer_Tick;

            this.Load += (s, e) => { ApplyModernBranding(); RefreshLayout(); };
            this.Resize += (s, e) => RefreshLayout();
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            if (isSidebarExpanded)
            {
                pnlSidebar.Width -= 15;
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
                pnlSidebar.Width += 15;
                if (pnlSidebar.Width >= sidebarMaxWidth)
                {
                    pnlSidebar.Width = sidebarMaxWidth;
                    isSidebarExpanded = true;
                    sidebarTimer.Stop();
                    ToggleSidebarText(true);
                }
            }
            RefreshLayout(); // Ensure main content updates during animation
        }

        private void ToggleSidebarText(bool show)
        {
            string addText = show ? "ADD ITEM" : "";
            string editText = show ? "EDIT RECORD" : "";
            string delText = show ? "DELETE ITEM" : "";

            btnCreate.Text = addText;
            btnEdit.Text = editText;
            btnDelete.Text = delText;

            // Center icons when retracted
            btnCreate.ImageAlign = show ? HorizontalAlignment.Left : HorizontalAlignment.Center;
            btnCreate.TextOffset = show ? new Point(10, 0) : new Point(0, 0);
        }

        private void ApplyModernBranding()
        {
            lblDashboardHeader.Text = "INVENTORY OVERVIEW";
            lblDashboardHeader.Font = new Font("Sitka Heading", 22F, FontStyle.Bold);
            lblDashboardHeader.ForeColor = Color.FromArgb(13, 71, 161);

            // Button Styles
            StyleNavButton(btnCreate, "ADD ITEM", Color.Teal);
            StyleNavButton(btnEdit, "EDIT RECORD", Color.FromArgb(33, 150, 243));
            StyleNavButton(btnDelete, "DELETE ITEM", Color.FromArgb(192, 0, 0));
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

        private void RefreshLayout()
        {
            if (pnlMainContent == null) return;

            // Align TopBar items
            lblDashboardHeader.Location = new Point(70, 20);
            txtSearch.Location = new Point(pnlTopBar.Width - txtSearch.Width - 30, 20);

            // Stat Cards
            int spacing = 20;
            int cardWidth = (pnlMainContent.Width - 100) / 3;
            cardTotal.Size = cardAvailable.Size = cardPending.Size = new Size(cardWidth, 120);

            cardTotal.Location = new Point(30, 30);
            cardAvailable.Location = new Point(cardTotal.Right + spacing, 30);
            cardPending.Location = new Point(cardAvailable.Right + spacing, 30);

            // Grid positioning
            pnlGridContainer.Location = new Point(30, 170);
            pnlGridContainer.Width = pnlMainContent.Width - 60;
            pnlGridContainer.Height = pnlMainContent.Height - 200;

            // Sidebar Buttons
            int btnWidth = pnlSidebar.Width - 20;
            btnCreate.Size = btnEdit.Size = btnDelete.Size = new Size(btnWidth, 50);
            btnCreate.Location = new Point(10, 120);
            btnEdit.Location = new Point(10, 180);
            btnDelete.Location = new Point(10, 240);
        }
    }
}