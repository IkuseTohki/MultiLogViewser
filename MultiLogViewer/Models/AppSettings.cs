using YamlDotNet.Serialization;

namespace MultiLogViewer.Models
{
    public class AppSettings
    {
        [YamlMember(Alias = "polling_interval_ms")]
        public int PollingIntervalMs { get; set; } = 1000;
    }
}
