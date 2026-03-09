using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
using Ventrix.Infrastructure.Data; 

namespace Ventrix.Infrastructure
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../Ventrix.App");

            if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
            {
                basePath = Directory.GetCurrentDirectory();
            }

            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            var builder = new DbContextOptionsBuilder<AppDbContext>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            builder.UseSqlite(connectionString);

            return new AppDbContext(builder.Options);
        }
    }
}