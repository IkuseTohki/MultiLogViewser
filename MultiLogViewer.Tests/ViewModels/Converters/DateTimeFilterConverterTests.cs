using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.Models;
using MultiLogViewer.ViewModels.Converters;
using System;

namespace MultiLogViewer.Tests.ViewModels.Converters
{
    [TestClass]
    public class DateTimeFilterConverterTests
    {
        private DateTimeFilterConverter _converter = null!;

        [TestInitialize]
        public void Setup()
        {
            _converter = new DateTimeFilterConverter();
        }

        [TestMethod]
        public void Convert_LogEntryAndBool_ReturnsValueTuple()
        {
            // テスト観点: LogEntry と bool(IsAfter) を渡した際、(DateTime, bool) のタプルが返されること。
            var time = new DateTime(2023, 1, 1);
            var entry = new LogEntry { Timestamp = time };

            // 以降フィルタの場合 (parameter = true)
            var resultAfter = _converter.Convert(entry, typeof(object), true, System.Globalization.CultureInfo.InvariantCulture);
            Assert.IsInstanceOfType(resultAfter, typeof(ValueTuple<DateTime, bool>));
            var tupleAfter = (ValueTuple<DateTime, bool>)resultAfter;
            Assert.AreEqual(time, tupleAfter.Item1);
            Assert.AreEqual(true, tupleAfter.Item2);

            // 以前フィルタの場合 (parameter = false)
            var resultBefore = _converter.Convert(entry, typeof(object), false, System.Globalization.CultureInfo.InvariantCulture);
            var tupleBefore = (ValueTuple<DateTime, bool>)resultBefore;
            Assert.AreEqual(false, tupleBefore.Item2);
        }

        [TestMethod]
        public void Convert_InvalidInput_ReturnsNull()
        {
            // テスト観点: 不正な入力に対してクラッシュせず null を返すこと。
            var result = _converter.Convert("not an entry", typeof(object), true, System.Globalization.CultureInfo.InvariantCulture);
            Assert.IsNull(result);
        }
    }
}
