using Ventrix.Infrastructure; // Adjust to match your exact namespace

namespace Ventrix.App
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // --- DATABASE INITIALIZATION START ---
            using (var db = new AppDbContext())
            {
                // This creates the file AND all your tables (InventoryItems, Users, etc.) 
                // if they don't exist yet.
                db.Database.EnsureCreated();
            }
            // --- DATABASE INITIALIZATION END ---

            Application.Run(new AdminDashboard()); // Or your login form
        }
    }
}