using Ventrix.Application.Services;
using Ventrix.Domain.Models ;
using System.Windows.Forms;


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
            btnRegister.Click += BtnRegister_Click;

            lblLoginLink.Click += (s, e) => {
                var portal = System.Windows.Forms.Application.OpenForms["BorrowerPortal"];

                if (portal != null)
                {
                    portal.Show();
                    this.Close(); 
                }
                else
                {
                    var newPortal = new BorrowerPortal(_inventoryService, _borrowService, _userService);
                    newPortal.Show();
                    this.Close();
                }
            };
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            // 1. Validation
            if (string.IsNullOrWhiteSpace(txtFirstName.Text) || string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("Please fill in all required fields.");
                return;
            }

            var newUser = new User
            {
                UserId = new Random().Next(20240000, 20249999).ToString(),
                FirstName = txtFirstName.Text,
                LastName = txtLastName.Text,
                Role = cmbRole.Text,
                Password = "123"
            };

            try
            {
                _userService.RegisterNewBorrower(newUser);
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
