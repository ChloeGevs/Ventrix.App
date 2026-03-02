using System; // Required for Enum and Random
using System.Windows.Forms;
using System.Threading.Tasks;
using Ventrix.Application.Services;
using Ventrix.Domain.Models;

namespace Ventrix.App
{
    public partial class BorrowerRegistration : Form
    {
        private readonly UserService _userService;
        private readonly InventoryService _inventoryService;
        private readonly BorrowService _borrowService;

        public BorrowerRegistration(InventoryService inventoryService, BorrowService borrowService, UserService userService)
        {
            _inventoryService = inventoryService;
            _borrowService = borrowService;
            _userService = userService;
            InitializeComponent();
            InitializeEventHandlers();
        }

        private void InitializeEventHandlers()
        {
            btnRegister.Click += async (s, e) => await BtnRegister_Click(s, e);

            lblLoginLink.Click += (s, e) => {
                var portal = System.Windows.Forms.Application.OpenForms["BorrowerPortal"];

                if (portal != null)
                {
                    portal.Show();
                    Close();
                }
                else
                {
                    var newPortal = new BorrowerPortal(_inventoryService, _borrowService, _userService);
                    newPortal.Show();
                    Close();
                }
            };
        }

        private async Task BtnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }

            // 1. Package the UI text into the DTO (No need to create random IDs or parse Enums here anymore!)
            var dto = new Ventrix.Application.DTOs.RegisterDto
            {
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                Role = cmbRole.Text, // Pass the raw string from the dropdown
                Password = "123"     // Default password for now
            };

            try
            {
                // 2. Pass the DTO to the service
                await _userService.RegisterNewBorrowerAsync(dto);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}