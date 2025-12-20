using System.Collections.Generic;

namespace MultiLogViewer.Models
{
    /// <summary>
    /// フィールド値の変換設定を保持するクラスです。
    /// </summary>
    public class FieldTransformConfig
    {
        /// <summary>
        /// 変換対象のフィールド名（正規表現のグループ名）。
        /// </summary>
        public string Field { get; set; } = string.Empty;

        /// <summary>
        /// 値の置換マッピング（完全一致）。
        /// </summary>
        public Dictionary<string, string>? Map { get; set; }

        /// <summary>
        /// 値のフォーマット文字列。{value} プレースホルダーを使用。
        /// </summary>
        public string? Format { get; set; }
    }
}
