using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.ViewModels.Converters;

namespace MultiLogViewer.Tests.ViewModels.Converters
{
    [TestClass]
    public class BooleanInvertConverterTests
    {
        [TestMethod]
        public void Convert_InvertsBooleanValue()
        {
            var converter = new BooleanInvertConverter();

            Assert.AreEqual(false, converter.Convert(true, typeof(bool), null!, System.Globalization.CultureInfo.InvariantCulture));
            Assert.AreEqual(true, converter.Convert(false, typeof(bool), null!, System.Globalization.CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void ConvertBack_InvertsBooleanValue()
        {
            var converter = new BooleanInvertConverter();

            Assert.AreEqual(false, converter.ConvertBack(true, typeof(bool), null!, System.Globalization.CultureInfo.InvariantCulture));
            Assert.AreEqual(true, converter.ConvertBack(false, typeof(bool), null!, System.Globalization.CultureInfo.InvariantCulture));
        }

        [TestMethod]
        public void Convert_ReturnsOriginal_WhenNotBoolean()
        {
            var converter = new BooleanInvertConverter();
            var value = "string";

            Assert.AreEqual(value, converter.Convert(value, typeof(bool), null!, System.Globalization.CultureInfo.InvariantCulture));
        }
    }
}
