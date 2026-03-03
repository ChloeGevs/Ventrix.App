using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Ventrix.Infrastructure.Data;

namespace Ventrix.Infrastructure
{
    // This class is ONLY used by EF Core tools (Add-Migration, Update-Database) at design time
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            // 1. Tell the tools to look inside the Ventrix.App folder for appsettings.json
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Ventrix.App");

            // Fallback just in case the tools are run directly from the UI folder
            if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
            {
                basePath = Directory.GetCurrentDirectory();
            }

            // 2. Read the appsettings.json file
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            // 3. Create the DbContext with the exact same connection string used in Program.cs
            var builder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlite(connectionString);

            return new AppDbContext(builder.Options);
        }
    }
}