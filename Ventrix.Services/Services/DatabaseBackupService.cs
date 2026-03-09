using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Ventrix.Application.Services
{
    public class DatabaseBackupService
    {
        private readonly string _dbFileName = "Ventrix.db";
        private readonly string _backupFolderName = "Backups";
        private readonly int _retentionDays = 30; // Keeps 1 month of backups

        public async Task RunDailyBackupAsync()
        {
            // Run this on a background thread so it never freezes the UI
            await Task.Run(() =>
            {
                try
                {
                    string baseDir = AppDomain.CurrentDomain.BaseDirectory;
                    string dbPath = Path.Combine(baseDir, _dbFileName);
                    string backupDir = Path.Combine(baseDir, _backupFolderName);

                    // 1. If there's no database yet, skip the backup
                    if (!File.Exists(dbPath)) return;

                    // 2. Create the Backups folder if it doesn't exist
                    if (!Directory.Exists(backupDir))
                    {
                        Directory.CreateDirectory(backupDir);
                    }

                    // 3. Check if we already backed up today (prevents duplicate backups every time the app opens)
                    string todayPattern = $"Ventrix_Backup_{DateTime.Now:yyyyMMdd}*.db";
                    if (Directory.GetFiles(backupDir, todayPattern).Any())
                    {
                        return; // Already backed up today!
                    }

                    // 4. Create the new Backup file
                    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmm");
                    string backupFilePath = Path.Combine(backupDir, $"Ventrix_Backup_{timestamp}.db");

                    File.Copy(dbPath, backupFilePath, true);

                    // 5. Clean up old backups so the hard drive doesn't fill up
                    CleanUpOldBackups(backupDir);
                }
                catch (Exception ex)
                {
                    // Fail silently - we don't want a background backup failure to crash the app
                    Console.WriteLine($"Automated backup failed: {ex.Message}");
                }
            });
        }

        private void CleanUpOldBackups(string backupDir)
        {
            var directory = new DirectoryInfo(backupDir);
            var files = directory.GetFiles("Ventrix_Backup_*.db");

            // Calculate the cutoff date (30 days ago)
            DateTime cutoffDate = DateTime.Now.AddDays(-_retentionDays);

            foreach (var file in files)
            {
                if (file.CreationTime < cutoffDate)
                {
                    try { file.Delete(); } catch { /* Ignore locked files */ }
                }
            }
        }
    }
}