namespace MultiLogViewer.Services
{
    /// <summary>
    /// 設定ファイルのパスを解決する責務を担います。
    /// </summary>
    public interface IConfigPathResolver
    {
        /// <summary>
        /// コマンドライン引数を基に、設定ファイルの絶対パスを解決します。
        /// </summary>
        /// <param name="args">アプリケーションのコマンドライン引数。</param>
        /// <returns>解決された設定ファイルの絶対パス。</returns>
        string ResolvePath(string[] args);
    }
}
