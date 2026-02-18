using MaterialSkin;
using MaterialSkin.Controls;
using Ventrix.Services.Service;

namespace Ventrix.App
{
    public partial class BorrowerPortal : MaterialForm
    {
        private readonly InventoryService _inventoryService;

        public BorrowerPortal(InventoryService inventoryService)
        {
            _inventoryService = inventoryService;
            InitializeComponent();
            SetupMaterialSkin();

            // Toggle Events
            btnStaffToggle.Click += (s, e) => SetLoginMode("Staff");
            btnStudentToggle.Click += (s, e) => SetLoginMode("Student");

            // Navigation to Registration
            lblCreateAccount.Click += (s, e) => {
                BorrowerRegister regForm = new BorrowerRegister(_inventoryService);
                regForm.ShowDialog();
            };

            btnLogin.Click += (s, e) => {
                // Navigate to Admin Dashboard and pass the service
                Admin dashboard = new Admin(_inventoryService);
                dashboard.Show();
                this.Hide();
            };

            SetLoginMode("Student");
        }

        private void SetupMaterialSkin()
        {
            var materialSkinManager = MaterialSkinManager.Instance;
            materialSkinManager.AddFormToManage(this);
            materialSkinManager.Theme = MaterialSkinManager.Themes.LIGHT;
            materialSkinManager.ColorScheme = new ColorScheme(
                Color.FromArgb(13, 71, 161), Color.FromArgb(10, 50, 120),
                Color.FromArgb(33, 150, 243), Color.FromArgb(30, 136, 229),
                TextShade.WHITE
            );
        }

        private void SetLoginMode(string mode)
        {
            bool isAdminLogin = (mode == "Staff");

            // Update Headers and Placeholders via logic
            lblLoginHeader.Text = isAdminLogin ? "ADMIN LOGIN" : "BORROWING PORTAL";
            txtStudentId.PlaceholderText = isAdminLogin ? "Admin ID" : "Student or Staff ID Number";

            // Visibility Toggles
            lblCreateAccount.Visible = !isAdminLogin;
            btnLogin.Visible = isAdminLogin;
            txtPassword.Visible = isAdminLogin;
            cmbListEquipments.Visible = !isAdminLogin;
            numQuantity.Visible = !isAdminLogin;
            txtSubject.Visible = !isAdminLogin;
            lblQuantity.Visible = !isAdminLogin;
            lblSubject.Visible = !isAdminLogin;
            btnBorrow.Visible = !isAdminLogin;
            btnReturn.Visible = !isAdminLogin;
            cmbGradeLevel.Visible = !isAdminLogin;

            // Visual feedback for toggles
            btnStaffToggle.FillColor = isAdminLogin ? Color.FromArgb(13, 71, 161) : Color.FromArgb(224, 224, 224);
            btnStudentToggle.FillColor = !isAdminLogin ? Color.FromArgb(13, 71, 161) : Color.FromArgb(224, 224, 224);
        }
    }
}