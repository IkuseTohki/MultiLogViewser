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

        public LogDataResult(List<LogEntry> entries, List<DisplayColumnConfig> displayColumns)
        {
            Entries = entries;
            DisplayColumns = displayColumns;
        }
    }
}
