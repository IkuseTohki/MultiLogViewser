using MultiLogViewer.Models;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using System.IO;

namespace MultiLogViewer.Services
{
    public interface IFilterPresetService
    {
        void Save(string filePath, FilterPreset preset);
        FilterPreset? Load(string filePath);
    }

    public class FilterPresetService : IFilterPresetService
    {
        private readonly ISerializer _serializer;
        private readonly IDeserializer _deserializer;

        public FilterPresetService()
        {
            _serializer = new SerializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            _deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .IgnoreUnmatchedProperties() // 未知のプロパティを無視してクラッシュを防ぐ
                .Build();
        }

        public void Save(string filePath, FilterPreset preset)
        {
            try
            {
                var yaml = _serializer.Serialize(preset);
                File.WriteAllText(filePath, yaml);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception($"プリセットの保存中にエラーが発生しました。\n{ex.Message}", ex);
            }
        }

        public FilterPreset? Load(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    var preset = _deserializer.Deserialize<FilterPreset>(reader);

                    if (preset != null && preset.ExtensionFilters != null)
                    {
                        foreach (var filter in preset.ExtensionFilters)
                        {
                            filter.Validate();
                        }
                    }

                    return preset;
                }
            }
            catch (YamlDotNet.Core.YamlException ex)
            {
                throw new System.Exception($"プリセットファイルの解析に失敗しました。書式が正しいか見直してください。\n詳細: {ex.Message}", ex);
            }
            catch (System.Exception ex)
            {
                throw new System.Exception($"プリセットファイルの読み込み中に予期せぬエラーが発生しました。\n{ex.Message}", ex);
            }
        }
    }
}
