using System.Windows;

namespace MultiLogViewer.Services
{
    public class SearchWindowService : ISearchWindowService
    {
        private Window? _searchWindow;

        public bool IsOpen => _searchWindow != null && _searchWindow.IsLoaded;

        public void Show(object viewModel)
        {
            if (_searchWindow == null || !_searchWindow.IsLoaded)
            {
                _searchWindow = new SearchWindow();
                if (Application.Current != null && Application.Current.MainWindow != null)
                {
                    _searchWindow.Owner = Application.Current.MainWindow;

                    // 親ウィンドウの右上に配置するための初期計算
                    var owner = _searchWindow.Owner;

                    // TODO: 親ウィンドウが最大化されている場合、owner.Left/Top がスクリーン座標外（マルチモニタ環境など）
                    // を指す可能性があるため、必要に応じて RestoreBounds やモニター情報を考慮した計算を行う。
                    // 現状は単純な座標加算で実装。

                    // 初期表示位置を直接指定できるように設定
                    _searchWindow.WindowStartupLocation = WindowStartupLocation.Manual;

                    // 座標の計算
                    double margin = 20;
                    double titleBarHeight = 30; // タイトルバーの概算高さ

                    // ウィンドウ幅を取得（XAMLで指定されている300を使用）
                    double width = _searchWindow.Width;
                    if (double.IsNaN(width)) width = 300;

                    _searchWindow.Left = owner.Left + owner.Width - width - margin;
                    _searchWindow.Top = owner.Top + titleBarHeight + margin;
                }
                _searchWindow.DataContext = viewModel;
                _searchWindow.Closed += (s, e) => _searchWindow = null;
                _searchWindow.Show();
            }
            else
            {
                _searchWindow.DataContext = viewModel; // 最新の ViewModel を反映
                _searchWindow.Activate();
            }
        }

        public void Close()
        {
            if (_searchWindow != null)
            {
                _searchWindow.Close();
                _searchWindow = null;
            }
        }
    }
}
