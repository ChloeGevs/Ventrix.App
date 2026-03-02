using MaterialSkin;
using MaterialSkin.Controls;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;
using System.Threading.Tasks;
using System;
using System.Windows.Forms;

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

            MaterialSkinManager.Instance.AddFormToManage(this);
            ApplyLocalBranding();

            lblItemHeader.Text = $"Borrowing: {_itemName}";
            cmbGrade.Items.AddRange(new[] { "Grade 7", "Grade 8", "Grade 9", "Grade 10", "Grade 11", "Grade 12", "Faculty" });
            cmbGrade.SelectedIndex = 0;
        }

        private void ApplyLocalBranding()
        {
            ThemeManager.ApplyCustomFont(lblItemHeader, ThemeManager.SubHeaderFont, ThemeManager.VentrixBlue);
            btnConfirm.BackColor = ThemeManager.VentrixBlue;
            btnConfirm.Font = ThemeManager.ButtonFont;
        }

        private async void btnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtBorrower.Text))
            {
                MessageBox.Show("Please enter a valid Borrower ID.", "Required Field");
                return;
            }

            var record = new BorrowRecord
            {
                BorrowerId = txtBorrower.Text,
                ItemName = _itemName,
                Purpose = txtPurpose.Text,
                GradeLevel = cmbGrade.Text,
                BorrowDate = DateTime.Now,
                Status = BorrowStatus.Active
            };

            await _borrowService.ProcessBorrowAsync(record, _itemId);
            DialogResult = DialogResult.OK;
            Close();
        }
    }
}