namespace MultiLogViewer.Services
{
    /// <summary>
    /// 検索ウィンドウの表示を制御するサービスのインターフェースです。
    /// </summary>
    public interface ISearchWindowService
    {
        /// <summary>
        /// 検索ウィンドウを表示します。
        /// </summary>
        /// <param name="viewModel">ウィンドウにバインドする ViewModel オブジェクト。</param>
        void Show(object viewModel);

        /// <summary>
        /// 検索ウィンドウを閉じます。
        /// </summary>
        void Close();

        /// <summary>
        /// 検索ウィンドウが開いているかどうかを示す値を取得します。
        /// </summary>
        bool IsOpen { get; }
    }
}
