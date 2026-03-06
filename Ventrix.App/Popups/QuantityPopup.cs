using System;
using System.Windows.Forms;
using MaterialSkin.Controls;

namespace Ventrix.App.Popups
{
    public partial class QuantityPopup : MaterialForm
    {
        // This property allows the main form to retrieve the entered number
        public int SelectedQuantity { get; private set; }

        public QuantityPopup()
        {
            InitializeComponent();

            ThemeManager.ApplyMaterialTheme(this);

            // Automatically highlight the "1" so the user can just start typing
            this.Load += (s, e) => {
                txtQuantity.Focus();
                txtQuantity.SelectAll();
            };

            txtQuantity.KeyDown += (s, e) => {
                if (e.KeyCode == Keys.Enter)
                {
                    e.SuppressKeyPress = true; // Stops the annoying Windows 'ding' sound
                    btnConfirm.PerformClick(); // Simulates clicking the button
                }
                else if (e.KeyCode == Keys.Escape)
                {
                    e.SuppressKeyPress = true;
                    btnCancel.PerformClick();
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