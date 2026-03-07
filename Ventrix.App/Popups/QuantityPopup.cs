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

            // --- IMPROVED SHORTCUT KEYS ---
            // These built-in properties automatically handle 'Enter' and 'Esc' anywhere in the form
            this.AcceptButton = btnConfirm;
            this.CancelButton = btnCancel;

            // Automatically highlight the "1" so the user can just start typing
            this.Load += (s, e) => {
                txtQuantity.Focus();
                txtQuantity.SelectAll();
            };

            // --- NEW: INPUT VALIDATION (USER CONVENIENCE) ---
            // Prevent the user from even typing letters or symbols by mistake
            txtQuantity.KeyPress += (s, e) => {
                // Only allow digits (0-9) and control keys (like Backspace)
                if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
                {
                    e.Handled = true; // Block the invalid keystroke
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