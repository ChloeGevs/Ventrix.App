using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Ventrix.Application.Services;
using Ventrix.Infrastructure.Data;

namespace Ventrix.App
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // 1. Setup Global Error Handling
            System.Windows.Forms.Application.ThreadException += GlobalThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware);
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            // 2. Initialize DB with Migrations
            try
            {
                InitializeDatabase(serviceProvider).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                // Log fatal startup error
                ErrorLogger.Log(ex, "System Startup - Database Initialization");
                MessageBox.Show($"Database Initialization Failed: {ex.Message}\n\nPlease check logs.txt for details.",
                                "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var startForm = serviceProvider.GetRequiredService<BorrowerPortal>();
            System.Windows.Forms.Application.Run(startForm);
        }

        private static void GlobalThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            // FIX: Log UI thread crashes using the existing ErrorLogger
            ErrorLogger.Log(e.Exception, "Global UI Thread Exception");
            MessageBox.Show("An unexpected UI error occurred. Please check the logs.txt file.", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // FIX: Log non-UI thread crashes
            if (e.ExceptionObject is Exception ex)
            {
                ErrorLogger.Log(ex, "Fatal Domain Unhandled Exception");
            }
        }

        private static IConfiguration ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string connectionString = config.GetConnectionString("DefaultConnection") ?? "Data Source=ventrix.db";
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

            // FIX: Use Singletons for services in WinForms to maintain state across the app life
            services.AddSingleton<UserService>();
            services.AddSingleton<InventoryService>();
            services.AddSingleton<BorrowService>();

            // Forms should remain Transient so they refresh every time they are opened
            services.AddTransient<AdminDashboard>();
            services.AddTransient<BorrowerPortal>();

            return config;
        }

        private static async Task InitializeDatabase(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // FIX: Use MigrateAsync instead of EnsureCreated to support migrations
            await db.Database.MigrateAsync();

            var inv = scope.ServiceProvider.GetRequiredService<InventoryService>();
            var allItems = await inv.GetAllItemsAsync();
            if (allItems == null || !allItems.Any())
            {
                await inv.RunInitialSeed();
            }

            var user = scope.ServiceProvider.GetRequiredService<UserService>();
            await user.InitializeDefaultAdminAsync();
        }
    }
}