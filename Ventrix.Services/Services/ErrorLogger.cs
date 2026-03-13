using System;
using System.IO;

namespace Ventrix.Application.Services
{
    public static class ErrorLogger
    {
        private static readonly string LogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs.txt");

        public static void Log(Exception ex, string context)
        {
            try
            {
                string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] CONTEXT: {context}{Environment.NewLine}" +
                                  $"EXCEPTION: {ex.Message}{Environment.NewLine}" +
                                  $"STACK TRACE: {ex.StackTrace}{Environment.NewLine}" +
                                  new string('-', 50) + Environment.NewLine;

                File.AppendAllText(LogFilePath, logEntry);
            }
            catch
            {
                // Fallback to console if file writing fails
                Console.WriteLine($"Critical Logging Failure: {ex.Message}");
            }
        }
    }
}