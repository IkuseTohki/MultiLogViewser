using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MultiLogViewer.Models;
using MultiLogViewer.Services;
using MultiLogViewer.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Collections.ObjectModel;

namespace MultiLogViewer.Tests
{
    [TestClass]
    public class MainViewModelTests
    {
        private Mock<ILogFileReader> _mockLogFileReader;
        private Mock<IUserDialogService> _mockUserDialogService;

        [TestInitialize]
        public void Setup()
        {
            _mockLogFileReader = new Mock<ILogFileReader>();
            _mockUserDialogService = new Mock<IUserDialogService>();
        }

        /// <summary>
        /// テスト観点: SortByTimestampCommandを実行すると、LogEntriesがTimestampで昇順にソートされることを確認する。
        /// </summary>
        [TestMethod]
        public void SortByTimestampCommand_SortsLogEntriesAscending()
        {
            // Arrange
            var viewModel = new MainViewModel(_mockLogFileReader.Object, _mockUserDialogService.Object);
            viewModel.LogEntries.Add(new LogEntry { Timestamp = new DateTime(2023, 1, 1, 12, 0, 0), Level = "INFO", Message = "Log 2" });
            viewModel.LogEntries.Add(new LogEntry { Timestamp = new DateTime(2023, 1, 1, 11, 0, 0), Level = "INFO", Message = "Log 1" });
            viewModel.LogEntries.Add(new LogEntry { Timestamp = new DateTime(2023, 1, 1, 13, 0, 0), Level = "INFO", Message = "Log 3" });

            // Act
            viewModel.SortByTimestampCommand.Execute(null);

            // Assert
            Assert.AreEqual("Log 1", viewModel.LogEntries[0].Message);
            Assert.AreEqual("Log 2", viewModel.LogEntries[1].Message);
            Assert.AreEqual("Log 3", viewModel.LogEntries[2].Message);
        }

        /// <summary>
        /// テスト観点: SortByTimestampCommandを複数回実行すると、ソート方向が昇順/降順に切り替わることを確認する。
        /// </summary>
        [TestMethod]
        public void SortByTimestampCommand_TogglesSortDirection()
        {
            // Arrange
            var viewModel = new MainViewModel(_mockLogFileReader.Object, _mockUserDialogService.Object);
            viewModel.LogEntries.Add(new LogEntry { Timestamp = new DateTime(2023, 1, 1, 12, 0, 0), Level = "INFO", Message = "Log 2" });
            viewModel.LogEntries.Add(new LogEntry { Timestamp = new DateTime(2023, 1, 1, 11, 0, 0), Level = "INFO", Message = "Log 1" });
            viewModel.LogEntries.Add(new LogEntry { Timestamp = new DateTime(2023, 1, 1, 13, 0, 0), Level = "INFO", Message = "Log 3" });

            // Act - 1回目 (昇順)
            viewModel.SortByTimestampCommand.Execute(null);

            // Assert - 昇順
            Assert.AreEqual("Log 1", viewModel.LogEntries[0].Message);
            Assert.AreEqual("Log 2", viewModel.LogEntries[1].Message);
            Assert.AreEqual("Log 3", viewModel.LogEntries[2].Message);

            // Act - 2回目 (降順)
            viewModel.SortByTimestampCommand.Execute(null);

            // Assert - 降順
            Assert.AreEqual("Log 3", viewModel.LogEntries[0].Message);
            Assert.AreEqual("Log 2", viewModel.LogEntries[1].Message);
            Assert.AreEqual("Log 1", viewModel.LogEntries[2].Message);
        }
    }
}
