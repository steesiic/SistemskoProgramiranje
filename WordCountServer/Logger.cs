using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace WordCountServer
{
    class Logger
    {
        private static readonly object _lock = new object();
        private static string _logFilePath;

        public static void Init(string logFolder)
        {
            Directory.CreateDirectory(logFolder);
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            _logFilePath = Path.Combine(logFolder, $"log_{timestamp}.txt");

            Log("logger pokrenut");
        }
        public static void Log(string message)
        {
            string threadId = System.Threading.Thread.CurrentThread.ManagedThreadId.ToString();
            string timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            string formatted = $"[{timestamp}] [nit {threadId,2}] {message}";

            lock (_lock)
            {
                Console.WriteLine(formatted);

                if (_logFilePath != null)
                    File.AppendAllText(_logFilePath, formatted + Environment.NewLine);
            }
        }
        public static void Info(string message) => Log($"[info]  {message}");
        public static void Warning(string message) => Log($"[WARN]  {message}");
        public static void Error(string message) => Log($"[ERROR] {message}");
        public static void Cache(string message) => Log($"[kes]   {message}");
    }
}
