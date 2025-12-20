using MultiLogViewer.Utils;
using System;
using System.Windows.Input;

namespace MultiLogViewer.ViewModels
{
    public class SearchViewModel : ViewModelBase
    {
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set => SetProperty(ref _searchText, value);
        }

        private bool _isCaseSensitive;
        public bool IsCaseSensitive
        {
            get => _isCaseSensitive;
            set => SetProperty(ref _isCaseSensitive, value);
        }

        private bool _isRegex;
        public bool IsRegex
        {
            get => _isRegex;
            set => SetProperty(ref _isRegex, value);
        }

        private string _statusText = string.Empty;
        public string StatusText
        {
            get => _statusText;
            set => SetProperty(ref _statusText, value);
        }

        public ICommand FindNextCommand { get; }
        public ICommand FindPreviousCommand { get; }
        public ICommand CloseCommand { get; }

        public SearchViewModel(Action findNext, Action findPrev, Action close)
        {
            FindNextCommand = new RelayCommand(_ => findNext?.Invoke());
            FindPreviousCommand = new RelayCommand(_ => findPrev?.Invoke());
            CloseCommand = new RelayCommand(_ => close?.Invoke());
        }
    }
}
