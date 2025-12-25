using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.Models;
using MultiLogViewer.ViewModels.Converters;
using System.Windows.Media;

namespace MultiLogViewer.Tests.ViewModels.Converters
{
    [TestClass]
    public class BookmarkColorToBrushConverterTests
    {
        private BookmarkColorToBrushConverter _converter = null!;

        [TestInitialize]
        public void Setup()
        {
            _converter = new BookmarkColorToBrushConverter();
        }

        [TestMethod]
        public void Convert_Null_ReturnsBlackBrush()
        {
            // テスト観点: フィルター(ALL)などで null が渡された場合、黒色が返されること
            var result = _converter.Convert(null!, typeof(Brush), null!, System.Globalization.CultureInfo.InvariantCulture) as SolidColorBrush;

            Assert.IsNotNull(result);
            Assert.AreEqual(Colors.Black, result.Color);
        }

        [TestMethod]
        public void Convert_Colors_ReturnsCorrectPastelBrushes()
        {
            // テスト観点: 各列挙値に対して、仕様通りのパステルカラーが返されること
            var red = _converter.Convert(BookmarkColor.Red, typeof(Brush), null!, System.Globalization.CultureInfo.InvariantCulture) as SolidColorBrush;
            var blue = _converter.Convert(BookmarkColor.Blue, typeof(Brush), null!, System.Globalization.CultureInfo.InvariantCulture) as SolidColorBrush;
            var green = _converter.Convert(BookmarkColor.Green, typeof(Brush), null!, System.Globalization.CultureInfo.InvariantCulture) as SolidColorBrush;
            var yellow = _converter.Convert(BookmarkColor.Yellow, typeof(Brush), null!, System.Globalization.CultureInfo.InvariantCulture) as SolidColorBrush;

            Assert.AreEqual("#FFE57373", red?.Color.ToString());
            Assert.AreEqual("#FF64B5F6", blue?.Color.ToString());
            Assert.AreEqual("#FF81C784", green?.Color.ToString());
            Assert.AreEqual("#FFFFF176", yellow?.Color.ToString());
        }

        [TestMethod]
        public void Convert_InvalidType_ReturnsDefaultBlue()
        {
            // テスト観点: 想定外の型が渡された場合、デフォルト色（Blue）が返されること
            var result = _converter.Convert("invalid", typeof(Brush), null!, System.Globalization.CultureInfo.InvariantCulture) as SolidColorBrush;

            Assert.IsNotNull(result);
            Assert.AreEqual("#FF64B5F6", result.Color.ToString());
        }
    }
}
