using System;
using System.IO;

namespace Ventrix.Application.Services
{
    public static class ErrorLogger
    {
        public static void Log(Exception ex, string context = "General Error")
        {
            try
            {
                string filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "error_log.txt");
                string logMessage = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] - {context}\n" +
                                    $"Message: {ex.Message}\n" +
                                    $"StackTrace: {ex.StackTrace}\n" +
                                    $"--------------------------------------------------\n";

                File.AppendAllText(filePath, logMessage);
            }
            catch
            {

            }
        }
    }
}