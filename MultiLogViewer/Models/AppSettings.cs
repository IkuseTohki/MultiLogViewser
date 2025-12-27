using YamlDotNet.Serialization;

namespace MultiLogViewer.Models
{
    public class AppSettings
    {
        public int PollingIntervalMs { get; set; } = 1000;
        public string? LogRetentionLimit { get; set; }
    }
}
