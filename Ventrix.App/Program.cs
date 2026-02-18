using System;
using System.Windows.Forms;
using Ventrix.Services.Service;
using Ventrix.Infrastructure.Repositories;


namespace Ventrix.App
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            var repo = new SQLiteMaterialRepository();
            // The name 'Application' now refers clearly to WinForms
            var service = new InventoryService(repo);

            Application.Run(new Admin(service));
        }
    }
}