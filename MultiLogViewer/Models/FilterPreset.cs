using System.Collections.Generic;

namespace MultiLogViewer.Models
{
    /// <summary>
    /// フィルタ設定を保存するためのモデルクラスです。
    /// </summary>
    public class FilterPreset
    {
        public string FilterText { get; set; } = string.Empty;
        public List<LogFilter> ExtensionFilters { get; set; } = new List<LogFilter>();
    }
}
