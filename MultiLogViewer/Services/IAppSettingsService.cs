using MultiLogViewer.Models;

namespace MultiLogViewer.Services
{
    /// <summary>
    /// アプリケーションの動作設定を永続化層（ファイル等）から読み書きするためのサービスインターフェースです。
    /// </summary>
    public interface IAppSettingsService
    {
        /// <summary>
        /// アプリケーション設定を読み込みます。
        /// </summary>
        /// <returns>読み込まれた設定を保持する <see cref="AppSettings"/> オブジェクト。ファイルが存在しないか読み込みに失敗した場合はデフォルト値を返します。</returns>
        AppSettings Load();

        /// <summary>
        /// アプリケーション設定を保存します。
        /// </summary>
        /// <param name="settings">保存対象となる設定情報を含む <see cref="AppSettings"/> オブジェクト。</param>
        void Save(AppSettings settings);
    }
}
