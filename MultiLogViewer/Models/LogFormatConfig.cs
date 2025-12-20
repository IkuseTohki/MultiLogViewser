using System.Collections.Generic;

namespace MultiLogViewer.Models
{
    public class LogFormatConfig
    {
        public string Name { get; set; } = string.Empty;
        public string Pattern { get; set; } = string.Empty;
        public string TimestampFormat { get; set; } = string.Empty;
        public bool IsMultiline { get; set; } = true;
        public List<string> LogFilePatterns { get; set; } = new List<string>();
        public List<SubPatternConfig> SubPatterns { get; set; } = new List<SubPatternConfig>();
    }
}
