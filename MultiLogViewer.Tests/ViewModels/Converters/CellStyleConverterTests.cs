using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.Models;
using MultiLogViewer.ViewModels.Converters;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace MultiLogViewer.Tests.ViewModels.Converters
{
    [TestClass]
    public class CellStyleConverterTests
    {
        private CellStyleConverter _converter = null!;

        [TestInitialize]
        public void Setup()
        {
            _converter = new CellStyleConverter();
        }

        [TestMethod]
        public void Convert_MatchingRule_ReturnsRuleBackground()
        {
            // テスト観点: 正規表現ルールにマッチした場合、指定された背景色が返されること
            var config = new ColumnStyleConfig
            {
                Rules = new List<ColorRuleConfig>
                {
                    new ColorRuleConfig { Pattern = "ERROR", Background = "Red" }
                }
            };
            _converter.StyleConfig = config;

            var result = _converter.Convert("system ERROR occured", typeof(Brush), "Background", System.Globalization.CultureInfo.InvariantCulture);

            Assert.IsInstanceOfType(result, typeof(SolidColorBrush));
            Assert.AreEqual(Colors.Red, ((SolidColorBrush)result).Color);
        }

        [TestMethod]
        public void Convert_MatchingRuleBold_ReturnsFontWeightBold()
        {
            // テスト観点: Bold指定のあるルールにマッチした場合、FontWeight.Boldが返されること
            var config = new ColumnStyleConfig
            {
                Rules = new List<ColorRuleConfig>
                {
                    new ColorRuleConfig { Pattern = "CRITICAL", FontWeight = "Bold" }
                }
            };
            _converter.StyleConfig = config;

            var result = _converter.Convert("CRITICAL", typeof(FontWeight), "FontWeight", System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual(FontWeights.Bold, result);
        }

        [TestMethod]
        public void Convert_SemanticColoring_ReturnsGeneratedColor()
        {
            // テスト観点: ルールにマッチせずセマンティックカラーリングが有効な場合、自動生成された色が返されること
            var config = new ColumnStyleConfig { SemanticColoring = true };
            _converter.StyleConfig = config;

            var result = _converter.Convert("UserA", typeof(Brush), "Background", System.Globalization.CultureInfo.InvariantCulture);

            Assert.IsInstanceOfType(result, typeof(SolidColorBrush));
            Assert.AreNotEqual(Colors.Transparent, ((SolidColorBrush)result).Color);
        }

        [TestMethod]
        public void Convert_NoMatchNoSemantic_ReturnsDefault()
        {
            // テスト観点: マッチするルールがなくセマンティックも無効な場合、デフォルト（透明）を返すこと
            var config = new ColumnStyleConfig { SemanticColoring = false };
            _converter.StyleConfig = config;

            var result = _converter.Convert("AnyValue", typeof(Brush), "Background", System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual(Brushes.Transparent, result);
        }

        [TestMethod]
        public void Convert_RulePriority_FirstRuleWins()
        {
            // テスト観点: 複数のルールにマッチする場合、最初のルールが優先されること
            var config = new ColumnStyleConfig
            {
                Rules = new List<ColorRuleConfig>
                {
                    new ColorRuleConfig { Pattern = "ABC", Background = "Blue" },
                    new ColorRuleConfig { Pattern = "AB", Background = "Green" }
                }
            };
            _converter.StyleConfig = config;

            var result = _converter.Convert("ABC", typeof(Brush), "Background", System.Globalization.CultureInfo.InvariantCulture);
            Assert.AreEqual(Colors.Blue, ((SolidColorBrush)result).Color);
        }
    }
}
