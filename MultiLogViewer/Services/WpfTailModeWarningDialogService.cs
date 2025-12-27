using MultiLogViewer.ViewModels;
using System.Windows;

namespace MultiLogViewer.Services
{
    public class WpfTailModeWarningDialogService : ITailModeWarningDialogService
    {
        public bool ShowWarning(out bool skipNextTime)
        {
            var viewModel = new TailModeWarningViewModel();
            var dialog = new TailModeWarningWindow
            {
                DataContext = viewModel,
                Owner = Application.Current.MainWindow
            };

            if (dialog.ShowDialog() == true)
            {
                skipNextTime = viewModel.SkipNextTime;
                return true;
            }

            skipNextTime = false;
            return false;
        }
    }
}
