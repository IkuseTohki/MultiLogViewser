using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.Utils;
using System.Windows.Media;

namespace MultiLogViewer.Tests.Utils
{
    [TestClass]
    public class ColorGeneratorTests
    {
        [TestMethod]
        public void GenerateFromString_SameInput_ReturnsSameBrush()
        {
            // テスト観点: 同じ文字列からは一貫して同じ色が生成されること
            var input = "TestUser";
            var brush1 = ColorGenerator.GenerateFromString(input);
            var brush2 = ColorGenerator.GenerateFromString(input);

            Assert.AreEqual(brush1.Color, brush2.Color);
        }

        [TestMethod]
        public void GenerateFromString_DifferentInput_ReturnsDifferentBrush()
        {
            // テスト観点: 異なる文字列からは異なる色が生成されること（ハッシュ衝突の可能性はゼロではないが、一般的に異なるはず）
            var brush1 = ColorGenerator.GenerateFromString("UserA");
            var brush2 = ColorGenerator.GenerateFromString("UserB");

            Assert.AreNotEqual(brush1.Color, brush2.Color);
        }

        [TestMethod]
        public void GenerateFromString_EmptyOrNull_ReturnsTransparent()
        {
            // テスト観点: 空文字やnullの場合、透明色を返すこと
            Assert.AreEqual(Colors.Transparent, ColorGenerator.GenerateFromString("").Color);
            Assert.AreEqual(Colors.Transparent, ColorGenerator.GenerateFromString(null!).Color);
        }

        [TestMethod]
        public void GetForegroundForBackground_LightBackground_ReturnsBlack()
        {
            // テスト観点: 明るい背景（白）に対して黒文字を返すこと
            var whiteBrush = new SolidColorBrush(Colors.White);
            var fg = ColorGenerator.GetForegroundForBackground(whiteBrush);
            Assert.AreEqual(Colors.Black, fg.Color);
        }

        [TestMethod]
        public void GetForegroundForBackground_DarkBackground_ReturnsWhite()
        {
            // テスト観点: 暗い背景（黒）に対して白文字を返すこと
            var blackBrush = new SolidColorBrush(Colors.Black);
            var fg = ColorGenerator.GetForegroundForBackground(blackBrush);
            Assert.AreEqual(Colors.White, fg.Color);
        }

        [TestMethod]
        public void GetForegroundForBackground_NullOrTransparent_ReturnsBlack()
        {
            // テスト観点: 背景がない場合はデフォルトで黒を返すこと
            Assert.AreEqual(Colors.Black, ColorGenerator.GetForegroundForBackground(null!).Color);
            Assert.AreEqual(Colors.Black, ColorGenerator.GetForegroundForBackground(Brushes.Transparent).Color);
        }
    }
}
