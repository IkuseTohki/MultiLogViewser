using System.Collections.Generic;

namespace MultiLogViewer.Models
{
    public class AppConfig
    {
        public List<DisplayColumnConfig> DisplayColumns { get; set; } = new List<DisplayColumnConfig>();
        public List<LogFormatConfig> LogFormats { get; set; } = new List<LogFormatConfig>();
    }
}
