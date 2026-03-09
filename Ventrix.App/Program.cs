using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Ventrix.Application.Services;
using Ventrix.Infrastructure.Data;

namespace Ventrix.App
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware);

            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();

            string connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

            services.AddScoped<UserService>();
            services.AddScoped<InventoryService>();
            services.AddScoped<BorrowService>();

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
            var startForm = serviceProvider.GetRequiredService<BorrowerPortal>();
            System.Windows.Forms.Application.Run(startForm);
        }
    }
}