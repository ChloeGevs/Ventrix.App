using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using Ventrix.Application.Services;
using Ventrix.Infrastructure;
using Ventrix.Infrastructure.Data;
// Notice we removed the Repositories and Interfaces 'using' statements here!

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

            // 4. Start App
            var startForm = serviceProvider.GetRequiredService<InitializingApp>();
            System.Windows.Forms.Application.Run(startForm);
        }
    }
}