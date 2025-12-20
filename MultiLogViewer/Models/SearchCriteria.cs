namespace MultiLogViewer.Models
{
    /// <summary>
    /// ログ検索の検索条件を保持するクラスです。
    /// </summary>
    public class SearchCriteria
    {
        public string SearchText { get; }
        public bool IsCaseSensitive { get; }
        public bool IsRegex { get; }

        public SearchCriteria(string searchText, bool isCaseSensitive, bool isRegex)
        {
            SearchText = searchText;
            IsCaseSensitive = isCaseSensitive;
            IsRegex = isRegex;
        }
    }
}
