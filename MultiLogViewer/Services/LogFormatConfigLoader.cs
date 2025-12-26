using MultiLogViewer.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.IO;
using System.Collections.Generic;

namespace MultiLogViewer.Services
{
    public class LogFormatConfigLoader : ILogFormatConfigLoader
    {
        public AppConfig Load(string logProfilePath, string appSettingsPath)
        {
            var appConfig = new AppConfig();
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();

            // Load AppSettings
            if (File.Exists(appSettingsPath))
            {
                try
                {
                    using (var reader = new StreamReader(appSettingsPath))
                    {
                        var appSettings = deserializer.Deserialize<AppSettings>(reader);
                        if (appSettings != null)
                        {
                            appConfig.PollingIntervalMs = appSettings.PollingIntervalMs;
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception($"アプリケーション設定({appSettingsPath})の読み込みに失敗しました。\n{ex.Message}", ex);
                }
            }

            // Load LogProfile
            if (File.Exists(logProfilePath))
            {
                try
                {
                    using (var reader = new StreamReader(logProfilePath))
                    {
                        var logProfile = deserializer.Deserialize<LogProfile>(reader);
                        if (logProfile != null)
                        {
                            appConfig.LogFormats = logProfile.LogFormats ?? new List<LogFormatConfig>();
                            appConfig.DisplayColumns = logProfile.DisplayColumns ?? new List<DisplayColumnConfig>();
                            appConfig.ColumnStyles = logProfile.ColumnStyles ?? new List<ColumnStyleConfig>();
                        }
                    }
                }
                catch (YamlDotNet.Core.YamlException ex)
                {
                    throw new System.Exception($"ログプロファイル({logProfilePath})の解析に失敗しました。書式が正しいか見直してください。\n詳細: {ex.Message}", ex);
                }
                catch (System.Exception ex)
                {
                    throw new System.Exception($"ログプロファイルの読み込み中に予期せぬエラーが発生しました。\n{ex.Message}", ex);
                }
            }

            return appConfig;
        }
    }
}
