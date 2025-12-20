using MultiLogViewer.Models;
using System.Collections.Generic;

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

        /// <summary>
        /// 指定された設定ファイルと現在のファイル状態に基づいて、追加分のログを読み込みます。
        /// </summary>
        LogDataResult LoadIncremental(string configPath, List<FileState> currentStates);
    }
}
