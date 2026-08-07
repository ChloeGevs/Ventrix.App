using ClosedXML.Excel;
using Guna.UI2.WinForms;
using iTextSharp.text;
using iTextSharp.text.pdf;
using MaterialSkin;
using MaterialSkin.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventrix.App.Controls;
using Ventrix.App.Popups;
using Ventrix.Application.DTOs;
using Ventrix.Application.Services;
using Ventrix.Domain.Enums;
using Ventrix.Domain.Models;
using DrawColor = System.Drawing.Color;
using DrawFont = System.Drawing.Font;
using DrawPoint = System.Drawing.Point;
using DrawRect = System.Drawing.Rectangle;
using DrawSize = System.Drawing.Size;

namespace Ventrix.App
{
    public partial class AdminDashboard : MaterialForm
    {
        private readonly InventoryService _inventoryService;
        private readonly BorrowService _borrowService;
        private readonly UserService _userService;

        // ── Second-screen tablet support ─────────────────────────────────────
        /// <summary>
        /// Reference to the Borrower Portal that launched this dashboard.
        /// When set, sign-out will restore that portal (which may be running on a
        /// tablet screen) instead of creating a fresh instance.
        /// </summary>
        private readonly BorrowerPortal? _borrowerPortal;

        private bool isSigningOut = false;
        private bool isSidebarExpanded = true;
        private const int sidebarMaxWidth = 240;
        private const int sidebarMinWidth = 70;
        private Guna.UI2.WinForms.Guna2Button btnFilterDates;
        private int historyCurrentPage = 1;
        private int historyTotalPages = 1;
        private const int historyPageSize = 100;
        private Guna2Transition viewTransition;
        private Panel loadingOverlay;
        private Label lblDateSeparator;


        private string historySortColumn = "Borrower";
        private bool historySortDescending = false;

        private Guna.UI2.WinForms.Guna2TextBox txtRegSchoolId;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpStartDate;
        private Guna.UI2.WinForms.Guna2DateTimePicker dtpEndDate;
        private Guna.UI2.WinForms.Guna2Button btnApplyFilters;
        private Guna.UI2.WinForms.Guna2Button btnPrevPage;
        private Guna.UI2.WinForms.Guna2Button btnNextPage;
        private Guna.UI2.WinForms.Guna2Button btnActionsMenu;
        private ContextMenuStrip inventoryActionsMenu;
        private Label lblPageInfo;
        private readonly BorrowerPortal? _borrowerPortalInstance;

        public AdminDashboard(InventoryService inventoryService, BorrowService borrowService, UserService userService, BorrowerPortal? borrowerPortal = null)
        {
            _inventoryService = inventoryService;
            _borrowService = borrowService;
            _userService = userService;
            _borrowerPortal = borrowerPortal;

            InitializeComponent();
            SetupHistoryAdvancedControls();
            ThemeManager.Initialize(this);
            ThemeManager.FixTransparency(this);
            // 2. Style your sidebar navigation buttons so they blend into the blue
            // (Replace these names with your actual sidebar button names)
            ThemeManager.StyleSidebarButton(btnHome);
            ThemeManager.StyleSidebarButton(btnHistoryNav);
            ThemeManager.StyleSidebarButton(btnNavAllItems); 
            ThemeManager.StyleSidebarButton(btnNavAvailable);
            ThemeManager.StyleSidebarButton(btnNavBorrowed);
            ThemeManager.StyleSidebarButton(btnNavBorrowers);

            ThemeManager.StyleDataGrid(dgvInventory);
            ThemeManager.StyleDataGrid(dgvHistory);

            ThemeManager.StyleTextBox(txtSearch);

            ThemeManager.StyleCard(cardTotal);
            ThemeManager.StyleCard(cardAvailable);
            ThemeManager.StyleCard(cardRecords);
            ThemeManager.StyleCard(cardBorrowed);

            // 4. Modernize the 'Clear All' button
            ThemeManager.StyleActionButton(btnClearActivity, Color.FromArgb(224, 224, 224), Color.Silver);
            btnClearActivity.ForeColor = Color.Black; // Since it's a light button

            // 5. Style Action Buttons (Excel, PDF, Create, Edit, Delete, etc.)
            Color successGreen = Color.FromArgb(46, 125, 50);   // Modern green for Excel
            Color dangerRed = Color.FromArgb(198, 40, 40);      // Modern red for Delete/PDF
            Color neutralBlue = Color.FromArgb(21, 101, 192);   // Ventrix blue for standard actions

            ThemeManager.StyleActionButton(btnExportExcel, successGreen, Color.FromArgb(56, 142, 60));
            ThemeManager.StyleActionButton(btnExportPDF, dangerRed, Color.FromArgb(211, 47, 47));
            ThemeManager.StyleActionButton(btnCreate, neutralBlue, Color.FromArgb(30, 136, 229));
            ThemeManager.StyleActionButton(btnEdit, neutralBlue, Color.FromArgb(30, 136, 229));
            ThemeManager.StyleActionButton(btnDelete, dangerRed, Color.FromArgb(211, 47, 47));
            ThemeManager.StyleActionButton(btnClearActivity, Color.FromArgb(224, 224, 224), Color.Silver);
            btnClearActivity.ForeColor = Color.Black; // Dark text for the light clear button

            isSidebarExpanded = true;

            ConfigureRuntimeUI();
            ApplyModernBranding();
            StyleDataGrids();

            Shown += async (s, e) => {

                int newOverdueItems = await _borrowService.ProcessAutomaticOverdueStrikesAsync(_userService);

                if (newOverdueItems > 0)
                {
                    // Notify the admin if the system caught people who didn't return items
                    ToastNotification.Show(this, $"System Auto-Check: {newOverdueItems} item(s) automatically marked Overdue & strikes applied.", ToastType.Warning);
                }

                RefreshLayout();
                await SwitchView("Home");
                btnHome?.Focus();
            };


        }

        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ExStyle |= 0x02000000;
                return cp;
            }
        }

        #region KEYBOARD SHORTCUTS
        private Control GetFocusedControl()
        {
            Control focused = this.ActiveControl;

            while (focused is ContainerControl container)
            {
                focused = container.ActiveControl;
            }
            return focused;
        }
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Shift | Keys.D))
            {
                SignOutAdmin();
                return true;
            }

            if (keyData == Keys.Tab || keyData == (Keys.Shift | Keys.Tab))
            {
                var navBtns = new[] { btnHome, btnHistoryNav, btnNavAllItems, btnNavAvailable, btnNavBorrowed, btnNavBorrowers };

                Control focusedCtrl = GetFocusedControl();

                int currentIndex = Array.IndexOf(navBtns, focusedCtrl);

                if (currentIndex != -1)
                {
                    int nextIndex;
                    if (keyData == Keys.Tab)
                    {
                        nextIndex = (currentIndex + 1) % navBtns.Length;
                    }
                    else
                    {
                        nextIndex = (currentIndex - 1 + navBtns.Length) % navBtns.Length;
                    }

                    var nextBtn = navBtns[nextIndex];
                    nextBtn.Focus();

                    if (nextBtn == btnHome) _ = SwitchView("Home");
                    else if (nextBtn == btnHistoryNav) _ = SwitchView("History");
                    else if (nextBtn == btnNavAllItems) _ = SwitchView("Inventory", "All");
                    else if (nextBtn == btnNavAvailable) _ = SwitchView("Inventory", "Available");
                    else if (nextBtn == btnNavBorrowed) _ = SwitchView("Inventory", "Borrowed");
                    else if (nextBtn == btnNavBorrowers) _ = SwitchView("Inventory", "Records");

                    return true;
                }
            }

            if (keyData == (Keys.Control | Keys.F))
            {
                if (txtSearch != null && txtSearch.Visible)
                {
                    txtSearch.Focus();
                    txtSearch.SelectAll();
                    return true;
                }
            }

            if (keyData == Keys.Enter)
            {
                if (txtSearch != null && txtSearch.Focused)
                {
                    _ = LoadFromDatabase("All");
                    if (dgvInventory != null) dgvInventory.Focus();
                    return true;
                }
            }

            if (keyData == Keys.Escape)
            {
                if (txtSearch != null && txtSearch.Focused)
                {
                    txtSearch.Clear();
                    if (dgvInventory != null) dgvInventory.Focus();
                    return true;
                }
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

        private void ConfigureRuntimeUI()
        {
            // =================================================================================
            // 1. FORM SETUP & RESIZING LOGIC
            // =================================================================================
            FormClosed += (s, e) => { if (!isSigningOut) System.Windows.Forms.Application.Exit(); };
            this.Resize += (s, e) => { if (this.WindowState != FormWindowState.Minimized) RefreshLayout(); };

            if (flowRecentActivity != null)
            {
                flowRecentActivity.Resize += (s, e) => {
                    flowRecentActivity.SuspendLayout();
                    int targetWidth = flowRecentActivity.ClientSize.Width - flowRecentActivity.Padding.Left - flowRecentActivity.Padding.Right - 10;
                    if (targetWidth > 0)
                    {
                        foreach (Control ctrl in flowRecentActivity.Controls)
                        {
                            if (ctrl is Ventrix.App.Controls.ActivityCard || ctrl is Ventrix.App.Controls.AlertTile)
                                ctrl.Width = targetWidth;
                        }
                    }
                    flowRecentActivity.ResumeLayout(true);
                };
            }



            viewTransition = new Guna2Transition();
            viewTransition.Interval = 5;
            viewTransition.AnimationType = Guna.UI2.AnimatorNS.AnimationType.VertSlide;
            // =================================================================================
            // 2. DOUBLE BUFFERING (Prevents UI Flickering)
            // =================================================================================
            var controlsToBuffer = new Control[] {
        pnlMainContent, pnlGridContainer, pnlHistory, pnlHomeSummary,
        flowRecentActivity, pnlSidebar, dgvInventory, dgvHistory,
        pnlRegisterBorrower, btnRegisterBorrower, txtRegFirstName,
        txtRegLastName, txtRegSuffix, cmbRegRole, txtRegSchoolId
    };

            foreach (var ctrl in controlsToBuffer)
            {
                if (ctrl != null)
                {
                    typeof(Control).InvokeMember("DoubleBuffered",
                        System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                        null, ctrl, new object[] { true });
                }
            }

            // =================================================================================
            // 3. REGISTRATION UI LAYOUT (Missing Text Box & Stretching)
            // =================================================================================
            if (pnlRegisterBorrower != null && txtRegFirstName != null)
            {
                if (txtRegSchoolId == null)
                {
                    txtRegSchoolId = new Guna.UI2.WinForms.Guna2TextBox
                    {
                        Name = "txtRegSchoolId",
                        PlaceholderText = "School ID Number",
                        BorderRadius = 8,
                        Font = new DrawFont("Segoe UI", 10F)
                    };
                    pnlRegisterBorrower.Controls.Add(txtRegSchoolId);
                }

                if (lblRegisterHeader != null) lblRegisterHeader.Anchor = AnchorStyles.Top | AnchorStyles.Left;

                TableLayoutPanel tlpReg = pnlRegisterBorrower.Controls.OfType<TableLayoutPanel>().FirstOrDefault();
                if (tlpReg == null)
                {
                    tlpReg = new TableLayoutPanel { Name = "tlpRegLayout", ColumnCount = 5, RowCount = 1 };
                    tlpReg.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));

                    typeof(Control).InvokeMember("DoubleBuffered",
                        System.Reflection.BindingFlags.SetProperty | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic,
                        null, tlpReg, new object[] { true });

                    tlpReg.Location = new DrawPoint(15, 50);
                    tlpReg.Size = new DrawSize(pnlRegisterBorrower.Width - 180, 45); // Left space for button
                    tlpReg.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;

                    tlpReg.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));
                    tlpReg.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));
                    tlpReg.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 22F));
                    tlpReg.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 16F));
                    tlpReg.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 18F));

                    Control[] inputControls = { txtRegSchoolId, txtRegFirstName, txtRegLastName, txtRegSuffix, cmbRegRole };
                    for (int i = 0; i < inputControls.Length; i++)
                    {
                        if (inputControls[i] != null)
                        {
                            inputControls[i].Dock = DockStyle.None;
                            inputControls[i].Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top;
                            inputControls[i].Height = 36;
                            inputControls[i].Margin = new Padding(5, 4, 5, 0);
                            tlpReg.Controls.Add(inputControls[i], i, 0);
                        }
                    }
                    pnlRegisterBorrower.Controls.Add(tlpReg);
                    tlpReg.BringToFront();
                }

                if (btnRegisterBorrower != null)
                {
                    pnlRegisterBorrower.Controls.Add(btnRegisterBorrower);
                    btnRegisterBorrower.BringToFront();
                    btnRegisterBorrower.Dock = DockStyle.None;
                    btnRegisterBorrower.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                    btnRegisterBorrower.Size = new DrawSize(120, 36);
                    btnRegisterBorrower.Location = new DrawPoint(pnlRegisterBorrower.Width - 140, 54);

                    // Wire up event once
                    btnRegisterBorrower.Click -= btnRegisterBorrower_Click;
                    btnRegisterBorrower.Click += btnRegisterBorrower_Click;
                }
                tlpReg?.BringToFront();
            }

            if (cmbRegRole != null && txtRegSchoolId != null)
            {
                cmbRegRole.SelectedIndexChanged += (s, e) => {
                    txtRegSchoolId.Enabled = true;
                    txtRegSchoolId.PlaceholderText = cmbRegRole.SelectedItem?.ToString() == "Faculty" ? "Employee ID" : "School ID Number";
                };
            }

            // =================================================================================
            // 4. NAVIGATION, SIDEBAR & DASHBOARD ACTIONS
            // =================================================================================
            if (btnHome != null) btnHome.Click += async (s, e) => await SwitchView("Home");
            if (btnHistoryNav != null) btnHistoryNav.Click += async (s, e) => await SwitchView("History");
            if (btnNavAllItems != null) btnNavAllItems.Click += async (s, e) => await SwitchView("Inventory", "All");
            if (btnNavAvailable != null) btnNavAvailable.Click += async (s, e) => await SwitchView("Inventory", "Available");
            if (btnNavBorrowed != null) btnNavBorrowed.Click += async (s, e) => await SwitchView("Inventory", "Borrowed");
            if (btnNavBorrowers != null) btnNavBorrowers.Click += async (s, e) => await SwitchView("Inventory", "Records");

            if (cardTotal != null) cardTotal.CardClicked += async (s, e) => await SwitchView("Inventory", "All");
            if (cardAvailable != null) cardAvailable.CardClicked += async (s, e) => await SwitchView("Inventory", "Available");
            if (cardBorrowed != null) cardBorrowed.CardClicked += async (s, e) => await SwitchView("Inventory", "Borrowed");
            if (cardRecords != null) cardRecords.CardClicked += async (s, e) => await SwitchView("Inventory", "Records");

            var navBtns = new[] { btnHome, btnHistoryNav, btnNavAllItems, btnNavAvailable, btnNavBorrowed, btnNavBorrowers };
            foreach (var btn in navBtns)
            {
                if (btn != null)
                {
                    btn.GotFocus += (s, e) => btn.FillColor = DrawColor.FromArgb(40, 255, 255, 255);
                    btn.LostFocus += (s, e) => btn.FillColor = DrawColor.Transparent;
                }
            }

            if (sidebarTimer != null && btnHamburger != null)
            {
                sidebarTimer.Interval = 10;
                btnHamburger.Click += (s, e) => {
                    if (pnlGridContainer != null) pnlGridContainer.ShadowDecoration.Enabled = false;
                    if (pnlHomeSummary != null) pnlHomeSummary.ShadowDecoration.Enabled = false;
                    if (btnRegisterBorrower != null) btnRegisterBorrower.ShadowDecoration.Enabled = false;

                    if (dgvInventory != null) dgvInventory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
                    if (pnlRegisterBorrower != null) pnlRegisterBorrower.Anchor = AnchorStyles.Top | AnchorStyles.Right;

                    if (isSidebarExpanded) UpdateSidebarInternalUI(false);
                    sidebarTimer.Start();
                };
                sidebarTimer.Tick += SidebarTimer_Tick;
            }

            // =================================================================================
            // 5. VERTICAL THREE-DOTS ACTIONS MENU SETUP
            // =================================================================================
            btnActionsMenu = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "⋮",
                Font = new DrawFont("Segoe UI", 16F, FontStyle.Bold),

                // CHANGE: Set to Transparent so the bounding box disappears
                FillColor = DrawColor.Transparent,

                ForeColor = DrawColor.FromArgb(51, 65, 85),
                BorderRadius = 8,
                BorderThickness = 0,
                CustomBorderThickness = new Padding(0),
                Size = new DrawSize(45, 40),
                Cursor = Cursors.Hand,
                Animated = false,
                UseTransparentBackground = true
            };

            // Keep shadow rendering disabled just to be safe
            btnActionsMenu.ShadowDecoration.Enabled = false;
            btnActionsMenu.ShadowDecoration.Shadow = new Padding(0);
            btnActionsMenu.ShadowDecoration.Depth = 0;

            // Ensure no borders accidentally render
            btnActionsMenu.BorderColor = DrawColor.Transparent;
            btnActionsMenu.HoverState.BorderColor = DrawColor.Transparent;
            btnActionsMenu.HoverState.CustomBorderColor = DrawColor.Transparent;

            // Maintain smooth hover states (This will show the nice highlight only when moused over)
            btnActionsMenu.HoverState.FillColor = DrawColor.FromArgb(224, 231, 255);
            btnActionsMenu.HoverState.ForeColor = DrawColor.FromArgb(79, 70, 229);

            inventoryActionsMenu = new ContextMenuStrip();
            inventoryActionsMenu.Font = new DrawFont("Segoe UI", 10F);

            var menuAdd = new ToolStripMenuItem("➕ Add Item");
            var menuEdit = new ToolStripMenuItem("✏️ Edit Record");
            var menuDelete = new ToolStripMenuItem("🗑️ Delete Item");
            var menuSeparator = new ToolStripSeparator();
            var menuExcel = new ToolStripMenuItem("📊 Export to Excel");
            var menuPdf = new ToolStripMenuItem("📄 Export to PDF");

            menuAdd.Click += async (s, e) => await btnCreate_Click(s, e);
            menuEdit.Click += async (s, e) => await btnEdit_Click(s, e);
            menuDelete.Click += async (s, e) => await btnDelete_Click(s, e);

            menuExcel.Click += (s, e) => {
                if (pnlHistory != null && pnlHistory.Visible) _ = ExportHistoryToExcelAsync();
                else ExportToExcel();
            };

            menuPdf.Click += (s, e) => {
                if (pnlHistory != null && pnlHistory.Visible) _ = ExportHistoryToPDFAsync();
                else ExportToPDF();
            };

            inventoryActionsMenu.Items.AddRange(new ToolStripItem[] { menuAdd, menuEdit, menuDelete, menuSeparator, menuExcel, menuPdf });

            btnActionsMenu.Click += (s, e) => {
                inventoryActionsMenu.Show(btnActionsMenu, new DrawPoint(0, btnActionsMenu.Height));
            };

            if (btnClearActivity != null) btnClearActivity.Click += async (s, e) => await ClearRecentActivity();
            if (cmbAccountActions != null)
            {
                cmbAccountActions.SelectedIndexChanged -= CmbAccountActions_SelectedIndexChanged; // Prevent duplicate binding
                cmbAccountActions.SelectedIndexChanged += CmbAccountActions_SelectedIndexChanged;
            }
            if (badgeHealth != null) { badgeHealth.Cursor = Cursors.Hand; badgeHealth.Click += async (s, e) => await LblUrgentHeader_Click(s, e); }

            if (txtSearch != null)
            {
                txtSearch.IconRightCursor = Cursors.Hand;
                txtSearch.IconRightSize = new DrawSize(0, 0);
                txtSearch.TextChanged += async (s, e) => {
                    txtSearch.IconRightSize = string.IsNullOrEmpty(txtSearch.Text) ? new DrawSize(0, 0) : new DrawSize(15, 15);
                    if (pnlGridContainer != null && pnlGridContainer.Visible) await LoadFromDatabase(lblDashboardHeader.Text.Split(':')[1].Trim());
                    else if (pnlHistory != null && pnlHistory.Visible) await LoadHistoryData();
                };
                txtSearch.IconRightClick += (s, e) => { if (!string.IsNullOrEmpty(txtSearch.Text)) { txtSearch.Clear(); txtSearch.Focus(); } };
            }

            // =================================================================================
            // 6. CONTEXT MENU & QR EXPORT ACTIONS
            // =================================================================================
            if (dgvInventory != null)
            {
                ContextMenuStrip actionMenu = new ContextMenuStrip();

                List<int> GetSelectedRecordIds()
                {
                    if (dgvInventory.SelectedRows.Count == 0 || !dgvInventory.Columns.Contains("RecordIDs")) return new List<int>();
                    string idsStr = dgvInventory.SelectedRows[0].Cells["RecordIDs"].Value?.ToString() ?? "";
                    return idsStr.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(int.Parse).ToList();
                }

                // --- UNIFIED QR MANAGER BUTTON ---
                var manageQRsBtn = new ToolStripMenuItem("🖨️ Manage QR Tags...");
                manageQRsBtn.Click += async (s, e) => {
                    bool isBorrowersTabLocal = dgvInventory.Columns.Contains("BorrowerID") && !dgvInventory.Columns.Contains("RecordIDs");
                    var qrService = new Ventrix.Application.Services.QrCodeService();

                    if (isBorrowersTabLocal && dgvInventory.SelectedRows.Count > 0)
                    {
                        // 1. MANAGE USER QRs
                        string userId = dgvInventory.SelectedRows[0].Cells["BorrowerID"].Value?.ToString() ?? "";
                        string userName = dgvInventory.SelectedRows[0].Cells["BorrowerName"].Value?.ToString() ?? "";
                        var sticker = qrService.GenerateUserQrCodeWithLabel(userId, userName);

                        using (var popup = new Ventrix.App.Popups.QrManagerPopup(sticker, userName, "users"))
                        {
                            if (ShowPopupWithFade(popup) == DialogResult.OK)
                            {
                                if (popup.SelectedOption == Ventrix.App.Popups.QrExportOption.ExportSpecific)
                                    await GenerateSpecificUserQRCodesAsync();
                                else if (popup.SelectedOption == Ventrix.App.Popups.QrExportOption.ExportAll)
                                    await BatchGenerateAllUserQRCodesAsync();
                            }
                        }
                    }
                    else if (!isBorrowersTabLocal && dgvInventory.SelectedRows.Count > 0)
                    {
                        // 2. MANAGE INVENTORY QRs
                        string itemNameColumn = dgvInventory.Columns.Contains("ItemName") ? "ItemName" : "Name";
                        string itemName = dgvInventory.SelectedRows[0].Cells[itemNameColumn].Value?.ToString() ?? "";

                        if (string.IsNullOrWhiteSpace(itemName)) return;

                        this.Cursor = Cursors.WaitCursor;
                        var allItems = await _inventoryService.GetAllItemsAsync();
                        var specificUnits = allItems.Where(i => GetBaseItemName(i.Name) == itemName).ToList();
                        this.Cursor = Cursors.Default;

                        if (!specificUnits.Any()) return;

                        // Generate a preview sticker for the first unit in the group
                        var firstUnit = specificUnits.First();
                        var sticker = qrService.GenerateItemQrCodeWithLabel(firstUnit.Id, firstUnit.Name);

                        using (var popup = new Ventrix.App.Popups.QrManagerPopup(sticker, itemName, "inventory items"))
                        {
                            if (ShowPopupWithFade(popup) == DialogResult.OK)
                            {
                                if (popup.SelectedOption == Ventrix.App.Popups.QrExportOption.ExportSpecific)
                                {
                                    // Open the specific item selection popup (Filtered ONLY to the selected item type)
                                    using (var selectPopup = new Ventrix.App.Popups.MultiItemSelectionPopup(
                                        $"Generate QRs for {itemName}",
                                        $"Select the specific '{itemName}' units you want to print tags for:",
                                        specificUnits,
                                        "Generate Selected",
                                        DrawColor.FromArgb(13, 71, 161)))
                                    {
                                        if (ShowPopupWithFade(selectPopup) == DialogResult.OK)
                                        {
                                            var selectedItems = selectPopup.SelectedItems;
                                            if (!selectedItems.Any()) return;

                                            this.Cursor = Cursors.WaitCursor;
                                            string saveDir = @"C:\Ventrix_QR_Tags\Items";
                                            if (!System.IO.Directory.Exists(saveDir)) System.IO.Directory.CreateDirectory(saveDir);

                                            int count = 0;
                                            foreach (var item in selectedItems)
                                            {
                                                var specSticker = qrService.GenerateItemQrCodeWithLabel(item.Id, item.Name);
                                                qrService.SaveQrSticker(specSticker, saveDir, $"Item_{item.Id}_{item.Name}");
                                                count++;
                                            }
                                            this.Cursor = Cursors.Default;
                                            MessageBox.Show($"Successfully generated {count} QR tags for {itemName}!\n\nSaved to:\n{saveDir}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                                        }
                                    }
                                }
                                else if (popup.SelectedOption == Ventrix.App.Popups.QrExportOption.ExportAll)
                                {
                                    await BatchGenerateAllItemQRCodesAsync();
                                }
                            }
                        }
                    }
                };

                // ...
                // Add it to the action menu at the bottom:
                actionMenu.Items.Add(new ToolStripSeparator());
                actionMenu.Items.Add(manageQRsBtn);
                // --- General Actions ---
                var addStrikeBtn = new ToolStripMenuItem("⚠️ Issue Warning (Add 1 Strike)");
                addStrikeBtn.Click += async (s, e) => {
                    if (dgvInventory.SelectedRows.Count > 0 && dgvInventory.Columns.Contains("BorrowerID"))
                    {
                        string userId = dgvInventory.SelectedRows[0].Cells["BorrowerID"].Value.ToString();
                        await _userService.AddStrikeAsync(userId);
                        ToastNotification.Show(this, "1 Strike added to student account.", ToastType.Warning);
                        await LoadFromDatabase("Records");
                    }
                };
                var lockAccountBtn = new ToolStripMenuItem("🔒 Lock Account (Force 3 Strikes)");
                lockAccountBtn.Click += async (s, e) => {
                    if (dgvInventory.SelectedRows.Count > 0 && dgvInventory.Columns.Contains("BorrowerID"))
                    {
                        if (MessageBox.Show("Are you sure you want to lock this account?\n\nThis will prevent the student from borrowing any equipment until an admin clears them.", "Lock Account", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            string userId = dgvInventory.SelectedRows[0].Cells["BorrowerID"].Value.ToString();
                            // Apply strikes until locked
                            await _userService.AddStrikeAsync(userId);
                            await _userService.AddStrikeAsync(userId);
                            await _userService.AddStrikeAsync(userId);

                            ToastNotification.Show(this, "Account is now LOCKED.", ToastType.Warning);
                            await LoadFromDatabase("Records");
                        }
                    }
                };
                var unlockAccountBtn = new ToolStripMenuItem("🔓 Unlock Account (Clear All Strikes)");
                unlockAccountBtn.Click += async (s, e) => {
                    if (dgvInventory.SelectedRows.Count > 0 && dgvInventory.Columns.Contains("BorrowerID"))
                    {
                        string userId = dgvInventory.SelectedRows[0].Cells["BorrowerID"].Value.ToString();
                        await _userService.ClearStrikesAsync(userId);
                        ToastNotification.Show(this, "Account UNLOCKED. Strikes reset to 0.", ToastType.Success);
                        await LoadFromDatabase("Records");
                    }
                };
                var deleteUserBtn = new ToolStripMenuItem("🗑️ Delete User Account");
                deleteUserBtn.Click += async (s, e) => {
                    if (dgvInventory.SelectedRows.Count > 0 && dgvInventory.Columns.Contains("BorrowerID"))
                    {
                        string userId = dgvInventory.SelectedRows[0].Cells["BorrowerID"].Value.ToString();
                        string userName = dgvInventory.SelectedRows[0].Cells["BorrowerName"].Value.ToString();

                        // Double confirmation for deletion
                        if (MessageBox.Show($"Are you sure you want to permanently delete the account for {userName} ({userId})?", "Delete User", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                        {
                            try
                            {
                                await _userService.DeleteUserAsync(userId);
                                ToastNotification.Show(this, "User account deleted successfully.", ToastType.Success);
                                await LoadFromDatabase("Records"); // Refresh grid
                                await UpdateDashboardCounts();
                            }
                            catch (Exception ex)
                            {
                                // This will trigger if they try to delete someone who still holds items!
                                MessageBox.Show(ex.Message, "Cannot Delete", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            }
                        }
                    }
                };
                var editUserBtn = new ToolStripMenuItem("✏️ Edit User Account");
                editUserBtn.Click += async (s, e) => {
                    if (dgvInventory.SelectedRows.Count > 0 && dgvInventory.Columns.Contains("BorrowerID"))
                    {
                        string userId = dgvInventory.SelectedRows[0].Cells["BorrowerID"].Value.ToString();

                        using (var popup = new Popups.EditUserPopup(userId, _userService))
                        {
                            // Dim the background for visual focus
                            Form background = new Form();
                            using (background)
                            {
                                background.StartPosition = FormStartPosition.Manual;
                                background.FormBorderStyle = FormBorderStyle.None;
                                background.Opacity = 0.50d;
                                background.BackColor = System.Drawing.Color.Black;
                                background.Size = this.Size;
                                background.Location = this.Location;
                                background.ShowInTaskbar = false;
                                background.Show();

                                popup.Owner = background;
                                popup.ShowDialog();
                            }

                            // If they saved changes, refresh the grid!
                            if (popup.WasUpdated)
                            {
                                ToastNotification.Show(this, "User updated successfully.", ToastType.Success);
                                await LoadFromDatabase("Records");
                            }
                        }
                    }
                };

                var approveBtn = new ToolStripMenuItem("✅ Approve All Items in Request");
                approveBtn.Click += async (s, e) =>
                {
                    var recordIds = GetSelectedRecordIds();
                    if (!recordIds.Any()) return;

                    try
                    {
                        this.Cursor = Cursors.WaitCursor;
                        foreach (var id in recordIds)
                        {
                            await _borrowService.ApproveBorrowAsync(id);
                        }
                        this.Cursor = Cursors.Default;

                        // Trigger our custom slide-in success toast notification!
                        ToastNotification.Show(this, "Borrow request approved successfully!", ToastType.Success);

                        await LoadFromDatabase("Borrowed");
                        await UpdateDashboardCounts();
                    }
                    catch (Exception ex)
                    {
                        this.Cursor = Cursors.Default;
                        ToastNotification.Show(this, "Error approving request: " + ex.Message, ToastType.Error);
                    }
                };
                var partialApproveBtn = new ToolStripMenuItem("📝 Select Specific Items to Approve...");
                partialApproveBtn.Click += async (s, e) =>
                {
                    var recordIds = GetSelectedRecordIds();
                    if (!recordIds.Any()) return;

                    try
                    {
                        var allRecords = await _borrowService.GetAllBorrowRecordsAsync();
                        var pendingRecords = allRecords.Where(r => recordIds.Contains(r.Id)).ToList();

                        // Open your multi-record selection popup
                        var selectedIdsToApprove = ShowMultiRecordSelectionPopup(
                            "Approve Specific Items",
                            "Select the specific items you wish to approve:",
                            pendingRecords,
                            "Approve Selected",
                            DrawColor.FromArgb(16, 185, 129) // Emerald Green
                        );

                        if (selectedIdsToApprove.Any())
                        {
                            this.Cursor = Cursors.WaitCursor;
                            foreach (var id in selectedIdsToApprove)
                            {
                                await _borrowService.ApproveBorrowAsync(id);
                            }
                            this.Cursor = Cursors.Default;

                            ToastNotification.Show(this, $"Successfully approved {selectedIdsToApprove.Count} items!", ToastType.Success);

                            await LoadFromDatabase("Borrowed");
                            await UpdateDashboardCounts();
                        }
                    }
                    catch (Exception ex)
                    {
                        this.Cursor = Cursors.Default;
                        ToastNotification.Show(this, "Error in partial approval: " + ex.Message, ToastType.Error);
                    }
                };

                var confirmReturnBtn = new ToolStripMenuItem("✅ Confirm Return (All Items)");
                confirmReturnBtn.Click += async (s, e) => {
                    var ids = GetSelectedRecordIds();
                    if (ids.Any())
                    {
                        foreach (var id in ids) await _borrowService.ReturnItemAsync(id);
                        ToastNotification.Show(this, "Return(s) Confirmed!", ToastType.Success);
                        await LoadFromDatabase("Borrowed");
                        await UpdateDashboardCounts();
                    }
                };
                var partialReturnBtn = new ToolStripMenuItem("📝 Select Specific Items to Return...");
                partialReturnBtn.Click += async (s, e) => {
                    var ids = GetSelectedRecordIds();
                    if (ids.Any())
                    {
                        var allRecords = await _borrowService.GetAllBorrowRecordsAsync();
                        var pendingReturnRecords = allRecords.Where(r => ids.Contains(r.Id) && r.Status == BorrowStatus.PendingReturn).ToList();

                        if (pendingReturnRecords.Any())
                        {
                            var selectedToReturn = ShowMultiRecordSelectionPopup("Return Items", "Select the specific items to confirm return:", pendingReturnRecords, "Confirm Return", DrawColor.MediumSeaGreen);
                            if (selectedToReturn.Any())
                            {
                                foreach (var id in selectedToReturn) await _borrowService.ReturnItemAsync(id);
                                ToastNotification.Show(this, $"Successfully returned {selectedToReturn.Count} item(s)!", ToastType.Success);
                                await LoadFromDatabase("Borrowed");
                                await UpdateDashboardCounts();
                            }
                        }
                    }
                };

                var confirmDamagedReturnBtn = new ToolStripMenuItem("⚠️ Select Specific Items to Mark Damaged...");
                confirmDamagedReturnBtn.Click += async (s, e) => {
                    var ids = GetSelectedRecordIds();
                    if (ids.Any())
                    {
                        var allRecords = await _borrowService.GetAllBorrowRecordsAsync();

                        // Get items that are currently out with the borrower
                        var validRecords = allRecords.Where(r => ids.Contains(r.Id) &&
                            (r.Status == BorrowStatus.PendingReturn || r.Status == BorrowStatus.Active || r.Status == BorrowStatus.Overdue)).ToList();

                        if (validRecords.Any())
                        {
                            // Show the checkbox popup, but styled red for danger
                            var damagedIds = ShowMultiRecordSelectionPopup(
                                "Report Damaged Items",
                                "Select ONLY the specific items that are damaged:\n(Unchecked items will be left alone to be returned normally)",
                                validRecords,
                                "Mark Damaged & Penalize",
                                DrawColor.IndianRed);

                            if (damagedIds.Any())
                            {
                                string borrowerId = dgvInventory.SelectedRows[0].Cells["BorrowerID"].Value.ToString();

                                // Mark only the checked items as damaged
                                foreach (var id in damagedIds)
                                {
                                    await _borrowService.ReturnItemAsDamagedAsync(id);
                                }

                                // Apply 1 strike to the borrower for this incident
                                await _userService.AddStrikeAsync(borrowerId);

                                ToastNotification.Show(this, $"{damagedIds.Count} item(s) marked Damaged & Strike issued!", ToastType.Warning);
                                await LoadFromDatabase("Borrowed");
                                await UpdateDashboardCounts();
                            }
                        }
                    }
                };
                var partialForceReturnBtn = new ToolStripMenuItem("🔙 Select Specific Items to Force Return...");
                partialForceReturnBtn.Click += async (s, e) => {
                    var ids = GetSelectedRecordIds();
                    if (ids.Any())
                    {
                        var allRecords = await _borrowService.GetAllBorrowRecordsAsync();
                        // Force return is valid for Active, Overdue, or PendingReturn
                        var validRecords = allRecords.Where(r => ids.Contains(r.Id) &&
                            (r.Status == BorrowStatus.Active || r.Status == BorrowStatus.Overdue || r.Status == BorrowStatus.PendingReturn)).ToList();

                        if (validRecords.Any())
                        {
                            var selectedToReturn = ShowMultiRecordSelectionPopup(
                                "Force Return Items",
                                "Select the specific items to manually force return to inventory:",
                                validRecords,
                                "Force Return",
                                DrawColor.IndianRed);

                            if (selectedToReturn.Any())
                            {
                                await _borrowService.ForceReturnItemsAsync(selectedToReturn);
                                ToastNotification.Show(this, $"Successfully force returned {selectedToReturn.Count} item(s)!", ToastType.Success);
                                await LoadFromDatabase("Borrowed");
                                await UpdateDashboardCounts();
                            }
                        }
                    }
                };
                var partialOverdueBtn = new ToolStripMenuItem("⏰ Select Specific Items to Mark Overdue...");
                partialOverdueBtn.Click += async (s, e) => {
                    var ids = GetSelectedRecordIds();
                    if (ids.Any())
                    {
                        var allRecords = await _borrowService.GetAllBorrowRecordsAsync();
                        var validRecords = allRecords.Where(r => ids.Contains(r.Id) &&
                            (r.Status == BorrowStatus.Active || r.Status == BorrowStatus.PendingReturn || r.Status == BorrowStatus.Overdue)).ToList();

                        if (validRecords.Any())
                        {
                            var selectedToOverdue = ShowMultiRecordSelectionPopup(
                                "Mark Overdue",
                                "Select the specific items to mark as overdue:",
                                validRecords,
                                "Mark Overdue",
                                DrawColor.DarkOrange);

                            if (selectedToOverdue.Any())
                            {
                                // 1. Update the database
                                await _borrowService.ManuallyMarkOverdueAsync(selectedToOverdue, _userService);

                                ToastNotification.Show(this, $"{selectedToOverdue.Count} item(s) marked Overdue & Strike issued!", ToastType.Warning);

                                // 2. CRITICAL: Refresh the grid data so the colors update
                                await LoadFromDatabase("Borrowed");

                                // 3. Update the dashboard cards
                                await UpdateDashboardCounts();
                            }
                        }
                    }
                };

                // --- Building the Menu ---
                actionMenu.Items.Add(addStrikeBtn);
                actionMenu.Items.Add(lockAccountBtn);
                actionMenu.Items.Add(unlockAccountBtn);
                actionMenu.Items.Add(new ToolStripSeparator());
                actionMenu.Items.Add(approveBtn);
                actionMenu.Items.Add(new ToolStripSeparator());
                actionMenu.Items.Add(deleteUserBtn);
                actionMenu.Items.Add(editUserBtn);
                actionMenu.Items.Add(partialApproveBtn);
                actionMenu.Items.Add(confirmReturnBtn);
                actionMenu.Items.Add(partialReturnBtn);
                actionMenu.Items.Add(confirmDamagedReturnBtn);
                actionMenu.Items.Add(partialForceReturnBtn);
                actionMenu.Items.Add(partialOverdueBtn);

                actionMenu.Items.Add(new ToolStripSeparator());
                actionMenu.Items.Add(manageQRsBtn);
                dgvInventory.ContextMenuStrip = actionMenu;

                // --- Visibility Rules ---
                actionMenu.Opening += (s, e) => {
                    bool isBorrowersTab = dgvInventory.Columns.Contains("BorrowerID") && !dgvInventory.Columns.Contains("RecordIDs");
                    bool isBorrowedTab = dgvInventory.Columns.Contains("RecordIDs") && dgvInventory.Columns.Contains("Status");

                    // Hide everything by default, then selectively enable
                    foreach (ToolStripItem item in actionMenu.Items) item.Visible = false;

                    // QR Menu logic
                    bool hasRowSelected = dgvInventory.SelectedRows.Count > 0;
                    // Only show the QR Manager if a row is selected and we are NOT looking at the grouped Borrowed requests
                    manageQRsBtn.Visible = hasRowSelected && !isBorrowedTab;

                    if (isBorrowersTab)
                    {
                        addStrikeBtn.Visible = true; lockAccountBtn.Visible = true; unlockAccountBtn.Visible = true;
                        deleteUserBtn.Visible = true; editUserBtn.Visible = true;
                    }
                    else if (isBorrowedTab && hasRowSelected)
                    {
                        string status = dgvInventory.SelectedRows[0].Cells["Status"].Value?.ToString() ?? "";
                        if (status == "Pending") { approveBtn.Visible = true; partialApproveBtn.Visible = true; }
                        if (status == "PendingReturn") { confirmReturnBtn.Visible = true; partialReturnBtn.Visible = true; partialOverdueBtn.Visible = true; confirmDamagedReturnBtn.Visible = true; }
                        if (status == "Active") { partialForceReturnBtn.Visible = true; partialOverdueBtn.Visible = true; confirmDamagedReturnBtn.Visible = true; }
                        if (status == "Overdue") { partialForceReturnBtn.Visible = true; confirmDamagedReturnBtn.Visible = true; }
                    }

                    // Prevent the menu from opening if they right-click an empty grid where no actions are available
                    if (!hasRowSelected && actionMenu.Items.Cast<ToolStripItem>().All(i => !i.Visible))
                    {
                        e.Cancel = true;
                    }
                };

                dgvInventory.CellDoubleClick += async (s, e) => await DgvInventory_CellDoubleClick(s, e);
                dgvInventory.CellFormatting += DgvInventory_CellFormatting;
            }

            // =================================================================================
            // 6. HISTORY GRID EVENTS
            // =================================================================================
            if (dgvHistory != null)
            {
                dgvHistory.CellFormatting += DgvHistory_CellFormatting;
                dgvHistory.ColumnHeaderMouseClick += async (s, e) => {
                    string clickedCol = dgvHistory.Columns[e.ColumnIndex].Name;
                    if (historySortColumn == clickedCol) historySortDescending = !historySortDescending;
                    else { historySortColumn = clickedCol; historySortDescending = false; }
                    historyCurrentPage = 1;
                    await LoadHistoryData();
                };
                dgvHistory.CellDoubleClick += DgvHistory_CellDoubleClick;
            }

            // =================================================================================
            // 7. TACTILE CARD HOVER STATES (Micro-Interactions)
            // =================================================================================
            var metricCards = new Control[] { cardTotal, cardAvailable, cardBorrowed, cardRecords };
            foreach (var card in metricCards)
            {
                if (card != null)
                {
                    card.Cursor = Cursors.Hand;
                    card.MouseEnter += (s, e) => {
                        // If the card is a Guna2Panel or container with a BorderColor property
                        if (card is Guna.UI2.WinForms.Guna2Panel panel)
                        {
                            panel.BorderColor = ThemeManager.AccentBlue;
                            panel.BorderThickness = 2; // Slight crisp border emphasis on hover
                        }
                        else
                        {
                            card.BackColor = DrawColor.FromArgb(243, 244, 246); // Soft highlight tint for custom UserControls
                        }
                    };
                    card.MouseLeave += (s, e) => {
                        if (card is Guna.UI2.WinForms.Guna2Panel panel)
                        {
                            panel.BorderColor = ThemeManager.BorderColor;
                            panel.BorderThickness = 1;
                        }
                        else
                        {
                            card.BackColor = ThemeManager.SurfaceColor;
                        }
                    };
                }
            }
        }

        private async Task BatchGenerateAllUserQRCodesAsync()
        {
            string saveDir = @"C:\Ventrix_QR_Tags\Users";
            if (!System.IO.Directory.Exists(saveDir)) System.IO.Directory.CreateDirectory(saveDir);

            var qrService = new Ventrix.Application.Services.QrCodeService();
            var borrowers = (await _userService.GetAllUsersAsync()).Where(u => u.Role != Ventrix.Domain.Enums.UserRole.Admin).ToList();

            if (!borrowers.Any()) return;

            foreach (var user in borrowers)
            {
                var sticker = qrService.GenerateUserQrCodeWithLabel(user.UserId, user.FullName);
                qrService.SaveQrSticker(sticker, saveDir, $"User_{user.UserId}_{user.FirstName}");
            }
            MessageBox.Show($"Generated {borrowers.Count} User QR tags!\nSaved at: {saveDir}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task GenerateSpecificUserQRCodesAsync()
        {
            var borrowers = (await _userService.GetAllUsersAsync()).Where(u => u.Role != Ventrix.Domain.Enums.UserRole.Admin).ToList();
            if (!borrowers.Any()) return;

            using (var popup = new Ventrix.App.Popups.MultiUserSelectionPopup("Generate Specific User QRs", "Select the users:", borrowers))
            {
                if (ShowPopupWithFade(popup) == DialogResult.OK)
                {
                    string saveDir = @"C:\Ventrix_QR_Tags\Users";
                    if (!System.IO.Directory.Exists(saveDir)) System.IO.Directory.CreateDirectory(saveDir);

                    var qrService = new Ventrix.Application.Services.QrCodeService();
                    foreach (var user in popup.SelectedUsers)
                    {
                        var sticker = qrService.GenerateUserQrCodeWithLabel(user.UserId, user.FullName);
                        qrService.SaveQrSticker(sticker, saveDir, $"User_{user.UserId}_{user.FirstName}");
                    }
                    MessageBox.Show($"Generated {popup.SelectedUsers.Count} specific User QR tags!\nSaved at: {saveDir}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private async Task GenerateSpecificItemQRCodesAsync()
        {
            var allItems = (await _inventoryService.GetAllItemsAsync()).ToList();

            if (!allItems.Any())
            {
                MessageBox.Show("There are no items in the inventory to generate codes for.", "Empty Inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Launch the new popup, utilizing your existing ShowPopupWithFade method!
            using (var popup = new Ventrix.App.Popups.MultiItemSelectionPopup(
                "Generate Specific QR Tags",
                "Select the specific equipment units you want to generate QR tags for:",
                allItems,
                "Generate Selected",
                Color.FromArgb(13, 71, 161))) // Ventrix Blue
            {
                if (ShowPopupWithFade(popup) == DialogResult.OK)
                {
                    var selectedItems = popup.SelectedItems;

                    if (!selectedItems.Any()) return;

                    this.Cursor = Cursors.WaitCursor;
                    try
                    {
                        string saveDirectory = @"C:\Ventrix_QR_Tags\Items";
                        if (!System.IO.Directory.Exists(saveDirectory))
                        {
                            System.IO.Directory.CreateDirectory(saveDirectory);
                        }

                        // Reference your service
                        var qrService = new Ventrix.Application.Services.QrCodeService();
                        int generatedCount = 0;

                        foreach (var item in selectedItems)
                        {
                            var sticker = qrService.GenerateItemQrCodeWithLabel(item.Id, item.Name);
                            string fileName = $"Item_{item.Id}_{item.Name}";
                            qrService.SaveQrSticker(sticker, saveDirectory, fileName);
                            generatedCount++;
                        }

                        MessageBox.Show($"Successfully generated {generatedCount} specific QR tags!\n\nYou can find them at:\n{saveDirectory}", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    finally
                    {
                        this.Cursor = Cursors.Default;
                    }
                }
            }
        }

        #region Helper Popups
        private List<int> ShowMultiRecordSelectionPopup(string title, string instruction, List<BorrowRecord> records, string btnText, DrawColor btnColor)
        {
            using (var popup = new MultiRecordSelectionPopup(title, instruction, records, btnText, btnColor))
            {
                if (ShowPopupWithFade(popup) == DialogResult.OK)
                {
                    return popup.SelectedIds;
                }
            }
            return new List<int>(); // Return empty list if cancelled
        }
        #endregion

        #region FLUID LAYOUT & ANIMATION (Strict Math, No Overlaps)
        private void RefreshLayout()
        {
            if (pnlMainContent == null || pnlSidebar == null || pnlTopBar == null) return;

            pnlTopBar.Dock = DockStyle.None;
            pnlSidebar.Dock = DockStyle.None;
            pnlMainContent.Dock = DockStyle.None;

            int topMargin = 64;
            int topBarHeight = 80;
            int contentY = topMargin + topBarHeight;
            int remainingHeight = this.ClientSize.Height - contentY;

            pnlTopBar.SetBounds(0, topMargin, this.ClientSize.Width, topBarHeight);
            pnlSidebar.SetBounds(0, contentY, isSidebarExpanded ? sidebarMaxWidth : sidebarMinWidth, remainingHeight);
            pnlMainContent.SetBounds(pnlSidebar.Width, contentY, this.ClientSize.Width - pnlSidebar.Width, remainingHeight);

            pnlTopBar.BringToFront();
            pnlSidebar.BringToFront();
            pnlMainContent.BringToFront();

            if (btnHamburger != null)
            {
                btnHamburger.Location = new DrawPoint(25, (pnlTopBar.Height - btnHamburger.Height) / 2);
            }

            if (lblDashboardHeader != null && btnHamburger != null)
            {
                lblDashboardHeader.Location = new DrawPoint(btnHamburger.Right + 20, (pnlTopBar.Height - lblDashboardHeader.Height) / 2);
            }
            if (txtSearch != null)
            {
                txtSearch.Location = new DrawPoint(pnlTopBar.Width - txtSearch.Width - 30, (pnlTopBar.Height - txtSearch.Height) / 2);
            }

            if (badgeHealth != null && txtSearch != null)
            {
                badgeHealth.Location = new DrawPoint(txtSearch.Left - badgeHealth.Width - 20, (pnlTopBar.Height - badgeHealth.Height) / 2);
            }

            int margin = 30;
            DrawRect safeArea = new DrawRect(margin, margin, pnlMainContent.Width - (margin * 2), pnlMainContent.Height - (margin * 2));

            if (pnlHomeSummary != null) pnlHomeSummary.Bounds = safeArea;
            if (pnlGridContainer != null) pnlGridContainer.Bounds = safeArea;
            if (pnlHistory != null) pnlHistory.Bounds = safeArea;

            if (pnlHomeSummary != null && pnlHomeSummary.Visible) ArrangeHomeView();
            if (pnlGridContainer != null && pnlGridContainer.Visible) ArrangeInventoryView();
            if (pnlHistory != null && pnlHistory.Visible) ArrangeHistoryView();

            UpdateSidebarInternalUI(isSidebarExpanded);
        }

        private void ArrangeHomeView()
        {
            if (pnlHomeSummary == null) return;

            int spacing = 20;
            int topMargin = 20;

            if (btnClearActivity != null)
            {
                btnClearActivity.Parent = pnlHomeSummary;
                btnClearActivity.Anchor = AnchorStyles.None;
                btnClearActivity.Location = new DrawPoint(pnlHomeSummary.Width - btnClearActivity.Width - spacing, topMargin);
                btnClearActivity.Anchor = AnchorStyles.Top | AnchorStyles.Right;
                btnClearActivity.BringToFront();
            }

            int cardY = 70;
            int cardWidth = (pnlHomeSummary.Width - (spacing * 5)) / 4;

            if (cardTotal != null) { cardTotal.Parent = pnlHomeSummary; cardTotal.SetBounds(spacing, cardY, cardWidth, 110); }
            if (cardAvailable != null) { cardAvailable.Parent = pnlHomeSummary; cardAvailable.SetBounds(cardTotal.Right + spacing, cardY, cardWidth, 110); }
            if (cardBorrowed != null) { cardBorrowed.Parent = pnlHomeSummary; cardBorrowed.SetBounds(cardAvailable.Right + spacing, cardY, cardWidth, 110); }
            if (cardRecords != null) { cardRecords.Parent = pnlHomeSummary; cardRecords.SetBounds(cardBorrowed.Right + spacing, cardY, cardWidth, 110); }

            if (flowRecentActivity != null)
            {
                flowRecentActivity.Parent = pnlHomeSummary;

                int activityY = cardTotal.Bottom + 30;

                flowRecentActivity.Anchor = AnchorStyles.None;
                flowRecentActivity.Location = new DrawPoint(spacing, activityY);
                flowRecentActivity.Size = new DrawSize(pnlHomeSummary.Width - (spacing * 2), pnlHomeSummary.Height - activityY - spacing);

                flowRecentActivity.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                flowRecentActivity.BringToFront();
            }
        }

        private void ArrangeInventoryView()
        {
            if (pnlGridContainer == null) return;

            int topRowY = 20;
            int margin = 25;
            int availableWidth = pnlGridContainer.Width - (margin * 2);

            // Dynamic width scaling: make Excel and PDF buttons smaller when the window is minimized/narrowed
            bool isCompact = availableWidth < 820;

            if (btnExportExcel != null)
            {
                btnExportExcel.Width = isCompact ? 75 : 105;
            }
            if (btnExportPDF != null)
            {
                btnExportPDF.Width = isCompact ? 65 : 95;
            }

            // ========================================================
            // 1. Position Left-Aligned Buttons (Export / PDF)
            // ========================================================
            int leftTracker = margin; // Tracks the physical right-edge of the last button

            if (btnExportExcel != null)
            {
                btnExportExcel.Parent = pnlGridContainer;
                btnExportExcel.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                btnExportExcel.Location = new DrawPoint(leftTracker, topRowY);
                btnExportExcel.BringToFront();
                leftTracker = btnExportExcel.Right + 15;
            }

            if (btnExportPDF != null)
            {
                btnExportPDF.Parent = pnlGridContainer;
                btnExportPDF.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                btnExportPDF.Location = new DrawPoint(leftTracker, topRowY);
                btnExportPDF.BringToFront();
                leftTracker = btnExportPDF.Right + 15;
            }

            // ========================================================
            // 2. Safely Position Right-Aligned Buttons (Create & Actions Menu)
            // ========================================================
            // We calculate their total width so they move together as a unified block.
            int actionGroupWidth = 0;

            if (btnActionsMenu != null && btnActionsMenu.Visible)
                actionGroupWidth += btnActionsMenu.Width;

            if (btnCreate != null && btnCreate.Visible)
                actionGroupWidth += (actionGroupWidth > 0 ? 15 : 0) + btnCreate.Width;

            // Enforce a strict right margin limit, while maintaining the "brick wall" collision on the left
            int rightEdgeLimit = pnlGridContainer.Width - margin;
            int startX = Math.Max(leftTracker + 30, rightEdgeLimit - actionGroupWidth);

            if (btnCreate != null && btnCreate.Visible)
            {
                btnCreate.Parent = pnlGridContainer;
                btnCreate.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                btnCreate.Location = new DrawPoint(startX, topRowY);
                btnCreate.BringToFront();
                startX = btnCreate.Right + 15;
            }

            if (btnActionsMenu != null && btnActionsMenu.Visible)
            {
                btnActionsMenu.Parent = pnlGridContainer;
                btnActionsMenu.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                btnActionsMenu.Location = new DrawPoint(startX, topRowY);
                btnActionsMenu.BringToFront();
            }

            // ========================================================
            // 3. Grid and Registration Panel Layout
            // ========================================================
            int gridY = topRowY + 50;

            if (pnlRegisterBorrower != null && pnlRegisterBorrower.Visible)
            {
                pnlRegisterBorrower.Parent = pnlGridContainer;
                pnlRegisterBorrower.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                pnlRegisterBorrower.Location = new DrawPoint(margin, gridY);
                pnlRegisterBorrower.Size = new DrawSize(pnlGridContainer.Width - (margin * 2), 100);
                pnlRegisterBorrower.BringToFront();

                gridY += pnlRegisterBorrower.Height + 20;
            }

            if (dgvInventory != null)
            {
                dgvInventory.Parent = pnlGridContainer;
                dgvInventory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
                dgvInventory.Location = new DrawPoint(margin, gridY);
                dgvInventory.Size = new DrawSize(pnlGridContainer.Width - (margin * 2), pnlGridContainer.Height - gridY - margin);
                dgvInventory.BringToFront();

                if (lblEmptyState != null && lblEmptyState.Visible && lblEmptyState.Parent == pnlGridContainer)
                {
                    lblEmptyState.Anchor = AnchorStyles.Top | AnchorStyles.Left;
                    lblEmptyState.Bounds = dgvInventory.Bounds;
                    lblEmptyState.BringToFront();
                }
            }
        }

        private void ArrangeHistoryView()
{
    if (pnlHistory == null) return;

    int topRowY = 20;
    int margin = 25;
    int leftTracker = margin;

    // ========================================================
    // 1. Date Pickers & Filter Button Group (Left-Aligned)
    // ========================================================
    if (dtpStartDate != null)
    {
        dtpStartDate.Parent = pnlHistory;
        dtpStartDate.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        dtpStartDate.Location = new DrawPoint(leftTracker, topRowY + 4);
        leftTracker = dtpStartDate.Right + 10;
    }

    if (lblDateSeparator == null)
    {
        lblDateSeparator = new Label
        {
            Text = "to",
            AutoSize = true,
            Font = new DrawFont("Segoe UI", 9.5F),
            ForeColor = Color.FromArgb(100, 116, 139)
        };
        pnlHistory.Controls.Add(lblDateSeparator);
    }
    lblDateSeparator.Parent = pnlHistory;
    lblDateSeparator.Anchor = AnchorStyles.Top | AnchorStyles.Left;
    lblDateSeparator.Location = new DrawPoint(leftTracker, topRowY + 10);
    leftTracker = lblDateSeparator.Right + 10;

    if (dtpEndDate != null)
    {
        dtpEndDate.Parent = pnlHistory;
        dtpEndDate.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        dtpEndDate.Location = new DrawPoint(leftTracker, topRowY + 4);
        leftTracker = dtpEndDate.Right + 15;
    }

    if (btnFilterDates != null)
    {
        btnFilterDates.Parent = pnlHistory;
        btnFilterDates.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        btnFilterDates.Location = new DrawPoint(leftTracker, topRowY);
    }

    // ========================================================
    // 2. DataGridView Layout
    // ========================================================
    int gridY = topRowY + 55;
    if (dgvHistory != null)
    {
        dgvHistory.Parent = pnlHistory;
        dgvHistory.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvHistory.Location = new DrawPoint(margin, gridY);
        dgvHistory.Size = new DrawSize(pnlHistory.Width - (margin * 2), pnlHistory.Height - gridY - margin);
        dgvHistory.BringToFront();
        
        if (lblEmptyState != null && lblEmptyState.Visible && lblEmptyState.Parent == pnlHistory)
        {
            lblEmptyState.Anchor = AnchorStyles.Top | AnchorStyles.Left;
            lblEmptyState.Bounds = dgvHistory.Bounds;
            lblEmptyState.BringToFront();
        }
    }

            // ========================================================
            // 3. Actions Menu (Right-Aligned - Brought to absolute front)
            // ========================================================
            if (btnActionsMenu != null && btnActionsMenu.Visible)
            {
                btnActionsMenu.Parent = pnlHistory;
                btnActionsMenu.Anchor = AnchorStyles.None; // Prevent anchoring interference with manual coordinates

                // Safeguard against unrendered panel width placing the button off-screen
                int menuX = pnlHistory.Width > 0 ? pnlHistory.Width - btnActionsMenu.Width - margin : margin;
                btnActionsMenu.Location = new DrawPoint(Math.Max(margin, menuX), topRowY);

                // Must be called last so it floats cleanly on top of the panel components
                btnActionsMenu.BringToFront();
            }
        }

        private async Task AnimateCountUp(Ventrix.App.Controls.MetricCard card, string title, int targetValue, DrawColor color)
        {
            if (card == null) return;

            int startValue = 0;
            int steps = 15; // Number of smooth animation frames
            int duration = 400; // Total duration in milliseconds
            int interval = duration / steps;

            for (int i = 1; i <= steps; i++)
            {
                int currentValue = (int)(startValue + (targetValue - startValue) * ((float)i / steps));
                card.UpdateMetrics(title, currentValue.ToString("N0"), color);
                await Task.Delay(interval);
            }

            // Ensure the exact final value is set at the end
            card.UpdateMetrics(title, targetValue.ToString("N0"), color);
        }

        private void StyleHistoryDataGridView()
        {
            if (dgvHistory == null) return;

            // Eliminate all visual flicker
            typeof(DataGridView).InvokeMember("DoubleBuffered",
                BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.SetProperty,
                null, dgvHistory, new object[] { true });

            // Container & Border Setup
            dgvHistory.BackgroundColor = Color.White;
            dgvHistory.BorderStyle = BorderStyle.None;

            // --- SHOW VERTICAL AND HORIZONTAL LINES ON HISTORY ---
            dgvHistory.CellBorderStyle = DataGridViewCellBorderStyle.Single;
            dgvHistory.GridColor = DrawColor.FromArgb(226, 232, 240);

            dgvHistory.RowHeadersVisible = false;
            dgvHistory.AllowUserToAddRows = false;
            dgvHistory.AllowUserToDeleteRows = false;
            dgvHistory.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvHistory.MultiSelect = false;
            dgvHistory.ReadOnly = true;

            // Premium SaaS Proportions (Spacious 52px rows with generous horizontal padding)
            dgvHistory.RowTemplate.Height = 52;
            dgvHistory.RowsDefaultCellStyle.BackColor = Color.White;
            dgvHistory.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(248, 250, 252);
            dgvHistory.DefaultCellStyle.ForeColor = Color.FromArgb(30, 41, 59);
            dgvHistory.DefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10F, System.Drawing.FontStyle.Regular);
            dgvHistory.DefaultCellStyle.Padding = new Padding(16, 0, 16, 0);

            // Soft Modern Indigo Selection State
            dgvHistory.DefaultCellStyle.SelectionBackColor = Color.FromArgb(224, 231, 255);
            dgvHistory.DefaultCellStyle.SelectionForeColor = Color.FromArgb(49, 46, 129);

            // Modern SaaS Header Styling
            dgvHistory.EnableHeadersVisualStyles = false;
            dgvHistory.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgvHistory.ColumnHeadersHeight = 56;
            dgvHistory.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(79, 70, 229);
            dgvHistory.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvHistory.ColumnHeadersDefaultCellStyle.Font = new System.Drawing.Font("Segoe UI", 10.5F, System.Drawing.FontStyle.Bold);
            dgvHistory.ColumnHeadersDefaultCellStyle.Padding = new Padding(16, 0, 0, 0);
            dgvHistory.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(79, 70, 229);
            dgvHistory.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.White;
        }

        private void StyleModernDatePickers()
        {
            // Style Start Date Picker to look like a modern SaaS input field
            if (dtpStartDate != null)
            {
                dtpStartDate.FillColor = Color.White;
                dtpStartDate.ForeColor = Color.FromArgb(51, 65, 85);
                dtpStartDate.BorderColor = Color.FromArgb(203, 213, 225); // Clean light border
                dtpStartDate.BorderRadius = 6;
                dtpStartDate.BorderThickness = 1;
                dtpStartDate.Font = new System.Drawing.Font("Segoe UI", 9F);
                dtpStartDate.Format = DateTimePickerFormat.Short;
            }

            // Style End Date Picker identically
            if (dtpEndDate != null)
            {
                dtpEndDate.FillColor = Color.White;
                dtpEndDate.ForeColor = Color.FromArgb(51, 65, 85);
                dtpEndDate.BorderColor = Color.FromArgb(203, 213, 225);
                dtpEndDate.BorderRadius = 6;
                dtpEndDate.BorderThickness = 1;
                dtpEndDate.Font = new System.Drawing.Font("Segoe UI", 9F);
                dtpEndDate.Format = DateTimePickerFormat.Short;
            }
        }

        private void AttachFilterButtonAnimations()
        {
            if (btnFilterDates == null) return;

            // Guna2Button controls styling through FillColor instead of FlatStyle/FlatAppearance
            btnFilterDates.FillColor = Color.FromArgb(30, 64, 175); // Deep Indigo-Blue
            btnFilterDates.ForeColor = Color.White;
            btnFilterDates.Cursor = Cursors.Hand;
            btnFilterDates.Font = new System.Drawing.Font("Segoe UI", 9.5F, System.Drawing.FontStyle.Bold);

            // Smooth transition event handlers
            btnFilterDates.MouseEnter += (s, e) => {
                btnFilterDates.FillColor = Color.FromArgb(79, 70, 229); // Brighter Indigo on hover
            };

            btnFilterDates.MouseLeave += (s, e) => {
                btnFilterDates.FillColor = Color.FromArgb(30, 64, 175);
            };
        }

        private async void btnNavHistory_Click(object sender, EventArgs e)
        {
            await SwitchView("History");
        }

        private void ShowLoadingState(Control container, bool show)
        {
            if (show)
            {
                if (loadingOverlay == null || loadingOverlay.Parent != container)
                {
                    loadingOverlay?.Dispose();
                    loadingOverlay = new Panel
                    {
                        BackColor = DrawColor.FromArgb(248, 250, 252),
                        Dock = DockStyle.Fill
                    };

                    Label lblLoading = new Label
                    {
                        Text = "Loading system data...",
                        Font = new DrawFont("Segoe UI", 11F, FontStyle.Bold),
                        ForeColor = ThemeManager.TextSecondary,
                        AutoSize = true
                    };

                    // Center the loading text
                    lblLoading.Location = new DrawPoint(
                        (container.Width - 160) / 2,
                        (container.Height - 40) / 2
                    );
                    lblLoading.Anchor = AnchorStyles.None;
                    loadingOverlay.Controls.Add(lblLoading);
                    container.Controls.Add(loadingOverlay);
                }
                loadingOverlay.BringToFront();
                loadingOverlay.Visible = true;
            }
            else
            {
                if (loadingOverlay != null)
                {
                    loadingOverlay.Visible = false;
                }
            }
        }

        private async Task LoadGridRowsStaggered(Guna.UI2.WinForms.Guna2DataGridView grid, List<object[]> rowDataList)
        {
            if (grid == null) return;

            grid.Rows.Clear();

            // If there is no data, exit early
            if (rowDataList == null || !rowDataList.Any()) return;

            // Temporarily disable redrawing to prevent flickering while rows stream in
            grid.SuspendLayout();

            foreach (var rowValues in rowDataList)
            {
                grid.Rows.Add(rowValues);

                // Refresh layout periodically or every few rows to keep it buttery smooth
                if (grid.Rows.Count % 3 == 0)
                {
                    grid.ResumeLayout(false);
                    grid.PerformLayout();
                    grid.SuspendLayout();
                    await Task.Delay(12); // Micro-delay for the staggered cascade effect
                }
            }

            grid.ResumeLayout(true);
        }

        private void UpdateSidebarInternalUI(bool showDetails)
        {
            var navBtns = new[] { btnHome, btnHistoryNav, btnNavAllItems, btnNavAvailable, btnNavBorrowed, btnNavBorrowers };
            string[] navTexts = { "HOME PAGE", "HISTORY", "ALL ITEMS", "AVAILABLE", "BORROWED", "RECORDS" };

            if (picUser != null) picUser.SetBounds((showDetails) ? 15 : 12, 25, (showDetails) ? 45 : 40, (showDetails) ? 45 : 40);
            if (lblOwnerRole != null) lblOwnerRole.Visible = showDetails;
            if (cmbAccountActions != null) cmbAccountActions.Visible = showDetails;

            int startY = 130;
            int endY = pnlSidebar.Height - 30;
            int availableHeight = endY - startY;

            int buttonHeight = 45;
            int totalButtonHeight = navBtns.Length * buttonHeight;

            int remainingSpace = availableHeight - totalButtonHeight;
            int gap = remainingSpace / (navBtns.Length - 1);

            if (gap < 5) gap = 5;
            if (gap > 35) gap = 35;

            int currentY = startY;

            for (int i = 0; i < navBtns.Length; i++)
            {
                if (navBtns[i] != null)
                {
                    navBtns[i].Width = pnlSidebar.Width - 20;
                    navBtns[i].Location = new DrawPoint(10, currentY);

                    currentY += buttonHeight + gap;

                    navBtns[i].ImageAlign = HorizontalAlignment.Left;

                    if (showDetails)
                    {
                        navBtns[i].Text = "   " + navTexts[i];
                    }
                    else
                    {
                        navBtns[i].Text = "";
                    }
                }
            }
        }

        private void SidebarTimer_Tick(object sender, EventArgs e)
        {
            // Suspend the specific panel that holds the grid to stop the flicker
            if (pnlMainContent != null) pnlMainContent.SuspendLayout();
            this.SuspendLayout();

            int targetWidth = isSidebarExpanded ? sidebarMinWidth : sidebarMaxWidth;
            int remainingDistance = Math.Abs(targetWidth - pnlSidebar.Width);

            int step = remainingDistance / 2;
            if (step < 40) step = 40;

            if (isSidebarExpanded)
            {
                pnlSidebar.Width -= step;
                if (pnlSidebar.Width <= sidebarMinWidth)
                {
                    pnlSidebar.Width = sidebarMinWidth;
                    isSidebarExpanded = false;
                    sidebarTimer.Stop();
                }
            }
            else
            {
                pnlSidebar.Width += step;
                if (pnlSidebar.Width >= sidebarMaxWidth)
                {
                    pnlSidebar.Width = sidebarMaxWidth;
                    isSidebarExpanded = true;
                    sidebarTimer.Stop();

                    UpdateSidebarInternalUI(true);
                }
            }

            DrawRect safeClientArea = this.DisplayRectangle;
            pnlMainContent.Left = pnlSidebar.Right;
            pnlMainContent.Width = safeClientArea.Right - pnlSidebar.Right;

            int margin = 30;
            DrawRect innerSafeArea = new DrawRect(margin, margin, pnlMainContent.Width - (margin * 2), pnlMainContent.Height - (margin * 2));

            if (pnlHomeSummary != null && pnlHomeSummary.Visible)
            {
                pnlHomeSummary.Bounds = innerSafeArea;
                ArrangeHomeView();
            }

            if (pnlGridContainer != null && pnlGridContainer.Visible)
            {
                pnlGridContainer.Bounds = innerSafeArea;
                ArrangeInventoryView();
            }

            if (pnlHistory != null && pnlHistory.Visible)
            {
                pnlHistory.Bounds = innerSafeArea;
                ArrangeHistoryView();
            }

            this.ResumeLayout(true);
            // Resume the main content layout immediately after the math is done
            if (pnlMainContent != null) pnlMainContent.ResumeLayout(true);
        }
        #endregion

        #region Navigation & Data Loading
        private async Task<List<InventoryItem>> GetDamagedItemsAsync()
        {
            return (await _inventoryService.GetAllItemsAsync())
                .Where(i => i.Condition.ToString() == "Damaged" ||
                            i.Condition.ToString() == "Broken" ||
                            i.Condition.ToString() == "NeedsRepair")
                .ToList();
        }

        private void HighlightActiveButton(Guna2Button activeBtn)
        {
            var navBtns = new[] { btnHome, btnHistoryNav, btnNavAllItems, btnNavAvailable, btnNavBorrowed, btnNavBorrowers };
            foreach (var btn in navBtns)
            {
                if (btn != null)
                {
                    btn.FillColor = DrawColor.Transparent;
                    btn.CustomBorderThickness = new Padding(0);
                    btn.Font = new DrawFont("Segoe UI Semibold", 10.5F, FontStyle.Bold);
                    btn.ForeColor = DrawColor.White;
                }
            }

            if (activeBtn != null)
            {
                activeBtn.FillColor = DrawColor.FromArgb(40, 255, 255, 255);
                activeBtn.CustomBorderColor = DrawColor.Orange;
                activeBtn.CustomBorderThickness = new Padding(4, 0, 0, 0);
                activeBtn.Font = new DrawFont("Segoe UI", 11F, FontStyle.Bold);
                activeBtn.ForeColor = DrawColor.White;
            }
        }

        private void SignOutAdmin()
        {
            if (MessageBox.Show("Are you sure you want to sign out?", "Ventrix System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                isSigningOut = true; // Prevents Application.Exit() from killing the app

                if (_borrowerPortal != null && !_borrowerPortal.IsDisposed)
                {
                    // Restore the hidden/tablet portal to the primary screen
                    _borrowerPortal.ReturnToMainScreen();
                    _borrowerPortal.Show();
                    _borrowerPortal.BringToFront();
                }
                else
                {
                    var borrowerPortal = new BorrowerPortal(_inventoryService, _borrowService, _userService);
                    borrowerPortal.Show();
                }

                // Close the dashboard
                this.Close();
            }
            else
            {
                // Reset the combobox selection if they cancel
                if (cmbAccountActions != null) cmbAccountActions.SelectedIndex = -1;
            }
        }

        private async Task BatchGenerateAllItemQRCodesAsync()
        {
            // Change this path if you want to save them somewhere else like your Desktop
            string saveDirectory = @"C:\Ventrix_QR_Tags\Items";

            // Create the folder immediately if it doesn't exist
            if (!Directory.Exists(saveDirectory))
            {
                Directory.CreateDirectory(saveDirectory);
            }

            // Initialize the service
            var qrService = new QrCodeService();

            // Fetch every single physical item unit from the database
            var allItems = await _inventoryService.GetAllItemsAsync();

            if (!allItems.Any())
            {
                MessageBox.Show("There are no items in the inventory to generate codes for.", "Empty Inventory", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            int generatedCount = 0;

            foreach (var item in allItems)
            {
                // Generate the composite image with the QR code and the text label
                var sticker = qrService.GenerateItemQrCodeWithLabel(item.Id, item.Name);

                // Save the image. We use the ID and Name to ensure every file is uniquely named!
                string fileName = $"Item_{item.Id}_{item.Name}";
                qrService.SaveQrSticker(sticker, saveDirectory, fileName);

                generatedCount++;
            }

            // Let the admin know it finished successfully
            MessageBox.Show($"Successfully generated and saved {generatedCount} QR tags!\n\nYou can find them all at:\n{saveDirectory}", "Batch Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private async Task SwitchView(string viewName, string filter = "All")
        {
            Control activePanel = null;
            if (pnlHomeSummary != null) pnlHomeSummary.Visible = false;
            if (pnlGridContainer != null) pnlGridContainer.Visible = false;
            if (pnlHistory != null) pnlHistory.Visible = false;

            Control targetPanel = null;
            if (viewName == "Home") targetPanel = pnlHomeSummary;
            else if (viewName == "Inventory") targetPanel = pnlGridContainer;
            else if (viewName == "History") targetPanel = pnlHistory;

            // Permanently hide the old individual legacy buttons
            if (btnCreate != null) btnCreate.Visible = false;
            if (btnEdit != null) btnEdit.Visible = false;
            if (btnDelete != null) btnDelete.Visible = false;
            if (btnExportExcel != null) btnExportExcel.Visible = false;
            if (btnExportPDF != null) btnExportPDF.Visible = false;

            // 1. Reset the Actions Menu visibility by default (Hidden on Home and Records)
            if (btnActionsMenu != null) btnActionsMenu.Visible = false;

            if (viewName == "Home")
            {
                HighlightActiveButton(btnHome);
                if (pnlHomeSummary != null) { pnlHomeSummary.Visible = true; pnlHomeSummary.BringToFront(); }
                await LoadHomeContent();

                if (lblDashboardHeader != null) lblDashboardHeader.Text = $"{GetGreeting()}, ADMIN";
            }
            else if (viewName == "Inventory")
            {
                if (lblDashboardHeader != null) lblDashboardHeader.Text = $"INVENTORY: {filter.ToUpper()}";

                if (filter == "All") HighlightActiveButton(btnNavAllItems);
                else if (filter == "Available") HighlightActiveButton(btnNavAvailable);
                else if (filter == "Borrowed") HighlightActiveButton(btnNavBorrowed);
                else if (filter == "Records") HighlightActiveButton(btnNavBorrowers);

                if (pnlGridContainer != null) { pnlGridContainer.Visible = true; pnlGridContainer.BringToFront(); }

                bool isRecords = filter.Equals("Records", StringComparison.OrdinalIgnoreCase);
                bool showCrud = (filter.Equals("All", StringComparison.OrdinalIgnoreCase) || filter.Equals("Available", StringComparison.OrdinalIgnoreCase));

                if (btnActionsMenu != null && inventoryActionsMenu != null)
                {
                    if (isRecords)
                    {
                        btnActionsMenu.Visible = false;
                    }
                    else
                    {
                        // Reparent back to grid container if it was on history page
                        if (btnActionsMenu.Parent != pnlGridContainer)
                        {
                            pnlGridContainer.Controls.Add(btnActionsMenu);
                        }

                        btnActionsMenu.Visible = true;
                        btnActionsMenu.BringToFront();

                        inventoryActionsMenu.Items[0].Visible = showCrud; // Add Item
                        inventoryActionsMenu.Items[1].Visible = showCrud; // Edit Record
                        inventoryActionsMenu.Items[2].Visible = showCrud; // Delete Item
                        inventoryActionsMenu.Items[3].Visible = showCrud; // Separator
                        inventoryActionsMenu.Items[4].Visible = true;     // Export to Excel
                        inventoryActionsMenu.Items[5].Visible = true;     // Export to PDF
                    }
                }

                if (pnlRegisterBorrower != null)
                {
                    pnlRegisterBorrower.Visible = (filter == "Records");
                    if (pnlRegisterBorrower.Visible) pnlRegisterBorrower.BringToFront();
                }

                await LoadFromDatabase(filter);
            }
            else if (viewName == "History")
            {
                if (lblDashboardHeader != null) lblDashboardHeader.Text = "TRANSACTION AUDIT HISTORY";
                HighlightActiveButton(btnHistoryNav);

                if (pnlHistory != null)
                {
                    pnlHistory.Visible = true;
                    pnlHistory.BringToFront();
                }

                if (btnActionsMenu != null && inventoryActionsMenu != null)
                {
                    // Reparent to history container if it was on inventory page
                    if (btnActionsMenu.Parent != pnlHistory)
                    {
                        pnlHistory.Controls.Add(btnActionsMenu);
                    }

                    btnActionsMenu.Visible = true;
                    btnActionsMenu.BringToFront();

                    inventoryActionsMenu.Items[0].Visible = false; // Add Item
                    inventoryActionsMenu.Items[1].Visible = false; // Edit Record
                    inventoryActionsMenu.Items[2].Visible = false; // Delete Item
                    inventoryActionsMenu.Items[3].Visible = false; // Separator
                    inventoryActionsMenu.Items[4].Visible = true;  // Export to Excel
                    inventoryActionsMenu.Items[5].Visible = true;  // Export to PDF
                }

                await LoadHistoryData();
            }

            RefreshLayout();

            if (activePanel != null && targetPanel != null && activePanel != targetPanel)
            {
                ThemeManager.SwapViews(viewTransition, activePanel, targetPanel);
            }
            else if (targetPanel != null)
            {
                targetPanel.Visible = true;
                targetPanel.BringToFront();
            }
            await UpdateDashboardCounts();
        }

        private void SetupHistoryAdvancedControls()
        {
            // Modernized Start Date Picker
            dtpStartDate = new Guna.UI2.WinForms.Guna2DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 140,
                Height = 38,
                Value = DateTime.Today.AddMonths(-1),
                BorderRadius = 8,
                BorderColor = Color.FromArgb(203, 213, 225),
                BorderThickness = 1,
                FillColor = Color.White,
                ForeColor = Color.FromArgb(51, 65, 85),
                Font = new DrawFont("Segoe UI", 9.5F),
                Animated = false,
                Cursor = Cursors.Hand
            };
            dtpStartDate.HoverState.BorderColor = Color.FromArgb(129, 140, 248); // Soft indigo hover

            // Modernized End Date Picker
            dtpEndDate = new Guna.UI2.WinForms.Guna2DateTimePicker
            {
                Format = DateTimePickerFormat.Short,
                Width = 140,
                Height = 38,
                Value = DateTime.Today,
                BorderRadius = 8,
                BorderColor = Color.FromArgb(203, 213, 225),
                BorderThickness = 1,
                FillColor = Color.White,
                ForeColor = Color.FromArgb(51, 65, 85),
                Font = new DrawFont("Segoe UI", 9.5F),
                Animated = false,
                Cursor = Cursors.Hand
            };
            dtpEndDate.HoverState.BorderColor = Color.FromArgb(129, 140, 248);

            // Modernized Apply Button matching the Ventrix Indigo theme
            btnApplyFilters = new Guna.UI2.WinForms.Guna2Button
            {
                Text = "Filter Dates",
                FillColor = Color.FromArgb(79, 70, 229), // Ventrix Indigo
                ForeColor = Color.White,
                BorderRadius = 8,
                Height = 40,
                Width = 125,
                Font = new DrawFont("Segoe UI", 9.5F, FontStyle.Bold),
                Animated = true,
                Cursor = Cursors.Hand
            };

            btnApplyFilters.HoverState.FillColor = Color.FromArgb(99, 102, 241);
            btnApplyFilters.Click += async (s, e) => { historyCurrentPage = 1; await LoadHistoryData(); };
            btnFilterDates = btnApplyFilters;

            btnPrevPage = new Guna.UI2.WinForms.Guna2Button { Text = "‹ Prev", FillColor = Color.FromArgb(241, 245, 249), ForeColor = Color.FromArgb(51, 65, 85), BorderRadius = 6, Width = 70, Height = 32 };
            btnPrevPage.Click += async (s, e) => { if (historyCurrentPage > 1) { historyCurrentPage--; await LoadHistoryData(); } };

            btnNextPage = new Guna.UI2.WinForms.Guna2Button { Text = "Next ›", FillColor = Color.FromArgb(241, 245, 249), ForeColor = Color.FromArgb(51, 65, 85), BorderRadius = 6, Width = 70, Height = 32 };
            btnNextPage.Click += async (s, e) => { if (historyCurrentPage < historyTotalPages) { historyCurrentPage++; await LoadHistoryData(); } };

            lblPageInfo = new Label { Text = "Page 1 of 1", AutoSize = true, Font = new DrawFont("Segoe UI", 9.5F, FontStyle.Bold), ForeColor = Color.FromArgb(100, 116, 139) };

            StyleModernDatePickers();
            AttachFilterButtonAnimations();
        }

        private async Task LoadFromDatabase(string filter)
        {
            if (dgvInventory == null) return;
            ShowLoadingState(pnlGridContainer, true);

            try
            {
                dgvInventory.Rows.Clear();
                dgvInventory.Columns.Clear();

                // FIXED: Added Async() to the method call
                var items = await _inventoryService.GetAllItemsAsync();
                string search = txtSearch?.Text?.ToLower() ?? "";

                if (!string.IsNullOrEmpty(search))
                {
                    items = items.Where(i => i.Name.ToLower().Contains(search) || i.Category.ToString().ToLower().Contains(search)).ToList();
                }

                // 1. UPDATE THIS LINE ("All" Filter)
                if (filter.Equals("All", StringComparison.OrdinalIgnoreCase))
                {
                    SetupColumns("Item Name", "Category", "Total Units", "Available", "Borrowed", "Damaged");
                    var groupedItems = items.GroupBy(i => new { BaseName = GetBaseItemName(i.Name), i.Category });

                    var allRecords = await _borrowService.GetAllBorrowRecordsAsync();
                    var outRecords = allRecords.Where(r => r.Status == BorrowStatus.Active || r.Status == BorrowStatus.PendingReturn || r.Status == BorrowStatus.Overdue).ToList();

                    // NEW: Get IDs of physical items tied up in pending requests
                    var pendingItemIds = allRecords.Where(r => r.Status == BorrowStatus.Pending).Select(r => r.InventoryItemId).ToList();

                    foreach (var group in groupedItems)
                    {
                        int total = group.Count();

                        // CHANGED: Available = Physical Available minus those in Pending status
                        int avail = group.Count(x => x.Status == ItemStatus.Available && !pendingItemIds.Contains(x.Id));

                        int borrowed = outRecords.Count(r => GetBaseItemName(r.ItemName ?? "") == group.Key.BaseName);
                        int damaged = group.Count(x => x.Condition == Condition.Damaged);

                        dgvInventory.Rows.Add(group.Key.BaseName, group.Key.Category.ToString(), total, avail, borrowed, damaged);
                    }
                    if (dgvInventory.Columns.Contains("ItemName")) dgvInventory.Columns["ItemName"].FillWeight = 150;
                }

                // 2. UPDATE THIS LINE ("Records" Filter - No changes needed here)
                else if (filter.Equals("Records", StringComparison.OrdinalIgnoreCase))
                {
                    SetupColumns("Borrower ID", "Borrower Name", "Role", "Items Held", "Strikes", "Account Status");

                    var users = (await _userService.GetAllUsersAsync())
                                .Where(u => u.Role != UserRole.Admin)
                                .ToList();

                    if (!string.IsNullOrEmpty(search))
                        users = users.Where(u => u.FirstName.ToLower().Contains(search) || u.LastName.ToLower().Contains(search) || u.UserId.ToLower().Contains(search)).ToList();

                    var records = await _borrowService.GetAllBorrowRecordsAsync();

                    foreach (var u in users)
                    {
                        int itemsHeld = records.Count(r => r.BorrowerId == u.UserId &&
                                     (r.Status == BorrowStatus.Active ||
                                      r.Status == BorrowStatus.Overdue ||
                                      r.Status == BorrowStatus.PendingReturn));
                        string accountStatus = u.Strikes >= 3 ? "LOCKED" : "ACTIVE";

                        dgvInventory.Rows.Add(u.UserId, u.FullName, u.Role.ToString(), itemsHeld, u.Strikes, accountStatus);
                    }
                }

                // 3. UPDATE THIS LINE ("Borrowed" Filter - No changes needed here)
                else if (filter.Equals("Borrowed", StringComparison.OrdinalIgnoreCase))
                {
                    SetupColumns("RecordIDs", "Borrower ID", "Borrower Name", "Items", "Requested/Approved On", "Due Date", "Status");
                    dgvInventory.Columns["RecordIDs"].Visible = false;

                    var allRecords = await _borrowService.GetAllBorrowRecordsAsync();
                    var pendingAndActive = allRecords
                        .Where(b => b.Status == BorrowStatus.Active || b.Status == BorrowStatus.Pending ||
                                    b.Status == BorrowStatus.Overdue || b.Status == BorrowStatus.PendingReturn)
                        .ToList();

                    if (!string.IsNullOrEmpty(search))
                    {
                        pendingAndActive = pendingAndActive.Where(b =>
                            (b.BorrowerId != null && b.BorrowerId.ToLower().Contains(search)) ||
                            (b.Borrower != null && b.Borrower.FullName.ToLower().Contains(search)) ||
                            (b.ItemName != null && b.ItemName.ToLower().Contains(search)) ||
                            b.Status.ToString().ToLower().Contains(search)
                        ).ToList();
                    }

                    var groupedRecords = pendingAndActive
                        .GroupBy(b => new { b.BorrowerId, b.Status })
                        .Select(g => new
                        {
                            BorrowerId = g.Key.BorrowerId,
                            BorrowerName = g.First().Borrower != null ? g.First().Borrower.FullName : "Unknown",
                            Status = g.Key.Status,
                            LastUpdate = g.Max(x => x.BorrowDate),
                            Items = string.Join(", ", g.Select(x => x.ItemName ?? "Unknown Item")),
                            RecordIDs = string.Join(",", g.Select(x => x.Id))
                        })
                        .OrderBy(g => g.Status)
                        .ThenByDescending(g => g.LastUpdate)
                        .ToList();

                    foreach (var group in groupedRecords)
                    {
                        string dueDateStr;

                        if (group.Status == BorrowStatus.Pending)
                        {
                            dueDateStr = "---";
                        }
                        else
                        {
                            dueDateStr = group.LastUpdate.AddDays(1).ToString("MMM dd, yyyy");
                        }

                        dgvInventory.Rows.Add(
                            group.RecordIDs,
                            group.BorrowerId,
                            group.BorrowerName,
                            group.Items,
                            group.LastUpdate.ToString("MMM dd, yyyy"),
                            dueDateStr,
                            group.Status.ToString()
                        );
                    }

                    if (dgvInventory.Columns.Contains("Items")) dgvInventory.Columns["Items"].FillWeight = 150;
                    if (dgvInventory.Columns.Contains("DueDate")) dgvInventory.Columns["DueDate"].FillWeight = 110;
                }

                // 4. UPDATE THIS LINE ("Available" Filter)
                else if (filter.Equals("Available", StringComparison.OrdinalIgnoreCase))
                {
                    SetupColumns("Item Name", "Category", "Available Units");

                    // NEW: Get IDs of items tied up in pending requests
                    var pendingItemIds = (await _borrowService.GetAllBorrowRecordsAsync())
                        .Where(b => b.Status == BorrowStatus.Pending)
                        .Select(b => b.InventoryItemId)
                        .ToList();

                    // CHANGED: Filter out items that are physically available BUT stuck in Pending
                    var groupedItems = items
                        .Where(i => i.Status == ItemStatus.Available && !pendingItemIds.Contains(i.Id))
                        .GroupBy(i => new { BaseName = GetBaseItemName(i.Name), i.Category })
                        .ToList();

                    var rowsToLoad = new List<object[]>();
                    foreach (var group in groupedItems)
                    {
                        rowsToLoad.Add(new object[] { group.Key.BaseName, group.Key.Category.ToString(), group.Count() });
                    }

                    // 2. Load with the smooth staggered waterfall animation
                    await LoadGridRowsStaggered(dgvInventory, rowsToLoad);

                    if (dgvInventory.Columns.Contains("ItemName")) dgvInventory.Columns["ItemName"].FillWeight = 150;
                }
            }
            finally
            {
                // Hide the loading overlay once rendering is complete
                ShowLoadingState(pnlGridContainer, false);
            }
            ToggleNoResultsState(dgvInventory.Rows.Count == 0);
        }

        private string GetBaseItemName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Unknown Item";
            int hashIndex = name.IndexOf(" #");
            return hashIndex > 0 ? name.Substring(0, hashIndex).Trim() : name.Trim();
        }

        private async Task LoadHomeContent()
        {
            await _borrowService.ProcessAutomaticOverdueStrikesAsync(_userService);

            if (flowRecentActivity == null) return;
            var damagedItems = await GetDamagedItemsAsync();

            flowRecentActivity.SuspendLayout();
            flowRecentActivity.Controls.Clear();

            if (!damagedItems.Any()) AddDashboardAlert("✓ All laboratory systems are operational.", DrawColor.Teal);
            else AddDashboardAlert($"⚠ REPAIR NEEDED: {damagedItems.Count} items require attention. (Click for details)", DrawColor.DarkRed);

            var lblActivityHeader = new Label 
{ 
    Text = "ACTIVITY FEED", 
    Font = new DrawFont("Segoe UI", 9.5F, FontStyle.Bold), 
    ForeColor = DrawColor.FromArgb(100, 116, 139), // Modern muted slate-gray
    AutoSize = true,
    Margin = new Padding(2, 25, 0, 12) // Give it more breathing room
};
flowRecentActivity?.Controls.Add(lblActivityHeader);

            var rawLogs = (await _borrowService.GetAllBorrowRecordsAsync())
                  .Where(b => b.IsHiddenFromDashboard == false)
                  .OrderByDescending(b => b.Status == BorrowStatus.Returned ? (b.ReturnDate ?? b.BorrowDate) : b.BorrowDate)
                  .ToList();

            var displayGroups = new List<List<BorrowRecord>>();

            foreach (var record in rawLogs)
            {
                var recordTime = record.Status == BorrowStatus.Returned ? (record.ReturnDate ?? record.BorrowDate) : record.BorrowDate;
                bool isReturned = record.Status == BorrowStatus.Returned;

                var existingGroup = displayGroups.FirstOrDefault(g =>
                    g.First().BorrowerId == record.BorrowerId &&
                    (g.First().Status == BorrowStatus.Returned) == isReturned &&
                    Math.Abs(((g.First().Status == BorrowStatus.Returned ? (g.First().ReturnDate ?? g.First().BorrowDate) : g.First().BorrowDate) - recordTime).TotalMinutes) <= 5
                );

                if (existingGroup != null)
                {
                    existingGroup.Add(record);
                }
                else
                {
                    displayGroups.Add(new List<BorrowRecord> { record });
                }
            }

            foreach (var group in displayGroups.Take(5))
            {
                var firstRecord = group.First();

                string friendlyName = firstRecord.Borrower != null && !string.IsNullOrWhiteSpace(firstRecord.Borrower.FirstName)
                    ? firstRecord.Borrower.FirstName
                    : (!string.IsNullOrWhiteSpace(firstRecord.BorrowerId) ? firstRecord.BorrowerId : "Unknown User");

                var itemSummaries = group
                    .GroupBy(b => GetBaseItemName(b.ItemName ?? "Item"))
                    .Select(g => $"{g.Count()} {g.Key}" + (g.Count() > 1 && !g.Key.EndsWith("s", StringComparison.OrdinalIgnoreCase) ? "s" : ""))
                    .ToList();

                string displayItem = string.Join(", ", itemSummaries);

                bool isReturned = firstRecord.Status == BorrowStatus.Returned;
                bool isPendingReturn = group.Any(b => b.Status == BorrowStatus.PendingReturn);
                bool hasActive = group.Any(b => b.Status == BorrowStatus.Active || b.Status == BorrowStatus.Overdue);

                string actionText;
                DrawColor statusColor;

                // Use active voice and HTML tags (<b>) so we can bold just the user's name later
                if (isReturned)
                {
                    actionText = $"<b>{friendlyName}</b> returned {displayItem}";
                    statusColor = DrawColor.FromArgb(16, 185, 129); // Modern Emerald Green
                }
                else if (isPendingReturn)
                {
                    actionText = $"<b>{friendlyName}</b> requested to return {displayItem}";
                    statusColor = DrawColor.FromArgb(139, 92, 246); // Modern Violet
                }
                else if (hasActive)
                {
                    actionText = $"<b>{friendlyName}</b> borrowed {displayItem}";
                    statusColor = DrawColor.FromArgb(59, 130, 246); // Modern Blue
                }
                else
                {
                    actionText = $"<b>{friendlyName}</b> requested {displayItem}";
                    statusColor = DrawColor.FromArgb(245, 158, 11); // Modern Amber
                }

                DateTime actionTime = isReturned ? (firstRecord.ReturnDate ?? firstRecord.BorrowDate) : firstRecord.BorrowDate;

                AddActivityCard(actionText, actionTime, statusColor);
            }

            flowRecentActivity.ResumeLayout(true);
        }

        private void AddDashboardAlert(string message, DrawColor color)
        {
            var alert = new AlertTile(message, color);

            int safeWidth = flowRecentActivity.ClientSize.Width > 0 ? flowRecentActivity.ClientSize.Width : flowRecentActivity.Width;
            alert.Width = safeWidth - flowRecentActivity.Padding.Left - flowRecentActivity.Padding.Right - 10;

            alert.AlertClicked += async (s, e) => {
                if (message.Contains("REPAIR"))
                {
                    var damagedItems = await GetDamagedItemsAsync();
                    using (var popup = new RepairDetailsPopup(damagedItems, _inventoryService, async () => {
                        await LoadHomeContent();
                        await UpdateDashboardCounts();
                    }))
                    {
                        ShowPopupWithFade(popup);
                    }
                }
            };
            flowRecentActivity?.Controls.Add(alert);
        }

        private void AddActivityCard(string message, DateTime time, DrawColor statusColor)
        {
            var card = new Ventrix.App.Controls.ActivityCard(message, time, statusColor);

            int safeWidth = flowRecentActivity.ClientSize.Width > 0 ? flowRecentActivity.ClientSize.Width : flowRecentActivity.Width;
            int targetWidth = safeWidth - flowRecentActivity.Padding.Left - flowRecentActivity.Padding.Right - 10;
            card.Width = targetWidth;

            // Start with height 0 and invisible to create a smooth upward rollout effect
            int targetHeight = card.Height;
            card.Height = 0;
            card.Visible = false;

            flowRecentActivity?.Controls.Add(card);
            card.Visible = true;

            // Use a high-performance local timer for the slide-up effect
            var animTimer = new System.Windows.Forms.Timer();
            animTimer.Interval = 10; // ~100 FPS for smoothness

            animTimer.Tick += (s, e) =>
            {
                // Smoothly step closer to the target height (easing calculation)
                int step = Math.Max(1, (targetHeight - card.Height) / 4);
                card.Height += step;

                // Force the layout engine to reflow surrounding cards smoothly
                flowRecentActivity.PerformLayout();

                // Stop when it reaches full size
                if (card.Height >= targetHeight)
                {
                    card.Height = targetHeight;
                    animTimer.Stop();
                    animTimer.Dispose();
                }
            };

            animTimer.Start();
        }

        private async Task<IEnumerable<BorrowRecord>> GetFilteredHistoryQuery()
        {
            var query = await _borrowService.GetAllBorrowRecordsAsync();

            if (dtpStartDate != null && dtpEndDate != null)
            {
                DateTime endOfDay = dtpEndDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(b => b.BorrowDate >= dtpStartDate.Value.Date && b.BorrowDate <= endOfDay);
            }

            if (txtSearch != null && !string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                string search = txtSearch.Text.ToLower();
                query = query.Where(l =>
                    (l.ItemName != null && l.ItemName.ToLower().Contains(search)) ||
                    (l.Borrower != null && l.Borrower.FullName.ToLower().Contains(search)) ||
                    (l.BorrowerId != null && l.BorrowerId.ToLower().Contains(search)));
            }

            switch (historySortColumn)
            {
                case "Item":
                    return historySortDescending ? query.OrderByDescending(b => b.ItemName) : query.OrderBy(b => b.ItemName);

                case "Borrower":
                    return historySortDescending ? query.OrderByDescending(b => b.BorrowerId).ThenByDescending(b => b.BorrowDate) : query.OrderBy(b => b.BorrowerId).ThenByDescending(b => b.BorrowDate);

                case "RTime":
                    return historySortDescending ? query.OrderByDescending(b => b.ReturnDate ?? DateTime.MaxValue) : query.OrderBy(b => b.ReturnDate ?? DateTime.MinValue);
                case "Status":
                    return historySortDescending ? query.OrderByDescending(b => b.Status.ToString()) : query.OrderBy(b => b.Status.ToString());
                case "BTime":
                default:
                    return historySortDescending ? query.OrderByDescending(b => b.BorrowDate) : query.OrderBy(b => b.BorrowDate);
            }
        }

        private async Task LoadHistoryData()
        {
            if (dgvHistory == null) return;

            ShowLoadingState(pnlHistory, true);
            dgvHistory.Rows.Clear();
            dgvHistory.Columns.Clear();

            // Setup columns
            dgvHistory.Columns.Add("SchoolId", "School ID");
            dgvHistory.Columns.Add("Borrower", "Borrower Name");
            dgvHistory.Columns.Add("TotalBorrowed", "Total Borrows (In Period)");
            dgvHistory.Columns.Add("ActiveHeld", "Currently Unreturned");
            dgvHistory.Columns.Add("LastActive", "Last Activity");

            var allLogs = await _borrowService.GetAllBorrowRecordsAsync();

            // 1. Apply Date Filters
            if (dtpStartDate != null && dtpEndDate != null)
            {
                DateTime endOfDay = dtpEndDate.Value.Date.AddDays(1).AddTicks(-1);
                allLogs = allLogs.Where(b => b.BorrowDate >= dtpStartDate.Value.Date && b.BorrowDate <= endOfDay).ToList();
            }

            // 2. Search Logic
            string search = txtSearch?.Text?.ToLower() ?? "";
            if (!string.IsNullOrEmpty(search))
            {
                allLogs = allLogs.Where(b =>
                    (b.BorrowerId != null && b.BorrowerId.ToLower().Contains(search)) ||
                    (b.Borrower != null && b.Borrower.FullName.ToLower().Contains(search)) ||
                    (b.ItemName != null && b.ItemName.ToLower().Contains(search))
                ).ToList();
            }

            // 3. Group the data 
            var userGroupsQuery = allLogs.GroupBy(b => b.BorrowerId).Select(g => new {
                BorrowerId = g.Key,
                BorrowerName = g.First().Borrower != null ? g.First().Borrower.FullName : "Unknown",
                TotalBorrowed = g.Count(),
                ActiveHeld = g.Count(b => b.Status == BorrowStatus.Active),
                LastActive = g.Max(b => b.BorrowDate)
            });

            // 4. Column Sorting
            switch (historySortColumn)
            {
                case "SchoolId":
                    userGroupsQuery = historySortDescending ? userGroupsQuery.OrderByDescending(x => x.BorrowerId) : userGroupsQuery.OrderBy(x => x.BorrowerId);
                    break;
                case "Borrower":
                    userGroupsQuery = historySortDescending ? userGroupsQuery.OrderByDescending(x => x.BorrowerName) : userGroupsQuery.OrderBy(x => x.BorrowerName);
                    break;
                case "TotalBorrowed":
                    userGroupsQuery = historySortDescending ? userGroupsQuery.OrderByDescending(x => x.TotalBorrowed) : userGroupsQuery.OrderBy(x => x.TotalBorrowed);
                    break;
                case "ActiveHeld":
                    userGroupsQuery = historySortDescending ? userGroupsQuery.OrderByDescending(x => x.ActiveHeld) : userGroupsQuery.OrderBy(x => x.ActiveHeld);
                    break;
                case "LastActive":
                default:
                    userGroupsQuery = historySortDescending ? userGroupsQuery.OrderByDescending(x => x.LastActive) : userGroupsQuery.OrderBy(x => x.LastActive);
                    break;
            }

            // 5. Build row collection for smooth animation
            var rowsToLoad = new List<object[]>();
            foreach (var group in userGroupsQuery)
            {
                rowsToLoad.Add(new object[] {
            group.BorrowerId,
            group.BorrowerName,
            group.TotalBorrowed,
            group.ActiveHeld,
            group.LastActive.ToString("MMM dd, yyyy")
        });
            }

            StyleHistoryDataGridView();

            // Stream rows in with buttery-smooth staggered animation
            await LoadGridRowsStaggered(dgvHistory, rowsToLoad);

            ShowLoadingState(pnlHistory, false);
            ToggleNoResultsState(dgvHistory.Rows.Count == 0);
        }

        private async Task UpdateDashboardCounts()
        {
            var items = (await _inventoryService.GetAllItemsAsync())?.ToList() ?? new List<InventoryItem>();
            var records = (await _borrowService.GetAllBorrowRecordsAsync())?.ToList() ?? new List<BorrowRecord>();
            var users = (await _userService.GetAllUsersAsync())?.ToList() ?? new List<User>();
            int damagedCount = (await GetDamagedItemsAsync()).Count;
            int borrowerCount = users.Count(u => u.Role != UserRole.Admin);

            if (badgeHealth != null)
            {
                if (damagedCount > 0)
                {
                    badgeHealth.Text = $"SYSTEM ALERTS: {damagedCount} ISSUES";
                    badgeHealth.FillColor = DrawColor.FromArgb(255, 235, 238);
                    badgeHealth.ForeColor = DrawColor.Red;
                }
                else
                {
                    badgeHealth.Text = "ALL SYSTEMS OPERATIONAL";
                    badgeHealth.FillColor = DrawColor.FromArgb(232, 245, 233);
                    badgeHealth.ForeColor = DrawColor.MediumSeaGreen;
                }
            }

            int borrowedCount = records.Count(x => x.Status == BorrowStatus.Active || x.Status == BorrowStatus.PendingReturn || x.Status == BorrowStatus.Overdue);

            // Upgraded with smooth count-up animations
            _ = AnimateCountUp(cardTotal, "TOTAL ITEMS", items.Count, DrawColor.FromArgb(79, 70, 229));
            _ = AnimateCountUp(cardAvailable, "AVAILABLE", items.Count(x => x.Status == ItemStatus.Available), DrawColor.Teal);
            _ = AnimateCountUp(cardBorrowed, "BORROWED", borrowedCount, DrawColor.FromArgb(225, 29, 72));
            _ = AnimateCountUp(cardRecords, "BORROWERS", borrowerCount, DrawColor.FromArgb(217, 119, 6));

        }

        private string GetGreeting()
        {
            int hour = DateTime.Now.Hour;
            if (hour < 12) return "GOOD MORNING";
            if (hour < 17) return "GOOD AFTERNOON";
            return "GOOD EVENING";
        }
        #endregion

        #region CRUD & User Registration Actions

        private async void btnRegisterBorrower_Click(object sender, EventArgs e)
        {
            // 1. Unified Validation
            if (string.IsNullOrWhiteSpace(txtRegFirstName.Text) || string.IsNullOrWhiteSpace(txtRegLastName.Text))
            {
                MessageBox.Show("Please fill in the required First and Last Name fields.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtRegSchoolId.Text))
            {
                MessageBox.Show("Please enter an ID number for this user.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // 2. Process Registration
            try
            {
                var registrationData = new Ventrix.Application.DTOs.RegisterDTO
                {
                    UserId = txtRegSchoolId.Text.Trim(),
                    FirstName = txtRegFirstName.Text.Trim(),
                    LastName = txtRegLastName.Text.Trim(),
                    Role = Enum.Parse<Ventrix.Domain.Enums.UserRole>(cmbRegRole.SelectedItem?.ToString() ?? "Student"),
                };

                var registeredUser = await _userService.RegisterNewBorrowerAsync(registrationData);

                // 3. Success & Reset
                MessageBox.Show($"Registration successful!\n\nUser ID: {registeredUser.UserId} has been added to the system.", "Account Created", MessageBoxButtons.OK, MessageBoxIcon.Information);
                // --- AUTOMATIC USER QR GENERATION ---
                // Grab the actual text the admin just typed into the text boxes
                string newUserId = txtRegSchoolId.Text.Trim();
                string newUserName = $"{txtRegFirstName.Text.Trim()} {txtRegLastName.Text.Trim()}";

                var qrService = new Ventrix.Application.Services.QrCodeService();
                var newSticker = qrService.GenerateUserQrCodeWithLabel(newUserId, newUserName);
                qrService.SaveQrSticker(newSticker, @"C:\Ventrix_QR_Tags\Users", $"User_{newUserId}_{txtRegFirstName.Text.Trim()}");

                using (var preview = new Ventrix.App.Popups.QrPreviewPopup(newSticker, "New User QR Tag"))
                {
                    ShowPopupWithFade(preview); // Prompts admin to print immediately
                }
                // -------------------------------------

                txtRegSchoolId.Clear();
                txtRegFirstName.Clear();
                txtRegLastName.Clear();

                await LoadFromDatabase("Records");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Registration Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async Task btnCreate_Click(object sender, EventArgs e)
        {
            using (var popup = new InventoryPopup(_inventoryService))
            {
                if (ShowPopupWithFade(popup) == DialogResult.OK)
                {
                    await LoadFromDatabase("All");
                    await UpdateDashboardCounts();
                    ToastNotification.Show(this, "New item added to inventory!", ToastType.Success);

                    // --- AUTOMATIC ITEM QR GENERATION ---
                    // Ask the database for the item that was just created (the one with the highest ID)
                    var allItems = await _inventoryService.GetAllItemsAsync();
                    var newlyCreatedItem = allItems.OrderByDescending(i => i.Id).FirstOrDefault();

                    if (newlyCreatedItem != null)
                    {
                        int newItemId = newlyCreatedItem.Id;
                        string newItemName = newlyCreatedItem.Name;

                        var qrService = new Ventrix.Application.Services.QrCodeService();
                        var newSticker = qrService.GenerateItemQrCodeWithLabel(newItemId, newItemName);
                        qrService.SaveQrSticker(newSticker, @"C:\Ventrix_QR_Tags\Items", $"Item_{newItemId}_{newItemName}");

                        using (var preview = new Ventrix.App.Popups.QrPreviewPopup(newSticker, "New Item QR Tag"))
                        {
                            ShowPopupWithFade(preview); // Prompts admin to print immediately
                        }
                    }
                    // -------------------------------------
                }
                RefreshLayout();
            }
        }

        private async Task btnEdit_Click(object sender, EventArgs e) { await OpenItemGroupDetails(); }
        private async Task btnDelete_Click(object sender, EventArgs e) { await OpenItemGroupDetails(); }

        private async Task OpenItemGroupDetails()
        {
            if (dgvInventory?.SelectedRows.Count == 0) return;

            if (dgvInventory.Columns.Contains("ItemName"))
            {
                string itemName = dgvInventory.SelectedRows[0].Cells["ItemName"].Value?.ToString() ?? "";
                using (var popup = new ItemGroupPopup(_inventoryService, _borrowService, itemName))
                {
                    ShowPopupWithFade(popup);
                    string currentFilter = lblDashboardHeader.Text.Contains("AVAILABLE") ? "Available" : "All";
                    await SwitchView("Inventory", currentFilter);
                }
            }
        }

        // UPDATED METHOD in AdminDashboard.cs
        private async Task ClearRecentActivity()
        {
            if (MessageBox.Show("Clear the current activity view?", "Update Dashboard", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                // 1. Get the same top 5 records currently being shown in LoadHomeContent
                var currentLogs = (await _borrowService.GetAllBorrowRecordsAsync())
                      .Where(b => b.IsHiddenFromDashboard == false)
                      .OrderByDescending(b => b.Status == BorrowStatus.Returned ? (b.ReturnDate ?? b.BorrowDate) : b.BorrowDate)
                      .Take(5)
                      .ToList();

                // 2. Hide only these specific records
                foreach (var record in currentLogs)
                {
                    await _borrowService.HideRecordFromDashboardAsync(record.Id);
                }

                // 3. Refresh the dashboard
                await LoadHomeContent();
                await UpdateDashboardCounts();

                ToastNotification.Show(this, "Dashboard updated with next recent activities.", ToastType.Info);
            }
        }

        private async Task DgvInventory_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvInventory == null) return;

            // --- NEW POPUP LOGIC FOR THE "BORROWED" TAB ---
            if (dgvInventory.Columns.Contains("RecordIDs") && dgvInventory.Columns.Contains("Items"))
            {
                // Note: Used "BorrowerName" (no space) because your SetupColumns removes spaces!
                string borrowerName = dgvInventory.Rows[e.RowIndex].Cells["BorrowerName"].Value?.ToString() ?? "Unknown Borrower";
                string itemsStr = dgvInventory.Rows[e.RowIndex].Cells["Items"].Value?.ToString() ?? "";
                string status = dgvInventory.Rows[e.RowIndex].Cells["Status"].Value?.ToString() ?? "";

                // Open the dedicated popup form
                using (var popup = new Ventrix.App.Popups.BorrowerItemsPopup(borrowerName, status, itemsStr))
                {
                    // USING YOUR EXISTING FADE METHOD HERE!
                    ShowPopupWithFade(popup);
                }

                // Return so it doesn't try to run OpenItemGroupDetails() on the Borrowed tab
                return;
            }

            // --- YOUR EXISTING LOGIC FOR OTHER TABS ---
            await OpenItemGroupDetails();
        }

        private async Task LblUrgentHeader_Click(object sender, EventArgs e)
        {
            var damagedItems = await GetDamagedItemsAsync();
            if (damagedItems.Any())
            {
                using (var popup = new RepairDetailsPopup(damagedItems, _inventoryService, async () => {
                    await LoadHomeContent();
                    await UpdateDashboardCounts();
                }))
                {
                    ShowPopupWithFade(popup);
                }
            }
        }

        private async void CmbAccountActions_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbAccountActions?.SelectedItem?.ToString() == "Sign out")
            {
                if (MessageBox.Show("Are you sure you want to sign out?", "Ventrix System", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    isSigningOut = true;

                    if (_borrowerPortalInstance != null && !_borrowerPortalInstance.IsDisposed)
                    {
                        _borrowerPortalInstance.Show();
                        _borrowerPortalInstance.ReturnToMainScreen();
                        _borrowerPortalInstance.BringToFront();
                    }
                    else
                    {
                        var portal = new BorrowerPortal(_inventoryService, _borrowService, _userService);
                        portal.Show();
                    }

                    this.Close();
                }
                else
                {
                    cmbAccountActions.SelectedIndex = -1;
                }
            }
        }
        #endregion

        #region Utilities (Export, Styling, Fade)
        private DialogResult ShowPopupWithFade(Form popup)
        {
            DialogResult result = DialogResult.Cancel;
            using (Form fadeOverlay = new Form())
            {
                fadeOverlay.StartPosition = FormStartPosition.Manual;
                fadeOverlay.FormBorderStyle = FormBorderStyle.None;
                fadeOverlay.Opacity = 0.50;
                fadeOverlay.BackColor = DrawColor.Black;
                fadeOverlay.ShowInTaskbar = false;
                fadeOverlay.Location = this.Location;
                fadeOverlay.Size = this.Size;
                fadeOverlay.Show(this);

                popup.StartPosition = FormStartPosition.CenterParent;
                result = popup.ShowDialog(fadeOverlay);
            }
            return result;
        }

        private void SetupColumns(params string[] names) { foreach (var n in names) dgvInventory.Columns.Add(n.Replace(" ", ""), n); dgvInventory.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill; }

        private void ExportToExcel()
        {
            if (dgvInventory == null || dgvInventory.Rows.Count == 0) { MessageBox.Show("There is no inventory data to export.", "Ventrix System", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            string currentFilter = lblDashboardHeader.Text.Replace("INVENTORY:", "").Trim();
            string filePrefix = "Ventrix_Data";

            if (currentFilter == "ALL") filePrefix = "Ventrix_All_Items";
            else if (currentFilter == "AVAILABLE") filePrefix = "Ventrix_Available_Items";
            else if (currentFilter == "BORROWED") filePrefix = "Ventrix_Borrowed_Items";
            else if (currentFilter == "RECORDS") filePrefix = "Ventrix_Records_List";

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx", FileName = $"{filePrefix}_{DateTime.Now:MMM-dd-yyyy_hh-mmtt}.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (XLWorkbook workbook = new XLWorkbook())
                        {
                            var worksheet = workbook.Worksheets.Add(currentFilter);
                            int colIndex = 1;
                            for (int i = 0; i < dgvInventory.Columns.Count; i++)
                            {
                                if (!dgvInventory.Columns[i].Visible) continue;
                                worksheet.Cell(1, colIndex).Value = dgvInventory.Columns[i].HeaderText;
                                worksheet.Cell(1, colIndex).Style.Font.Bold = true;
                                worksheet.Cell(1, colIndex).Style.Fill.BackgroundColor = XLColor.FromHtml("#0D47A1");
                                worksheet.Cell(1, colIndex).Style.Font.FontColor = XLColor.White;
                                colIndex++;
                            }

                            for (int i = 0; i < dgvInventory.Rows.Count; i++)
                            {
                                int cellIndex = 1;
                                for (int j = 0; j < dgvInventory.Columns.Count; j++)
                                {
                                    if (!dgvInventory.Columns[j].Visible) continue;
                                    worksheet.Cell(i + 2, cellIndex).Value = dgvInventory.Rows[i].Cells[j].Value?.ToString() ?? "";
                                    cellIndex++;
                                }
                            }
                            worksheet.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                            ToastNotification.Show(this, $"{filePrefix.Replace("_", " ")} exported successfully!", ToastType.Success);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Log(ex, "AdminDashboard - Export to Excel Failed");
                        MessageBox.Show("Failed to save the Excel file. Please ensure the file is not currently open in another program.", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async Task ExportHistoryToExcelAsync()
        {
            var allData = (await GetFilteredHistoryQuery()).ToList();

            if (!allData.Any()) { MessageBox.Show("No data matching these filters was found to export.", "Ventrix System", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel Workbook|*.xlsx", FileName = $"Ventrix_Audit_History_{DateTime.Now:MMM-dd-yyyy_hh-mmtt}.xlsx" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        using (XLWorkbook workbook = new XLWorkbook())
                        {
                            var ws = workbook.Worksheets.Add("Audit History");
                            string[] headers = { "Record ID", "Item Name", "Borrower ID", "Borrower Name", "Role/Grade", "Purpose", "Time Borrowed", "Time Returned", "Status" };

                            for (int i = 0; i < headers.Length; i++)
                            {
                                ws.Cell(1, i + 1).Value = headers[i];
                                ws.Cell(1, i + 1).Style.Font.Bold = true;
                                ws.Cell(1, i + 1).Style.Fill.BackgroundColor = XLColor.FromHtml("#0D47A1");
                                ws.Cell(1, i + 1).Style.Font.FontColor = XLColor.White;
                            }

                            for (int i = 0; i < allData.Count; i++)
                            {
                                var log = allData[i];
                                ws.Cell(i + 2, 1).Value = log.Id;
                                ws.Cell(i + 2, 2).Value = log.ItemName;
                                ws.Cell(i + 2, 3).Value = log.BorrowerId;
                                ws.Cell(i + 2, 4).Value = log.Borrower != null ? log.Borrower.FullName : "Unknown";
                                ws.Cell(i + 2, 5).Value = log.GradeLevel.ToString();
                                ws.Cell(i + 2, 6).Value = log.Purpose;
                                ws.Cell(i + 2, 7).Value = log.BorrowDate.ToString("MMM dd, yyyy - hh:mm tt");
                                ws.Cell(i + 2, 8).Value = log.ReturnDate?.ToString("MMM dd, yyyy - hh:mm tt") ?? "---";
                                ws.Cell(i + 2, 9).Value = log.Status.ToString();
                            }

                            ws.Columns().AdjustToContents();
                            workbook.SaveAs(sfd.FileName);
                            ToastNotification.Show(this, $"Successfully exported {allData.Count} records!", ToastType.Success);
                        }
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Log(ex, "AdminDashboard - Export to Excel Failed");
                        MessageBox.Show("Failed to save the Excel file. Please ensure the file is not currently open in another program.", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void ExportToPDF()
        {
            if (dgvInventory == null || dgvInventory.Rows.Count == 0) { MessageBox.Show("There is no inventory data to export.", "Ventrix System", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            string currentFilter = lblDashboardHeader.Text.Replace("INVENTORY:", "").Trim();
            string filePrefix = "Ventrix_Data";
            string reportTitle = "VENTRIX SYSTEM REPORT";

            if (currentFilter == "ALL") { filePrefix = "Ventrix_All_Items"; reportTitle = "VENTRIX SYSTEM - ALL ITEMS REPORT\n\n"; }
            else if (currentFilter == "AVAILABLE") { filePrefix = "Ventrix_Available_Items"; reportTitle = "VENTRIX SYSTEM - AVAILABLE ITEMS\n\n"; }
            else if (currentFilter == "BORROWED") { filePrefix = "Ventrix_Borrowed_Items"; reportTitle = "VENTRIX SYSTEM - ACTIVELY BORROWED ITEMS\n\n"; }
            else if (currentFilter == "RECORDS") { filePrefix = "Ventrix_RecordsList"; reportTitle = "VENTRIX SYSTEM - REGISTERED RECORDS\n\n"; }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "PDF Document|*.pdf", FileName = $"{filePrefix}_{DateTime.Now:MMM-dd-yyyy_hh-mmtt}.pdf" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
                        PdfWriter.GetInstance(pdfDoc, new FileStream(sfd.FileName, FileMode.Create));
                        pdfDoc.Open();

                        iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD);

                        Paragraph title = new Paragraph(reportTitle, titleFont);
                        title.Alignment = Element.ALIGN_CENTER;
                        pdfDoc.Add(title);

                        PdfPTable pdfTable = new PdfPTable(dgvInventory.Columns.Cast<DataGridViewColumn>().Count(c => c.Visible));
                        pdfTable.WidthPercentage = 100;

                        foreach (DataGridViewColumn column in dgvInventory.Columns)
                        {
                            if (!column.Visible) continue;
                            PdfPCell cell = new PdfPCell(new Phrase(column.HeaderText, FontFactory.GetFont("Arial", 10, iTextSharp.text.Font.BOLD)));
                            cell.BackgroundColor = new BaseColor(13, 71, 161);
                            cell.Padding = 5;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfTable.AddCell(cell);
                        }

                        foreach (DataGridViewRow row in dgvInventory.Rows)
                        {
                            foreach (DataGridViewCell cell in row.Cells)
                            {
                                if (!dgvInventory.Columns[cell.ColumnIndex].Visible) continue;
                                PdfPCell pdfCell = new PdfPCell(new Phrase(cell.Value?.ToString() ?? "", FontFactory.GetFont("Arial", 9)));
                                pdfCell.Padding = 5;
                                pdfCell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pdfTable.AddCell(pdfCell);
                            }
                        }

                        pdfDoc.Add(pdfTable);
                        pdfDoc.Close();
                        ToastNotification.Show(this, $"{filePrefix.Replace("_", " ")} exported successfully!", ToastType.Success);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Log(ex, "AdminDashboard - Export to PDF Failed");
                        MessageBox.Show("Failed to save the PDF file. Please ensure the file is not currently open in another program.", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private async Task ExportHistoryToPDFAsync()
        {
            var allData = (await GetFilteredHistoryQuery()).ToList();

            if (!allData.Any()) { MessageBox.Show("No data matching these filters was found to export.", "Ventrix System", MessageBoxButtons.OK, MessageBoxIcon.Information); return; }

            using (SaveFileDialog sfd = new SaveFileDialog() { Filter = "PDF Document|*.pdf", FileName = $"Ventrix_Audit_History_{DateTime.Now:MMM-dd-yyyy_hh-mmtt}.pdf" })
            {
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    try
                    {
                        iTextSharp.text.Document pdfDoc = new iTextSharp.text.Document(PageSize.A4.Rotate(), 20f, 20f, 20f, 20f);
                        PdfWriter.GetInstance(pdfDoc, new FileStream(sfd.FileName, FileMode.Create));
                        pdfDoc.Open();

                        iTextSharp.text.Font titleFont = FontFactory.GetFont("Arial", 16, iTextSharp.text.Font.BOLD);
                        Paragraph title = new Paragraph("VENTRIX SYSTEM - FULL AUDIT HISTORY\n\n", titleFont);
                        title.Alignment = Element.ALIGN_CENTER;
                        pdfDoc.Add(title);

                        string[] headers = { "ID", "Item Name", "Borrower ID", "Borrower Name", "Role", "Purpose", "Time Borrowed", "Time Returned", "Status" };
                        PdfPTable pdfTable = new PdfPTable(headers.Length);
                        pdfTable.WidthPercentage = 100;

                        float[] widths = new float[] { 8f, 15f, 12f, 15f, 10f, 12f, 15f, 15f, 10f };
                        pdfTable.SetWidths(widths);

                        foreach (var header in headers)
                        {
                            PdfPCell cell = new PdfPCell(new Phrase(header, FontFactory.GetFont("Arial", 9, iTextSharp.text.Font.BOLD)));
                            cell.BackgroundColor = new BaseColor(13, 71, 161);
                            cell.Padding = 5;
                            cell.HorizontalAlignment = Element.ALIGN_CENTER;
                            pdfTable.AddCell(cell);
                        }

                        foreach (var log in allData)
                        {
                            string[] rowData = {
                                log.Id.ToString(),
                                log.ItemName,
                                log.BorrowerId,
                                log.Borrower != null ? log.Borrower.FullName : "Unknown",
                                log.GradeLevel.ToString(),
                                log.Purpose,
                                log.BorrowDate.ToString("MMM dd, yyyy - hh:mm tt"),
                                log.ReturnDate?.ToString("MMM dd, yyyy - hh:mm tt") ?? "---",
                                log.Status.ToString()
                            };

                            foreach (var data in rowData)
                            {
                                PdfPCell cell = new PdfPCell(new Phrase(data ?? "", FontFactory.GetFont("Arial", 8)));
                                cell.Padding = 5;
                                cell.HorizontalAlignment = Element.ALIGN_CENTER;
                                pdfTable.AddCell(cell);
                            }
                        }

                        pdfDoc.Add(pdfTable);
                        pdfDoc.Close();
                        ToastNotification.Show(this, $"Successfully exported {allData.Count} PDF records!", ToastType.Success);
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Log(ex, "AdminDashboard - Export to PDF Failed");
                        MessageBox.Show("Failed to save the PDF file. Please ensure the file is not currently open in another program.", "Export Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void InitializeMaterialSkin()
        {
            var manager = MaterialSkinManager.Instance; manager.AddFormToManage(this); manager.Theme = MaterialSkinManager.Themes.LIGHT; manager.ColorScheme = new ColorScheme(DrawColor.FromArgb(13, 71, 161), DrawColor.FromArgb(10, 50, 120), DrawColor.FromArgb(33, 150, 243), DrawColor.FromArgb(30, 136, 229), TextShade.WHITE);
        }

        private void ApplyModernBranding()
        {
            if (lblDashboardHeader != null)
            {
                lblDashboardHeader.Font = null;
                ThemeManager.ApplyCustomFont(lblDashboardHeader, ThemeManager.HeaderFont, ThemeManager.VentrixBlue);
            }

            var actionBtns = new[] { btnCreate, btnEdit, btnDelete, btnClearActivity };
            var actionTxts = new[] { "ADD ITEM", "EDIT RECORD", "DELETE ITEM", "CLEAR ALL" };
            var actionClrs = new[] { DrawColor.Teal, ThemeManager.VentrixLightBlue, DrawColor.IndianRed, DrawColor.IndianRed };

            for (int i = 0; i < actionBtns.Length; i++)
            {
                if (actionBtns[i] != null)
                {
                    actionBtns[i].Text = actionTxts[i];
                    actionBtns[i].Font = ThemeManager.ButtonFont;
                    actionBtns[i].FillColor = DrawColor.Transparent;
                    actionBtns[i].HoverState.FillColor = actionClrs[i];
                    actionBtns[i].HoverState.ForeColor = DrawColor.White;
                    actionBtns[i].TextAlign = HorizontalAlignment.Left;
                    actionBtns[i].TextOffset = new DrawPoint(10, 0);
                    actionBtns[i].BorderRadius = 8;
                    actionBtns[i].Animated = true;
                }
            }
            if (btnClearActivity != null) { btnClearActivity.TextAlign = HorizontalAlignment.Center; btnClearActivity.Image = null; btnClearActivity.TextOffset = new DrawPoint(0, 0); }

            if (txtSearch != null)
            {
                txtSearch.BorderRadius = txtSearch.Height / 2;
                txtSearch.FillColor = DrawColor.FromArgb(245, 248, 252);
                txtSearch.HoverState.FillColor = DrawColor.FromArgb(250, 252, 255);
                txtSearch.FocusedState.FillColor = DrawColor.White;
            }
            if (pnlTopBar != null)
            {
                pnlTopBar.FillColor = DrawColor.White;
                pnlTopBar.ShadowDecoration.Enabled = false;
                pnlTopBar.CustomBorderColor = DrawColor.FromArgb(230, 235, 240);
                pnlTopBar.CustomBorderThickness = new Padding(0, 0, 0, 1);
            }
        }

        private void StyleDataGrids()
        {
            // Only apply generic styling to inventory; history has its own dedicated StyleHistoryDataGridView method
            var grids = new[] { dgvInventory };
            foreach (var grid in grids)
            {
                if (grid == null) continue;
                grid.BackgroundColor = DrawColor.White;
                grid.BorderStyle = BorderStyle.None;

                // --- HIDE VERTICAL LINES ON INVENTORY & RECORDS ---
                grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                grid.GridColor = DrawColor.FromArgb(241, 245, 249);

                grid.RowHeadersVisible = false;
                grid.AllowUserToAddRows = false;
                grid.AllowUserToDeleteRows = false;
                grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                grid.MultiSelect = false;
                grid.ReadOnly = true;

                grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
                grid.ColumnHeadersHeight = 55;
                grid.ThemeStyle.HeaderStyle.BackColor = DrawColor.FromArgb(13, 71, 161);
                grid.ThemeStyle.HeaderStyle.ForeColor = DrawColor.White;
                grid.ThemeStyle.HeaderStyle.Font = new DrawFont("Segoe UI", 11F, FontStyle.Bold);
                grid.ColumnHeadersDefaultCellStyle.Font = new DrawFont("Segoe UI", 11F, FontStyle.Bold);

                grid.RowTemplate.Height = 50;
                grid.ThemeStyle.RowsStyle.BackColor = DrawColor.White;
                grid.ThemeStyle.RowsStyle.ForeColor = DrawColor.FromArgb(64, 64, 64);
                grid.ThemeStyle.RowsStyle.Font = new DrawFont("Segoe UI", 10.5F, FontStyle.Regular);
                grid.DefaultCellStyle.Font = new DrawFont("Segoe UI", 10.5F, FontStyle.Regular);
                grid.ThemeStyle.AlternatingRowsStyle.BackColor = DrawColor.FromArgb(250, 252, 255);
                grid.DefaultCellStyle.Padding = new Padding(10, 0, 10, 0);

                grid.DefaultCellStyle.SelectionBackColor = DrawColor.FromArgb(220, 235, 255);
                grid.DefaultCellStyle.SelectionForeColor = DrawColor.FromArgb(13, 71, 161);
                grid.ThemeStyle.RowsStyle.SelectionBackColor = DrawColor.FromArgb(220, 235, 255);
                grid.ThemeStyle.RowsStyle.SelectionForeColor = DrawColor.FromArgb(13, 71, 161);
                grid.ThemeStyle.AlternatingRowsStyle.SelectionBackColor = DrawColor.FromArgb(220, 235, 255);
                grid.ThemeStyle.AlternatingRowsStyle.SelectionForeColor = DrawColor.FromArgb(13, 71, 161);

                grid.CellMouseEnter += (s, e) => { if (e.RowIndex >= 0) { grid.Cursor = Cursors.Hand; } };
                grid.CellMouseLeave += (s, e) => { if (e.RowIndex >= 0) { grid.Cursor = Cursors.Default; } };
            }
        }

        private void DgvInventory_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.Value != null)
            {
                DataGridView dgv = sender as DataGridView;
                string colName = dgv.Columns[e.ColumnIndex].Name;
                string value = e.Value.ToString();

                if (txtSearch != null && !string.IsNullOrEmpty(txtSearch.Text) && value.ToLower().Contains(txtSearch.Text.ToLower()))
                {
                    e.CellStyle.BackColor = DrawColor.FromArgb(255, 248, 200);
                    e.CellStyle.Font = new DrawFont("Segoe UI", 10.5F, FontStyle.Bold);
                }

                if (colName == "Status" || colName == "Condition")
                {
                    e.CellStyle.Font = new DrawFont("Segoe UI", 10.5F, FontStyle.Bold);

                    if (value == nameof(ItemStatus.Available) || value == nameof(Condition.Good))
                        e.CellStyle.ForeColor = DrawColor.MediumSeaGreen;
                    else if (value == nameof(BorrowStatus.Active) || value == nameof(ItemStatus.Borrowed))
                        e.CellStyle.ForeColor = DrawColor.DarkOrange;
                    else if (value == nameof(BorrowStatus.Pending))
                        e.CellStyle.ForeColor = DrawColor.Goldenrod;
                    else if (value == nameof(BorrowStatus.Overdue))
                        e.CellStyle.ForeColor = DrawColor.Red;
                    else e.CellStyle.ForeColor = DrawColor.IndianRed;

                    // --- NEW: Visual Warning for unconfirmed returns older than 24 hours ---
                    if (value == nameof(BorrowStatus.PendingReturn))
                    {
                        e.CellStyle.ForeColor = DrawColor.DarkMagenta;

                        // Ensure we are looking at the correct column for the timestamp
                        if (dgv.Columns.Contains("RequestedApprovedOn"))
                        {
                            var cellValue = dgv.Rows[e.RowIndex].Cells["RequestedApprovedOn"].Value;
                            if (cellValue != null && DateTime.TryParse(cellValue.ToString(), out DateTime lastUpdate))
                            {
                                // Highlight the row if it's been waiting for admin confirmation for over 24 hours
                                if (DateTime.Now > lastUpdate.AddDays(1))
                                {
                                    e.CellStyle.BackColor = DrawColor.MistyRose; // Subtle red background
                                    e.CellStyle.ForeColor = DrawColor.Firebrick;  // Darker red text
                                }
                            }
                        }
                    }
                }

                if (colName == "AccountStatus")
                {
                    e.CellStyle.Font = new DrawFont("Segoe UI", 10.5F, FontStyle.Bold);
                    if (value == "ACTIVE") e.CellStyle.ForeColor = DrawColor.MediumSeaGreen;
                    else if (value == "LOCKED") e.CellStyle.ForeColor = DrawColor.IndianRed;
                }
                if (colName == "DueDate" && value != "Awaiting Approval")
                {
                    DateTime dueDate;
                    if (DateTime.TryParseExact(value, "MMM dd, yyyy - hh:mm tt", null, System.Globalization.DateTimeStyles.None, out dueDate))
                    {
                        if (DateTime.Now > dueDate)
                        {
                            e.CellStyle.ForeColor = DrawColor.Red;
                            e.CellStyle.Font = new DrawFont("Segoe UI", 10.5F, FontStyle.Bold);
                        }
                    }
                }
                if (colName == "Strikes" && value == "2")
                {
                    e.CellStyle.ForeColor = DrawColor.DarkOrange;
                    e.CellStyle.Font = new DrawFont("Segoe UI", 10.5F, FontStyle.Bold);
                }
            }
        }

        private void DgvHistory_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex >= 0 && e.Value != null)
            {
                DataGridView dgv = sender as DataGridView;
                string colName = dgv.Columns[e.ColumnIndex].Name;
                string value = e.Value.ToString();

                if (colName == "Status")
                {
                    e.CellStyle.Font = new DrawFont("Segoe UI", 10.5F, FontStyle.Bold);
                    if (value == nameof(BorrowStatus.Returned)) e.CellStyle.ForeColor = DrawColor.MediumSeaGreen;
                    else if (value == nameof(BorrowStatus.Active)) e.CellStyle.ForeColor = DrawColor.DarkOrange;
                    else if (value == nameof(BorrowStatus.PendingReturn)) e.CellStyle.ForeColor = DrawColor.DarkMagenta;
                }
                if (colName == "RDate" && value == "---") e.CellStyle.ForeColor = DrawColor.LightGray;
            }
        }
        private void DgvHistory_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || dgvHistory == null) return;

            string schoolId = dgvHistory.Rows[e.RowIndex].Cells["SchoolId"].Value.ToString();
            string studentName = dgvHistory.Rows[e.RowIndex].Cells["Borrower"].Value.ToString();

            // NEW: Grab the current dates from the filter pickers
            DateTime? start = dtpStartDate?.Value;
            DateTime? end = dtpEndDate?.Value;

            // PASS the dates into the popup so it matches the dashboard view perfectly
            using (var popup = new UserHistoryPopup(schoolId, studentName, _borrowService, start, end))
            {
                ShowPopupWithFade(popup);
            }
        }

        private void ToggleNoResultsState(bool showNoResults)
        {
            if (pnlNoResults != null) pnlNoResults.Visible = false;

            if (lblEmptyState != null)
            {
                lblEmptyState.Visible = showNoResults;

                if (showNoResults)
                {
                    lblEmptyState.Dock = DockStyle.None;

                    if (pnlGridContainer != null && pnlGridContainer.Visible && dgvInventory != null)
                    {
                        lblEmptyState.Parent = pnlGridContainer;
                        lblEmptyState.Bounds = dgvInventory.Bounds;
                        lblEmptyState.BringToFront();
                    }
                    else if (pnlHistory != null && pnlHistory.Visible && dgvHistory != null)
                    {
                        lblEmptyState.Parent = pnlHistory;
                        lblEmptyState.Bounds = dgvHistory.Bounds;
                        lblEmptyState.BringToFront();
                    }
                }
            }
        }

        #endregion
    }
}