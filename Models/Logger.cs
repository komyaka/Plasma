// Logger.cs — Утилита логирования ошибок
using System;
using System.IO;

namespace Plazma.Controllers
{
    /// <summary>Статический логгер для записи ошибок и событий в файл.</summary>
    public static class Logger
    {
        private static readonly object _lock = new object();

        public static void Error(string message, Exception ex = null)
        {
            Write("ERROR", message + (ex != null ? " | " + ex.Message + " | " + ex.StackTrace : ""));
        }

        public static void Info(string message) => Write("INFO", message);
        public static void Sql(string query) { if (AppConfig.LogSqlQueries) Write("SQL", query); }
        public static void Warn(string message) => Write("WARN", message);

        private static void Write(string level, string message)
        {
            try
            {
                lock (_lock)
                {
                    string logPath = AppConfig.LogFilePath;
                    string dir = Path.GetDirectoryName(logPath);
                    if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
                    File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] [{level}] {message}" + Environment.NewLine);
                }
            }
            catch { }
        }
    }
}
