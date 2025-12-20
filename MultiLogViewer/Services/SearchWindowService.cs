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
                }
                _searchWindow.DataContext = viewModel;
                _searchWindow.Closed += (s, e) => _searchWindow = null;
                _searchWindow.Show();
            }
            else
            {
                _searchWindow.DataContext = viewModel; // Ensure latest ViewModel
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
