namespace MultiLogViewer.Services
{
    /// <summary>
    /// クリップボード操作を提供するサービスのインターフェースです。
    /// </summary>
    public interface IClipboardService
    {
        /// <summary>
        /// 指定されたテキストをクリップボードに設定します。
        /// </summary>
        /// <param name="text">設定するテキスト。</param>
        void SetText(string text);

        /// <summary>
        /// クリップボードからテキストを取得します。
        /// </summary>
        /// <returns>クリップボード内のテキスト。テキストが含まれていない場合は null。</returns>
        string? GetText();
    }
}
