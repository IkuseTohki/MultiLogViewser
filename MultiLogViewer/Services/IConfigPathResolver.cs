namespace MultiLogViewer.Services
{
    /// <summary>
    /// 設定ファイルのパスを解決する責務を担います。
    /// </summary>
    public interface IConfigPathResolver
    {
        /// <summary>
        /// コマンドライン引数を基に、ログプロファイル(LogProfile.yaml)の絶対パスを解決します。
        /// </summary>
        /// <param name="args">アプリケーションのコマンドライン引数。</param>
        /// <returns>解決されたログプロファイルの絶対パス。</returns>
        string ResolveLogProfilePath(string[] args);

        /// <summary>
        /// アプリケーション設定(AppSettings.yaml)の絶対パスを取得します。
        /// </summary>
        /// <returns>AppSettings.yamlの絶対パス。</returns>
        string GetAppSettingsPath();
    }
}
