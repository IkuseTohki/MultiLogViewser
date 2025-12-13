using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MultiLogViewer.Models;
using MultiLogViewer.Services;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq; // OrderBy, OrderByDescending を使用するため追加

namespace MultiLogViewer.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly ILogFileReader _logFileReader;
        private readonly IUserDialogService _userDialogService;

        [ObservableProperty]
        private ObservableCollection<LogEntry> _logEntries = new();

        private bool _isAscending = true; // ソート方向を保持するフラグ

        public MainViewModel(ILogFileReader logFileReader, IUserDialogService userDialogService)
        {
            _logFileReader = logFileReader;
            _userDialogService = userDialogService;
        }

        [RelayCommand]
        private void OpenFile()
        {
            var filePath = _userDialogService.OpenFileDialog();
            if (filePath != null)
            {
                // TODO: どのLogFormatConfigを使用するかを選択するUIが必要。
                //       現時点では、最初のConfigを決め打ちで使用する。
                //       将来的には、設定ファイルから読み込んだConfigを選択できるようにする。
                var logFormatConfig = new LogFormatConfig
                {
                    Name = "ApplicationLog",
                    Pattern = @"^(?<timestamp>\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2}) \[(?<level>\w+)\] (?<message>.*)$",
                    TimestampFormat = "yyyy-MM-dd HH:mm:ss"
                };

                LogEntries.Clear();
                foreach (var entry in _logFileReader.Read(filePath, logFormatConfig))
                {
                    LogEntries.Add(entry);
                }
            }
        }

        /// <summary>
        /// ログエントリをTimestampでソートするコマンド。
        /// 実行ごとに昇順/降順を切り替えます。
        /// </summary>
        [RelayCommand]
        private void SortByTimestamp()
        {
            if (!LogEntries.Any()) return;

            if (_isAscending)
            {
                // 昇順ソート
                var sorted = new ObservableCollection<LogEntry>(LogEntries.OrderBy(e => e.Timestamp));
                LogEntries.Clear();
                foreach (var item in sorted)
                {
                    LogEntries.Add(item);
                }
            }
            else
            {
                // 降順ソート
                var sorted = new ObservableCollection<LogEntry>(LogEntries.OrderByDescending(e => e.Timestamp));
                LogEntries.Clear();
                foreach (var item in sorted)
                {
                    LogEntries.Add(item);
                }
            }
            _isAscending = !_isAscending; // ソート方向を反転
        }
    }
}
