using System;
using System.Threading.Tasks;

namespace MultiLogViewer.Services
{
    /// <summary>
    /// バックグラウンドタスクの実行を抽象化するインターフェースです（テスト容易性のため）。
    /// </summary>
    public interface ITaskRunner
    {
        /// <summary>
        /// 指定されたアクションを非同期で実行します。
        /// </summary>
        /// <param name="action">実行するアクション。</param>
        /// <returns>実行を表す Task。</returns>
        Task Run(Action action);
    }

    public class TaskRunner : ITaskRunner
    {
        public Task Run(Action action)
        {
            return Task.Run(action);
        }
    }
}
