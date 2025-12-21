using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.ViewModels.Converters;
using System;
using System.Windows;

namespace MultiLogViewer.Tests.ViewModels.Converters
{
    [TestClass]
    public class DoubleToGridLengthConverterTests
    {
        private DoubleToGridLengthConverter _converter = null!;

        [TestInitialize]
        public void Setup()
        {
            _converter = new DoubleToGridLengthConverter();
        }

        [TestMethod]
        public void Convert_DoubleToPixel_ReturnsGridLength()
        {
            // テスト観点: double値が正しくPixel単位のGridLengthに変換されること
            var result = _converter.Convert(250.5, typeof(GridLength), null!, System.Globalization.CultureInfo.InvariantCulture);
            Assert.IsInstanceOfType(result, typeof(GridLength));
            var gl = (GridLength)result;
            Assert.AreEqual(250.5, gl.Value);
            Assert.IsTrue(gl.IsAbsolute);
        }

        [TestMethod]
        public void Convert_Zero_ReturnsZeroPixel()
        {
            // テスト観点: 0.0が正しく変換されること
            var result = _converter.Convert(0.0, typeof(GridLength), null!, System.Globalization.CultureInfo.InvariantCulture);
            var gl = (GridLength)result;
            Assert.AreEqual(0.0, gl.Value);
        }

        [TestMethod]
        public void Convert_NonDouble_ReturnsDefaultStar()
        {
            // テスト観点: double以外が渡された場合、デフォルト（1*）を返すこと
            var result = _converter.Convert("invalid", typeof(GridLength), null!, System.Globalization.CultureInfo.InvariantCulture);
            var gl = (GridLength)result;
            Assert.AreEqual(1.0, gl.Value);
            Assert.IsTrue(gl.IsStar);
        }

        [TestMethod]
        public void ConvertBack_PixelGridLength_ReturnsDouble()
        {
            // テスト観点: Pixel単位のGridLengthが正しくdoubleに戻されること
            var gl = new GridLength(300, GridUnitType.Pixel);
            var result = _converter.ConvertBack(gl, typeof(double), null!, System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual(300.0, result);
        }

        [TestMethod]
        public void ConvertBack_StarGridLength_ReturnsDouble()
        {
            // テスト観点: Star単位のGridLengthでも、その数値部分が返されること
            var gl = new GridLength(2, GridUnitType.Star);
            var result = _converter.ConvertBack(gl, typeof(double), null!, System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual(2.0, result);
        }

        [TestMethod]
        public void ConvertBack_NonGridLength_ReturnsZero()
        {
            // テスト観点: GridLength以外が渡された場合、0.0を返すこと
            var result = _converter.ConvertBack(200.0, typeof(double), null!, System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual(0.0, result);
        }
    }
}
