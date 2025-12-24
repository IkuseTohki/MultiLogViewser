using MultiLogViewer.Models;
using System.Collections.Generic;

namespace MultiLogViewer.Services
{
    /// <summary>
    /// ログファイルの読み込みと解析を行うサービスのインターフェースです。
    /// </summary>
    public interface ILogFileReader
    {
        /// <summary>
        /// 指定されたログファイルを単一のフォーマット設定で読み込みます。
        /// </summary>
        /// <param name="filePath">読み込むファイルのパス。</param>
        /// <param name="config">適用するログフォーマット設定。</param>
        /// <returns>パースされたログエントリの一覧。</returns>
        IEnumerable<LogEntry> Read(string filePath, LogFormatConfig config);

        /// <summary>
        /// 指定された位置からログをインクリメンタルに読み込み、新しいエントリと更新された状態を返します（単一フォーマット用）。
        /// </summary>
        /// <param name="currentState">現在の読み込み状態（パス、位置など）。</param>
        /// <param name="config">適用するログフォーマット設定。</param>
        /// <returns>Entries: 新しく読み込まれたログエントリ, UpdatedState: 更新されたファイル状態。</returns>
        (IEnumerable<LogEntry> Entries, FileState UpdatedState) ReadIncremental(FileState currentState, LogFormatConfig config);

        /// <summary>
        /// 指定された複数のログファイルを読み込み、解析します。
        /// </summary>
        /// <param name="filePaths">読み込むログファイルのパス一覧。</param>
        /// <param name="config">適用するログフォーマット設定。</param>
        /// <returns>解析されたすべてのログエントリ。</returns>
        IEnumerable<LogEntry> ReadFiles(IEnumerable<string> filePaths, LogFormatConfig config);

        /// <summary>
        /// 指定されたログファイルを、複数のフォーマット候補を使用して読み込みます。
        /// </summary>
        /// <param name="filePath">読み込むファイルのパス。</param>
        /// <param name="configs">試行するログフォーマット設定の優先順リスト。</param>
        /// <returns>パースされたログエントリの一覧。</returns>
        IEnumerable<LogEntry> Read(string filePath, IEnumerable<LogFormatConfig> configs);

        /// <summary>
        /// 指定された位置からログをインクリメンタルに読み込み、複数のフォーマット候補を使用して解析します。
        /// </summary>
        /// <param name="currentState">現在の読み込み状態。</param>
        /// <param name="configs">試行するログフォーマット設定の優先順リスト。</param>
        /// <returns>Entries: 新しく読み込まれたログエントリ, UpdatedState: 更新されたファイル状態。</returns>
        (IEnumerable<LogEntry> Entries, FileState UpdatedState) ReadIncremental(FileState currentState, IEnumerable<LogFormatConfig> configs);
    }
}
