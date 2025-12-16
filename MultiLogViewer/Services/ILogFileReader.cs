using MultiLogViewer.Models;
using System.Collections.Generic;

namespace MultiLogViewer.Services
{
    public interface ILogFileReader
    {
        IEnumerable<LogEntry> Read(string filePath, LogFormatConfig config);

        /// <summary>
        /// 指定されたログフォーマット設定に基づいて、複数のログファイルを読み込み、解析します。
        /// </summary>
        /// <param name="filePaths">読み込むログファイルのパスのリスト。</param>
        /// <param name="config">ログファイルの解析に使用するフォーマット設定。</param>
        /// <returns>解析されたログエントリのリスト。</returns>
        IEnumerable<LogEntry> ReadFiles(IEnumerable<string> filePaths, LogFormatConfig config);
    }
}
