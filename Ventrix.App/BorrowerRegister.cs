using MaterialSkin;
using MaterialSkin.Controls;
using Ventrix.Services.Service; // Ensure this matches your service namespace

namespace Ventrix.App
{
    public partial class BorrowerRegister : MaterialForm
    {
        // The "slot" to hold the tool
        private readonly InventoryService _inventoryService;

        // UPDATED CONSTRUCTOR: This fixes the '1 arguments' error
        public BorrowerRegister(InventoryService service)
        {
            _inventoryService = service; // Receives the tool
            InitializeComponent();
            SetupTheme();

            // Logic Subscriptions (Pure behavior, no styling)
            cmbRole.SelectedIndexChanged += (s, e) => {
                lblHeader.Text = (cmbRole.Text == "Student") ? "STUDENT REGISTRATION" : "STAFF REGISTRATION";
            };

            chkNoSuffix.CheckedChanged += (s, e) => {
                txtSuffix.Enabled = !chkNoSuffix.Checked;
                if (chkNoSuffix.Checked) txtSuffix.Text = "";
            };

            btnRegister.Click += (s, e) => {
                string fullName = chkNoSuffix.Checked ? $"{txtFirstName.Text} {txtLastName.Text}" : $"{txtFirstName.Text} {txtLastName.Text}, {txtSuffix.Text}";

                // You can now use _inventoryService here if needed
                using (var popup = new FormRegistrationSuccess(cmbRole.Text, fullName))
                {
                    if (popup.ShowDialog() == DialogResult.OK) this.Close();
                }
            };

            lblLoginLink.Click += (s, e) => this.Close();
        }

        private void SetupTheme()
        {
            MaterialSkinManager.Instance.AddFormToManage(this);
        }
    }
}