using System.Windows;

namespace MultiLogViewer
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// テスト観点: 「ファイル」メニュー内の「終了」項目がクリックされた際、アプリケーションが終了することを確認する。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベントデータ。</param>
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        /// <summary>
        /// テスト観点: 「設定」メニュー内の「設定画面を開く」項目がクリックされた際、設定画面が開くことを確認する。
        /// </summary>
        /// <param name="sender">イベントのソース。</param>
        /// <param name="e">イベントデータ。</param>
        private void OpenSettingsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            // 設定画面を開くロジックをここに記述
            SettingsWindow settingsWindow = new SettingsWindow();
            settingsWindow.ShowDialog();
        }
    }
}
