using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MultiLogViewer.Models;
using MultiLogViewer.Services;
using System.Collections.Generic;
using System.Linq;

namespace MultiLogViewer.Tests
{
    [TestClass]
    public class LogServiceTests
    {
        private Mock<ILogFormatConfigLoader> _mockConfigLoader = null!;
        private Mock<IFileResolver> _mockFileResolver = null!;
        private Mock<ILogFileReader> _mockLogFileReader = null!;
        private Mock<IConfigPathResolver> _mockConfigPathResolver = null!;
        private Mock<ITimeProvider> _mockTimeProvider = null!;
        private LogService _logService = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockConfigLoader = new Mock<ILogFormatConfigLoader>();
            _mockFileResolver = new Mock<IFileResolver>();
            _mockLogFileReader = new Mock<ILogFileReader>();
            _mockConfigPathResolver = new Mock<IConfigPathResolver>();
            _mockTimeProvider = new Mock<ITimeProvider>();

            // デフォルトの設定: 2023-12-27 を当日とする
            _mockTimeProvider.Setup(t => t.Today).Returns(new System.DateTime(2023, 12, 27));

            _mockConfigPathResolver.Setup(r => r.GetAppSettingsPath()).Returns("AppSettings.yaml");

            _logService = new LogService(
                _mockConfigLoader.Object,
                _mockFileResolver.Object,
                _mockLogFileReader.Object,
                _mockConfigPathResolver.Object,
                _mockTimeProvider.Object);
        }

        [TestMethod]
        public void LoadFromConfig_Success_ReturnsSortedEntriesAndColumns()
        {
            // Arrange
            var configPath = "LogProfile.yaml";
            var appConfig = new AppConfig
            {
                LogRetentionLimit = "2000-01-01",
                DisplayColumns = new List<DisplayColumnConfig> { new DisplayColumnConfig { Header = "Col1" } },
                LogFormats = new List<LogFormatConfig>
                {
                    new LogFormatConfig { Name = "F1", LogFilePatterns = new List<string> { "*.log" } }
                }
            };

            var logs = new List<LogEntry>
            {
                new LogEntry { Timestamp = new System.DateTime(2023, 1, 2), Message = "Later" },
                new LogEntry { Timestamp = new System.DateTime(2023, 1, 1), Message = "Earlier" }
            };

            _mockConfigLoader.Setup(l => l.Load(configPath, It.IsAny<string>())).Returns(appConfig);
            _mockFileResolver.Setup(r => r.Resolve(It.IsAny<List<string>>())).Returns(new List<string> { "file.log" });
            _mockLogFileReader.Setup(r => r.ReadIncremental(It.IsAny<FileState>(), It.IsAny<IEnumerable<LogFormatConfig>>()))
                .Returns((logs, new FileState("file.log", 100, 2)));

            // Act
            var result = _logService.LoadFromConfig(configPath);

            // Assert
            Assert.AreEqual(2, result.Entries.Count);
            Assert.AreEqual("Earlier", result.Entries[0].Message);
        }

        [TestMethod]
        public void LoadFromConfig_SameTimestamp_PreservesOriginalOrder()
        {
            var configPath = "LogProfile.yaml";
            var appConfig = new AppConfig
            {
                LogRetentionLimit = "2000-01-01",
                LogFormats = new List<LogFormatConfig>
                {
                    new LogFormatConfig { Name = "F1", LogFilePatterns = new List<string> { "*.log" } }
                }
            };

            var sameTime = new System.DateTime(2023, 1, 1, 10, 0, 0);
            var logs = new List<LogEntry>
            {
                new LogEntry { Timestamp = sameTime, Message = "First" },
                new LogEntry { Timestamp = sameTime, Message = "Second" },
                new LogEntry { Timestamp = sameTime, Message = "Third" }
            };

            _mockConfigLoader.Setup(l => l.Load(configPath, It.IsAny<string>())).Returns(appConfig);
            _mockFileResolver.Setup(r => r.Resolve(It.IsAny<List<string>>())).Returns(new List<string> { "file.log" });
            _mockLogFileReader.Setup(r => r.ReadIncremental(It.IsAny<FileState>(), It.IsAny<IEnumerable<LogFormatConfig>>()))
                .Returns((logs, new FileState("file.log", 300, 3)));

            // Act
            var result = _logService.LoadFromConfig(configPath);

            // Assert
            Assert.AreEqual(3, result.Entries.Count);
            Assert.AreEqual("First", result.Entries[0].Message);
        }

        [TestMethod]
        public void LoadFromConfig_MultipleFiles_SameTimestamp_PreservesFileOrder()
        {
            var configPath = "LogProfile.yaml";
            var sameTime = new System.DateTime(2023, 1, 1, 10, 0, 0);

            var appConfig = new AppConfig
            {
                LogRetentionLimit = "2000-01-01",
                LogFormats = new List<LogFormatConfig>
                {
                    new LogFormatConfig { Name = "F1", LogFilePatterns = new List<string> { "fileA.log", "fileB.log" } }
                }
            };

            _mockConfigLoader.Setup(l => l.Load(configPath, It.IsAny<string>())).Returns(appConfig);
            _mockFileResolver.Setup(r => r.Resolve(It.IsAny<List<string>>())).Returns(new List<string> { "fileA.log", "fileB.log" });

            var logsA = new List<LogEntry> { new LogEntry { Timestamp = sameTime, Message = "A1" }, new LogEntry { Timestamp = sameTime, Message = "A2" } };
            _mockLogFileReader.Setup(r => r.ReadIncremental(It.Is<FileState>(s => s.FilePath == "fileA.log"), It.IsAny<IEnumerable<LogFormatConfig>>()))
                .Returns((logsA, new FileState("fileA.log", 100, 2)));

            var logsB = new List<LogEntry> { new LogEntry { Timestamp = sameTime, Message = "B1" }, new LogEntry { Timestamp = sameTime, Message = "B2" } };
            _mockLogFileReader.Setup(r => r.ReadIncremental(It.Is<FileState>(s => s.FilePath == "fileB.log"), It.IsAny<IEnumerable<LogFormatConfig>>()))
                .Returns((logsB, new FileState("fileB.log", 100, 2)));

            // Act
            var result = _logService.LoadFromConfig(configPath);

            // Assert
            Assert.AreEqual(4, result.Entries.Count);
            Assert.AreEqual("A1", result.Entries[0].Message);
        }

        [TestMethod]
        public void LoadFromConfig_FiltersOldLogs_BasedOnRetentionLimit()
        {
            // テスト観点: 設定された読み込み制限日時より古いログが除外されることを確認する。
            var configPath = "LogProfile.yaml";

            // 当日が 12/27 の時、-1d は 12/26 00:00:00 以降になる。
            var limit = new System.DateTime(2023, 12, 26, 0, 0, 0);

            var appConfig = new AppConfig
            {
                LogRetentionLimit = "-1d",
                LogFormats = new List<LogFormatConfig>
                {
                    new LogFormatConfig { Name = "F1", LogFilePatterns = new List<string> { "file.log" } }
                }
            };

            var logs = new List<LogEntry>
            {
                new LogEntry { Timestamp = new System.DateTime(2023, 12, 25, 23, 59, 59), Message = "Old" },
                new LogEntry { Timestamp = new System.DateTime(2023, 12, 26, 0, 0, 0), Message = "Exact" },
                new LogEntry { Timestamp = new System.DateTime(2023, 12, 27, 0, 0, 0), Message = "New" }
            };

            _mockConfigLoader.Setup(l => l.Load(configPath, It.IsAny<string>())).Returns(appConfig);
            _mockFileResolver.Setup(r => r.Resolve(It.IsAny<List<string>>())).Returns(new List<string> { "file.log" });
            _mockLogFileReader.Setup(r => r.ReadIncremental(It.IsAny<FileState>(), It.IsAny<IEnumerable<LogFormatConfig>>()))
                .Returns((logs, new FileState("file.log", 100, 3)));

            // Act
            var result = _logService.LoadFromConfig(configPath);

            // Assert
            Assert.AreEqual(2, result.Entries.Count, "Old logs should be excluded.");
            Assert.IsTrue(result.Entries.All(e => e.Timestamp >= limit));
            Assert.AreEqual("Exact", result.Entries[0].Message);
            Assert.AreEqual("New", result.Entries[1].Message);
        }

        [TestMethod]
        public void LoadIncremental_ContinuesSequenceNumber()
        {
            var configPath = "LogProfile.yaml";
            var sameTime = new System.DateTime(2023, 1, 1, 10, 0, 0);
            var appConfig = new AppConfig
            {
                LogRetentionLimit = "2000-01-01",
                LogFormats = new List<LogFormatConfig>
                {
                    new LogFormatConfig { Name = "F1", LogFilePatterns = new List<string> { "file.log" } }
                }
            };

            _mockConfigLoader.Setup(l => l.Load(configPath, It.IsAny<string>())).Returns(appConfig);
            _mockFileResolver.Setup(r => r.Resolve(It.IsAny<List<string>>())).Returns(new List<string> { "file.log" });

            var nextLogs = new List<LogEntry>
            {
                new LogEntry { Timestamp = sameTime, Message = "New 1" },
                new LogEntry { Timestamp = sameTime, Message = "New 2" }
            };

            _mockLogFileReader.Setup(r => r.ReadIncremental(It.IsAny<FileState>(), It.IsAny<IEnumerable<LogFormatConfig>>()))
                .Returns((nextLogs, new FileState("file.log", 200, 12)));

            var result = _logService.LoadIncremental(configPath, new List<FileState>(), 10);

            Assert.AreEqual(2, result.Entries.Count);
            Assert.AreEqual(10, result.Entries[0].SequenceNumber);
        }
    }
}
