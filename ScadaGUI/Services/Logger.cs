using System;
using System.IO;

namespace ScadaGUI.Services
{
    public static class Logger
    {
        // Fajl se kreira u folderu gde je pokrenuta aplikacija
        private static readonly string LogFilePath = "system.log";
        private static readonly object _lock = new object();

        public static void Log(string message)
        {
            // Lock osigurava da više niti ne pokušavaju pisati u isti fajl istovremeno
            lock (_lock)
            {
                try
                {
                    string logEntry = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
                    File.AppendAllText(LogFilePath, logEntry);
                }
                catch (Exception ex)
                {
                    // U produkciji bi trebalo obraditi grešku upisa (npr. Event Viewer)
                    Console.WriteLine($"Greška pri upisu u log: {ex.Message}");
                }
            }
        }
    }
}