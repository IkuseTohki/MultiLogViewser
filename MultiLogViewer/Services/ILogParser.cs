using MultiLogViewer.Models;

namespace MultiLogViewer.Services
{
    public interface ILogParser
    {
        LogEntry? Parse(string logLine, string fileName, int lineNumber);
    }
}
