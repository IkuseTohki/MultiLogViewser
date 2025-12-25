using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.Models;
using MultiLogViewer.Utils;
using System;
using System.Collections.Generic;

namespace MultiLogViewer.Tests.Utils
{
    [TestClass]
    public class LogEntryValueConverterTests
    {
        [TestMethod]
        public void GetStringValue_StandardProperties_ReturnsCorrectValues()
        {
            // テスト観点: 標準的なプロパティ（Timestamp, Message等）が正しく取得できること。
            var timestamp = new DateTime(2023, 12, 25, 10, 30, 0);
            var entry = new LogEntry
            {
                Timestamp = timestamp,
                Message = "Hello World",
                FileName = "test.log",
                LineNumber = 123
            };

            Assert.AreEqual("Hello World", LogEntryValueConverter.GetStringValue(entry, "Message", null));
            Assert.AreEqual("test.log", LogEntryValueConverter.GetStringValue(entry, "FileName", null));
            Assert.AreEqual("123", LogEntryValueConverter.GetStringValue(entry, "LineNumber", null));

            // 書式指定ありのテスト
            Assert.AreEqual("2023-12-25", LogEntryValueConverter.GetStringValue(entry, "Timestamp", "yyyy-MM-dd"));
        }

        [TestMethod]
        public void GetStringValue_AdditionalData_ReturnsValue()
        {
            // テスト観点: AdditionalData[key] 形式のパスから値が取得できること。
            var entry = new LogEntry
            {
                AdditionalData = new Dictionary<string, string> { { "User", "Alice" }, { "Level", "INFO" } }
            };

            Assert.AreEqual("Alice", LogEntryValueConverter.GetStringValue(entry, "AdditionalData[User]", null));
            Assert.AreEqual("INFO", LogEntryValueConverter.GetStringValue(entry, "AdditionalData[Level]", null));
            Assert.AreEqual("", LogEntryValueConverter.GetStringValue(entry, "AdditionalData[NonExistent]", null));
        }

        [TestMethod]
        public void ExtractAdditionalDataKey_ReturnsKeyOnly()
        {
            // テスト観点: 複雑なバインドパスからキー名だけを正しく抽出できること。
            Assert.AreEqual("level", LogEntryValueConverter.ExtractAdditionalDataKey("AdditionalData[level]"));
            Assert.AreEqual("process_id", LogEntryValueConverter.ExtractAdditionalDataKey("AdditionalData[process_id]"));

            // 該当しない場合は null
            Assert.IsNull(LogEntryValueConverter.ExtractAdditionalDataKey("Message"));
            Assert.IsNull(LogEntryValueConverter.ExtractAdditionalDataKey("Timestamp"));
            Assert.IsNull(LogEntryValueConverter.ExtractAdditionalDataKey("Invalid[Key]"));
        }
    }
}
