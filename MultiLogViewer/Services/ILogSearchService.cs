using MultiLogViewer.Models;
using System;
using System.Collections.Generic;

namespace MultiLogViewer.Services
{
    /// <summary>
    /// ログエントリの検索およびフィルタリングロジックを提供するサービスのインターフェースです。
    /// </summary>
    public interface ILogSearchService
    {
        /// <summary>
        /// 指定されたログエントリが検索条件に一致するかどうかを判定します。
        /// </summary>
        /// <param name="entry">判定対象のログエントリ。</param>
        /// <param name="criteria">検索条件。</param>
        /// <returns>一致する場合は true、そうでない場合は false。</returns>
        bool IsMatch(LogEntry entry, SearchCriteria criteria);

        /// <summary>
        /// 指定された条件に合致するログエントリを検索します。
        /// </summary>
        /// <param name="entries">検索対象のログエントリ一覧。</param>
        /// <param name="currentSelection">現在の選択行。ここを起点に検索を開始します。</param>
        /// <param name="criteria">検索条件。</param>
        /// <param name="forward">前方検索（次を検索）の場合は true、後方検索（前を検索）の場合は false。</param>
        /// <returns>条件に合致したログエントリ。見つからない場合は null。</returns>
        LogEntry? Find(IEnumerable<LogEntry> entries, LogEntry? currentSelection, SearchCriteria criteria, bool forward);

        /// <summary>
        /// 現在の検索条件における統計情報（ヒット件数と現在の位置）を取得します。
        /// </summary>
        /// <param name="logs">ログエントリ一覧。</param>
        /// <param name="currentEntry">現在の選択行。</param>
        /// <param name="criteria">検索条件。</param>
        /// <returns>Count: 総ヒット件数, CurrentIndex: 現在の選択行が何番目のヒットか（1オリジン）。</returns>
        (int Count, int CurrentIndex) GetSearchStatistics(IEnumerable<LogEntry> logs, LogEntry? currentEntry, SearchCriteria criteria);

        /// <summary>
        /// 指定された日時に最も近い（指定日時以降の最初の）ログエントリを検索します。
        /// </summary>
        /// <param name="logs">検索対象のログエントリ一覧。Timestamp でソートされている必要があります。</param>
        /// <param name="targetTime">ジャンプ先のターゲット日時。</param>
        /// <returns>見つかったログエントリ。一覧が空の場合は null。</returns>
        LogEntry? FindByDateTime(IEnumerable<LogEntry> logs, DateTime targetTime);

        /// <summary>
        /// ログエントリが指定されたフィルター条件によって非表示にされるべきかどうかを判定します。
        /// </summary>
        /// <param name="entry">判定対象のログエントリ。</param>
        /// <param name="filters">適用されている拡張フィルター一覧。</param>
        /// <returns>非表示にすべき場合は true、表示すべき場合は false。</returns>
        bool ShouldHide(LogEntry entry, IEnumerable<LogFilter> filters);
    }
}
