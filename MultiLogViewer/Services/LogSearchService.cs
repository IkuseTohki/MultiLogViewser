using MultiLogViewer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MultiLogViewer.Services
{
    public class LogSearchService : ILogSearchService
    {
        public bool IsMatch(LogEntry entry, SearchCriteria criteria)
        {
            if (string.IsNullOrEmpty(criteria.SearchText)) return false;

            if (criteria.IsRegex)
            {
                try
                {
                    var options = criteria.IsCaseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase;
                    var regex = new Regex(criteria.SearchText, options);

                    if (regex.IsMatch(entry.Message)) return true;
                    if (regex.IsMatch(entry.FileName)) return true;
                    foreach (var value in entry.AdditionalData.Values)
                    {
                        if (regex.IsMatch(value)) return true;
                    }
                }
                catch
                {
                    return false;
                }
            }
            else
            {
                var comparison = criteria.IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                if (entry.Message.IndexOf(criteria.SearchText, comparison) >= 0) return true;
                if (entry.FileName.IndexOf(criteria.SearchText, comparison) >= 0) return true;
                foreach (var value in entry.AdditionalData.Values)
                {
                    if (value.IndexOf(criteria.SearchText, comparison) >= 0) return true;
                }
            }
            return false;
        }

        public LogEntry? Find(IEnumerable<LogEntry> entries, LogEntry? currentSelection, SearchCriteria criteria, bool forward)
        {
            var items = entries.ToList();
            if (!items.Any() || string.IsNullOrEmpty(criteria.SearchText)) return null;

            int startIndex = currentSelection != null ? items.IndexOf(currentSelection) : -1;
            int count = items.Count;

            for (int i = 1; i <= count; i++)
            {
                int k = forward
                    ? (startIndex + i) % count
                    : (startIndex - i + count) % count;

                if (IsMatch(items[k], criteria))
                {
                    return items[k];
                }
            }

            return null;
        }

        public (int Count, int CurrentIndex) GetSearchStatistics(IEnumerable<LogEntry> entries, LogEntry? currentSelection, SearchCriteria criteria)
        {
            if (string.IsNullOrEmpty(criteria.SearchText)) return (0, 0);

            int matchCount = 0;
            int currentIndex = 0;
            bool selectionFound = false;

            foreach (var entry in entries)
            {
                if (IsMatch(entry, criteria))
                {
                    matchCount++;
                    if (entry == currentSelection)
                    {
                        currentIndex = matchCount;
                        selectionFound = true;
                    }
                }
            }

            return (matchCount, selectionFound ? currentIndex : 0);
        }

        public bool ShouldHide(LogEntry entry, IEnumerable<LogFilter> filters)
        {
            if (entry == null || filters == null || !filters.Any()) return false;

            // 1. カラムフィルター（空チェック）の判定
            // 全ての指定項目が空の場合にのみ非表示とするため、項目を抽出して一括判定する
            var columnKeys = filters.Where(f => f != null && f.Type == FilterType.ColumnEmpty).Select(f => f.Key).ToList();
            if (columnKeys.Any() && entry.AdditionalData != null)
            {
                bool allEmpty = true;
                foreach (var key in columnKeys)
                {
                    if (entry.AdditionalData.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value))
                    {
                        allEmpty = false;
                        break;
                    }
                }
                if (allEmpty) return true; // 全て空なので非表示
            }

            // 2. その他のフィルター判定
            foreach (var filter in filters)
            {
                if (filter == null) continue;

                if (filter.Type == FilterType.DateTimeAfter)
                {
                    if (entry.Timestamp < filter.Value) return true; // 指定より前なので非表示
                }
                else if (filter.Type == FilterType.DateTimeBefore)
                {
                    if (entry.Timestamp > filter.Value) return true; // 指定より後なので非表示
                }
                else if (filter.Type == FilterType.Bookmark)
                {
                    if (!entry.IsBookmarked) return true; // ブックマークされていないので非表示

                    // 色指定がある場合はチェック
                    if (filter is BookmarkFilter bf)
                    {
                        if (bf.TargetColor.HasValue && entry.BookmarkColor != bf.TargetColor.Value)
                        {
                            return true; // 色が指定されているが一致しないので非表示
                        }
                    }
                }
            }

            return false;
        }

        public LogEntry? FindByDateTime(IEnumerable<LogEntry> logs, DateTime targetTime)
        {
            if (logs == null) return null;

            // リストのコピーを避けるため、キャストを試みる
            // ListCollectionView の場合、GetItemAt が使えるためコピー不要
            if (logs is System.Windows.Data.ListCollectionView view)
            {
                if (view.Count == 0) return null;
                return BinarySearchInView(view, targetTime);
            }

            var list = logs as IList<LogEntry>;
            if (list == null)
            {
                // ここで ToList() が走ると全件コピーでメモリ不足の原因になるため、
                // 最悪のケースでも一歩ずつ走査するか、事前にリスト化されていることを期待する
                list = logs.ToList();
            }

            if (list.Count == 0) return null;

            // 範囲外チェック
            if (list[0].Timestamp >= targetTime) return list[0];

            // 全てのログが指定時刻より前の場合、最後のログを返す
            if (list[list.Count - 1].Timestamp < targetTime) return list[list.Count - 1];

            // 二分探索 (Lower Bound)
            int left = 0;
            int right = list.Count - 1;
            LogEntry? result = null;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                if (list[mid].Timestamp >= targetTime)
                {
                    result = list[mid];
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }

            return result ?? list[list.Count - 1];
        }

        private LogEntry? BinarySearchInView(System.Windows.Data.ListCollectionView view, DateTime targetTime)
        {
            int count = view.Count;
            if (count == 0) return null;

            if (((LogEntry)view.GetItemAt(0)).Timestamp >= targetTime) return (LogEntry)view.GetItemAt(0);
            if (((LogEntry)view.GetItemAt(count - 1)).Timestamp < targetTime) return (LogEntry)view.GetItemAt(count - 1);

            int left = 0;
            int right = count - 1;
            LogEntry? result = null;

            while (left <= right)
            {
                int mid = left + (right - left) / 2;
                var entry = (LogEntry)view.GetItemAt(mid);
                if (entry.Timestamp >= targetTime)
                {
                    result = entry;
                    right = mid - 1;
                }
                else
                {
                    left = mid + 1;
                }
            }
            return result ?? (LogEntry)view.GetItemAt(count - 1);
        }
    }
}
