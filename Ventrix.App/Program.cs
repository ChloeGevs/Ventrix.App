using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.IO;
using Ventrix.Infrastructure;
using Ventrix.Infrastructure.Repositories;
using Ventrix.Application.Services;
using Ventrix.Domain.Interfaces;
using Ventrix.Application.Services;

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

            // Infrastructure (Database & Repositories)
            string connectionString = config.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IInventoryRepository, InventoryRepository>();
            services.AddScoped<IBorrowRepository, BorrowRepository>();

            // Application Services (Business Logic)
            services.AddScoped<UserService>();
            services.AddScoped<InventoryService>();
            services.AddScoped<BorrowService>();

            // UI Forms
            services.AddTransient<InitializingApp>();
            services.AddTransient<AdminDashboard>();

            var serviceProvider = services.BuildServiceProvider();

            // 4. Start App
            var startForm = serviceProvider.GetRequiredService<InitializingApp>();
            System.Windows.Forms.Application.Run(startForm);
        }
    }
}