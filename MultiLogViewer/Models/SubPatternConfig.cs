using System.Collections.Generic;

namespace MultiLogViewer.Models
{
    public class SubPatternConfig
    {
        public string SourceField { get; set; } = string.Empty;
        public string Pattern { get; set; } = string.Empty;
        public List<FieldTransformConfig> FieldTransforms { get; set; } = new List<FieldTransformConfig>();
    }
}
