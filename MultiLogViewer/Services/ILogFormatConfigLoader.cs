using MultiLogViewer.Models;

namespace MultiLogViewer.Services
{
    /// <summary>
    /// 設定ファイル（YAML）からアプリケーション設定を読み込むサービスのインターフェースです。
    /// </summary>
    public interface ILogFormatConfigLoader
    {
        /// <summary>
        /// 指定されたパスの設定ファイル群を読み込み、統合された設定オブジェクトを生成します。
        /// </summary>
        /// <param name="logProfilePath">ログプロファイル(LogProfile.yaml)のパス。</param>
        /// <param name="appSettingsPath">アプリケーション設定(AppSettings.yaml)のパス。</param>
        /// <returns>統合された AppConfig オブジェクト。</returns>
        AppConfig Load(string logProfilePath, string appSettingsPath);
    }
}
