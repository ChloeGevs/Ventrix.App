using MaterialSkin.Controls;
using Ventrix.Application.Services;
using Ventrix.Domain.Models ;       
using Ventrix.Infrastructure;

namespace Ventrix.App
{
    public partial class BorrowerRegistration : MaterialForm
    {
        private readonly UserService _userService;

        public BorrowerRegistration(UserService userService)
        {
            _userService = userService;
            InitializeComponent();
            InitializeEventHandlers();
        }
        private void InitializeEventHandlers()
        {
            btnRegister.Click += BtnRegister_Click;
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
