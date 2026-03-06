using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
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
            services.AddTransient<BorrowerPortal>();       // Good practice to register all your forms
            services.AddTransient<BorrowerRegistration>(); // Good practice to register all your forms

            var serviceProvider = services.BuildServiceProvider();

            using (var scope = serviceProvider.CreateScope())
            {
                var inventoryService = scope.ServiceProvider.GetRequiredService<InventoryService>();

                // Fetch existing items using GetAwaiter().GetResult() since Main is not an async method
                var existingItems = inventoryService.GetAllItemsAsync().GetAwaiter().GetResult();

                // If the database is completely empty, run the seeding process!
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