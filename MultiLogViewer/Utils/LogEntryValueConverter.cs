using MultiLogViewer.Models;
using System;
using System.Globalization;

namespace MultiLogViewer.Utils
{
    /// <summary>
    /// LogEntry の特定の列に対応する値を取得・整形するためのユーティリティクラスです。
    /// </summary>
    public static class LogEntryValueConverter
    {
        /// <summary>
        /// 指定された列定義に基づいて、ログエントリから表示用の文字列を取得します。
        /// </summary>
        /// <param name="entry">対象のログエントリ。</param>
        /// <param name="column">列定義。</param>
        /// <returns>整形済みの文字列。</returns>
        public static string GetStringValue(LogEntry entry, DisplayColumnConfig column)
        {
            return GetStringValue(entry, column.BindingPath, column.StringFormat);
        }

        /// <summary>
        /// 指定されたバインドパスと書式に基づいて、ログエントリから表示用の文字列を取得します。
        /// </summary>
        /// <param name="entry">対象のログエントリ。</param>
        /// <param name="bindingPath">バインドパス。</param>
        /// <param name="format">表示書式（オプション）。</param>
        /// <returns>整形済みの文字列。</returns>
        public static string GetStringValue(LogEntry entry, string bindingPath, string? format)
        {
            if (string.IsNullOrEmpty(bindingPath)) return string.Empty;

            object? rawValue = null;

            if (bindingPath == "Timestamp")
            {
                rawValue = entry.Timestamp;
            }
            else if (bindingPath == "Message")
            {
                rawValue = entry.Message;
            }
            else if (bindingPath == "FileName")
            {
                rawValue = entry.FileName;
            }
            else if (bindingPath == "LineNumber")
            {
                rawValue = entry.LineNumber;
            }
            else if (bindingPath.StartsWith("AdditionalData[") && bindingPath.EndsWith("]"))
            {
                var key = bindingPath.Substring(15, bindingPath.Length - 16);
                if (entry.AdditionalData.TryGetValue(key, out var val))
                {
                    rawValue = val;
                }
            }

            if (rawValue == null) return string.Empty;

            if (!string.IsNullOrEmpty(format))
            {
                return string.Format(CultureInfo.InvariantCulture, $"{{0:{format}}}", rawValue);
            }

            return rawValue.ToString() ?? string.Empty;
        }

        /// <summary>
        /// バインドパスから AdditionalData のキー名を抽出します。
        /// </summary>
        /// <param name="bindingPath">バインドパス (例: AdditionalData[key])。</param>
        /// <returns>抽出されたキー名。該当しない場合は null。</returns>
        public static string? ExtractAdditionalDataKey(string bindingPath)
        {
            if (string.IsNullOrEmpty(bindingPath)) return null;
            if (bindingPath == "Timestamp" || bindingPath == "Message" || bindingPath == "FileName" || bindingPath == "LineNumber") return null;

            if (bindingPath.StartsWith("AdditionalData[") && bindingPath.EndsWith("]"))
            {
                return bindingPath.Substring(15, bindingPath.Length - 16);
            }
            return null;
        }
    }
}
