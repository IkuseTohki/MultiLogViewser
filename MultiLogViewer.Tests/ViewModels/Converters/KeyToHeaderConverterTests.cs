using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.Models;
using MultiLogViewer.ViewModels.Converters;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace MultiLogViewer.Tests.ViewModels.Converters
{
    [TestClass]
    public class KeyToHeaderConverterTests
    {
        private KeyToHeaderConverter _converter = null!;
        private List<DisplayColumnConfig> _columns = null!;

        [TestInitialize]
        public void Setup()
        {
            _converter = new KeyToHeaderConverter();
            _columns = new List<DisplayColumnConfig>
            {
                new DisplayColumnConfig { Header = "LevelHeader", BindingPath = "AdditionalData[level]" },
                new DisplayColumnConfig { Header = "UserHeader", BindingPath = "AdditionalData[user]" },
                new DisplayColumnConfig { Header = "", BindingPath = "AdditionalData[empty]" }
            };
        }

        [TestMethod]
        public void Convert_StringKey_ReturnsHeader()
        {
            // Arrange
            var values = new object[] { "level", _columns };

            // Act
            var result = _converter.Convert(values, typeof(string), null!, CultureInfo.InvariantCulture);

            // Assert
            Assert.AreEqual("LevelHeader", result);
        }

        [TestMethod]
        public void Convert_LogFilter_ReturnsHeader()
        {
            // Arrange
            var filter = new LogFilter(FilterType.ColumnEmpty, "user", default, "Original");
            var values = new object[] { filter, _columns };

            // Act
            var result = _converter.Convert(values, typeof(string), null!, CultureInfo.InvariantCulture);

            // Assert
            Assert.AreEqual("UserHeader", result);
        }

        [TestMethod]
        public void Convert_DateTimeFilter_ReturnsOriginalDisplayText()
        {
            // Arrange
            var filter = new LogFilter(FilterType.DateTimeAfter, "", System.DateTime.Now, "2023-01-01以降");
            var values = new object[] { filter, _columns };

            // Act
            var result = _converter.Convert(values, typeof(string), null!, CultureInfo.InvariantCulture);

            // Assert
            Assert.AreEqual("2023-01-01以降", result, "DateTime filters should preserve their dynamic DisplayText.");
        }

        [TestMethod]
        public void Convert_UnknownKey_ReturnsKey()
        {
            // Arrange
            var values = new object[] { "unknown", _columns };

            // Act
            var result = _converter.Convert(values, typeof(string), null!, CultureInfo.InvariantCulture);

            // Assert
            Assert.AreEqual("unknown", result);
        }

        [TestMethod]
        public void Convert_EmptyHeader_ReturnsKey()
        {
            // Arrange
            var values = new object[] { "empty", _columns };

            // Act
            var result = _converter.Convert(values, typeof(string), null!, CultureInfo.InvariantCulture);

            // Assert
            Assert.AreEqual("empty", result);
        }
    }
}
