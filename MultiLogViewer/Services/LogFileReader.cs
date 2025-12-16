using MultiLogViewer.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MultiLogViewer.Services
{
    public class LogFileReader : ILogFileReader
    {
        public LogFileReader()
        {
        }

        public IEnumerable<LogEntry> Read(string filePath, LogFormatConfig config)
        {
            if (!File.Exists(filePath))
            {
                yield break;
            }

            var parser = new LogParser(config);

            foreach (var line in File.ReadLines(filePath))
            {
                var entry = parser.Parse(line);
                if (entry != null)
                {
                    yield return entry;
                }
            }
        }

        public IEnumerable<LogEntry> ReadFiles(IEnumerable<string> filePaths, LogFormatConfig config)
        {
            if (filePaths == null || !filePaths.Any())
            {
                return Enumerable.Empty<LogEntry>();
            }

            var allLogEntries = new List<LogEntry>();
            foreach (var filePath in filePaths)
            {
                allLogEntries.AddRange(Read(filePath, config));
            }
            return allLogEntries;
        }
    }
}
