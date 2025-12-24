using MultiLogViewer.ViewModels;
using System;
using System.Windows;

namespace MultiLogViewer.Services
{
    public class WpfGoToDateDialogService : IGoToDateDialogService
    {
        private Window? _window;

        public bool IsOpen => _window != null && _window.IsLoaded;

        public void Show(GoToDateViewModel viewModel, Action<DateTime> onJump)
        {
            if (_window == null || !_window.IsLoaded)
            {
                _window = new GoToDateWindow
                {
                    DataContext = viewModel,
                    Owner = Application.Current.MainWindow
                };

                // 初回表示位置の設定（右上に配置）
                if (_window.Owner != null)
                {
                    _window.WindowStartupLocation = WindowStartupLocation.Manual;
                    _window.Left = _window.Owner.Left + _window.Owner.ActualWidth - _window.Width - 20;
                    _window.Top = _window.Owner.Top + 60;
                }

                SubscribeEvents(viewModel, onJump);

                _window.Closed += (s, e) =>
                {
                    UnsubscribeEvents(viewModel, onJump);
                    _window = null;
                };
                _window.Show();
            }
            else
            {
                // すでに開いている場合
                var oldViewModel = _window.DataContext as GoToDateViewModel;
                if (oldViewModel != null)
                {
                    UnsubscribeEvents(oldViewModel, onJump);
                }

                _window.DataContext = viewModel;
                SubscribeEvents(viewModel, onJump);
                _window.Activate();
            }
        }

        private void SubscribeEvents(GoToDateViewModel viewModel, Action<DateTime> onJump)
        {
            viewModel.JumpRequested += onJump;
            viewModel.RequestClose += Close;
        }

        private void UnsubscribeEvents(GoToDateViewModel viewModel, Action<DateTime> onJump)
        {
            viewModel.JumpRequested -= onJump;
            viewModel.RequestClose -= Close;
        }

        public void Close()
        {
            if (_window != null)
            {
                _window.Close();
                _window = null;
            }
        }
    }
}
