using MultiLogViewer.ViewModels;

namespace MultiLogViewer.Services
{
    /// <summary>
    /// 日時指定ジャンプダイアログの表示を制御するサービスのインターフェースです。
    /// </summary>
    public interface IGoToDateDialogService
    {
        /// <summary>
        /// 日時指定ジャンプダイアログを表示します。
        /// </summary>
        /// <param name="viewModel">ダイアログに使用する ViewModel。</param>
        /// <param name="onJump">ジャンプが要求された際に実行されるコールバック。</param>
        void Show(GoToDateViewModel viewModel, System.Action<System.DateTime> onJump);

        /// <summary>
        /// ダイアログを閉じます。
        /// </summary>
        void Close();

        /// <summary>
        /// ダイアログが開いているかどうかを示す値を取得します。
        /// </summary>
        bool IsOpen { get; }
    }
}
