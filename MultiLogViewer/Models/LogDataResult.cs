using System.Collections.Generic;

namespace MultiLogViewer.Models
{
    /// <summary>
    /// 読み込まれたログエントリと表示設定を保持するクラスです。
    /// </summary>
    public class LogDataResult
    {
        public List<LogEntry> Entries { get; }
        public List<DisplayColumnConfig> DisplayColumns { get; }
        public List<FileState> FileStates { get; }
        public int PollingIntervalMs { get; }
        public bool SkipTailModeWarning { get; }

        public LogDataResult(List<LogEntry> entries, List<DisplayColumnConfig> displayColumns, List<FileState> fileStates, int pollingIntervalMs = 1000, bool skipTailModeWarning = false)
        {
            Entries = entries;
            DisplayColumns = displayColumns;
            FileStates = fileStates;
            PollingIntervalMs = pollingIntervalMs;
            SkipTailModeWarning = skipTailModeWarning;
        }
    }
}
