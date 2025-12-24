using MultiLogViewer.Utils;
using System;
using System.Windows.Input;

namespace MultiLogViewer.ViewModels
{
    public class GoToDateViewModel : ViewModelBase
    {
        private bool _isSecondsEnabled;
        public bool IsSecondsEnabled
        {
            get => _isSecondsEnabled;
            set => SetProperty(ref _isSecondsEnabled, value);
        }

        private DateTime _targetDate = DateTime.Today;
        public DateTime TargetDate
        {
            get => _targetDate;
            set => SetProperty(ref _targetDate, value);
        }

        private string _targetTime = "00:00";
        public string TargetTime
        {
            get => _targetTime;
            set => SetProperty(ref _targetTime, value);
        }

        private int _relativeValue = 5;
        public int RelativeValue
        {
            get => _relativeValue;
            set => SetProperty(ref _relativeValue, value);
        }

        private string _relativeUnit = "Minutes";
        public string RelativeUnit
        {
            get => _relativeUnit;
            set => SetProperty(ref _relativeUnit, value);
        }

        private bool _isRelativeVisible = false;
        public bool IsRelativeVisible
        {
            get => _isRelativeVisible;
            set => SetProperty(ref _isRelativeVisible, value);
        }

        public ICommand JumpCommand { get; }
        public ICommand NextCommand { get; }
        public ICommand PreviousCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand OpenCalendarCommand { get; }
        public ICommand ToggleRelativeCommand { get; }

        public event Action<DateTime>? JumpRequested;
        public event Action? RequestClose;

        public GoToDateViewModel(DateTime initialDateTime, bool isSecondsEnabled = false)
        {
            _isSecondsEnabled = isSecondsEnabled;
            TargetDate = initialDateTime.Date;
            TargetTime = initialDateTime.ToString(isSecondsEnabled ? "HH:mm:ss" : "HH:mm");

            JumpCommand = new RelayCommand(_ => ExecuteJump());
            NextCommand = new RelayCommand(_ => ExecuteRelativeJump(1));
            PreviousCommand = new RelayCommand(_ => ExecuteRelativeJump(-1));
            CancelCommand = new RelayCommand(_ => RequestClose?.Invoke());
            ToggleRelativeCommand = new RelayCommand(_ => IsRelativeVisible = !IsRelativeVisible);
            OpenCalendarCommand = new RelayCommand(param =>
            {
                if (param is System.Windows.Controls.DatePicker dp)
                {
                    dp.IsDropDownOpen = true;
                }
            });
        }

        private void ExecuteJump()
        {
            if (TryParseCurrentDateTime(out var dt))
            {
                JumpRequested?.Invoke(dt);
            }
        }

        private void ExecuteRelativeJump(int direction)
        {
            if (!TryParseCurrentDateTime(out var currentDt)) return;

            var value = RelativeValue * direction;
            var newDt = RelativeUnit switch
            {
                "Seconds" => currentDt.AddSeconds(value),
                "Minutes" => currentDt.AddMinutes(value),
                "Hours" => currentDt.AddHours(value),
                "Days" => currentDt.AddDays(value),
                _ => currentDt
            };

            // 値を更新して反映
            TargetDate = newDt.Date;
            TargetTime = newDt.ToString(IsSecondsEnabled ? "HH:mm:ss" : "HH:mm");

            JumpRequested?.Invoke(newDt);
        }

        private bool TryParseCurrentDateTime(out DateTime result)
        {
            result = default;
            // 指定された秒の有無設定に関わらず、入力されている文字列からパースを試みる
            if (TimeSpan.TryParse(TargetTime, out var time))
            {
                result = TargetDate.Add(time);
                return true;
            }
            return false;
        }
    }
}
