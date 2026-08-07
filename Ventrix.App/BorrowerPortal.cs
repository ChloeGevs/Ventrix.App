using DocumentFormat.OpenXml.Office2010.PowerPoint;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventrix.App.Popups;
using Ventrix.Application.DTOs;
using Ventrix.Application.Services;
using Ventrix.Domain.Enums;
using Ventrix.Domain.Models;
using LibVLCSharp.Shared;
using LibVLCSharp.WinForms;
using System.IO;

namespace Ventrix.App
{
    public partial class BorrowerPortal : Form
    {
        private readonly InventoryService _inventoryService;
        private readonly BorrowService _borrowService;
        private readonly UserService _userService;

        private Guna2AnimateWindow formAnimator;
        private Guna2Elipse formElipse;
        private PictureBox _videoBackground;
        private Label _nativeLogo;
        private Label _nativeDesc;
        // ── Second-screen tablet support ─────────────────────────────────────
        private readonly DualScreenService _DualScreenService = new DualScreenService();

        /// <summary>
        /// True while this portal is running on a secondary screen (tablet mode).
        /// When true the FormClosed handler suppresses Application.Exit() so the
        /// Admin Dashboard stays alive if the portal window is accidentally closed.
        /// </summary>
        public bool IsOnSecondScreen { get; private set; } = false;

        private bool isReturnMode = false;
        private List<CartItem> _cart = new List<CartItem>();

        // Modern Color Palette
        private readonly Color PrimaryBlue = Color.FromArgb(37, 99, 235);    // Tailwind Blue 600
        private readonly Color PrimaryHover = Color.FromArgb(29, 78, 216);   // Tailwind Blue 700
        private readonly Color SurfaceGray = Color.FromArgb(243, 244, 246);  // Tailwind Gray 100
        private readonly Color TextDark = Color.FromArgb(31, 41, 55);        // Tailwind Gray 800
        private readonly Color TextMuted = Color.FromArgb(107, 114, 128);    // Tailwind Gray 500
        private readonly Color SuccessGreen = Color.FromArgb(16, 185, 129);  // Tailwind Emerald 500
        private readonly Color DisabledGray = Color.FromArgb(209, 213, 219); // Tailwind Gray 300

        public BorrowerPortal(InventoryService invService, BorrowService borrowService, UserService userService)
        {
            _inventoryService = invService;
            _borrowService = borrowService;
            _userService = userService;

            InitializeComponent();
            SetupAnimations();
            SetupEvents();
            SetupFocusHighlighting();
            this.Load += BorrowerPortal_Load;
        }

        private void SetupAnimations()
        {
            // 1. Smooth Form Load Animation
            formAnimator = new Guna2AnimateWindow
            {
                TargetForm = this,
                AnimationType = Guna2AnimateWindow.AnimateWindowType.AW_BLEND,
                Interval = 400
            };

            // Round the form corners
            formElipse = new Guna2Elipse
            {
                TargetControl = this,
                BorderRadius = 20
            };
        }

        private void SetupVideoBackground()
        {
            try
            {
                string bgPath = Path.Combine(System.Windows.Forms.Application.StartupPath, "Resources", "computercilab_bg.gif");

                if (File.Exists(bgPath))
                {
                    _videoBackground = new System.Windows.Forms.PictureBox
                    {
                        Dock = DockStyle.Fill,
                        Image = Image.FromFile(bgPath),
                        SizeMode = PictureBoxSizeMode.StretchImage,
                        BackColor = Color.Transparent
                    };
                    _videoBackground.Paint += VideoBackground_Paint;
                    pnlLeftBranding.Controls.Add(_videoBackground);
                    _videoBackground.SendToBack();

                    // Hide the Guna HTML labels so they don't interfere
                    lblBrandLogo.Visible = false;
                    lblBrandTitle.Visible = false;
                    lblBrandDesc.Visible = false;

                    // Generate standard WinForms labels
                    _nativeLogo = new Label
                    {
                        Text = "❖ Ventrix",
                        Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                        ForeColor = Color.White,
                        Location = new Point(60, 60),
                        AutoSize = true,
                        BackColor = Color.Transparent,
                        Parent = _videoBackground
                    };

                    _nativeDesc = new Label
                    {
                        Text = "Your hub for borrowing tech accessories, tools, and lab\nequipment for your coursework. Select your grade level,\nsubject, and the gear you need to get started.",
                        Font = new Font("Segoe UI", 12F, FontStyle.Regular),
                        ForeColor = Color.White,
                        Location = new Point(60, 520),
                        AutoSize = true,
                        BackColor = Color.Transparent,
                        Parent = _videoBackground
                    };
                }
                else
                {
                    pnlLeftBranding.BackColor = Color.FromArgb(15, 23, 42);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Background Error: " + ex.Message);
                pnlLeftBranding.BackColor = Color.FromArgb(15, 23, 42);
            }
        }

        private void VideoBackground_Paint(object sender, PaintEventArgs e)
        {
            // Enable smooth, high-quality text rendering
            e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            // Scale font size dynamically based on window state
            float fontSize = WindowState == FormWindowState.Maximized ? 78F : 36F;
            int titleY = WindowState == FormWindowState.Maximized ? 400 : 300;

            using (Font titleFont = new Font("Segoe UI", fontSize, FontStyle.Bold))
            using (SolidBrush textBrush = new SolidBrush(Color.White))
            {
                e.Graphics.DrawString("Equipment\nBorrowing Portal", titleFont, textBrush, new Point(60, titleY));
            }
        }

        private async void BorrowerPortal_Load(object sender, EventArgs e)
        {
            await _userService.InitializeDefaultAdminAsync();
            SetupVideoBackground();
            var backupService = new DatabaseBackupService();
            _ = backupService.RunDailyBackupAsync();

            await ToggleMode("Student");
            await EnterBorrowMode();

            // Force an immediate layout update so the Guna shadow renders correctly before showing
            pnlLoginCard.Refresh();
        }

        private async Task AnimateCardEntry()
        {
            int steps = 30; // Number of pixels to move
            int delay = 10; // ms per step

            for (int i = 0; i < steps; i++)
            {
                // Slide up 1 pixel at a time for a smooth effect
                pnlLoginCard.Top -= 1;
                await Task.Delay(delay);
            }
        }

        private void SetupEvents()
        {
            Load += BorrowerPortal_Load;
            Resize += BorrowerPortal_Resize;

            // Only exit the whole application when the portal itself is the primary (and only) window.
            FormClosed += (s, e) =>
            {
                if (!IsOnSecondScreen)
                    System.Windows.Forms.Application.Exit();
            };

            btnAdminToggle.Click += async (s, e) => await ToggleMode("Admin");
            btnStudentToggle.Click += async (s, e) => { await ToggleMode("Student"); await EnterBorrowMode(); };

            btnLogin.Click += BtnLogin_Click;
            btnBorrow.Click += BtnBorrow_Click;
            btnReturn.Click += BtnReturn_Click;
            btnAddToCart.Click += BtnAddToCart_Click;
            btnClearCart.Click += (s, e) => { _cart.Clear(); UpdateCartUI(); _ = ValidateUserRoleAndLimits(); };


            txtPassword.IconRightClick += TxtPassword_IconRightClick;
            txtPassword.MouseMove += txtPassword_MouseMove;
            cmbGradeLevel.SelectedIndexChanged += CmbGradeLevel_SelectedIndexChanged;
            txtStudentId.Leave += async (s, e) => await ValidateUserRoleAndLimits();

            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnLogin.PerformClick(); };
            txtSubject.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnAddToCart.PerformClick(); };

            txtStudentId.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Enter)
                {
                    if (txtPassword.Visible) txtPassword.Focus();
                    else if (isReturnMode) btnReturn.PerformClick();
                    else cmbListEquipments.Focus();

                    e.SuppressKeyPress = true;
                }
            };
        }

        private void BorrowerPortal_Resize(object sender, EventArgs e)
        {
            if (WindowState == FormWindowState.Maximized)
            {
                if (_nativeLogo != null) { _nativeLogo.Font = new Font("Segoe UI", 36F, FontStyle.Bold); }

                if (_nativeDesc != null)
                {
                    _nativeDesc.Font = new Font("Segoe UI", 28F, FontStyle.Regular);
                    _nativeDesc.Location = new Point(60, 750);
                }

                lblBrandLogo.Font = new Font("Segoe UI", 36F, FontStyle.Bold);
                lblBrandTitle.Font = new Font("Segoe UI", 78F, FontStyle.Bold);
                lblBrandDesc.Font = new Font("Segoe UI", 28F, FontStyle.Regular);
                lblBrandDesc.Location = new Point(60, 750);
            }
            else if (WindowState == FormWindowState.Normal)
            {
                if (_nativeLogo != null) { _nativeLogo.Font = new Font("Segoe UI", 22F, FontStyle.Bold); }

                if (_nativeDesc != null)
                {
                    _nativeDesc.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
                    _nativeDesc.Location = new Point(60, 520);
                }

                lblBrandLogo.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
                lblBrandTitle.Font = new Font("Segoe UI", 36F, FontStyle.Bold);
                lblBrandDesc.Font = new Font("Segoe UI", 12F, FontStyle.Regular);
                lblBrandDesc.Location = new Point(60, 520);
            }

            // Force redraw to keep static text crisp and updated on resize
            if (_videoBackground != null)
            {
                _videoBackground.Invalidate();
            }
        }

        #region Modes & UI State

        public async Task ToggleMode(string mode)
        {
            // Subtle transition: Dim the entire form slightly before swapping UI
            for (double i = 1; i > 0.85; i -= 0.05) { this.Opacity = i; await Task.Delay(10); }

            this.SuspendLayout();

            txtStudentId.Clear();
            txtPassword.Clear();
            txtSubject.Clear();

            btnAdminToggle.Text = "Admin Mode";
            btnStudentToggle.Text = "Student Mode";

            bool isAdmin = mode == "Admin";

            if (IsOnSecondScreen)
            {
                btnAdminToggle.Visible = false;
                btnStudentToggle.Visible = false;
            }
            else
            {
                btnAdminToggle.Visible = true;
                btnStudentToggle.Visible = true;
            }

            lblLoginHeader.Text = isAdmin ? "Admin Access" : "Borrowing Portal";
            txtStudentId.PlaceholderText = isAdmin ? "Username / Admin ID" : "Student / Faculty ID Number";

            txtPassword.Visible = isAdmin;
            btnLogin.Visible = isAdmin;

            cmbListEquipments.Visible = !isAdmin;
            numQuantity.Visible = !isAdmin;
            txtSubject.Visible = !isAdmin;
            lblGradeLevelTitle.Visible = !isAdmin;
            cmbGradeLevel.Visible = !isAdmin;
            btnBorrow.Visible = !isAdmin;
            btnReturn.Visible = !isAdmin;
            lblQuantity.Visible = !isAdmin;
            lblSubject.Visible = !isAdmin;
            lblCreateAccount.Visible = !isAdmin;
            lblEquipmentList.Visible = !isAdmin;
            pnlInputContainer.Visible = !isAdmin;

            btnAddToCart.Visible = !isAdmin;
            btnClearCart.Visible = !isAdmin;
            flwCartContainer.Visible = !isAdmin;

            // Modern Toggle Styling
            btnAdminToggle.FillColor = isAdmin ? PrimaryBlue : SurfaceGray;
            btnAdminToggle.ForeColor = isAdmin ? Color.White : TextMuted;

            btnStudentToggle.FillColor = !isAdmin ? PrimaryBlue : SurfaceGray;
            btnStudentToggle.ForeColor = !isAdmin ? Color.White : TextMuted;

            numQuantity.Maximum = isAdmin ? 10 : 2;
            txtStudentId.Focus();

            this.ResumeLayout();

            // Bring the form back to full opacity
            for (double i = this.Opacity; i <= 1; i += 0.05) { this.Opacity = i; await Task.Delay(10); }
            this.Opacity = 1.0;
        }

        private async Task EnterBorrowMode()
        {
            isReturnMode = false;
            _cart.Clear();
            UpdateCartUI();

            txtSubject.Visible = true;
            cmbGradeLevel.Visible = true;
            numQuantity.Visible = true;
            lblSubject.Visible = true;
            lblQuantity.Visible = true;
            btnAddToCart.Visible = true;
            btnClearCart.Visible = true;
            flwCartContainer.Visible = true;

            txtStudentId.Enabled = true;

            await LoadEquipmentListAsync();
        }

        private async Task EnterReturnMode(string studentId)
        {
            SetLoadingState(true);
            try
            {
                var activeRecords = (await _borrowService.GetAllBorrowRecordsAsync())
                    .Where(b => b.BorrowerId == studentId && (b.Status == BorrowStatus.Active || b.Status == BorrowStatus.Overdue))
                    .ToList();

                if (!activeRecords.Any())
                {
                    MessageBox.Show("You currently have no active or overdue items to return.", "No Items Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                isReturnMode = true;

                txtSubject.Visible = false;
                cmbGradeLevel.Visible = false;
                numQuantity.Visible = false;
                lblSubject.Visible = false;
                lblQuantity.Visible = false;
                btnAddToCart.Visible = false;
                btnClearCart.Visible = false;
                flwCartContainer.Visible = false;

                cmbListEquipments.Items.Clear();
                foreach (var record in activeRecords)
                {
                    cmbListEquipments.Items.Add(new RecordComboItem { Text = $"{record.ItemName} (Borrowed: {record.BorrowDate.ToShortDateString()})", RecordId = record.Id });
                }
                if (cmbListEquipments.Items.Count > 0) cmbListEquipments.SelectedIndex = 0;
            }
            finally
            {
                SetLoadingState(false);
            }
        }
        #endregion

        #region Actions (Login, Cart, Borrow, Return)
        private async void BtnAddToCart_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtStudentId.Text) || cmbListEquipments.SelectedIndex == -1 || cmbGradeLevel.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill out your ID, Equipment, and Grade Level before adding to cart.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string studentId = txtStudentId.Text.Trim();
            var userAccount = (await _userService.GetAllUsersAsync()).FirstOrDefault(u => u.UserId == studentId && u.Role != UserRole.Admin);

            if (userAccount == null)
            {
                MessageBox.Show("Student ID not found. Please register first.", "Not Registered", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (userAccount.Strikes >= 3 && userAccount.Role != UserRole.Faculty)
            {
                MessageBox.Show("Your account is locked due to strikes.", "Locked", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (userAccount.Role.ToString() == "Student" && cmbGradeLevel.Text.Equals("Faculty", StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show("Students cannot select the Faculty grade level. Please choose your correct year/grade.", "Invalid Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cmbGradeLevel.SelectedIndex = -1;
                return;
            }
            int requestedQty = (int)numQuantity.Value;
            string baseItemName = cmbListEquipments.Text;

            var allUserRecords = (await _borrowService.GetAllBorrowRecordsAsync()).Where(b => b.BorrowerId == studentId).ToList();
            int currentlyHolding = allUserRecords.Count(b => b.Status == BorrowStatus.Active || b.Status == BorrowStatus.Overdue || b.Status == BorrowStatus.PendingReturn);
            int currentlyPending = allUserRecords.Count(b => b.Status == BorrowStatus.Pending);
            int cartTotal = _cart.Sum(c => c.Quantity);

            if (userAccount.Role == UserRole.Student && (currentlyHolding + currentlyPending + cartTotal + requestedQty > 3))
            {
                MessageBox.Show($"Limit reached!\n\nYou currently hold: {currentlyHolding} item(s)\nPending Requests: {currentlyPending}\nItems in Cart: {cartTotal}\n\nYou can only have up to 3 items at a time.", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var existing = _cart.FirstOrDefault(c => c.BaseItemName == baseItemName);
            if (existing != null)
            {
                existing.Quantity += requestedQty;
            }
            else
            {
                _cart.Add(new CartItem { BaseItemName = baseItemName, Quantity = requestedQty });
            }

            UpdateCartUI();
            await ValidateUserRoleAndLimits();
        }

        private void UpdateCartUI()
        {
            flwCartContainer.Controls.Clear();

            if (_cart.Count == 0)
            {
                // Modern Empty State Layout
                Guna2Panel pnlEmpty = new Guna2Panel
                {
                    Size = new Size(225, 170),
                    BackColor = Color.Transparent,
                    FillColor = Color.Transparent
                };

                Label lblEmpty = new Label
                {
                    Text = "Your cart is empty\n\nChoose equipment above and click\n'+ Add to Selection'",
                    Font = new Font("Segoe UI", 9.5F, FontStyle.Regular),
                    ForeColor = Color.FromArgb(156, 163, 175), // Muted Gray
                    TextAlign = ContentAlignment.MiddleCenter,
                    Dock = DockStyle.Fill
                };

                pnlEmpty.Controls.Add(lblEmpty);
                flwCartContainer.Controls.Add(pnlEmpty);
            }
            else
            {
                foreach (var item in _cart)
                {
                    // 1. Main Card Container
                    Guna2Panel pnlCard = new Guna2Panel
                    {
                        Size = new Size(226, 65),
                        BorderRadius = 8,
                        FillColor = Color.White,
                        BorderColor = Color.FromArgb(229, 231, 235), // Subtle border
                        BorderThickness = 1,
                        Margin = new Padding(0, 0, 0, 6)
                    };

                    // 2. Product Thumbnail / Icon Placeholder
                    Guna2Panel pnlImage = new Guna2Panel
                    {
                        Size = new Size(45, 45),
                        BorderRadius = 6,
                        FillColor = Color.FromArgb(243, 244, 246), // Gray placeholder
                        Location = new Point(8, 10)
                    };

                    Label lblIcon = new Label
                    {
                        Text = "📦", // Placeholder for actual equipment image/icon
                        Font = new Font("Segoe UI", 16F),
                        Dock = DockStyle.Fill,
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.Transparent
                    };
                    pnlImage.Controls.Add(lblIcon);

                    // 3. Item Name Label
                    Label lblName = new Label
                    {
                        Text = item.BaseItemName,
                        Font = new Font("Segoe UI Semibold", 9F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(31, 41, 55),
                        Location = new Point(60, 8),
                        Size = new Size(130, 20),
                        AutoEllipsis = true,
                        BackColor = Color.Transparent
                    };

                    // 4. FOOLPROOF Interactive Quantity Selector [ - ] [ 1 ] [ + ]
                    var currentItem = item; // Safe closure

                    Label btnMinus = new Label
                    {
                        Text = "−", // IMPORTANT: Using the true mathematical minus symbol here, not a hyphen!
                        Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                        ForeColor = currentItem.Quantity > 1 ? Color.FromArgb(75, 85, 99) : Color.FromArgb(209, 213, 219),
                        BackColor = Color.FromArgb(243, 244, 246),
                        Size = new Size(26, 26),
                        Location = new Point(60, 32),
                        Cursor = currentItem.Quantity > 1 ? Cursors.Hand : Cursors.Default,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Padding = new Padding(0, 0, 0, 2) // Nudges the text UP by 2 pixels to perfectly center it
                    };

                    Label lblQty = new Label
                    {
                        Text = currentItem.Quantity.ToString(),
                        Font = new Font("Segoe UI Semibold", 9.5F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(31, 41, 55),
                        Size = new Size(26, 26),
                        Location = new Point(86, 32),
                        TextAlign = ContentAlignment.MiddleCenter,
                        BackColor = Color.Transparent
                    };

                    Label btnPlus = new Label
                    {
                        Text = "+",
                        Font = new Font("Segoe UI", 14F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(75, 85, 99),
                        BackColor = Color.FromArgb(243, 244, 246),
                        Size = new Size(26, 26),
                        Location = new Point(112, 32),
                        Cursor = Cursors.Hand,
                        TextAlign = ContentAlignment.MiddleCenter,
                        Padding = new Padding(0, 0, 0, 2) // Nudges the text UP by 2 pixels
                    };

                    // Quantity Events
                    btnMinus.Click += async (s, ev) =>
                    {
                        if (currentItem.Quantity > 1)
                        {
                            currentItem.Quantity--;
                            UpdateCartUI();
                            await ValidateUserRoleAndLimits();
                        }
                    };

                    btnPlus.Click += async (s, ev) =>
                    {
                        currentItem.Quantity++;
                        UpdateCartUI();
                        await ValidateUserRoleAndLimits();
                    };

                    // 5. Delete / Remove Item Button (✕)
                    // Also changed to a Label to ensure the 'X' doesn't disappear
                    Label btnRemoveItem = new Label
                    {
                        Text = "✕",
                        Font = new Font("Segoe UI", 9F, FontStyle.Bold),
                        ForeColor = Color.FromArgb(156, 163, 175),
                        BackColor = Color.Transparent,
                        Size = new Size(24, 24),
                        Location = new Point(195, 8),
                        Cursor = Cursors.Hand,
                        TextAlign = ContentAlignment.MiddleCenter
                    };

                    // Simple hover effect for the remove label
                    btnRemoveItem.MouseEnter += (s, ev) => { btnRemoveItem.ForeColor = Color.FromArgb(239, 68, 68); };
                    btnRemoveItem.MouseLeave += (s, ev) => { btnRemoveItem.ForeColor = Color.FromArgb(156, 163, 175); };

                    btnRemoveItem.Click += async (s, ev) =>
                    {
                        _cart.Remove(currentItem);
                        UpdateCartUI();
                        await ValidateUserRoleAndLimits();
                    };

                    // Assemble the card components
                    pnlCard.Controls.Add(pnlImage);
                    pnlCard.Controls.Add(lblName);
                    pnlCard.Controls.Add(btnMinus);
                    pnlCard.Controls.Add(lblQty);
                    pnlCard.Controls.Add(btnPlus);
                    pnlCard.Controls.Add(btnRemoveItem);

                    flwCartContainer.Controls.Add(pnlCard);
                }
            }
        }

        private void RemoveSelectedCartItem()
        {   
            if (flwCartContainer.Controls.Count > 0)
            {
                var itemToRemove = _cart[0]; // This is a simplified approach; you might want to implement a more robust way to identify the selected item
                var confirm = MessageBox.Show(
                    $"Are you sure you want to remove {itemToRemove.BaseItemName} from your cart?",
                    "Remove Item",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    _cart.RemoveAt(0);
                    UpdateCartUI();
                    _ = ValidateUserRoleAndLimits();
                }
            }
        }

        private async void BtnLogin_Click(object sender, EventArgs e)
        {
            string inputId = txtStudentId.Text.Trim();

            if (string.IsNullOrWhiteSpace(inputId))
            {
                MessageBox.Show("Please enter your ID.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetLoadingState(true);
            try
            {
                if (inputId.ToLower() == "admin")
                {
                    if (string.IsNullOrWhiteSpace(txtPassword.Text))
                    {
                        MessageBox.Show("Please enter the admin password.", "Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    var adminUser = await _userService.LoginAsync(new LoginDTO { UserId = inputId, Password = txtPassword.Text });

                    if (adminUser != null)
                    {
                        await LaunchAdminDashboardAsync();
                    }
                    else
                    {
                        MessageBox.Show("Invalid Admin Credentials. Please try again.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                else
                {
                    var users = await _userService.GetAllUsersAsync();
                    var userAccount = users.FirstOrDefault(u => u.UserId == inputId && u.Role != UserRole.Admin);

                    if (userAccount != null) MessageBox.Show($"Welcome back, {userAccount.FirstName}! Student portal features coming soon.", "Welcome", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    else MessageBox.Show("ID not found. Please register an account first.", "Login Failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch (Exception ex) { MessageBox.Show($"Login error: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
            finally { SetLoadingState(false); }
        }

        private async void BtnBorrow_Click(object sender, EventArgs e)
        {
            if (isReturnMode) { await EnterBorrowMode(); return; }

            if (_cart.Count == 0)
            {
                MessageBox.Show("Your cart is empty. Please add items to your cart first.", "Empty Cart", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSubject.Text))
            {
                MessageBox.Show("Please enter your Subject/Purpose before checking out.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetLoadingState(true);
            try
            {
                string studentId = txtStudentId.Text.Trim();
                string safeGrade = cmbGradeLevel.Text.Replace(" ", "");
                string purpose = txtSubject.Text;

                int successfulCheckouts = 0;
                List<CartItem> itemsToRemoveFromCart = new List<CartItem>();

                foreach (var cartItem in _cart)
                {
                    var allAvailableItems = await _inventoryService.GetTrueAvailableItemsAsync();
                    var specificUnits = allAvailableItems
                        .Where(i => GetBaseItemName(i.Name).Equals(cartItem.BaseItemName, StringComparison.OrdinalIgnoreCase))
                        .ToList();

                    if (specificUnits.Count < cartItem.Quantity)
                    {
                        MessageBox.Show($"Not enough available stock for {cartItem.BaseItemName}. Skipping...", "Insufficient Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue;
                    }

                    using (var popup = new Popups.ShowMultiUnitSelectionPopup(specificUnits, cartItem.BaseItemName, cartItem.Quantity))
                    {
                        if (popup.ShowDialog(this) == DialogResult.OK)
                        {
                            var selectedUnits = popup.SelectedUnits;

                            if (selectedUnits != null && selectedUnits.Count == cartItem.Quantity)
                            {
                                foreach (var unit in selectedUnits)
                                {
                                    var record = new BorrowRecord
                                    {
                                        BorrowerId = studentId,
                                        ItemName = unit.Name,
                                        Quantity = 1,
                                        Purpose = purpose,
                                        GradeLevel = Enum.Parse<GradeLevel>(safeGrade),
                                        Status = BorrowStatus.Pending,
                                        InventoryItemId = unit.Id
                                    };

                                    await _borrowService.ProcessBorrowAsync(record, unit.Id);
                                    successfulCheckouts++;
                                }

                                itemsToRemoveFromCart.Add(cartItem);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Selection cancelled for {cartItem.BaseItemName}. These will not be borrowed.", "Cancelled", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        }
                    }
                }

                if (successfulCheckouts > 0)
                {
                    MessageBox.Show("Borrow request successful! Please wait for the admin to approve your items.", "Borrow Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    foreach (var processedItem in itemsToRemoveFromCart)
                    {
                        _cart.Remove(processedItem);
                    }

                    if (_cart.Count == 0)
                    {
                        ClearAllInputs();
                    }
                    else
                    {
                        UpdateCartUI();
                    }
                }
                else
                {
                    MessageBox.Show("No items were borrowed. Your selection has not been changed.", "Borrowing Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                if (_cart.Count > 0)
                {
                    await ValidateUserRoleAndLimits();
                }

                await LoadEquipmentListAsync();
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "BorrowerPortal - Checkout Failed");
                MessageBox.Show(ex.Message, "System Restriction", MessageBoxButtons.OK, MessageBoxIcon.Stop);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private async void BtnReturn_Click(object sender, EventArgs e)
        {
            string studentId = txtStudentId.Text.Trim();
            if (string.IsNullOrWhiteSpace(studentId))
            {
                MessageBox.Show("Please enter your Student ID first.", "ID Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetLoadingState(true);
            try
            {
                var userAccount = (await _userService.GetAllUsersAsync()).FirstOrDefault(u => u.UserId == studentId && u.Role != UserRole.Admin);

                if (userAccount == null)
                {
                    MessageBox.Show("Student ID not found. Please register an account first.", "Not Registered", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var activeRecords = (await _borrowService.GetAllBorrowRecordsAsync())
                    .Where(b => b.BorrowerId == studentId && (b.Status == BorrowStatus.Active || b.Status == BorrowStatus.Overdue))
                    .ToList();

                if (!activeRecords.Any())
                {
                    MessageBox.Show("You currently have no active or overdue items to return.", "No Items Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                if (!isReturnMode)
                {
                    await EnterReturnMode(studentId);
                    return;
                }

                using (var popup = new Popups.ShowMultiReturnSelectionPopup(activeRecords))
                {
                    if (popup.ShowDialog(this) == DialogResult.OK)
                    {
                        var itemsToReturn = popup.SelectedRecords;

                        if (itemsToReturn != null && itemsToReturn.Count > 0)
                        {
                            foreach (var record in itemsToReturn)
                            {
                                await _borrowService.RequestReturnAsync(record.Id);
                            }

                            MessageBox.Show($"Successfully requested return for {itemsToReturn.Count} item(s)!\n\nPlease present the physical item(s) to the admin/technician for final confirmation.", "Return Pending Confirmation", MessageBoxButtons.OK, MessageBoxIcon.Information);

                            ClearAllInputs();
                            await EnterBorrowMode();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, $"BorrowerPortal - Return Failed for ID {studentId}");
                MessageBox.Show("Error processing return. Please contact the lab technician.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                SetLoadingState(false);
            }
        }

        private async Task ValidateUserRoleAndLimits()
        {
            if (string.IsNullOrWhiteSpace(txtStudentId.Text) || txtPassword.Visible) return;

            string inputId = txtStudentId.Text.Trim();
            var userAccount = (await _userService.GetAllUsersAsync()).FirstOrDefault(u => u.UserId == inputId);

            if (userAccount != null)
            {
                if (userAccount.Strikes >= 3 && userAccount.Role.ToString() != "Admin" && userAccount.Role.ToString() != "Faculty")
                {
                    MessageBox.Show($"ACCOUNT LOCKED: You have accumulated {userAccount.Strikes} strikes for late or damaged returns.\n\nYou are prohibited from using the borrowing system until a faculty member clears your account.", "Security Lockout", MessageBoxButtons.OK, MessageBoxIcon.Stop);

                    cmbListEquipments.Enabled = false;
                    txtSubject.Enabled = false;
                    cmbGradeLevel.Enabled = false;
                    numQuantity.Enabled = false;
                    btnAddToCart.Enabled = false;
                    btnBorrow.Enabled = false;
                    btnBorrow.FillColor = DisabledGray;
                    return;
                }

                cmbListEquipments.Enabled = true;
                txtSubject.Enabled = true;
                cmbGradeLevel.Enabled = true;
                numQuantity.Enabled = true;
                btnAddToCart.Enabled = true;
                btnBorrow.Enabled = true;
                btnBorrow.FillColor = PrimaryBlue;

                var allUserRecords = (await _borrowService.GetAllBorrowRecordsAsync()).Where(b => b.BorrowerId == inputId).ToList();
                int currentlyHolding = allUserRecords.Count(b => b.Status == BorrowStatus.Active || b.Status == BorrowStatus.Overdue || b.Status == BorrowStatus.PendingReturn);
                int currentlyPending = allUserRecords.Count(b => b.Status == BorrowStatus.Pending);
                int cartTotal = _cart.Sum(c => c.Quantity);

                if (userAccount.Role.ToString() == "Student")
                {
                    if (cmbGradeLevel.Items.Contains("Faculty"))
                    {
                        cmbGradeLevel.Items.Remove("Faculty");
                    }

                    if (cmbGradeLevel.SelectedItem?.ToString() == "Faculty")
                    {
                        cmbGradeLevel.SelectedIndex = -1;
                    }

                    int remainingAllowed = 3 - currentlyHolding - currentlyPending - cartTotal;
                    numQuantity.Maximum = Math.Max(0, remainingAllowed);

                    cmbGradeLevel.Enabled = true;
                }
                else
                {
                    if (!cmbGradeLevel.Items.Contains("Faculty"))
                    {
                        cmbGradeLevel.Items.Add("Faculty");
                    }

                    cmbGradeLevel.SelectedItem = "Faculty";
                    cmbGradeLevel.Enabled = false;
                    numQuantity.Maximum = 50;
                }
            }
        }
        #endregion

        #region Utility Methods & Popups
        private async Task LoadEquipmentListAsync()
        {
            cmbListEquipments.Items.Clear();
            var availableItems = await _inventoryService.GetTrueAvailableItemsAsync();

            var distinctItemNames = availableItems
                .Select(item => GetBaseItemName(item.Name))
                .Distinct()
                .OrderBy(name => name)
                .ToArray();

            if (distinctItemNames.Any()) cmbListEquipments.Items.AddRange(distinctItemNames);
        }

        private string GetBaseItemName(string name)
        {
            if (string.IsNullOrWhiteSpace(name)) return "Unknown Item";
            int hashIndex = name.IndexOf(" #");
            return hashIndex > 0 ? name.Substring(0, hashIndex).Trim() : name.Trim();
        }

        private void SetLoadingState(bool isLoading)
        {
            this.Cursor = isLoading ? Cursors.WaitCursor : Cursors.Default;
            btnLogin.Enabled = !isLoading;
            btnBorrow.Enabled = !isLoading;
            btnReturn.Enabled = !isLoading;
            btnAddToCart.Enabled = !isLoading;
        }


        
        private void SetupFocusHighlighting() { }
        private void TxtPassword_IconRightClick(object sender, EventArgs e)
        {
            // Toggle system password mask state
            txtPassword.UseSystemPasswordChar = !txtPassword.UseSystemPasswordChar;

            // Explicitly update the character mask so the view updates instantly
            if (txtPassword.UseSystemPasswordChar)
            {
                txtPassword.PasswordChar = '●';
            }
            else
            {
                txtPassword.PasswordChar = '\0'; // Reveals the plain text
            }
        }
        private void txtPassword_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.X >= txtPassword.Width - 40)
            {
                txtPassword.Cursor = Cursors.Hand;
            }
            else
            {
                txtPassword.Cursor = Cursors.IBeam; // Standard text input cursor
            }
        }
        private void CmbGradeLevel_SelectedIndexChanged(object sender, EventArgs e) { }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Shift | Keys.D))
            {
                _ = LaunchAdminDashboardAsync();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void ClearAllInputs()
        {
            txtStudentId.Clear();
            txtSubject.Clear();

            if (cmbListEquipments.Items.Count > 0)
                cmbListEquipments.SelectedIndex = -1;

            if (cmbGradeLevel.Items.Count > 0)
                cmbGradeLevel.SelectedIndex = -1;

            numQuantity.Maximum = 50;
            numQuantity.Value = 1;

            _cart.Clear();
            UpdateCartUI();

            txtStudentId.Focus();
        }
        #endregion

        // ── Second-screen tablet support ─────────────────────────────────────

        private async Task LaunchAdminDashboardAsync()
        {
            using var prompt = new DualScreenPopup(_DualScreenService);
            var promptResult = prompt.ShowDialog(this);

            var dashboard = new AdminDashboard(_inventoryService, _borrowService, _userService, this);
            var primaryScreen = Screen.PrimaryScreen ?? Screen.AllScreens[0];
            dashboard.StartPosition = FormStartPosition.Manual;

            int x = primaryScreen.WorkingArea.Left + (primaryScreen.WorkingArea.Width - dashboard.Width) / 2;
            int y = primaryScreen.WorkingArea.Top + (primaryScreen.WorkingArea.Height - dashboard.Height) / 2;
            dashboard.Location = new Point(x, y);

            dashboard.Show();

            if (promptResult == DialogResult.OK && _DualScreenService.HasSecondScreen())
            {
                var secondScreen = _DualScreenService.GetSecondaryScreen()!;
                _DualScreenService.PositionFormOnScreen(this, secondScreen);
                IsOnSecondScreen = true;

                await ToggleMode("Student");
                await EnterBorrowMode();
            }
            else
            {
                this.Hide();
            }
        }
        private class TransparentPanel : Panel
        {
            public TransparentPanel()
            {
                SetStyle(ControlStyles.SupportsTransparentBackColor |
                         ControlStyles.OptimizedDoubleBuffer |
                         ControlStyles.AllPaintingInWmPaint |
                         ControlStyles.UserPaint, true);
                BackColor = Color.Transparent;
            }

            protected override CreateParams CreateParams
            {
                get
                {
                    CreateParams cp = base.CreateParams;
                    cp.ExStyle |= 0x20; // WS_EX_TRANSPARENT makes the panel click-through and fully transparent
                    return cp;
                }
            }
        }
        public async void ReturnToMainScreen()
        {
            IsOnSecondScreen = false;
            _DualScreenService.ReturnFormToPrimaryScreen(this);
            await ToggleMode("Admin");
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (IsOnSecondScreen && e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;
                return;
            }

            base.OnFormClosing(e);
        }

        private class CartItem
        {
            public string BaseItemName { get; set; }
            public int Quantity { get; set; }
            public override string ToString() => $"[x{Quantity}] {BaseItemName}";
        }

        private class RecordComboItem
        {
            public string Text { get; set; }
            public int RecordId { get; set; }
            public override string ToString() => Text;
        }

        private class UnitComboItem
        {
            public string Text { get; set; }
            public InventoryItem Unit { get; set; }
            public override string ToString() => Text;
        }
    }
}