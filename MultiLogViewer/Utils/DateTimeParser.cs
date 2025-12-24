using System;
using System.Globalization;

namespace MultiLogViewer.Utils
{
    public static class DateTimeParser
    {
        private static readonly string[] Formats = {
            "yyyy-MM-dd HH:mm:ss.fff",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy/MM/dd HH:mm:ss.fff",
            "yyyy/MM/dd HH:mm:ss",
            "MM/dd/yyyy HH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss.fffZ",
            "yyyy-MM-ddTHH:mm:ssZ"
        };

        /// <summary>
        /// 様々な形式の文字列から DateTime へのパースを試みます。
        /// </summary>
        public static DateTime? TryParse(string? text)
        {
            if (string.IsNullOrWhiteSpace(text)) return null;

            text = text.Trim();

            // 標準的なパースを試行
            if (DateTime.TryParse(text, out var result))
            {
                return result;
            }

            // 特定のフォーマットを試行
            if (DateTime.TryParseExact(text, Formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                return result;
            }

            // ログ行に含まれる日時を抽出するような複雑なロジックが必要な場合はここに追加するが、
            // 現状はコピーされた「日時そのもの」を対象とする。

            return null;
        }
    }
}
