using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.Models;
using System;

namespace MultiLogViewer.Tests.Models
{
    [TestClass]
    public class LogFilterTests
    {
        [TestMethod]
        public void Equals_SameColumnFilter_ReturnsTrue()
        {
            // テスト観点: 同じキーのカラムフィルターは同一とみなされること
            var f1 = new LogFilter(FilterType.ColumnEmpty, "User", default, "User");
            var f2 = new LogFilter(FilterType.ColumnEmpty, "User", default, "User Different Display");

            Assert.AreEqual(f1, f2);
        }

        [TestMethod]
        public void Equals_DifferentType_ReturnsFalse()
        {
            // テスト観点: タイプが異なれば別物とみなされること
            var f1 = new LogFilter(FilterType.ColumnEmpty, "User", default, "User");
            var f2 = new LogFilter(FilterType.DateTimeAfter, "User", default, "User");

            Assert.AreNotEqual(f1, f2);
        }

        [TestMethod]
        public void Equals_DateTimeFilter_SameTypeIsEqual()
        {
            // テスト観点: 日時フィルターは、タイプが同じなら同一（上書き対象）とみなされること
            var f1 = new LogFilter(FilterType.DateTimeAfter, "", new DateTime(2025, 1, 1), "A");
            var f2 = new LogFilter(FilterType.DateTimeAfter, "", new DateTime(2025, 1, 2), "B");

            Assert.AreEqual(f1, f2);
        }
    }
}
