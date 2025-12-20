using System.Collections.Generic;

namespace MultiLogViewer.Models
{
    public class AppConfig
    {
        public int PollingIntervalMs { get; set; } = 1000;
        public List<DisplayColumnConfig> DisplayColumns { get; set; } = new List<DisplayColumnConfig>();
        public List<LogFormatConfig> LogFormats { get; set; } = new List<LogFormatConfig>();
    }
}
