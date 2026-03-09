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
            lblCreateAccount.Click += LblCreateAccount_Click;
            txtPassword.IconRightClick += TxtPassword_IconRightClick;
            txtPassword.MouseMove += txtPassword_MouseMove;
            cmbGradeLevel.SelectedIndexChanged += CmbGradeLevel_SelectedIndexChanged;
            txtStudentId.Leave += async (s, e) => await ValidateUserRoleAndLimits();

            txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnLogin.PerformClick(); };
            txtSubject.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) btnBorrow.PerformClick(); };

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

            btnAdminToggle.Text = "&Admin Mode";
            btnStudentToggle.Text = "&Student Mode";

            bool isAdmin = mode == "Admin";

            lblLoginHeader.Text = isAdmin ? "ADMIN LOGIN" : "BORROWING PORTAL";
            txtStudentId.PlaceholderText = isAdmin ? "Username / Admin ID" : "Student/Faculty ID Number";

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

            btnAdminToggle.FillColor = isAdmin ? Color.FromArgb(13, 71, 161) : Color.FromArgb(240, 240, 240);
            btnAdminToggle.ForeColor = isAdmin ? Color.White : Color.Gray;

            btnStudentToggle.FillColor = !isAdmin ? Color.FromArgb(13, 71, 161) : Color.FromArgb(240, 240, 240);
            btnStudentToggle.ForeColor = !isAdmin ? Color.White : Color.Gray;

            numQuantity.Maximum = isAdmin ? 10 : 2;
            txtStudentId.Focus();
        }

        private async Task EnterBorrowMode()
        {
            isReturnMode = false;

            txtSubject.Visible = true;
            cmbGradeLevel.Visible = true;
            numQuantity.Visible = true;
            lblSubject.Visible = true;
            lblQuantity.Visible = true;

            btnBorrow.Text = "BORROW ITEM";
            btnBorrow.FillColor = Color.FromArgb(13, 71, 161);

            btnReturn.Text = "RETURN ITEM";
            btnReturn.FillColor = Color.IndianRed;

            lblLoginHeader.Text = "BORROWING PORTAL";
            lblEquipmentList.Text = "Select Equipment:";

            txtStudentId.Enabled = true; 

            await LoadEquipmentListAsync();
        }

        private async Task EnterReturnMode(string studentId)
        {
            SetLoadingState(true);
            try
            {
                var activeRecords = (await _borrowService.GetAllBorrowRecordsAsync())
                    .Where(b => b.BorrowerId == studentId && b.Status == BorrowStatus.Active)
                    .ToList();

                if (!activeRecords.Any())
                {
                    MessageBox.Show("You currently have no active borrowed items to return.", "No Items Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                isReturnMode = true;

                txtSubject.Visible = false;
                cmbGradeLevel.Visible = false;
                numQuantity.Visible = false;
                lblSubject.Visible = false;
                lblQuantity.Visible = false;

                lblLoginHeader.Text = "RETURN PORTAL";
                lblEquipmentList.Text = "Select Item to Return:";

                txtStudentId.Enabled = false;

                btnBorrow.Text = "CANCEL / GO BACK";
                btnBorrow.FillColor = Color.Gray;

                btnReturn.Text = "CONFIRM RETURN";
                btnReturn.FillColor = Color.MediumSeaGreen;

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

        #region Actions (Login, Borrow, Return)
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

            if (string.IsNullOrWhiteSpace(txtStudentId.Text) || cmbListEquipments.SelectedIndex == -1 || string.IsNullOrWhiteSpace(txtSubject.Text) || cmbGradeLevel.SelectedIndex == -1)
            {
                MessageBox.Show("Please fill out all fields before borrowing.", "Missing Information", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            SetLoadingState(true);
            try
            {
                string studentId = txtStudentId.Text.Trim();
                var userAccount = (await _userService.GetAllUsersAsync()).FirstOrDefault(u => u.UserId == studentId && u.Role != UserRole.Admin);

                if (userAccount == null)
                {
                    MessageBox.Show("Student ID not found. Please register first.", "Not Registered", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                int requestedQty = (int)numQuantity.Value;
                string baseItemName = cmbListEquipments.Text;
                var allAvailableItems = await _inventoryService.GetFilteredInventoryAsync("Available", "");
                var specificUnits = allAvailableItems.Where(i => GetBaseItemName(i.Name).Equals(baseItemName, StringComparison.OrdinalIgnoreCase)).ToList();

                if (specificUnits.Count < requestedQty)
                {
                    MessageBox.Show($"Not enough available stock. You requested {requestedQty}, but only {specificUnits.Count} are available.", "Insufficient Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                List<InventoryItem> selectedUnits = ShowMultiUnitSelectionPopup(specificUnits, baseItemName, requestedQty);

                if (selectedUnits != null && selectedUnits.Count == requestedQty)
                {
                    string safeGrade = cmbGradeLevel.Text.Replace(" ", "");

                    foreach (var unit in selectedUnits)
                    {
                        var record = new BorrowRecord
                        {
                            BorrowerId = studentId,
                            ItemName = unit.Name,
                            Quantity = 1, 
                            Purpose = txtSubject.Text,
                            GradeLevel = Enum.Parse<GradeLevel>(safeGrade),
                            Status = BorrowStatus.Active,
                            InventoryItemId = unit.Id
                        };

                        await _borrowService.ProcessBorrowAsync(record, unit.Id);
                    }

                    MessageBox.Show($"Success! {requestedQty} {baseItemName}(s) have been dispensed.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtSubject.Clear();
                    await LoadEquipmentListAsync();
                    await ValidateUserRoleAndLimits(); 
                }
            }
            catch (Exception ex)
            {
                ErrorLogger.Log(ex, "BorrowerPortal - Borrowing Failed");
                MessageBox.Show("Borrowing error: The database could not be reached or an unexpected error occurred.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally { SetLoadingState(false); }
        }

        private async void BtnReturn_Click(object sender, EventArgs e)
        {
            string studentId = txtStudentId.Text.Trim();
            if (string.IsNullOrWhiteSpace(studentId))
            {
                MessageBox.Show("Please enter your Student ID first.", "ID Required", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!isReturnMode)
            {
                await EnterReturnMode(studentId);
            }
            else
            {
                if (cmbListEquipments.SelectedItem is RecordComboItem selectedRecord)
                {
                    SetLoadingState(true);
                    try
                    {
                        await _borrowService.ReturnItemAsync(selectedRecord.RecordId);
                        MessageBox.Show("Item successfully returned to inventory!", "Return Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        await EnterReturnMode(studentId);

                        if (!isReturnMode) await EnterBorrowMode();
                    }
                    catch (Exception ex)
                    {
                        ErrorLogger.Log(ex, $"BorrowerPortal - Return Failed for ID {studentId}");
                        MessageBox.Show("Error processing return. Please contact the lab technician.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    finally { SetLoadingState(false); }
                }
            }
        }

        private async Task ValidateUserRoleAndLimits()
        {
            if (isReturnMode || string.IsNullOrWhiteSpace(txtStudentId.Text) || txtPassword.Visible) return;

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
                    btnBorrow.Enabled = false;
                    btnBorrow.FillColor = Color.Gray;
                    return; 
                }

                cmbListEquipments.Enabled = true;
                txtSubject.Enabled = true;
                cmbGradeLevel.Enabled = true;
                numQuantity.Enabled = true;
                btnBorrow.Enabled = true;
                btnBorrow.FillColor = Color.FromArgb(13, 71, 161); 
               
                var activeRecords = (await _borrowService.GetAllBorrowRecordsAsync())
                    .Where(b => b.BorrowerId == inputId && b.Status == BorrowStatus.Active).ToList();
                int currentlyHolding = activeRecords.Count;

                if (userAccount.Role.ToString() == "Student")
                {
                    int remainingAllowed = 3 - currentlyHolding;
                    if (remainingAllowed <= 0)
                    {
                        MessageBox.Show("You have reached your maximum limit of 3 items. Please return an item first.", "Limit Reached", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        numQuantity.Maximum = 0;
                        btnBorrow.Enabled = false;
                        btnBorrow.FillColor = Color.Gray;
                    }
                    else
                    {
                        numQuantity.Maximum = remainingAllowed;
                    }

                    if (cmbGradeLevel.SelectedItem?.ToString() == "Faculty") cmbGradeLevel.SelectedIndex = -1;
                    cmbGradeLevel.Enabled = true;
                }
                else
                {
                    numQuantity.Maximum = 50;
                    if (!cmbGradeLevel.Items.Contains("Faculty")) cmbGradeLevel.Items.Add("Faculty");
                    cmbGradeLevel.SelectedItem = "Faculty";
                    cmbGradeLevel.Enabled = false;
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
                popup.Text = $"Select {requiredQuantity} Specific Unit(s)";
                popup.Size = new Size(380, 250);
                popup.StartPosition = FormStartPosition.CenterParent;
                popup.FormBorderStyle = FormBorderStyle.FixedDialog;
                popup.MaximizeBox = false;
                popup.MinimizeBox = false;
                popup.BackColor = Color.White;

                Label lbl = new Label { Text = $"Please check exactly {requiredQuantity} unit(s) to borrow:", Location = new Point(20, 15), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };

                CheckedListBox clb = new CheckedListBox { Location = new Point(20, 45), Width = 320, Height = 110, Font = new Font("Segoe UI", 10) };
                foreach (var unit in units)
                {
                    clb.Items.Add(new UnitComboItem { Text = $"{unit.Name} (Condition: {unit.Condition})", Unit = unit });
                }

                Button btnOk = new Button { Text = "Confirm", Location = new Point(130, 165), Width = 100, Height = 35, BackColor = Color.FromArgb(13, 71, 161), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
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

        private void SetLoadingState(bool isLoading)
        {
            this.Cursor = isLoading ? Cursors.WaitCursor : Cursors.Default;
            btnLogin.Enabled = !isLoading;
            btnBorrow.Enabled = !isLoading;
            btnReturn.Enabled = !isLoading;
        }

        private void LblCreateAccount_Click(object sender, EventArgs e)
        {
            BorrowerRegistration registrationForm = new BorrowerRegistration(_inventoryService, _borrowService, _userService);
            registrationForm.Show();
            this.Hide();
        }

        private void SetupFocusHighlighting() {  }
        private void TxtPassword_IconRightClick(object sender, EventArgs e) {  }
        private void txtPassword_MouseMove(object sender, MouseEventArgs e) { }
        private void CmbGradeLevel_SelectedIndexChanged(object sender, EventArgs e) {  }

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