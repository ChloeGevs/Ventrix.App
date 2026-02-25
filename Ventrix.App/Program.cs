using Ventrix.Infrastructure;
using System;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace Ventrix.App
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // 1. Show Splash at full opacity
            InitializingApp splash = new InitializingApp();
            splash.Show();
            Application.DoEvents();

            // 2. Initialize Database
            using (var db = new AppDbContext())
            {
                db.Database.EnsureCreated();
            }

            // 3. INCREASED DELAY: Adjust the number in Task.Delay
            // Change 5000 to your preferred time in milliseconds (e.g., 5000 = 5 seconds)
            Task.Delay(6000).Wait();

            // 4. Trigger the Slow Fade Out logic you implemented earlier
            splash.StartFadeOut();

            // Keep the application responsive while the fade-out timer runs
            while (splash.Visible)
            {
                Application.DoEvents();
            }

            splash.Dispose();

            // 5. Run the BorrowerPortal after the splash is completely gone
            Application.Run(new AdminDashboard());
        }
    }
}