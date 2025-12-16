using System.Collections.Generic;

namespace MultiLogViewer.Services
{
    /// <summary>
    /// globパターンを解決し、一致するファイルパスのリストを返すサービスを定義します。
    /// </summary>
    public interface IFileResolver
    {
        /// <summary>
        /// 指定されたglobパターンのリストを解決し、一致する全てのファイルパスを返します。
        /// </summary>
        /// <param name="patterns">解決するglobパターンのリスト。</param>
        /// <returns>globパターンに一致するファイルの絶対パスのリスト。</returns>
        IEnumerable<string> Resolve(IEnumerable<string> patterns);
    }
}
