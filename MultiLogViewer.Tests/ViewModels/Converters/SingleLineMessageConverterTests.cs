using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.ViewModels.Converters;
using System;

namespace MultiLogViewer.Tests.ViewModels.Converters
{
    [TestClass]
    public class SingleLineMessageConverterTests
    {
        private SingleLineMessageConverter _converter = null!;

        [TestInitialize]
        public void Setup()
        {
            _converter = new SingleLineMessageConverter();
        }

        [TestMethod]
        public void Convert_SingleLine_ReturnsSameString()
        {
            // テスト観点: 改行のない文字列は、そのまま返されること
            var input = "Simple message";
            var result = _converter.Convert(input, typeof(string), null!, System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual("Simple message", result);
        }

        [TestMethod]
        public void Convert_CrLf_ReturnsFirstLine()
        {
            // テスト観点: \r\n で区切られた複数行文字列の、1行目のみが返されること
            var input = "First line\r\nSecond line";
            var result = _converter.Convert(input, typeof(string), null!, System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual("First line", result);
        }

        [TestMethod]
        public void Convert_LfOnly_ReturnsFirstLine()
        {
            // テスト観点: \n のみで区切られた複数行文字列の、1行目のみが返されること
            var input = "First line\nSecond line";
            var result = _converter.Convert(input, typeof(string), null!, System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual("First line", result);
        }

        [TestMethod]
        public void Convert_CrOnly_ReturnsFirstLine()
        {
            // テスト観点: \r のみで区切られた複数行文字列の、1行目のみが返されること
            var input = "First line\rSecond line";
            var result = _converter.Convert(input, typeof(string), null!, System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual("First line", result);
        }

        [TestMethod]
        public void Convert_Empty_ReturnsEmpty()
        {
            // テスト観点: 空文字列やnullの場合、空文字列が返されること
            Assert.AreEqual(string.Empty, _converter.Convert("", typeof(string), null!, System.Globalization.CultureInfo.InvariantCulture));
            Assert.AreEqual(string.Empty, _converter.Convert(null!, typeof(string), null!, System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
