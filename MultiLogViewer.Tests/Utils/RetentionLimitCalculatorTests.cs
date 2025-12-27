using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MultiLogViewer.Services;
using MultiLogViewer.Utils;
using System;

namespace MultiLogViewer.Tests.Utils
{
    [TestClass]
    public class RetentionLimitCalculatorTests
    {
        private Mock<ITimeProvider> _mockTimeProvider = null!;
        private DateTime _now;

        [TestInitialize]
        public void Setup()
        {
            _mockTimeProvider = new Mock<ITimeProvider>();
            // 基準日: 2025-12-27 (土)
            _now = new DateTime(2025, 12, 27, 10, 0, 0);
            _mockTimeProvider.Setup(t => t.Now).Returns(_now);
            _mockTimeProvider.Setup(t => t.Today).Returns(_now.Date);
        }

        [TestMethod]
        [DataRow(null, "2025-12-27 00:00:00", DisplayName = "未指定の場合は当日の0時")]
        [DataRow("", "2025-12-27 00:00:00", DisplayName = "空文字の場合は当日の0時")]
        [DataRow("today", "2025-12-27 00:00:00", DisplayName = "today")]
        [DataRow("本日", "2025-12-27 00:00:00", DisplayName = "本日")]
        [DataRow("-1d", "2025-12-26 00:00:00", DisplayName = "1日前 (-1d)")]
        [DataRow("-2日前", "2025-12-25 00:00:00", DisplayName = "2日前 (-2日前)")]
        [DataRow("-1w", "2025-12-20 00:00:00", DisplayName = "1週間前 (-1w)")]
        [DataRow("-1週間前", "2025-12-20 00:00:00", DisplayName = "1週間前 (-1週間前)")]
        [DataRow("-1m", "2025-11-27 00:00:00", DisplayName = "1ヶ月前 (-1m)")]
        [DataRow("-1ヶ月前", "2025-11-27 00:00:00", DisplayName = "1ヶ月前 (-1ヶ月前)")]
        [DataRow("2025-12-20 15:30:00", "2025-12-20 15:30:00", DisplayName = "絶対指定")]
        public void Calculate_ReturnsExpectedDateTime(string input, string expectedStr)
        {
            // Arrange
            var expected = DateTime.Parse(expectedStr);
            var calculator = new RetentionLimitCalculator(_mockTimeProvider.Object);

            // Act
            var result = calculator.Calculate(input);

            // Assert
            Assert.AreEqual(expected, result, $"Input: {input}");
        }
    }
}
