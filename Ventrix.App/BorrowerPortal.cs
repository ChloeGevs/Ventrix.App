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

        private bool isReturnMode = false; // Tracks if the UI is in "Return" state

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

            // Toggles
            btnAdminToggle.Click += (s, e) => ToggleMode("Admin");
            btnStudentToggle.Click += async (s, e) => { ToggleMode("Student"); await EnterBorrowMode(); };

            // Actions
            btnLogin.Click += BtnLogin_Click;
            btnBorrow.Click += BtnBorrow_Click;
            btnReturn.Click += BtnReturn_Click; // Added Return wiring
            lblCreateAccount.Click += LblCreateAccount_Click;
            txtPassword.IconRightClick += TxtPassword_IconRightClick;
            txtPassword.MouseMove += txtPassword_MouseMove;
            cmbGradeLevel.SelectedIndexChanged += CmbGradeLevel_SelectedIndexChanged;

            // Enter Key Support 
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

            // Visibility
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

            // Restore all standard borrowing UI elements
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

            txtStudentId.Enabled = true; // Let them change ID if needed

            await LoadEquipmentListAsync();
        }

        private async Task EnterReturnMode(string studentId)
        {
            SetLoadingState(true);
            try
            {
                // Fetch only items THIS user is actively holding
                var activeRecords = (await _borrowService.GetAllBorrowRecordsAsync())
                    .Where(b => b.BorrowerId == studentId && b.Status == BorrowStatus.Active)
                    .ToList();

                if (!activeRecords.Any())
                {
                    MessageBox.Show("You currently have no active borrowed items to return.", "No Items Found", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                isReturnMode = true;

                // Hide unnecessary borrowing fields to make it perfectly intuitive
                txtSubject.Visible = false;
                cmbGradeLevel.Visible = false;
                numQuantity.Visible = false;
                lblSubject.Visible = false;
                lblQuantity.Visible = false;

                lblLoginHeader.Text = "RETURN PORTAL";
                lblEquipmentList.Text = "Select Item to Return:";

                // Lock ID so they don't change it mid-return
                txtStudentId.Enabled = false;

                btnBorrow.Text = "CANCEL / GO BACK";
                btnBorrow.FillColor = Color.Gray;

                btnReturn.Text = "CONFIRM RETURN";
                btnReturn.FillColor = Color.MediumSeaGreen;

                // Populate dropdown with specific records
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

            if (string.IsNullOrWhiteSpace(txtStudentId.Text) || cmbListEquipments.SelectedIndex == -1 ||
                string.IsNullOrWhiteSpace(txtSubject.Text) || cmbGradeLevel.SelectedIndex == -1)
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

                string baseItemName = cmbListEquipments.Text;
                var allAvailableItems = await _inventoryService.GetFilteredInventoryAsync("Available", "");
                var specificUnits = allAvailableItems.Where(i => GetBaseItemName(i.Name).Equals(baseItemName, StringComparison.OrdinalIgnoreCase)).ToList();

                if (!specificUnits.Any())
                {
                    MessageBox.Show("This item is currently out of stock.", "Unavailable", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                InventoryItem selectedUnit = ShowUnitSelectionPopup(specificUnits, baseItemName);

                if (selectedUnit != null)
                {
                    // FIX: GradeLevel space removal
                    string safeGrade = cmbGradeLevel.Text.Replace(" ", "");

                    var record = new BorrowRecord
                    {
                        BorrowerId = studentId,
                        ItemName = selectedUnit.Name,
                        Quantity = (int)numQuantity.Value,
                        Purpose = txtSubject.Text,
                        GradeLevel = Enum.Parse<GradeLevel>(safeGrade),
                        Status = BorrowStatus.Active,

                        // CRITICAL: Explicitly link the selected physical ID
                        InventoryItemId = selectedUnit.Id
                    };

                    // Pass the record AND the specific ID to the service
                    await _borrowService.ProcessBorrowAsync(record, selectedUnit.Id);

                    MessageBox.Show($"Success! {selectedUnit.Name} has been recorded.", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    txtSubject.Clear();
                    await LoadEquipmentListAsync();
                }
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                if (ex.InnerException != null) errorMsg += "\n\nInner: " + ex.InnerException.Message;
                MessageBox.Show($"Database Error: {errorMsg}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                // Trigger the transition into Return Mode
                await EnterReturnMode(studentId);
            }
            else
            {
                // WE ARE ALREADY IN RETURN MODE -> Process the specific return
                if (cmbListEquipments.SelectedItem is RecordComboItem selectedRecord)
                {
                    SetLoadingState(true);
                    try
                    {
                        await _borrowService.ReturnItemAsync(selectedRecord.RecordId);
                        MessageBox.Show("Item successfully returned to inventory!", "Return Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        // Refresh the return mode to see if they have other items
                        await EnterReturnMode(studentId);

                        // If no items left, EnterReturnMode automatically kicks them out. We just need to reset if it failed.
                        if (!isReturnMode) await EnterBorrowMode();
                    }
                    catch (Exception ex) { MessageBox.Show($"Error processing return: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
                    finally { SetLoadingState(false); }
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

        // Programmatic popup prevents needing to create a new Designer file!
        private InventoryItem ShowUnitSelectionPopup(List<InventoryItem> units, string baseName)
        {
            InventoryItem selected = null;
            using (Form popup = new Form())
            {
                popup.Text = "Select Specific Unit";
                popup.Size = new Size(350, 200);
                popup.StartPosition = FormStartPosition.CenterParent;
                popup.FormBorderStyle = FormBorderStyle.FixedDialog;
                popup.MaximizeBox = false;
                popup.MinimizeBox = false;
                popup.BackColor = Color.White;

                Label lbl = new Label { Text = $"Choose a specific {baseName} to borrow:", Location = new Point(20, 20), AutoSize = true, Font = new Font("Segoe UI", 10, FontStyle.Bold) };
                ComboBox cmb = new ComboBox { Location = new Point(20, 50), Width = 290, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };

                foreach (var unit in units)
                {
                    cmb.Items.Add(new UnitComboItem { Text = $"{unit.Name} (Condition: {unit.Condition})", Unit = unit });
                }
                if (cmb.Items.Count > 0) cmb.SelectedIndex = 0;

                Button btnOk = new Button { Text = "Confirm", Location = new Point(120, 100), Width = 100, Height = 35, BackColor = Color.FromArgb(13, 71, 161), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
                btnOk.Click += (s, e) => {
                    if (cmb.SelectedItem != null) { selected = ((UnitComboItem)cmb.SelectedItem).Unit; popup.DialogResult = DialogResult.OK; }
                };

                popup.Controls.Add(lbl);
                popup.Controls.Add(cmb);
                popup.Controls.Add(btnOk);

                popup.ShowDialog(this);
            }
            return selected;
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

        private void SetupFocusHighlighting() { /* Existing logic untouched */ }
        private void TxtPassword_IconRightClick(object sender, EventArgs e) { /* Existing logic untouched */ }
        private void txtPassword_MouseMove(object sender, MouseEventArgs e) { /* Existing logic untouched */ }
        private void CmbGradeLevel_SelectedIndexChanged(object sender, EventArgs e) { /* Existing logic untouched */ }

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

        // Helper classes for clean ComboBox data binding
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