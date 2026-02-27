using Ventrix.Infrastructure;
using Ventrix.Infrastructure.Repositories;
using Ventrix.Application.Services;

namespace Ventrix.App
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // Manual initialization to bypass the 'Ventrix.Application' naming conflict
            global::System.Windows.Forms.Application.EnableVisualStyles();
            global::System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);
            global::System.Windows.Forms.Application.SetHighDpiMode(global::System.Windows.Forms.HighDpiMode.SystemAware);

            // 1. Initialize DB Context (Infrastructure)
            var dbContext = new AppDbContext();

            // 2. Initialize Repositories (Infrastructure)
            var invRepo = new InventoryRepository(dbContext);
            var borrowRepo = new BorrowRepository(dbContext);
            var userRepo = new UserRepository(dbContext);

            // 3. Initialize Services (Application Layer)
            var invService = new InventoryService(invRepo);
            var borrowService = new BorrowService(borrowRepo, invRepo);
            var userService = new UserService(userRepo);
            userService.InitializeDefaultAdmin();

            // 4. Start the Application
            // Ensure BorrowerPortal is updated to accept these 3 services in its constructor
            global::System.Windows.Forms.Application.Run(new InitializingApp(invService, borrowService, userService));
        }
    }
}