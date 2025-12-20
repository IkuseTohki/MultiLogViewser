using MultiLogViewer.Models;
using MultiLogViewer.Services;
using MultiLogViewer.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using System.Windows.Input;

namespace MultiLogViewer.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly ILogFileReader _logFileReader;
        private readonly IUserDialogService _userDialogService;
        private readonly ISearchWindowService _searchWindowService;
        private readonly ILogFormatConfigLoader _logFormatConfigLoader;
        private readonly IFileResolver _fileResolver;
        private readonly IConfigPathResolver _configPathResolver;

        private string _configPath = string.Empty;

        private readonly ObservableCollection<LogEntry> _logEntries = new ObservableCollection<LogEntry>();
        public ICollectionView LogEntriesView { get; }

        private LogEntry? _selectedLogEntry;
        public LogEntry? SelectedLogEntry
        {
            get => _selectedLogEntry;
            set
            {
                if (SetProperty(ref _selectedLogEntry, value))
                {
                    UpdateSearchStatus();
                }
            }
        }

        private SearchViewModel? _searchViewModel;

        private ObservableCollection<DisplayColumnConfig> _displayColumns = new ObservableCollection<DisplayColumnConfig>();
        public ObservableCollection<DisplayColumnConfig> DisplayColumns
        {
            get => _displayColumns;
            set => SetProperty(ref _displayColumns, value);
        }

        private string _filterText = string.Empty;
        public string FilterText
        {
            get => _filterText;
            set
            {
                if (SetProperty(ref _filterText, value))
                {
                    LogEntriesView.Refresh();
                }
            }
        }

        public ICommand RefreshCommand { get; }
        public ICommand OpenSearchCommand { get; }

        public MainViewModel(
            ILogFileReader logFileReader,
            IUserDialogService userDialogService,
            ISearchWindowService searchWindowService,
            ILogFormatConfigLoader logFormatConfigLoader,
            IFileResolver fileResolver,
            IConfigPathResolver configPathResolver)
        {
            _logFileReader = logFileReader;
            _userDialogService = userDialogService;
            _searchWindowService = searchWindowService;
            _logFormatConfigLoader = logFormatConfigLoader;
            _fileResolver = fileResolver;
            _configPathResolver = configPathResolver;

            LogEntriesView = CollectionViewSource.GetDefaultView(_logEntries);
            LogEntriesView.Filter = FilterLogEntries;

            RefreshCommand = new RelayCommand(_ => LoadLogs(_configPath));
            OpenSearchCommand = new RelayCommand(_ => OpenSearch());
        }

        private void OpenSearch()
        {
            if (_searchViewModel == null)
            {
                _searchViewModel = new SearchViewModel(
                    () => FindLogEntry(true),
                    () => FindLogEntry(false),
                    () => _searchWindowService.Close());

                _searchViewModel.PropertyChanged += (s, e) =>
                {
                    if (e.PropertyName == nameof(SearchViewModel.SearchText) ||
                        e.PropertyName == nameof(SearchViewModel.IsCaseSensitive) ||
                        e.PropertyName == nameof(SearchViewModel.IsRegex))
                    {
                        UpdateSearchStatus();
                    }
                };
            }
            UpdateSearchStatus();
            _searchWindowService.Show(_searchViewModel);
        }

        private void UpdateSearchStatus()
        {
            if (_searchViewModel == null) return;

            if (string.IsNullOrEmpty(_searchViewModel.SearchText))
            {
                _searchViewModel.StatusText = "";
                return;
            }

            var items = LogEntriesView.Cast<LogEntry>().ToList();
            if (!items.Any())
            {
                _searchViewModel.StatusText = "0/0";
                return;
            }

            // Calculate matches
            // Optimization: In a real large app, this should be async or debounced.
            int matchCount = 0;
            int currentIndex = 0; // 1-based index among matches
            bool currentFound = false;

            for (int i = 0; i < items.Count; i++)
            {
                if (IsMatch(items[i], _searchViewModel.SearchText, _searchViewModel.IsCaseSensitive, _searchViewModel.IsRegex))
                {
                    matchCount++;
                    if (items[i] == SelectedLogEntry)
                    {
                        currentIndex = matchCount;
                        currentFound = true;
                    }
                }
            }

            if (matchCount == 0)
            {
                _searchViewModel.StatusText = "No matches";
            }
            else
            {
                // If selected item is not a match (e.g. user clicked somewhere else), we just show count or "0/N" or "?/N"
                // Let's show "?/N" or just "N matches" if not on a match.
                string currentStr = currentFound ? currentIndex.ToString() : "?";
                _searchViewModel.StatusText = $"{currentStr}/{matchCount}";
            }
        }

        private void FindLogEntry(bool forward)
        {
            if (_searchViewModel == null || string.IsNullOrEmpty(_searchViewModel.SearchText)) return;

            var items = LogEntriesView.Cast<LogEntry>().ToList();
            if (!items.Any()) return;

            int startIndex = -1;
            if (SelectedLogEntry != null)
            {
                startIndex = items.IndexOf(SelectedLogEntry);
            }

            int index = startIndex;
            int count = items.Count;
            int foundIndex = -1;

            // Prevent infinite loop if logic is wrong, though for loop handles it.
            // Loop through all items once starting from next/prev item.
            for (int i = 1; i <= count; i++)
            {
                int k;
                if (forward)
                {
                    k = (startIndex + i) % count;
                }
                else
                {
                    k = (startIndex - i + count) % count;
                }

                if (IsMatch(items[k], _searchViewModel.SearchText, _searchViewModel.IsCaseSensitive, _searchViewModel.IsRegex))
                {
                    foundIndex = k;
                    break;
                }
            }

            if (foundIndex != -1)
            {
                SelectedLogEntry = items[foundIndex];
                // UpdateSearchStatus will be called via PropertyChanged of SelectedLogEntry?
                // No, I need to call it manually or subscribe to SelectedLogEntry changes.
                // Since SelectedLogEntry setter calls OnPropertyChanged, I can override the setter or subscribe to my own event.
                // Easier: just call UpdateSearchStatus() here.
                UpdateSearchStatus();
            }
            else
            {
                // Optional: Show "No more matches" toast or beep
            }
        }

        private bool IsMatch(LogEntry entry, string searchText, bool caseSensitive, bool useRegex)
        {
            if (useRegex)
            {
                try
                {
                    var options = caseSensitive ? System.Text.RegularExpressions.RegexOptions.None : System.Text.RegularExpressions.RegexOptions.IgnoreCase;
                    var regex = new System.Text.RegularExpressions.Regex(searchText, options);

                    if (regex.IsMatch(entry.Message)) return true;
                    if (regex.IsMatch(entry.FileName)) return true;
                    foreach (var value in entry.AdditionalData.Values)
                    {
                        if (regex.IsMatch(value)) return true;
                    }
                }
                catch
                {
                    // Invalid regex, treat as no match or maybe show error? 
                    // For now, ignore invalid regex to prevent crash.
                    return false;
                }
            }
            else
            {
                var comparison = caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

                if (entry.Message.IndexOf(searchText, comparison) >= 0) return true;
                if (entry.FileName.IndexOf(searchText, comparison) >= 0) return true;
                foreach (var value in entry.AdditionalData.Values)
                {
                    if (value.IndexOf(searchText, comparison) >= 0) return true;
                }
            }
            return false;
        }

        /// <summary>
        /// ViewModel を初期化し、指定された設定ファイルからログを読み込みます。
        /// </summary>
        /// <param name="configPath">設定ファイルのパス。</param>
        public void Initialize(string configPath)
        {
            _configPath = configPath;
            LoadLogs(_configPath);
        }

        private void LoadLogs(string configPath)
        {
            if (string.IsNullOrEmpty(configPath)) return;

            var appConfig = _logFormatConfigLoader.Load(configPath);
            if (appConfig == null)
            {
                _userDialogService.ShowError("Could not load or parse config.yaml.", "Error");
                return;
            }

            // DisplayColumns を設定
            if (appConfig.DisplayColumns != null && appConfig.DisplayColumns.Any())
            {
                DisplayColumns = new ObservableCollection<DisplayColumnConfig>(appConfig.DisplayColumns);
            }

            // LogFormats からログを読み込む
            if (appConfig.LogFormats == null || !appConfig.LogFormats.Any())
            {
                _logEntries.Clear(); // ログフォーマットがない場合はクリアする
                return;
            }

            var allEntries = new List<LogEntry>();
            foreach (var logFormatConfig in appConfig.LogFormats)
            {
                if (logFormatConfig.LogFilePatterns != null && logFormatConfig.LogFilePatterns.Any())
                {
                    var filePaths = _fileResolver.Resolve(logFormatConfig.LogFilePatterns);
                    var entries = _logFileReader.ReadFiles(filePaths, logFormatConfig);
                    allEntries.AddRange(entries);
                }
            }

            // エントリをタイムスタンプでソートしてコレクションに追加
            _logEntries.Clear();
            foreach (var entry in allEntries.OrderBy(e => e.Timestamp))
            {
                _logEntries.Add(entry);
            }

            LogEntriesView.Refresh();
        }

        private bool FilterLogEntries(object obj)
        {
            if (obj is LogEntry entry)
            {
                if (string.IsNullOrWhiteSpace(FilterText))
                {
                    return true;
                }
                return entry.Message.Contains(FilterText, System.StringComparison.OrdinalIgnoreCase);
            }
            return false;
        }
    }
}
