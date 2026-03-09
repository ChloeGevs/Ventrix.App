using MaterialSkin.Controls;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using System;
using System.Windows.Forms;
using Ventrix.Domain.Enums;

namespace Ventrix.App.Popups
{
    public partial class BorrowPopup : MaterialForm
    {
        private readonly BorrowService _borrowService;
        private readonly int _itemId;
        private readonly string _itemName;

        public BorrowPopup(BorrowService service, int id, string name)
        {
            _borrowService = service;
            _itemId = id;
            _itemName = name;

            InitializeComponent();
            ThemeManager.ApplyMaterialTheme(this);
            AcceptButton = btnConfirm;

            this.Text = $"Checkout: {_itemName} (Unit #{id})";

            cmbGrade.Items.Clear();
            foreach (var gradeName in Enum.GetNames(typeof(GradeLevel)))
            {
                string displayName = gradeName.StartsWith("Grade") ? gradeName.Insert(5, " ") : gradeName;
                cmbGrade.Items.Add(displayName);
            }

            if (cmbGrade.Items.Count > 0) cmbGrade.SelectedIndex = 0;
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBorrower.Text))
            {
                MessageBox.Show("Please enter a valid Borrower ID.", "Required Field", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtBorrower.Focus();
                return;
            }

            var record = new BorrowRecord
            {
                BorrowerId = txtBorrower.Text,
                ItemName = _itemName,
                Purpose = txtPurpose.Text,
                GradeLevel = Enum.Parse<GradeLevel>(cmbGrade.Text.Replace(" ", "")), 
                BorrowDate = DateTime.Now,
                Status = BorrowStatus.Active
            };

            await _borrowService.ProcessBorrowAsync(record, _itemId);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}