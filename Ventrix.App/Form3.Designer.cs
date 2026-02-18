namespace Ventrix.App
{
    partial class Form3
    {
        private void InitializeComponent()
        {
<<<<<<< Updated upstream
            // --- 1. INSTANTIATION ---
            this.pnlSidebar = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlTopBar = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlMainContent = new Guna.UI2.WinForms.Guna2Panel();

            this.cardTotal = new Guna.UI2.WinForms.Guna2Panel();
            this.lblTotalCount = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblTotalTitle = new Guna.UI2.WinForms.Guna2HtmlLabel();

            this.btnCreate = new Guna.UI2.WinForms.Guna2Button();
            this.dgvInventory = new Guna.UI2.WinForms.Guna2DataGridView();
            this.txtSearch = new Guna.UI2.WinForms.Guna2TextBox();

            this.SuspendLayout();

            // --- 2. SIDEBAR CONFIGURATION (Matches image_5c7a5e.png) ---
            this.pnlSidebar.Dock = DockStyle.Left;
            this.pnlSidebar.Width = 240;
            this.pnlSidebar.FillColor = Color.FromArgb(13, 71, 161);
            // CRITICAL: Add the button to the Sidebar's controls, not the Form
            this.pnlSidebar.Controls.Add(this.btnCreate);

            // --- 3. DASHBOARD CARD CONFIGURATION ---
            this.cardTotal.FillColor = Color.White;
            this.cardTotal.BorderRadius = 15;
            this.cardTotal.ShadowDecoration.Enabled = true;
            // CRITICAL: Parent the labels to the Card panel
            this.cardTotal.Controls.Add(this.lblTotalCount);
            this.cardTotal.Controls.Add(this.lblTotalTitle);

            // --- 4. TOP BAR & SEARCH ---
            this.pnlTopBar.Dock = DockStyle.Top;
            this.pnlTopBar.Height = 80;
            this.pnlTopBar.FillColor = Color.White;
            this.pnlTopBar.Controls.Add(this.txtSearch);

            // --- 5. MAIN CONTENT ASSEMBLY ---
            this.pnlMainContent.Dock = DockStyle.Fill;
            this.pnlMainContent.Controls.Add(this.cardTotal);
            this.pnlMainContent.Controls.Add(this.dgvInventory);

            // --- 6. FINAL Z-ORDER (Order prevents image_5c0183.png overlap) ---
            this.Controls.Add(this.pnlMainContent);
=======
            this.components = new System.ComponentModel.Container();

            // --- 1. INSTANTIATION (Creates the objects in memory) ---
            this.pnlTopBar = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlSidebar = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlMainContent = new Guna.UI2.WinForms.Guna2Panel();
            this.pnlGridContainer = new Guna.UI2.WinForms.Guna2Panel();

            this.cardTotal = new Guna.UI2.WinForms.Guna2Panel();
            this.cardAvailable = new Guna.UI2.WinForms.Guna2Panel();
            this.cardPending = new Guna.UI2.WinForms.Guna2Panel();
            this.cardBorrowers = new Guna.UI2.WinForms.Guna2Panel();

            this.lblTotalCount = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblAvailCount = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblPendingCount = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.lblBorrowersCount = new Guna.UI2.WinForms.Guna2HtmlLabel();

            this.dgvInventory = new Guna.UI2.WinForms.Guna2DataGridView();
            this.lblDashboardHeader = new Guna.UI2.WinForms.Guna2HtmlLabel();
            this.btnCreate = new Guna.UI2.WinForms.Guna2Button();
            this.btnEdit = new Guna.UI2.WinForms.Guna2Button();
            this.btnDelete = new Guna.UI2.WinForms.Guna2Button();
            this.picUser = new Guna.UI2.WinForms.Guna2CirclePictureBox();
            this.lblOwnerRole = new Guna.UI2.WinForms.Guna2HtmlLabel();

            this.SuspendLayout();

            // --- 2. PARENTING FIX: SIDEBAR (Fixes missing buttons) ---
            this.pnlSidebar.Controls.Add(this.btnCreate);
            this.pnlSidebar.Controls.Add(this.btnEdit);
            this.pnlSidebar.Controls.Add(this.btnDelete);
            this.pnlSidebar.Controls.Add(this.picUser);
            this.pnlSidebar.Controls.Add(this.lblOwnerRole);
            this.pnlSidebar.Dock = System.Windows.Forms.DockStyle.Left;
            this.pnlSidebar.Width = 240;
            this.pnlSidebar.FillColor = System.Drawing.Color.FromArgb(13, 71, 161);

            // --- 3. PARENTING FIX: CARDS (Fixes floating counts) ---
            this.cardTotal.Controls.Add(this.lblTotalCount);
            this.cardAvailable.Controls.Add(this.lblAvailCount);
            this.cardPending.Controls.Add(this.lblPendingCount);
            this.cardBorrowers.Controls.Add(this.lblBorrowersCount);
            this.cardTotal.FillColor = System.Drawing.Color.White;
            this.cardAvailable.FillColor = System.Drawing.Color.White;
            this.cardPending.FillColor = System.Drawing.Color.White;
            this.cardBorrowers.FillColor = System.Drawing.Color.White;

            // --- 4. Z-ORDER FIX: MAIN CONTENT ---
            this.pnlMainContent.Controls.Add(this.cardTotal);
            this.pnlMainContent.Controls.Add(this.cardAvailable);
            this.pnlMainContent.Controls.Add(this.cardPending);
            this.pnlMainContent.Controls.Add(this.cardBorrowers);
            this.pnlMainContent.Controls.Add(this.pnlGridContainer);
            this.pnlGridContainer.Controls.Add(this.dgvInventory);
            this.pnlMainContent.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlMainContent.BackColor = System.Drawing.Color.FromArgb(242, 245, 250);

            // Top Bar
            this.pnlTopBar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlTopBar.Height = 80;
            this.pnlTopBar.FillColor = System.Drawing.Color.White;
            this.pnlTopBar.Controls.Add(this.lblDashboardHeader);

            // --- 5. FINAL FORM ASSEMBLY (Order is crucial for docking) ---
            this.ClientSize = new System.Drawing.Size(1280, 850);
            this.Controls.Add(this.pnlMainContent); // Fill added FIRST
>>>>>>> Stashed changes
            this.Controls.Add(this.pnlSidebar);
            this.Controls.Add(this.pnlTopBar);

            this.ResumeLayout(false);
        }

<<<<<<< Updated upstream
        // Field declarations...
        private Guna.UI2.WinForms.Guna2Panel pnlSidebar, pnlTopBar, pnlMainContent, cardTotal;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTotalCount, lblTotalTitle;
        private Guna.UI2.WinForms.Guna2Button btnCreate;
        private Guna.UI2.WinForms.Guna2DataGridView dgvInventory;
        private Guna.UI2.WinForms.Guna2TextBox txtSearch;
=======
        private Guna.UI2.WinForms.Guna2Panel pnlSidebar, pnlTopBar, pnlMainContent, pnlGridContainer;
        private Guna.UI2.WinForms.Guna2Panel cardTotal, cardAvailable, cardPending, cardBorrowers;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTotalCount, lblAvailCount, lblPendingCount, lblBorrowersCount, lblDashboardHeader, lblOwnerRole;
        private Guna.UI2.WinForms.Guna2DataGridView dgvInventory;
        private Guna.UI2.WinForms.Guna2Button btnCreate, btnEdit, btnDelete;
        private Guna.UI2.WinForms.Guna2CirclePictureBox picUser;
>>>>>>> Stashed changes
    }
}