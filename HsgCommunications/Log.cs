using System;
using System.IO;
using System.Linq;
using System.Diagnostics;

namespace HsgCommunications
{
    public class Log
    {
        public string FilePath { get; set; }

        public string WindowsEventSource { get; set; }
        public string WindowsEventLogName { get; set; }

        private string _fileName;
        public string FileName {
            get => $"{DateTime.Now.ToString("yyMMdd")} {_fileName}";
            set => _fileName = value;
        }
        public string _file => $"{FilePath}\\{FileName}";

        public Log(string eSource, string eLog, string eEvent)
        {
            WindowsEventSource = eSource;
            WindowsEventLogName = eLog;
        }

        public Log(string path, string fileName)
        {
            FilePath = path;
            FileName = fileName;
        }

        public void LogWindowsEvent(string message)
        {
            if (!EventLog.SourceExists(WindowsEventSource))
            {
                EventLog.CreateEventSource(WindowsEventSource, WindowsEventLogName);
            }
            EventLog.WriteEntry(WindowsEventSource, message);
        }

        /// <summary>
        /// Creates a line in the text file and includes the timestamp before the message.
        /// </summary>
        /// <param name="msg">The message to be logged</param>
        /// <param name="args">Arguments to concatenate to the message</param>
        public void LogMessageToFile(string msg, params object[] args)
        {
            CheckIfExists();
            var sw = File.AppendText(_file);

            try
            {
                sw.WriteLine("{0:G}: {1}", DateTime.Now, string.Format(msg, args));
            }
            finally
            {
                sw.Close();
            }
        }

        /// <summary>
        /// Calls LogMessageToFile and displays the message in console
        /// </summary>
        /// <param name="msg">The message to be logged</param>
        /// <param name="args">Arguments to concatenate to the message</param>
        public void LogMessageToFileAndConsole(string msg, params object[] args)
        {
            LogMessageToFile(msg, args);
            Console.WriteLine("{0:G}: {1}", DateTime.Now, string.Format(msg, args));
        }


        /// <summary>
        /// Checks if the directory exists and creates it if not.
        /// </summary>
        private void CheckIfExists()
        {
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
        }

        /// <summary>
        /// Deletes old log files.
        /// </summary>
        /// <param name="days">the days to keep logs for. This should be a positive Int</param>
        public void DeleteOldFiles(int days)
        {
            Directory.GetFiles(FilePath)
                .Select(f => new FileInfo(f))
                .Where(f => f.LastWriteTime < DateTime.Now.AddDays(days * (-1)) && f.Name.EndsWith(_fileName))
                .ToList()
                .ForEach(f => f.Delete());
        }

    }
}
