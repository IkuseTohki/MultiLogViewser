using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using MultiLogViewer.Models;
using MultiLogViewer.Services;
using System.IO;

namespace MultiLogViewer.Tests.Services
{
    [TestClass]
    public class AppSettingsServiceTests
    {
        private Mock<IConfigPathResolver> _mockConfigPathResolver = null!;
        private string _tempFilePath = null!;

        [TestInitialize]
        public void Setup()
        {
            _mockConfigPathResolver = new Mock<IConfigPathResolver>();
            _tempFilePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            _mockConfigPathResolver.Setup(r => r.GetAppSettingsPath()).Returns(_tempFilePath);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_tempFilePath)) File.Delete(_tempFilePath);
        }

        [TestMethod]
        public void SaveAndLoad_RoundTrip_Works()
        {
            // Arrange
            var service = new AppSettingsService(_mockConfigPathResolver.Object);
            var settings = new AppSettings
            {
                PollingIntervalMs = 500,
                LogRetentionLimit = "-3d",
                SkipTailModeWarning = true
            };

            // Act
            service.Save(settings);
            var loaded = service.Load();

            // Assert
            Assert.AreEqual(500, loaded.PollingIntervalMs);
            Assert.AreEqual("-3d", loaded.LogRetentionLimit);
            Assert.AreEqual(true, loaded.SkipTailModeWarning);
        }

        [TestMethod]
        public void Load_ReturnsDefault_WhenFileMissing()
        {
            // Arrange
            var service = new AppSettingsService(_mockConfigPathResolver.Object);

            // Act
            var loaded = service.Load();

            // Assert
            Assert.IsNotNull(loaded);
            Assert.AreEqual(1000, loaded.PollingIntervalMs); // Default value
            Assert.IsFalse(loaded.SkipTailModeWarning);
        }
    }
}
