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

        public Form3()
        {
            InitializeComponent();
            ApplyMaterialTheme();
            SetupNavigation();
            LoadData();
        }

        private void ApplyMaterialTheme()
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Primary.BlueGrey800, Primary.BlueGrey900,
                Primary.BlueGrey500, Accent.LightBlue200,
                TextShade.WHITE
            );
        }

        private void SetupNavigation()
        {
            btnMenuToggle.Click += (s, e) => {
                isSidebarExpanded = !isSidebarExpanded;
                pnlSidebar.Width = isSidebarExpanded ? 260 : 70;
                btnDashboard.Text = isSidebarExpanded ? "      Dashboard" : "";
                btnInventory.Text = isSidebarExpanded ? "      Inventory" : "";
                btnDashboard.ImageAlign = isSidebarExpanded ? HorizontalAlignment.Left : HorizontalAlignment.Center;
                btnInventory.ImageAlign = isSidebarExpanded ? HorizontalAlignment.Left : HorizontalAlignment.Center;
            };

            btnDashboard.Click += (s, e) => {
                tcMain.SelectedTab = tpDashboard;
                lblPageTitle.Text = "DASHBOARD OVERVIEW";
            };

            btnInventory.Click += (s, e) => {
                tcMain.SelectedTab = tpInventory;
                lblPageTitle.Text = "INVENTORY MANAGEMENT";
            };
        }

        private void LoadData()
        {
            // Updated sample data to include Date Borrowed and Return Date
            dgvInventory.Rows.Add("John Doe", "2023-0001", "Projector", "Feb 10, 2026", "Feb 15, 2026", "Borrowed");
            dgvInventory.Rows.Add("Jane Smith", "2023-0042", "Laptop Charger", "Feb 12, 2026", "Feb 14, 2026", "Returned");
        }
    }
}