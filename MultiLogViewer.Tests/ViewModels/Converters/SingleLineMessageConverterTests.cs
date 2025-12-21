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

        [TestMethod]
        public void Convert_NonStringInput_ReturnsEmpty()
        {
            // テスト観点: string型以外のオブジェクトが渡された場合、空文字列を返すこと（例外を投げない）
            Assert.AreEqual(string.Empty, _converter.Convert(12345, typeof(string), null!, System.Globalization.CultureInfo.InvariantCulture));
            Assert.AreEqual(string.Empty, _converter.Convert(DateTime.Now, typeof(string), null!, System.Globalization.CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void Convert_StartsByNewLine_ReturnsEmpty()
        {
            // テスト観点: 文字列の先頭が改行コードの場合、空文字列を返すこと
            Assert.AreEqual(string.Empty, _converter.Convert("\nSecond line", typeof(string), null!, System.Globalization.CultureInfo.InvariantCulture));
            Assert.AreEqual(string.Empty, _converter.Convert("\r\nSecond line", typeof(string), null!, System.Globalization.CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void Convert_NewLineOnly_ReturnsEmpty()
        {
            // テスト観点: 改行コードのみの入力の場合、空文字列を返すこと
            Assert.AreEqual(string.Empty, _converter.Convert("\n", typeof(string), null!, System.Globalization.CultureInfo.InvariantCulture));
            Assert.AreEqual(string.Empty, _converter.Convert("\r\n", typeof(string), null!, System.Globalization.CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void Convert_WhitespaceOnly_ReturnsInput()
        {
            // テスト観点: スペースやタブのみの場合、改行がなければそのまま返すこと
            var input = "   \t   ";
            Assert.AreEqual(input, _converter.Convert(input, typeof(string), null!, System.Globalization.CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void Convert_ConsecutiveNewLines_ReturnsFirstLine()
        {
            // テスト観点: 連続する改行がある場合、最初の改行までを返すこと
            var input = "First\n\nThird";
            Assert.AreEqual("First", _converter.Convert(input, typeof(string), null!, System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
