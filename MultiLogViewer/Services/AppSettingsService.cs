using MultiLogViewer.Models;
using System.IO;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace MultiLogViewer.Services
{
    public class AppSettingsService : IAppSettingsService
    {
        private readonly IConfigPathResolver _configPathResolver;
        private readonly ISerializer _serializer;
        private readonly IDeserializer _deserializer;

        public AppSettingsService(IConfigPathResolver configPathResolver)
        {
            _configPathResolver = configPathResolver;

            _serializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .IgnoreUnmatchedProperties()
                .Build();
        }

        public AppSettings Load()
        {
            var path = _configPathResolver.GetAppSettingsPath();
            if (!File.Exists(path)) return new AppSettings();

            try
            {
                using (var reader = new StreamReader(path))
                {
                    return _deserializer.Deserialize<AppSettings>(reader) ?? new AppSettings();
                }
            }
            catch
            {
                return new AppSettings();
            }
        }

        public void Save(AppSettings settings)
        {
            var path = _configPathResolver.GetAppSettingsPath();
            using (var writer = new StreamWriter(path))
            {
                _serializer.Serialize(writer, settings);
            }
        }
    }
}
