using System.Collections.Generic;

namespace MultiLogViewer.Models
{
    public class LogFormatConfig
    {
        public string Name { get; set; } = string.Empty;
        public string Pattern { get; set; } = string.Empty;
        public string TimestampFormat { get; set; } = string.Empty;
        public List<DisplayColumnConfig> DisplayColumns { get; set; } = new List<DisplayColumnConfig>();
        public List<SubPatternConfig> SubPatterns { get; set; } = new List<SubPatternConfig>();
    }
}

