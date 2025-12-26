using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.Models;
using System.Collections.Generic;

namespace MultiLogViewer.Tests.Models
{
    [TestClass]
    public class ConfigModelsTests
    {
        [TestMethod]
        public void AppSettings_ShouldHaveDefaultValues()
        {
            // Arrange & Act
            var settings = new AppSettings();

            // Assert
            Assert.AreEqual(1000, settings.PollingIntervalMs);
        }

        [TestMethod]
        public void LogProfile_ShouldHaveDefaultValues()
        {
            // Arrange & Act
            var profile = new LogProfile();

            // Assert
            Assert.IsNotNull(profile.LogFormats);
            Assert.IsNotNull(profile.DisplayColumns);
            Assert.IsNotNull(profile.ColumnStyles);
            Assert.AreEqual(0, profile.LogFormats.Count);
            Assert.AreEqual(0, profile.DisplayColumns.Count);
            Assert.AreEqual(0, profile.ColumnStyles.Count);
        }
    }
}
