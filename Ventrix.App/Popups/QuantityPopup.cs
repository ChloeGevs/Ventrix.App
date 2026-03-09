using System;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace Ventrix.App.Popups
{
    public partial class QuantityPopup : MaterialForm
    {
        public int SelectedQuantity { get; private set; }

        public QuantityPopup()
        {
            InitializeComponent();
            ThemeManager.ApplyMaterialTheme(this);

            this.AcceptButton = btnConfirm;
            this.CancelButton = btnCancel;

            this.Load += (s, e) => {
                txtQuantity.Focus();
                txtQuantity.SelectAll();
            };

            txtQuantity.KeyPress += (s, e) => {
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true; 
                }
            };
        }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (int.TryParse(txtQuantity.Text, out int quantity) && quantity > 0)
            {
                SelectedQuantity = quantity;
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("Please enter a valid number greater than 0.", "Invalid Input", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                txtQuantity.Focus();
                txtQuantity.SelectAll();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}