using MultiLogViewer.Models;
using System.Collections.Generic;

namespace MultiLogViewer.Services
{
    public interface ILogFileReader
    {
        IEnumerable<LogEntry> Read(string filePath, LogFormatConfig config);
    }
}
