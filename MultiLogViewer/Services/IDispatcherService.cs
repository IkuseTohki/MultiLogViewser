using System;

namespace MultiLogViewer.Services
{
    /// <summary>
    /// UI スレッド（Dispatcher）への処理の委譲を行うサービスのインターフェースです。
    /// </summary>
    public interface IDispatcherService
    {
        /// <summary>
        /// 指定されたアクションを UI スレッドで実行します。
        /// </summary>
        /// <param name="action">実行するアクション。</param>
        void Invoke(Action action);
    }
}
