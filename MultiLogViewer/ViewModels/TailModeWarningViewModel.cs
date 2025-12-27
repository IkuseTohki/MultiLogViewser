using MultiLogViewer.Utils;
using System.Windows.Input;

namespace MultiLogViewer.ViewModels
{
    /// <summary>
    /// Tailモード有効化時の警告ダイアログ用 ViewModel です。
    /// </summary>
    public class TailModeWarningViewModel : ViewModelBase
    {
        private bool _skipNextTime;
        private bool? _dialogResult;

        /// <summary>
        /// 「次回から表示しない」チェックボックスの状態を取得または設定します。
        /// </summary>
        public bool SkipNextTime
        {
            get => _skipNextTime;
            set => SetProperty(ref _skipNextTime, value);
        }

        /// <summary>
        /// ウィンドウの DialogResult を制御するための値を取得または設定します。
        /// </summary>
        public bool? DialogResult
        {
            get => _dialogResult;
            set => SetProperty(ref _dialogResult, value);
        }

        /// <summary>
        /// Tailモードを有効にするコマンドを取得します。
        /// </summary>
        public ICommand EnableCommand { get; }

        public TailModeWarningViewModel()
        {
            EnableCommand = new RelayCommand(_ => DialogResult = true);
        }
    }
}
