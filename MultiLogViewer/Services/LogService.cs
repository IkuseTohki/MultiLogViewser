using MultiLogViewer.Models;
using System.Collections.Generic;
using System.Linq;

namespace MultiLogViewer.Services
{
    public class LogService : ILogService
    {
        private readonly ILogFormatConfigLoader _logFormatConfigLoader;
        private readonly IFileResolver _fileResolver;
        private readonly ILogFileReader _logFileReader;

        public LogService(
            ILogFormatConfigLoader logFormatConfigLoader,
            IFileResolver fileResolver,
            ILogFileReader logFileReader)
        {
            _logFormatConfigLoader = logFormatConfigLoader;
            _fileResolver = fileResolver;
            _logFileReader = logFileReader;
        }

        public LogDataResult LoadFromConfig(string configPath)
        {
            var appConfig = _logFormatConfigLoader.Load(configPath);
            if (appConfig == null)
            {
                return new LogDataResult(new List<LogEntry>(), new List<DisplayColumnConfig>());
            }

            var allEntries = new List<LogEntry>();
            if (appConfig.LogFormats != null)
            {
                foreach (var logFormatConfig in appConfig.LogFormats)
                {
                    if (logFormatConfig.LogFilePatterns != null && logFormatConfig.LogFilePatterns.Any())
                    {
                        var filePaths = _fileResolver.Resolve(logFormatConfig.LogFilePatterns);
                        var entries = _logFileReader.ReadFiles(filePaths, logFormatConfig);
                        allEntries.AddRange(entries);
                    }
                }
            }

            var sortedEntries = allEntries.OrderBy(e => e.Timestamp).ToList();
            var displayColumns = appConfig.DisplayColumns?.ToList() ?? new List<DisplayColumnConfig>();

            return new LogDataResult(sortedEntries, displayColumns);
        }
    }
}
