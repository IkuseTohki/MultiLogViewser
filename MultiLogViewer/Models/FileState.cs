namespace MultiLogViewer.Models
{
    /// <summary>
    /// ファイルの読み込み状態（末尾位置など）を保持するクラスです。
    /// </summary>
    public class FileState
    {
        public string FilePath { get; set; } = string.Empty;
        public long LastPosition { get; set; }
        public int LastLineNumber { get; set; }

        public FileState(string filePath, long lastPosition, int lastLineNumber)
        {
            FilePath = filePath;
            LastPosition = lastPosition;
            LastLineNumber = lastLineNumber;
        }
    }
}
