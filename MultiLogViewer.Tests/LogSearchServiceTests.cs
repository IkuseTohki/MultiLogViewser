using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.Models;
using MultiLogViewer.Services;
using System.Collections.Generic;
using System.Linq;

namespace MultiLogViewer.Tests
{
    [TestClass]
    public class LogSearchServiceTests
    {
        private ILogSearchService _searchService = null!;

        [TestInitialize]
        public void Setup()
        {
            _searchService = new LogSearchService();
        }

        [TestMethod]
        public void IsMatch_SimpleText_MatchesCorrect()
        {
            var entry = new LogEntry { Message = "Hello World" };
            var criteria = new SearchCriteria("Hello", false, false);

            Assert.IsTrue(_searchService.IsMatch(entry, criteria));
        }

        [TestMethod]
        public void IsMatch_CaseSensitive_Respected()
        {
            var entry = new LogEntry { Message = "Hello World" };
            var criteriaTrue = new SearchCriteria("hello", true, false);
            var criteriaFalse = new SearchCriteria("hello", false, false);

            Assert.IsFalse(_searchService.IsMatch(entry, criteriaTrue));
            Assert.IsTrue(_searchService.IsMatch(entry, criteriaFalse));
        }

        [TestMethod]
        public void Find_Forward_WrapsAround()
        {
            var logs = new List<LogEntry>
            {
                new LogEntry { Message = "Target 1" },
                new LogEntry { Message = "Other" },
                new LogEntry { Message = "Target 2" }
            };
            var criteria = new SearchCriteria("Target", false, false);

            // 最後の項目を選択している状態で次を検索
            var result = _searchService.Find(logs, logs[2], criteria, true);

            Assert.AreEqual(logs[0], result, "Should wrap around to the first match.");
        }

        [TestMethod]
        public void GetSearchStatistics_ReturnsCorrectCounts()
        {
            var logs = new List<LogEntry>
            {
                new LogEntry { Message = "Target" }, // 1
                new LogEntry { Message = "Other" },
                new LogEntry { Message = "Target" }  // 2
            };
            var criteria = new SearchCriteria("Target", false, false);

            var (matchCount, currentIndex) = _searchService.GetSearchStatistics(logs, logs[2], criteria);

            Assert.AreEqual(2, matchCount);
            Assert.AreEqual(2, currentIndex);
        }
    }
}
