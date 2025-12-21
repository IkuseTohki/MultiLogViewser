using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.Models;
using MultiLogViewer.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MultiLogViewer.Tests.Services
{
    [TestClass]
    public class FilterPresetServiceTests
    {
        private string _tempFile = "";
        private FilterPresetService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _tempFile = Path.GetTempFileName();
            _service = new FilterPresetService();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (File.Exists(_tempFile)) File.Delete(_tempFile);
        }

        [TestMethod]
        public void SaveAndLoad_RoundTrip_ReturnsIdenticalObject()
        {
            // テスト観点: オブジェクトを保存し、再度読み込んだ時に、内容が完全に一致すること（シリアライズの検証）
            var original = new FilterPreset
            {
                FilterText = "ErrorSearch",
                ExtensionFilters = new List<LogFilter>
                {
                    new LogFilter(FilterType.ColumnEmpty, "Level", default, "LevelBadge"),
                    new LogFilter(FilterType.DateTimeAfter, "", new DateTime(2025, 12, 21), "DateBadge")
                }
            };

            // Act: 保存
            _service.Save(_tempFile, original);

            // Act: 読み込み
            var loaded = _service.Load(_tempFile);

            // Assert
            Assert.IsNotNull(loaded);
            Assert.AreEqual(original.FilterText, loaded.FilterText);
            Assert.AreEqual(original.ExtensionFilters.Count, loaded.ExtensionFilters.Count);

            Assert.AreEqual(original.ExtensionFilters[0].Type, loaded.ExtensionFilters[0].Type);
            Assert.AreEqual(original.ExtensionFilters[0].Key, loaded.ExtensionFilters[0].Key);
            Assert.AreEqual(original.ExtensionFilters[0].DisplayText, loaded.ExtensionFilters[0].DisplayText);

            Assert.AreEqual(original.ExtensionFilters[1].Type, loaded.ExtensionFilters[1].Type);
            Assert.AreEqual(original.ExtensionFilters[1].Value, loaded.ExtensionFilters[1].Value);
        }

        [TestMethod]
        public void Load_NonExistentFile_ReturnsNull()
        {
            // テスト観点: 存在しないファイルを読み込もうとした場合、nullを返すこと
            var result = _service.Load("non_existent_file.yaml");
            Assert.IsNull(result);
        }

        [TestMethod]
        public void Load_UnknownProperties_IgnoredSuccessfully()
        {
            // テスト観点: 未定義のプロパティがファイルに含まれていても、エラーにならず無視されること
            var yaml = @"
filter_text: 'Test'
extra_bogus_field: 'should be ignored'
extension_filters: []
";
            File.WriteAllText(_tempFile, yaml);

            var loaded = _service.Load(_tempFile);

            Assert.IsNotNull(loaded);
            Assert.AreEqual("Test", loaded.FilterText);
        }

        [TestMethod]
        public void Load_InvalidYaml_ThrowsFriendlyException()
        {
            // テスト観点: 構文エラーがある場合に、親切なメッセージを含む例外がスローされること
            File.WriteAllText(_tempFile, "invalid: [ : : yaml");

            try
            {
                _service.Load(_tempFile);
                Assert.Fail("Should have thrown an exception.");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("書式が正しいか見直してください"), $"Actual: {ex.Message}");
            }
        }

        [TestMethod]
        public void Load_MissingFilterKey_ThrowsFriendlyException()
        {
            // テスト観点: カラムフィルターなのに key が空（または存在しない）場合に、バリデーションエラーが発生すること
            var yaml = @"
extension_filters:
  - type: ColumnEmpty
    key: ''
";
            File.WriteAllText(_tempFile, yaml);

            try
            {
                _service.Load(_tempFile);
                Assert.Fail("Should have thrown an exception due to missing key.");
            }
            catch (Exception ex)
            {
                Assert.IsTrue(ex.Message.Contains("有効なキーが指定されていません"), $"Actual: {ex.Message}");
            }
        }
    }
}
