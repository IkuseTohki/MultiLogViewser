using MultiLogViewer.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Ude;

namespace MultiLogViewer.Services
{
    public class LogFileReader : ILogFileReader
    {
        public LogFileReader()
        {
        }

        public IEnumerable<LogEntry> Read(string filePath, LogFormatConfig config)
        {
            var (entries, _) = ReadInternal(filePath, 0, 0, config);
            return entries;
        }

        public (IEnumerable<LogEntry> Entries, FileState UpdatedState) ReadIncremental(FileState currentState, LogFormatConfig config)
        {
            if (!File.Exists(currentState.FilePath))
            {
                return (Enumerable.Empty<LogEntry>(), currentState);
            }

            var fileInfo = new FileInfo(currentState.FilePath);
            if (fileInfo.Length < currentState.LastPosition)
            {
                // ファイルが小さくなった（ローテーション等）場合は最初から読み直す
                var (entries, newState) = ReadInternal(currentState.FilePath, 0, 0, config);
                return (entries, newState);
            }

            if (fileInfo.Length == currentState.LastPosition)
            {
                return (Enumerable.Empty<LogEntry>(), currentState);
            }

            var (newEntries, updatedState) = ReadInternal(currentState.FilePath, currentState.LastPosition, currentState.LastLineNumber, config);
            return (newEntries, updatedState);
        }

        private (IEnumerable<LogEntry> Entries, FileState State) ReadInternal(string filePath, long startPosition, int startLineNumber, LogFormatConfig config)
        {
            if (!File.Exists(filePath))
            {
                return (Enumerable.Empty<LogEntry>(), new FileState(filePath, 0, 0));
            }

            var parser = new LogParser(config);
            var results = new List<LogEntry>();
            long endPosition = startPosition;
            int currentLineNumber = startLineNumber;

            using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                // エンコーディングを自動判別
                System.Text.Encoding encoding = DetectFileEncoding(fs);
                fs.Seek(startPosition, SeekOrigin.Begin);

                using (var streamReader = new StreamReader(fs, encoding))
                {
                    string? line;
                    string fileName = Path.GetFileName(filePath);
                    LogEntry? currentEntry = null;

                    while ((line = streamReader.ReadLine()) != null)
                    {
                        currentLineNumber++;
                        var entry = parser.Parse(line, fileName, currentLineNumber);

                        if (entry != null)
                        {
                            if (currentEntry != null)
                            {
                                results.Add(currentEntry);
                            }
                            entry.RawLine = line;
                            entry.FileFullPath = filePath;
                            currentEntry = entry;
                        }
                        else if (currentEntry != null && config.IsMultiline)
                        {
                            currentEntry.Message += System.Environment.NewLine + line;
                            currentEntry.RawLine += System.Environment.NewLine + line;
                        }
                    }

                    if (currentEntry != null)
                    {
                        results.Add(currentEntry);
                    }

                    // 読み込み終わった時点のストリームの絶対位置を取得する
                    // 注意: StreamReaderがバッファリングしているため、fs.Positionではなく工夫が必要
                    // 簡易的にfs.Lengthを使うか、自前でバイト数を計算するが、ここではfs.Lengthを最終位置とする
                    endPosition = fs.Length;
                }
            }

            return (results, new FileState(filePath, endPosition, currentLineNumber));
        }

        /// <summary>
        /// ファイルの文字コードを自動判別します。
        /// </summary>
        /// <param name="fileStream">判別するファイルのFileStream。</param>
        /// <returns>判別された文字コード。判別できなかった場合はUTF8を返します。</returns>
        private System.Text.Encoding DetectFileEncoding(FileStream fileStream)
        {
            var cdet = new CharsetDetector();
            cdet.Feed(fileStream); // ここでストリームの一部を読み込む
            cdet.DataEnd();

            if (cdet.Charset != null)
            {
                // Shift-JIS の場合は明示的にコードページ 932 を指定
                if (cdet.Charset.Equals("Shift-JIS", System.StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        return System.Text.Encoding.GetEncoding(932); // コードページ932 (Shift-JIS) を試す
                    }
                    catch (System.ArgumentException)
                    {
                        return System.Text.Encoding.UTF8; // フォールバック
                    }
                }

                try
                {
                    string detectedCharset = cdet.Charset; // ローカル変数に代入し、nullではないことを保証
                    string charsetName = detectedCharset.Replace('-', '_'); // ハイフンをアンダースコアに変換
                    return System.Text.Encoding.GetEncoding(charsetName);
                }
                catch (System.ArgumentException)
                {
                    // サポートされていないエンコーディングの場合はUTF8をフォールバック
                    return System.Text.Encoding.UTF8;
                }
            }
            // 判別できなかった場合はUTF8を返す
            return System.Text.Encoding.UTF8;
        }

        public IEnumerable<LogEntry> ReadFiles(IEnumerable<string> filePaths, LogFormatConfig config)
        {
            if (filePaths == null || !filePaths.Any())
            {
                return Enumerable.Empty<LogEntry>();
            }

            var allLogEntries = new List<LogEntry>();
            foreach (var filePath in filePaths)
            {
                allLogEntries.AddRange(Read(filePath, config));
            }
            return allLogEntries;
        }
    }
}
