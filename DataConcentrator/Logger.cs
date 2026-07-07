using System;
using System.IO;

namespace DataConcentrator
{
    public static class Logger
    {
        private static readonly string LogFilePath = "system.log";
        private static readonly object _lock = new object();

        public static void Log(string message)
        {
            lock (_lock)
            {
                try
                {
                    string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
                    File.AppendAllText(LogFilePath, logEntry);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Logger error: {ex.Message}");
                }
            }
        }
    }
}
