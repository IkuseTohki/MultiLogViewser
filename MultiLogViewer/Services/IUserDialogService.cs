namespace MultiLogViewer.Services
{
    /// <summary>
    /// ファイル選択ダイアログやメッセージボックスなどのユーザー通知を提供するサービスのインターフェースです。
    /// </summary>
    public interface IUserDialogService
    {
        /// <summary>
        /// ファイルを開くダイアログを表示します。
        /// </summary>
        /// <param name="filter">ファイルフィルター（例: "YAML files (*.yaml)|*.yaml"）。</param>
        /// <returns>選択されたファイルのフルパス。キャンセルされた場合は null。</returns>
        string? OpenFileDialog(string filter = "All files (*.*)|*.*");

        /// <summary>
        /// ファイルを保存するダイアログを表示します。
        /// </summary>
        /// <param name="filter">ファイルフィルター。</param>
        /// <param name="defaultFileName">デフォルトのファイル名。</param>
        /// <returns>保存先のフルパス。キャンセルされた場合は null。</returns>
        string? SaveFileDialog(string filter = "All files (*.*)|*.*", string defaultFileName = "");

        /// <summary>
        /// エラーメッセージを表示します。
        /// </summary>
        /// <param name="title">ダイアログのタイトル。</param>
        /// <param name="message">表示するメッセージ内容。</param>
        void ShowError(string title, string message);
    }
}
