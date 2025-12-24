using MultiLogViewer.Models;

namespace MultiLogViewer.Services
{
    /// <summary>
    /// 設定ファイル（YAML）からアプリケーション設定を読み込むサービスのインターフェースです。
    /// </summary>
    public interface ILogFormatConfigLoader
    {
        /// <summary>
        /// 指定されたパスの設定ファイルを読み込み、解析します。
        /// </summary>
        /// <param name="filePath">設定ファイルのパス。</param>
        /// <returns>解析された AppConfig オブジェクト。読み込み失敗時は null。</returns>
        AppConfig? Load(string filePath);
    }
}
