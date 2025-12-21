using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.ViewModels.Converters;
using System;
using System.Windows;

namespace MultiLogViewer.Tests.ViewModels.Converters
{
    [TestClass]
    public class MultilineToVisibilityConverterTests
    {
        private MultilineToVisibilityConverter _converter = null!;

        [TestInitialize]
        public void Setup()
        {
            _converter = new MultilineToVisibilityConverter();
        }

        [TestMethod]
        public void Convert_MultipleLines_ReturnsVisible()
        {
            // テスト観点: 各種改行コードを含む場合、Visibleを返すこと
            Assert.AreEqual(Visibility.Visible, _converter.Convert("Line1\nLine2", typeof(Visibility), null!, System.Globalization.CultureInfo.InvariantCulture));
            Assert.AreEqual(Visibility.Visible, _converter.Convert("Line1\r\nLine2", typeof(Visibility), null!, System.Globalization.CultureInfo.InvariantCulture));
            Assert.AreEqual(Visibility.Visible, _converter.Convert("Line1\rLine2", typeof(Visibility), null!, System.Globalization.CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void Convert_SingleLine_ReturnsCollapsed()
        {
            // テスト観点: 改行を含まない場合、Collapsedを返すこと
            Assert.AreEqual(Visibility.Collapsed, _converter.Convert("Single line message", typeof(Visibility), null!, System.Globalization.CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void Convert_EmptyOrNull_ReturnsCollapsed()
        {
            // テスト観点: 空文字やnullの場合、Collapsedを返すこと
            Assert.AreEqual(Visibility.Collapsed, _converter.Convert("", typeof(Visibility), null!, System.Globalization.CultureInfo.InvariantCulture));
            Assert.AreEqual(Visibility.Collapsed, _converter.Convert(null!, typeof(Visibility), null!, System.Globalization.CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void Convert_NonStringInput_ReturnsCollapsed()
        {
            // テスト観点: 文字列以外が入力された場合、Collapsedを返すこと
            Assert.AreEqual(Visibility.Collapsed, _converter.Convert(123, typeof(Visibility), null!, System.Globalization.CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void Convert_EndsWithNewLine_ReturnsVisible()
        {
            // テスト観点: 文字列の末尾に改行がある場合、複数行として扱いVisibleを返すこと
            // (実質的に2行目が空文字として存在するため)
            Assert.AreEqual(Visibility.Visible, _converter.Convert("Line1\n", typeof(Visibility), null!, System.Globalization.CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void Convert_StartsWithNewLine_ReturnsVisible()
        {
            // テスト観点: 文字列の先頭に改行がある場合、複数行として扱いVisibleを返すこと
            Assert.AreEqual(Visibility.Visible, _converter.Convert("\nLine2", typeof(Visibility), null!, System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
