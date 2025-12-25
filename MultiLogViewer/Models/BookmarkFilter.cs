using System;

namespace MultiLogViewer.Models
{
    /// <summary>
    /// ブックマークフィルターを表すクラスです。
    /// LogFilter を継承し、WPF のテンプレート切り替えに使用します。
    /// </summary>
    public class BookmarkFilter : LogFilter
    {
        public BookmarkColor? TargetColor { get; }

        public BookmarkFilter(BookmarkColor? targetColor = null)
            : base(FilterType.Bookmark, "", default, "Bookmark")
        {
            TargetColor = targetColor;
        }
    }
}
