using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventrix.Application.DTOs;
using Ventrix.Application.Services;
using Ventrix.Domain.Enums;
using Ventrix.Domain.Models;

namespace Ventrix.App
{
    public partial class BorrowerPortal : Form
    {
        private readonly InventoryService _inventoryService;
        private readonly BorrowService _borrowService;
        private readonly UserService _userService;

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
            SetupEvents();
            SetupFocusHighlighting();
        }

        private void SetupEvents()
        {
            FormClosed += (s, e) => System.Windows.Forms.Application.Exit();

            btnAdminToggle.Click += (s, e) => ToggleMode("Admin");
            btnStudentToggle.Click += async (s, e) => { ToggleMode("Student"); await EnterBorrowMode(); };

            btnLogin.Click += BtnLogin_Click;
            btnBorrow.Click += BtnBorrow_Click;
            btnReturn.Click += BtnReturn_Click;
            btnAddToCart.Click += BtnAddToCart_Click;
            btnClearCart.Click += (s, e) => { _cart.Clear(); UpdateCartUI(); _ = ValidateUserRoleAndLimits(); };

            // NEW: Double-click to remove a specific item
            lstCart.DoubleClick += (s, e) => RemoveSelectedCartItem();

            // NEW: Press 'Delete' or 'Backspace' key to remove a specific item
            lstCart.KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
                {
                    RemoveSelectedCartItem();
                }
            };

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

            Load += async (s, e) =>
            {
                await _userService.InitializeDefaultAdminAsync();
                var backupService = new DatabaseBackupService();
                _ = backupService.RunDailyBackupAsync();
                ToggleMode("Student");
                await EnterBorrowMode();
            };
        }

        #region Modes & UI State
        public void ToggleMode(string mode)
        {
            txtStudentId.Clear();
            txtPassword.Clear();
            txtSubject.Clear();

            btnAdminToggle.Text = "Admin Mode";
            btnStudentToggle.Text = "Student Mode";

            bool isAdmin = mode == "Admin";

            lblLoginHeader.Text = isAdmin ? "Admin Access" : "Borrowing Portal";
            txtStudentId.PlaceholderText = isAdmin ? "Username / Admin ID" : "Student / Faculty ID Number";

            txtPassword.Visible = isAdmin;
            btnLogin.Visible = isAdmin;

            cmbListEquipments.Visible = !isAdmin;
            numQuantity.Visible = !isAdmin;
            txtSubject.Visible = !isAdmin;
            cmbGradeLevel.Visible = !isAdmin;
            btnBorrow.Visible = !isAdmin;
            btnReturn.Visible = !isAdmin;
            lblQuantity.Visible = !isAdmin;
            lblSubject.Visible = !isAdmin;
            lblCreateAccount.Visible = !isAdmin;
            lblEquipmentList.Visible = !isAdmin;

            btnAddToCart.Visible = !isAdmin;
            btnClearCart.Visible = !isAdmin;
            lstCart.Visible = !isAdmin;

            // Modern Toggle Styling
            btnAdminToggle.FillColor = isAdmin ? PrimaryBlue : SurfaceGray;
            btnAdminToggle.ForeColor = isAdmin ? Color.White : TextMuted;

            btnStudentToggle.FillColor = !isAdmin ? PrimaryBlue : SurfaceGray;
            btnStudentToggle.ForeColor = !isAdmin ? Color.White : TextMuted;

            numQuantity.Maximum = isAdmin ? 10 : 2;
            txtStudentId.Focus();
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
            lstCart.Visible = true;


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
                lstCart.Visible = false;


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
            lstCart.Items.Clear();
            foreach (var item in _cart)
            {
                lstCart.Items.Add(item.ToString());
            }
        }

        // NEW METHOD: Removes the specifically selected item from the cart
        private void RemoveSelectedCartItem()
        {
            if (lstCart.SelectedIndex != -1)
            {
                var itemToRemove = _cart[lstCart.SelectedIndex];
                var confirm = MessageBox.Show(
                    $"Are you sure you want to remove {itemToRemove.BaseItemName} from your cart?",
                    "Remove Item",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (confirm == DialogResult.Yes)
                {
                    _cart.RemoveAt(lstCart.SelectedIndex);
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
                        var dashboard = new AdminDashboard(_inventoryService, _borrowService, _userService);
                        dashboard.Show();
                        this.Hide();
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
                    var allAvailableItems = await _inventoryService.GetFilteredInventoryAsync("Available", "");
                    var specificUnits = allAvailableItems.Where(i => GetBaseItemName(i.Name).Equals(cartItem.BaseItemName, StringComparison.OrdinalIgnoreCase)).ToList();

                    if (specificUnits.Count < cartItem.Quantity)
                    {
                        MessageBox.Show($"Not enough available stock for {cartItem.BaseItemName}. Skipping...", "Insufficient Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        continue; // Skips to the next item, leaves this one in the cart
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
                        txtSubject.Clear();
                    }
                }
                else
                {
                    MessageBox.Show("No items were borrowed. Your selection has not been changed.", "Borrowing Incomplete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

                UpdateCartUI();
                await LoadEquipmentListAsync();
                await ValidateUserRoleAndLimits();
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

                            await ValidateUserRoleAndLimits();
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
                // 1. Lockout Check
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

                // 2. Enable UI
                cmbListEquipments.Enabled = true;
                txtSubject.Enabled = true;
                cmbGradeLevel.Enabled = true;
                numQuantity.Enabled = true;
                btnAddToCart.Enabled = true;
                btnBorrow.Enabled = true;
                btnBorrow.FillColor = PrimaryBlue;

                // 3. Calculate Limits
                var allUserRecords = (await _borrowService.GetAllBorrowRecordsAsync()).Where(b => b.BorrowerId == inputId).ToList();
                int currentlyHolding = allUserRecords.Count(b => b.Status == BorrowStatus.Active || b.Status == BorrowStatus.Overdue || b.Status == BorrowStatus.PendingReturn);
                int currentlyPending = allUserRecords.Count(b => b.Status == BorrowStatus.Pending);
                int cartTotal = _cart.Sum(c => c.Quantity);

                // 4. Role-Specific Logic
                if (userAccount.Role.ToString() == "Student")
                {
                    // Remove "Faculty" from options so students cannot select it
                    if (cmbGradeLevel.Items.Contains("Faculty"))
                    {
                        cmbGradeLevel.Items.Remove("Faculty");
                    }

                    // Clear the selection if it was previously set to Faculty
                    if (cmbGradeLevel.SelectedItem?.ToString() == "Faculty")
                    {
                        cmbGradeLevel.SelectedIndex = -1;
                    }

                    // Set student limit to 3 items max
                    int remainingAllowed = 3 - currentlyHolding - currentlyPending - cartTotal;
                    numQuantity.Maximum = Math.Max(0, remainingAllowed);

                    cmbGradeLevel.Enabled = true;
                }
                else // Faculty or Admin
                {
                    // Re-add "Faculty" if it was removed, then lock it as the selection
                    if (!cmbGradeLevel.Items.Contains("Faculty"))
                    {
                        cmbGradeLevel.Items.Add("Faculty");
                    }

                    cmbGradeLevel.SelectedItem = "Faculty";
                    cmbGradeLevel.Enabled = false;

                    // Faculty can borrow up to 50 items
                    numQuantity.Maximum = 50;
                }
            }
        }
        #endregion

        #region Utility Methods & Popups
        private async Task LoadEquipmentListAsync()
        {
            cmbListEquipments.Items.Clear();
            var availableItems = await _inventoryService.GetFilteredInventoryAsync("Available", "");

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

        private List<InventoryItem> ShowMultiUnitSelectionPopup(List<InventoryItem> units, string baseName, int requiredQuantity)
        {
            var selectedUnits = new List<InventoryItem>();
            using (Form popup = new Form())
            {
                popup.Text = $"Select Units: {baseName}";
                popup.Size = new Size(380, 250);
                popup.StartPosition = FormStartPosition.CenterParent;
                popup.FormBorderStyle = FormBorderStyle.FixedDialog;
                popup.MaximizeBox = false;
                popup.MinimizeBox = false;
                popup.BackColor = Color.White;

                Label lbl = new Label { Text = $"Please check exactly {requiredQuantity} specific {baseName}(s):", Location = new Point(20, 15), AutoSize = true, Font = new Font("Segoe UI Semibold", 10) };

                CheckedListBox clb = new CheckedListBox { Location = new Point(20, 45), Width = 320, Height = 110, Font = new Font("Segoe UI", 10), BorderStyle = BorderStyle.FixedSingle };
                foreach (var unit in units)
                {
                    clb.Items.Add(new UnitComboItem { Text = $"{unit.Name} (Condition: {unit.Condition})", Unit = unit });
                }

                Button btnOk = new Button { Text = "Confirm", Location = new Point(130, 165), Width = 100, Height = 35, BackColor = PrimaryBlue, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                btnOk.FlatAppearance.BorderSize = 0;
                btnOk.Click += (s, e) => {
                    if (clb.CheckedItems.Count != requiredQuantity)
                    {
                        MessageBox.Show($"You requested {requiredQuantity} item(s). Please check exactly {requiredQuantity} box(es).", "Selection Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    foreach (UnitComboItem item in clb.CheckedItems) selectedUnits.Add(item.Unit);
                    popup.DialogResult = DialogResult.OK;
                };

                popup.Controls.Add(lbl);
                popup.Controls.Add(clb);
                popup.Controls.Add(btnOk);
                popup.ShowDialog(this);
            }

            return selectedUnits;
        }

        private List<BorrowRecord> ShowMultiReturnSelectionPopup(List<BorrowRecord> records)
        {
            var selectedRecords = new List<BorrowRecord>();
            using (Form popup = new Form())
            {
                popup.Text = "Bulk Return Items";
                popup.Size = new Size(420, 320);
                popup.StartPosition = FormStartPosition.CenterParent;
                popup.FormBorderStyle = FormBorderStyle.FixedDialog;
                popup.MaximizeBox = false;
                popup.MinimizeBox = false;
                popup.BackColor = Color.White;

                Label lbl = new Label { Text = "Check ALL the items you are returning right now:", Location = new Point(20, 15), AutoSize = true, Font = new Font("Segoe UI Semibold", 10) };

                CheckedListBox clb = new CheckedListBox { Location = new Point(20, 45), Width = 360, Height = 170, Font = new Font("Segoe UI", 10), CheckOnClick = true, BorderStyle = BorderStyle.FixedSingle };
                foreach (var record in records)
                {
                    string statusTag = record.Status == BorrowStatus.Overdue ? "[OVERDUE] " : "";
                    clb.Items.Add(new RecordComboItem { Text = $"{statusTag}{record.ItemName} (Borrowed: {record.BorrowDate:MMM dd, yyyy})", RecordId = record.Id });
                }

                Button btnOk = new Button { Text = "Request Return", Location = new Point(150, 230), Width = 120, Height = 35, BackColor = SuccessGreen, ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                btnOk.FlatAppearance.BorderSize = 0;
                btnOk.Click += (s, e) => {
                    if (clb.CheckedItems.Count == 0)
                    {
                        MessageBox.Show("Please select at least one item to return.", "Selection Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    foreach (RecordComboItem item in clb.CheckedItems)
                    {
                        selectedRecords.Add(records.First(r => r.Id == item.RecordId));
                    }
                    popup.DialogResult = DialogResult.OK;
                };

                popup.Controls.Add(lbl);
                popup.Controls.Add(clb);
                popup.Controls.Add(btnOk);
                popup.ShowDialog(this);
            }

            return selectedRecords;
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
        private void TxtPassword_IconRightClick(object sender, EventArgs e) { }
        private void txtPassword_MouseMove(object sender, MouseEventArgs e) { }
        private void CmbGradeLevel_SelectedIndexChanged(object sender, EventArgs e) { }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.Shift | Keys.D))
            {
                var dashboard = new AdminDashboard(_inventoryService, _borrowService, _userService);
                dashboard.Show();
                this.Hide();
                return true;
            }
            return base.ProcessCmdKey(ref msg, keyData);
        }
        #endregion

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