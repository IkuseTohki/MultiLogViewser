using MultiLogViewer.Models;
using System;
using System.Collections.Generic;

namespace MultiLogViewer.Services
{
    /// <summary>
    /// ログエントリの検索ロジックを提供するサービスのインターフェースです。
    /// </summary>
    public interface ILogSearchService
    {
        /// <summary>
        /// 指定されたログエントリが検索条件に一致するかどうかを判定します。
        /// </summary>
        bool IsMatch(LogEntry entry, SearchCriteria criteria);

        /// <summary>
        /// 現在の選択位置から、次の（または前の）一致するログエントリを探索します。
        /// </summary>
        LogEntry? Find(IEnumerable<LogEntry> entries, LogEntry? currentSelection, SearchCriteria criteria, bool forward);

        /// <summary>
        /// 指定された条件で検索を行い、マッチした数を返します。
        /// </summary>
        (int Count, int CurrentIndex) GetSearchStatistics(IEnumerable<LogEntry> logs, LogEntry? currentEntry, SearchCriteria criteria);

        /// <summary>
        /// 指定された日時以降の最初のログエントリを検索します。
        /// </summary>
        LogEntry? FindByDateTime(IEnumerable<LogEntry> logs, DateTime targetTime);

        /// <summary>
        /// ログエントリがフィルター条件に該当し、非表示にすべきかどうかを判定します。
        /// </summary>
        bool ShouldHide(LogEntry entry, IEnumerable<LogFilter> filters);
    }
}
