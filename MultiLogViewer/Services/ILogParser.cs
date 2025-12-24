using MultiLogViewer.Models;

namespace MultiLogViewer.Services
{
    /// <summary>
    /// ログの 1 行をパースして構造化データに変換するサービスのインターフェースです。
    /// </summary>
    public interface ILogParser
    {
        /// <summary>
        /// ログ行を解析し、LogEntry オブジェクトに変換します。
        /// </summary>
        /// <param name="logLine">解析対象のログ行文字列。</param>
        /// <param name="fileName">ログが発生したファイル名。</param>
        /// <param name="lineNumber">行番号。</param>
        /// <returns>パースに成功した場合は LogEntry オブジェクト、失敗した場合は null。</returns>
        LogEntry? Parse(string logLine, string fileName, int lineNumber);
    }
}
