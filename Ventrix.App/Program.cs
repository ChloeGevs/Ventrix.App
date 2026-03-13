using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
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
            System.Windows.Forms.Application.ThreadException += GlobalThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            System.Windows.Forms.Application.SetHighDpiMode(HighDpiMode.SystemAware);
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            var services = new ServiceCollection();
            ConfigureServices(services);
            var serviceProvider = services.BuildServiceProvider();

            // 2. Initialize DB
            try
            {
                InitializeDatabase(serviceProvider).GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Database Initialization Failed: {ex.Message}", "Critical Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var startForm = serviceProvider.GetRequiredService<BorrowerPortal>();
            System.Windows.Forms.Application.Run(startForm);
        }

        private static void GlobalThreadException(object sender, ThreadExceptionEventArgs e)
        {
            // Log error using your ErrorLogger service logic
            MessageBox.Show("An unexpected error occurred. Please check the logs.", "System Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            // Handle non-UI thread crashes
        }

        private static IConfiguration ConfigureServices(IServiceCollection services)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            string connectionString = config.GetConnectionString("DefaultConnection") ?? "Data Source=ventrix.db";
            services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

            services.AddScoped<UserService>();
            services.AddScoped<InventoryService>();
            services.AddScoped<BorrowService>();
            services.AddTransient<AdminDashboard>();
            services.AddTransient<BorrowerPortal>();

            return config;
        }

        private static async Task InitializeDatabase(IServiceProvider sp)
        {
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await db.Database.EnsureCreatedAsync();

            var inv = scope.ServiceProvider.GetRequiredService<InventoryService>();
            if (!(await inv.GetAllItemsAsync()).Any()) await inv.RunInitialSeed();

            var user = scope.ServiceProvider.GetRequiredService<UserService>();
            await user.InitializeDefaultAdminAsync();
        }
    }
}