using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.AccessControl;
using System.Text;
using System.Threading.Tasks;

namespace Skyclient
{
    public class DebugLogger
    {
        private static DebugLogger Instance
        {
            get 
            { 
                if (_instance is null)
                    _instance = new DebugLogger();
                return _instance;
            }
        }

        private static DebugLogger _instance;

        public string FileName;
        public string TotalFilePath;
        public string TotalFileName => Path.Combine(TotalFilePath, FileName);

        private DebugLogger()
        {
            var today = DateTime.Now.ToShortDateString();
            var salt = new Random().Next(1000,10000).ToString();
            FileName = $"debug {today}-{salt}.txt";

            var userhome = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            TotalFilePath = Path.Combine(userhome, ".skyclient-temp", "logs");

            Directory.CreateDirectory(TotalFilePath);
            File.WriteAllBytes(TotalFileName, new byte[0]);
        }

        private static string Now => $"[{DateTime.Now.ToShortTimeString()}]";
        private static string NowLine(string line) => $"{Now} {line}";

        public static void Log(string info)
        {
            foreach (var line in info.Split('\n'))
            {
                File.AppendAllText(Instance.TotalFileName, NowLine(line) + "\n");
            }
        }

        public static void Log(Exception exception)
        {
            Log(exception.Source);
            Log(exception.Message);
            Log(exception.StackTrace);

            var e = exception.InnerException;

            while (e is not null)
            {
                Log(e.Source);
                Log(e.Message);
                Log(e.StackTrace);
                
                e = e.InnerException;
            }
        }
    }
}
