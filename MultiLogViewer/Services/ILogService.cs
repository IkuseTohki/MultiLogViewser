using MultiLogViewer.Models;

namespace MultiLogViewer.Services
{
    /// <summary>
    /// ログデータの取得と管理を担当するサービスのインターフェースです。
    /// </summary>
    public interface ILogService
    {
        /// <summary>
        /// 指定された設定ファイルに基づいてログを読み込みます。
        /// </summary>
        LogDataResult LoadFromConfig(string configPath);
    }
}
