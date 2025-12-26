using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace MultiLogViewer.Models
{
    public class LogProfile
    {
        [YamlMember(Alias = "display_columns")]
        public List<DisplayColumnConfig> DisplayColumns { get; set; } = new List<DisplayColumnConfig>();

        [YamlMember(Alias = "column_styles")]
        public List<ColumnStyleConfig> ColumnStyles { get; set; } = new List<ColumnStyleConfig>();

        [YamlMember(Alias = "log_formats")]
        public List<LogFormatConfig> LogFormats { get; set; } = new List<LogFormatConfig>();
    }
}
