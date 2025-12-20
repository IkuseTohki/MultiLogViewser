using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MultiLogViewer.Models;
using MultiLogViewer.Services;
using MultiLogViewer.ViewModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MultiLogViewer.Tests
{
    [TestClass]
    public class MainViewModelTests
    {
        private Mock<ILogFileReader> _mockLogFileReader = null!;
        private Mock<IUserDialogService> _mockUserDialogService = null!;
        private Mock<ISearchWindowService> _mockSearchWindowService = null!;
        private Mock<ILogFormatConfigLoader> _mockLogFormatConfigLoader = null!;
        private Mock<IFileResolver> _mockFileResolver = null!;
        private Mock<IConfigPathResolver> _mockConfigPathResolver = null!;
        private MainViewModel _viewModel = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockLogFileReader = new Mock<ILogFileReader>();
            _mockUserDialogService = new Mock<IUserDialogService>();
            _mockSearchWindowService = new Mock<ISearchWindowService>();
            _mockLogFormatConfigLoader = new Mock<ILogFormatConfigLoader>();
            _mockFileResolver = new Mock<IFileResolver>();
            _mockConfigPathResolver = new Mock<IConfigPathResolver>();
        }

        private MainViewModel CreateViewModel()
        {
            return new MainViewModel(
                _mockLogFileReader.Object,
                _mockUserDialogService.Object,
                _mockSearchWindowService.Object,
                _mockLogFormatConfigLoader.Object,
                _mockFileResolver.Object,
                _mockConfigPathResolver.Object);
        }

        [TestMethod]
        public void Initialize_Successful_LoadsLogsAndColumns()
        {
            // Arrange
            var displayColumns = new List<DisplayColumnConfig>
            {
                new DisplayColumnConfig { Header = "Timestamp", BindingPath = "Timestamp" }
            };

            var logFormats = new List<LogFormatConfig>
            {
                new LogFormatConfig
                {
                    Name = "TestFormat",
                    LogFilePatterns = new List<string> { "test-*.log" }
                }
            };

            var appConfig = new AppConfig
            {
                DisplayColumns = displayColumns,
                LogFormats = logFormats
            };

            var filePaths = new List<string> { "C:\\dummy\\test-1.log" };
            var logEntries = new List<LogEntry>
            {
                new LogEntry { Message = "Entry 1" }
            };

            _mockLogFormatConfigLoader.Setup(l => l.Load(It.IsAny<string>())).Returns(appConfig);
            _mockFileResolver.Setup(r => r.Resolve(logFormats[0].LogFilePatterns)).Returns(filePaths);
            _mockLogFileReader.Setup(r => r.ReadFiles(filePaths, logFormats[0])).Returns(logEntries);

            // Act
            _viewModel = CreateViewModel();
            _viewModel.Initialize("dummy_path");

            // Assert
            Assert.AreEqual(1, _viewModel.LogEntriesView.Cast<LogEntry>().Count());
            Assert.AreEqual("Entry 1", _viewModel.LogEntriesView.Cast<LogEntry>().First().Message);

            Assert.AreEqual(1, _viewModel.DisplayColumns.Count);
            Assert.AreEqual("Timestamp", _viewModel.DisplayColumns[0].Header);

            _mockLogFormatConfigLoader.Verify(l => l.Load(It.IsAny<string>()), Times.Once);
            _mockFileResolver.Verify(r => r.Resolve(logFormats[0].LogFilePatterns), Times.Once);
            _mockLogFileReader.Verify(r => r.ReadFiles(filePaths, logFormats[0]), Times.Once);
        }

        [TestMethod]
        public void Initialize_ConfigLoadFails_ShowsError()
        {
            // Arrange
            _mockLogFormatConfigLoader.Setup(l => l.Load(It.IsAny<string>())).Returns((AppConfig?)null);

            // Act
            _viewModel = CreateViewModel();
            _viewModel.Initialize("dummy_path");

            // Assert
            _mockUserDialogService.Verify(s => s.ShowError(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            Assert.AreEqual(0, _viewModel.LogEntriesView.Cast<LogEntry>().Count());
        }

        [TestMethod]
        public void FilterText_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            var appConfig = new AppConfig();
            _mockLogFormatConfigLoader.Setup(l => l.Load(It.IsAny<string>())).Returns(appConfig);

            _viewModel = CreateViewModel();

            var propertyChangedFired = false;
            _viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.FilterText))
                {
                    propertyChangedFired = true;
                }
            };

            // Act
            _viewModel.FilterText = "new filter";

            // Assert
            Assert.IsTrue(propertyChangedFired, "PropertyChanged event for FilterText was not raised.");
        }

        [TestMethod]
        public void DisplayColumns_WhenSet_RaisesPropertyChanged()
        {
            // Arrange
            var appConfig = new AppConfig();
            _mockLogFormatConfigLoader.Setup(l => l.Load(It.IsAny<string>())).Returns(appConfig);

            _viewModel = CreateViewModel();

            var propertyChangedFired = false;
            _viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(MainViewModel.DisplayColumns))
                {
                    propertyChangedFired = true;
                }
            };

            // Act
            _viewModel.DisplayColumns = new ObservableCollection<DisplayColumnConfig>();

            // Assert
            Assert.IsTrue(propertyChangedFired, "PropertyChanged event for DisplayColumns was not raised.");
        }

        [TestMethod]
        public void RefreshCommand_WhenSubPatternsChanged_ReloadsLogWithNewPattern()
        {
            // テスト観点: Refreshコマンドが実行された際に、変更されたsub_patternsが
            //            正しくログの再パースに適用されることを確認する。

            // Arrange
            var tempLogFileName = Path.GetTempFileName();
            var logLine = "[INFO] Initial message";
            File.WriteAllText(tempLogFileName, logLine);

            try
            {
                // 1. 初期設定 (config1): sub_pattern 'level' が 'INFO' を抽出する
                var config1 = new AppConfig
                {
                    LogFormats = new List<LogFormatConfig>
                    {
                        new LogFormatConfig
                        {
                            Name = "TestFormat",
                            LogFilePatterns = new List<string> { tempLogFileName },
                            Pattern = @"^(?<message>.*)$", // ログ行全体をmessageとしてキャプチャ
                            SubPatterns = new List<SubPatternConfig>
                            {
                                new SubPatternConfig { SourceField = "message", Pattern = @"\[(?<level>\w+)\]" }
                            }
                        }
                    }
                };

                // LogFormatConfigLoaderのモック: 最初にconfig1を返す
                _mockLogFormatConfigLoader.Setup(l => l.Load(It.IsAny<string>())).Returns(config1);

                _mockFileResolver.Setup(r => r.Resolve(It.IsAny<List<string>>())).Returns(new List<string> { tempLogFileName });

                var realLogFileReader = new LogFileReader();

                // ViewModelを生成
                _viewModel = new MainViewModel(
                    realLogFileReader, // 実インスタンスを使用
                    _mockUserDialogService.Object,
                    _mockSearchWindowService.Object,
                    _mockLogFormatConfigLoader.Object,
                    _mockFileResolver.Object,
                    _mockConfigPathResolver.Object);

                // Act (1): 初期読み込み
                _viewModel.Initialize("dummy_path");

                // Assert (1): 初期状態で正しくパースされていることを確認
                var entry1 = _viewModel.LogEntriesView.Cast<LogEntry>().FirstOrDefault();
                Assert.IsNotNull(entry1, "Entry should not be null after initial load.");
                Assert.AreEqual(logLine, entry1.Message.Trim(), "Message should be the full log line initially.");
                Assert.IsTrue(entry1.AdditionalData.ContainsKey("level"), "The 'level' field should be extracted initially.");
                Assert.AreEqual("INFO", entry1.AdditionalData["level"], "The 'level' should be 'INFO' initially.");

                // Arrange (2)

                // 2. 新しい設定 (config2): sub_pattern 'level' が 'DEBUG' のみを抽出する
                var config2 = new AppConfig
                {
                    LogFormats = new List<LogFormatConfig>
                    {
                        new LogFormatConfig
                        {
                            Name = "TestFormat",
                            LogFilePatterns = new List<string> { tempLogFileName },
                            Pattern = @"^(?<message>.*)$", // メインパターンは同じ
                            SubPatterns = new List<SubPatternConfig>
                            {
                                new SubPatternConfig { SourceField = "message", Pattern = @"\[(?<level>DEBUG)\]" }
                            }
                        }
                    }
                };

                // LogFormatConfigLoaderのモックを更新: 次にLoadが呼ばれたらconfig2を返す
                _mockLogFormatConfigLoader.Setup(l => l.Load(It.IsAny<string>())).Returns(config2);

                // Act (2): 更新を実行
                _viewModel.RefreshCommand.Execute(null);

                // Assert (2): 更新後、新しいパターンでパースされていることを確認
                var entry2 = _viewModel.LogEntriesView.Cast<LogEntry>().FirstOrDefault();
                Assert.IsNotNull(entry2, "Entry should not be null after refresh.");
                Assert.AreEqual(logLine, entry2.Message.Trim(), "Message should be the full log line after refresh.");

                // 'DEBUG'パターンにはマッチしないため、'level'フィールドは抽出されないはず
                Assert.IsFalse(entry2.AdditionalData.ContainsKey("level"), "The 'level' field should not be extracted after refresh.");
            }
            finally
            {
                if (File.Exists(tempLogFileName))
                {
                    File.Delete(tempLogFileName);
                }
            }
        }
        [TestMethod]
        public void RefreshCommand_WhenSubPatternSourceFieldCountChanges_AppliesNewSubPatterns()
        {
            // テスト観点: 依存関係を持つsub_patternが追加された場合に、Refreshコマンドの実行によって
            //            全てのサブパターンが正しい順序で適用されるかを確認する。

            // Arrange
            var tempLogFileName = Path.GetTempFileName();
            var logLine = "[MyCoolApp.exe] - Initial message";
            File.WriteAllText(tempLogFileName, logLine);

            try
            {
                // 1. 初期設定 (config1): サブパターンは1つ
                var config1 = new AppConfig
                {
                    LogFormats = new List<LogFormatConfig>
                    {
                        new LogFormatConfig
                        {
                            Name = "TestFormat",
                            LogFilePatterns = new List<string> { tempLogFileName },
                            Pattern = @"^(?<proc_info>\[.*?\]) - (?<message>.*)$",
                            SubPatterns = new List<SubPatternConfig>
                            {
                                new SubPatternConfig { SourceField = "proc_info", Pattern = @"\[(?<process_name>\w+\.exe)\]" }
                            }
                        }
                    }
                };

                _mockLogFormatConfigLoader.Setup(l => l.Load(It.IsAny<string>())).Returns(config1);
                _mockFileResolver.Setup(r => r.Resolve(It.IsAny<List<string>>())).Returns(new List<string> { tempLogFileName });
                var realLogFileReader = new LogFileReader();

                _viewModel = new MainViewModel(realLogFileReader, _mockUserDialogService.Object, _mockSearchWindowService.Object, _mockLogFormatConfigLoader.Object, _mockFileResolver.Object, _mockConfigPathResolver.Object);

                // Act (1): 初期読み込み
                _viewModel.Initialize("dummy_path");

                // Assert (1): 初期状態で正しくパースされていることを確認
                var entry1 = _viewModel.LogEntriesView.Cast<LogEntry>().FirstOrDefault();
                Assert.IsNotNull(entry1, "Entry should not be null after initial load.");
                Assert.IsTrue(entry1.AdditionalData.ContainsKey("process_name"), "The 'process_name' field should be extracted initially.");
                Assert.AreEqual("MyCoolApp.exe", entry1.AdditionalData["process_name"]);

                // Arrange (2)
                // 2. 新しい設定 (config2): サブパターンを2つに増やす。2つ目は1つ目の結果に依存する。
                var config2 = new AppConfig
                {
                    LogFormats = new List<LogFormatConfig>
                    {
                        new LogFormatConfig
                        {
                            Name = "TestFormat",
                            LogFilePatterns = new List<string> { tempLogFileName },
                            Pattern = @"^(?<proc_info>\[.*?\]) - (?<message>.*)$",
                            SubPatterns = new List<SubPatternConfig>
                            {
                                new SubPatternConfig { SourceField = "proc_info", Pattern = @"\[(?<process_name>\w+\.exe)\]" },
                                new SubPatternConfig { SourceField = "process_name", Pattern = @"(?<app_name>\w+)\.exe" } // process_nameをソースにする
                            }
                        }
                    }
                };

                _mockLogFormatConfigLoader.Setup(l => l.Load(It.IsAny<string>())).Returns(config2);

                // Act (2): 更新を実行
                _viewModel.RefreshCommand.Execute(null);

                // Assert (2): 更新後、新しいサブパターンも適用されていることを確認
                var entry2 = _viewModel.LogEntriesView.Cast<LogEntry>().FirstOrDefault();
                Assert.IsNotNull(entry2, "Entry should not be null after refresh.");
                Assert.IsTrue(entry2.AdditionalData.ContainsKey("process_name"), "The 'process_name' field should still exist after refresh.");
                Assert.IsTrue(entry2.AdditionalData.ContainsKey("app_name"), "The 'app_name' field should be extracted after refresh.");
                Assert.AreEqual("MyCoolApp", entry2.AdditionalData["app_name"]);
            }
            finally
            {
                if (File.Exists(tempLogFileName))
                {
                    File.Delete(tempLogFileName);
                }
            }
        }

        // --- Search Function Tests ---

        [TestMethod]
        public void OpenSearchCommand_ShowsSearchWindow()
        {
            // Arrange
            _viewModel = CreateViewModel();

            // Act
            _viewModel.OpenSearchCommand.Execute(null);

            // Assert
            _mockSearchWindowService.Verify(s => s.Show(It.IsAny<SearchViewModel>()), Times.Once);
        }

        [TestMethod]
        public void FindNext_WhenFound_SelectsEntry()
        {
            // Arrange
            _viewModel = CreateViewModel();
            var logs = new List<LogEntry>
            {
                new LogEntry { Message = "Alpha" },
                new LogEntry { Message = "Beta" },
                new LogEntry { Message = "Gamma" }
            };
            SetLogsToViewModel(_viewModel, logs);

            // Capture the SearchViewModel passed to Show
            SearchViewModel? capturedSearchVM = null;
            _mockSearchWindowService.Setup(s => s.Show(It.IsAny<SearchViewModel>()))
                .Callback<object>(vm => capturedSearchVM = vm as SearchViewModel);

            _viewModel.OpenSearchCommand.Execute(null);
            Assert.IsNotNull(capturedSearchVM);

            capturedSearchVM.SearchText = "Beta";

            // Act
            capturedSearchVM.FindNextCommand.Execute(null);

            // Assert
            Assert.IsNotNull(_viewModel.SelectedLogEntry);
            Assert.AreEqual("Beta", _viewModel.SelectedLogEntry.Message);
        }

        [TestMethod]
        public void FindNext_MatchesAdditionalData()
        {
            // Arrange
            _viewModel = CreateViewModel();
            var logs = new List<LogEntry>
            {
                new LogEntry { Message = "Msg1", AdditionalData = new Dictionary<string, string> { { "Level", "INFO" } } },
                new LogEntry { Message = "Msg2", AdditionalData = new Dictionary<string, string> { { "Level", "ERROR" } } },
                new LogEntry { Message = "Msg3", AdditionalData = new Dictionary<string, string> { { "Level", "INFO" } } }
            };
            SetLogsToViewModel(_viewModel, logs);

            SearchViewModel capturedSearchVM = GetSearchViewModel(_viewModel);
            capturedSearchVM.SearchText = "ERROR";

            // Act
            capturedSearchVM.FindNextCommand.Execute(null);

            // Assert
            Assert.IsNotNull(_viewModel.SelectedLogEntry);
            Assert.AreEqual("Msg2", _viewModel.SelectedLogEntry.Message);
        }

        [TestMethod]
        public void FindNext_MatchesFileName()
        {
            // Arrange
            _viewModel = CreateViewModel();
            var logs = new List<LogEntry>
            {
                new LogEntry { Message = "Msg1", FileName = "App.log" },
                new LogEntry { Message = "Msg2", FileName = "Error.log" },
            };
            SetLogsToViewModel(_viewModel, logs);

            SearchViewModel capturedSearchVM = GetSearchViewModel(_viewModel);
            capturedSearchVM.SearchText = "Error.log";

            // Act
            capturedSearchVM.FindNextCommand.Execute(null);

            // Assert
            Assert.IsNotNull(_viewModel.SelectedLogEntry);
            Assert.AreEqual("Msg2", _viewModel.SelectedLogEntry.Message);
        }

        [TestMethod]
        public void FindNext_CaseSensitivity_Respected()
        {
            // Arrange
            _viewModel = CreateViewModel();
            var logs = new List<LogEntry>
            {
                new LogEntry { Message = "apple" },
                new LogEntry { Message = "Apple" },
            };
            SetLogsToViewModel(_viewModel, logs);

            SearchViewModel capturedSearchVM = GetSearchViewModel(_viewModel);
            capturedSearchVM.SearchText = "Apple";
            capturedSearchVM.IsCaseSensitive = true;

            // Act
            capturedSearchVM.FindNextCommand.Execute(null);

            // Assert
            Assert.IsNotNull(_viewModel.SelectedLogEntry);
            Assert.AreEqual("Apple", _viewModel.SelectedLogEntry.Message, "Should skip 'apple' and find 'Apple' when case sensitive.");
        }

        [TestMethod]
        public void FindNext_InvalidRegex_DoesNotCrash()
        {
            // Arrange
            _viewModel = CreateViewModel();
            var logs = new List<LogEntry> { new LogEntry { Message = "Test" } };
            SetLogsToViewModel(_viewModel, logs);

            SearchViewModel capturedSearchVM = GetSearchViewModel(_viewModel);
            capturedSearchVM.SearchText = "[Invalid Regex"; // Missing closing bracket
            capturedSearchVM.IsRegex = true;

            // Act
            try
            {
                capturedSearchVM.FindNextCommand.Execute(null);
            }
            catch (System.Exception ex)
            {
                Assert.Fail($"Should not throw exception on invalid regex: {ex.Message}");
            }

            // Assert
            // Should just not match anything, keeping selection null or same
            // (In this test setup, expected behavior is silent failure/no move)
        }

        [TestMethod]
        public void FindNext_UpdatesStatusText()
        {
            // Arrange
            _viewModel = CreateViewModel();
            var logs = new List<LogEntry>
            {
                new LogEntry { Message = "Target" },
                new LogEntry { Message = "Target" },
                new LogEntry { Message = "Target" }
            };
            SetLogsToViewModel(_viewModel, logs);

            SearchViewModel capturedSearchVM = GetSearchViewModel(_viewModel);
            capturedSearchVM.SearchText = "Target";

            // Act
            // Trigger search via property change (simulated by setting text above)
            // But MainViewModel subscribes to PropertyChanged which is triggered by setter.
            // Wait, GetSearchViewModel uses OpenSearchCommand, which creates the VM and subscribes.
            // So setting SearchText above should trigger UpdateSearchStatus.

            // Assert
            // Initially, since no item is selected, it might say "?/3" or "0/3" depending on implementation.
            // In my impl: if currentFound is false, it shows "?/3".
            Assert.AreEqual("?/3", capturedSearchVM.StatusText);

            // Act 2: Find Next (Selects first one)
            capturedSearchVM.FindNextCommand.Execute(null);

            // Assert 2: First item selected, so "1/3"
            Assert.AreEqual("1/3", capturedSearchVM.StatusText);

            // Act 3: Find Next again (Selects second one)
            capturedSearchVM.FindNextCommand.Execute(null);
            Assert.AreEqual("2/3", capturedSearchVM.StatusText);
        }

        [TestMethod]
        public void FindNext_WrapsAround()
        {
            // Arrange
            _viewModel = CreateViewModel();
            var logs = new List<LogEntry>
            {
                new LogEntry { Message = "Target" },
                new LogEntry { Message = "Other" },
                new LogEntry { Message = "Target" }
            };
            SetLogsToViewModel(_viewModel, logs);

            SearchViewModel capturedSearchVM = GetSearchViewModel(_viewModel);
            capturedSearchVM.SearchText = "Target";

            // Select last item
            _viewModel.SelectedLogEntry = logs[2];

            // Act
            capturedSearchVM.FindNextCommand.Execute(null);

            // Assert
            Assert.IsNotNull(_viewModel.SelectedLogEntry);
            Assert.AreEqual(logs[0], _viewModel.SelectedLogEntry, "Should wrap around to the first item.");
        }

        [TestMethod]
        public void FindPrevious_WhenFound_SelectsEntry()
        {
            // Arrange
            _viewModel = CreateViewModel();
            var logs = new List<LogEntry>
            {
                new LogEntry { Message = "Target" }, // 0
                new LogEntry { Message = "Other" },  // 1
                new LogEntry { Message = "Target" }  // 2
            };
            SetLogsToViewModel(_viewModel, logs);

            SearchViewModel capturedSearchVM = GetSearchViewModel(_viewModel);
            capturedSearchVM.SearchText = "Target";

            // Select middle item
            _viewModel.SelectedLogEntry = logs[1];

            // Act
            capturedSearchVM.FindPreviousCommand.Execute(null);

            // Assert
            Assert.AreEqual(logs[0], _viewModel.SelectedLogEntry, "Should find the previous target.");
        }

        private void SetLogsToViewModel(MainViewModel vm, List<LogEntry> logs)
        {
            // Mock LogFileReader to return these logs
            var config = new LogFormatConfig { Name = "Test", LogFilePatterns = new List<string> { "dummy" } };
            var appConfig = new AppConfig { LogFormats = new List<LogFormatConfig> { config } };

            _mockLogFormatConfigLoader.Setup(l => l.Load(It.IsAny<string>())).Returns(appConfig);
            _mockFileResolver.Setup(r => r.Resolve(It.IsAny<List<string>>())).Returns(new List<string> { "dummy" });
            _mockLogFileReader.Setup(r => r.ReadFiles(It.IsAny<List<string>>(), It.IsAny<LogFormatConfig>())).Returns(logs);

            vm.Initialize("dummy");
        }

        private SearchViewModel GetSearchViewModel(MainViewModel vm)
        {
            SearchViewModel? captured = null;
            _mockSearchWindowService.Setup(s => s.Show(It.IsAny<SearchViewModel>()))
                .Callback<object>(obj => captured = obj as SearchViewModel);

            vm.OpenSearchCommand.Execute(null);
            return captured!;
        }
    }
}
