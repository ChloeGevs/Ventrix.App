using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Windows.Forms;
using Ventrix.Application.Services;
using Ventrix.Infrastructure;
using Ventrix.Infrastructure.Data;

namespace Ventrix.App
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            // --- NEW: GLOBAL EXCEPTION HANDLING ---
            // Explicitly use System.Windows.Forms to avoid namespace collisions
            System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // 1. Catch unexpected errors on the main UI thread (e.g., button clicks)
            System.Windows.Forms.Application.ThreadException += (sender, args) =>
            {
                ErrorLogger.Log(args.Exception, "Unhandled UI Exception");
                MessageBox.Show("An unexpected error occurred. The issue has been logged for IT support.", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            };

            // 2. Catch unexpected errors on background threads (e.g., async database tasks)
            AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
            {
                if (args.ExceptionObject is Exception ex)
                {
                    ErrorLogger.Log(ex, "Unhandled Background Exception");
                }
            };
            // --------------------------------------

            // 1. FIX BLURRY UI: Enable High DPI awareness before any UI is initialized
            System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware);

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            // 2. Setup Configuration
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 3. Setup DI Container
            var services = new ServiceCollection();

            // Infrastructure (Database only - Repositories removed!)
            string connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

            // Application Services (Business Logic)
            services.AddScoped<UserService>();
            services.AddScoped<InventoryService>();
            services.AddScoped<BorrowService>();

            // UI Forms
            services.AddTransient<InitializingApp>();
            services.AddTransient<AdminDashboard>();
            services.AddTransient<BorrowerPortal>();
            services.AddTransient<BorrowerRegistration>();

            var serviceProvider = services.BuildServiceProvider();
            using (var scope = serviceProvider.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                db.Database.EnsureCreated();

                var inventoryService = scope.ServiceProvider.GetRequiredService<InventoryService>();

                var existingItems = inventoryService.GetAllItemsAsync().GetAwaiter().GetResult();

                if (existingItems.Count == 0)
                {
                    inventoryService.RunInitialSeed().GetAwaiter().GetResult();
                }
            }

            // 4. Start App
            var startForm = serviceProvider.GetRequiredService<InitializingApp>();
            System.Windows.Forms.Application.Run(startForm);
        }
    }
}