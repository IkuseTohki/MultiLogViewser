using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MultiLogViewer.Tests
{
    [TestClass]
    public class LogFormatConfigLoaderTests
    {
        /// <summary>
        /// テスト観点: 有効なYAML設定ファイルが正しくデシリアライズされ、AppConfigオブジェクトが生成されることを確認する。
        /// </summary>
        [TestMethod]
        public void LoadConfig_ValidYaml_ReturnsAppConfig()
        {
            // Arrange
            var yamlContent = @"
log_formats:
  - name: ""ApplicationLog""
    pattern: ""^(?<timestamp>\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2}) \\[(?<level>\\w+)\\] (?<message>.*)$""
    timestamp_format: ""yyyy-MM-dd HH:mm:ss""
  - name: ""WebServerLog""
    pattern: ""^(?<level>\\w+) \\d+ (?<timestamp>\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}.\\d{3}Z) (?<message>.*)$""
    timestamp_format: ""yyyy-MM-ddTHH:mm:ss.fffZ""
";
            // TODO: このロジックはまだ存在しないため、コンパイルエラーになる。
            //       後で LogFormatConfigLoader クラスを作成する。
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            // Act
            var config = deserializer.Deserialize<AppConfig>(yamlContent);

            // Assert
            Assert.IsNotNull(config);
            Assert.IsNotNull(config.LogFormats);
            Assert.HasCount(2, config.LogFormats);

            var appLog = config.LogFormats[0];
            Assert.AreEqual("ApplicationLog", appLog.Name);
            Assert.AreEqual("^(?<timestamp>\\d{4}-\\d{2}-\\d{2} \\d{2}:\\d{2}:\\d{2}) \\[(?<level>\\w+)\\] (?<message>.*)$", appLog.Pattern);
            Assert.AreEqual("yyyy-MM-dd HH:mm:ss", appLog.TimestampFormat);

            var webLog = config.LogFormats[1];
            Assert.AreEqual("WebServerLog", webLog.Name);
            Assert.AreEqual("^(?<level>\\w+) \\d+ (?<timestamp>\\d{4}-\\d{2}-\\d{2}T\\d{2}:\\d{2}:\\d{2}.\\d{3}Z) (?<message>.*)$", webLog.Pattern);
            Assert.AreEqual("yyyy-MM-ddTHH:mm:ss.fffZ", webLog.TimestampFormat);
        }
    }
}
