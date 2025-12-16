using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

namespace MultiLogViewer.Services
{
    /// <summary>
    /// globパターンを解決し、一致するファイルパスのリストを返すサービスを実装します。
    /// '**' (二重アスタリスク) を含む再帰的なパターンもサポートします。
    /// </summary>
    public class FileResolver : IFileResolver
    {
        /// <summary>
        /// 指定されたglobパターンのリストを解決し、一致する全てのファイルパスを返します。
        /// </summary>
        /// <param name="patterns">解決するglobパターンのリスト。</param>
        /// <returns>globパターンに一致するファイルの絶対パスのリスト。</returns>
        public IEnumerable<string> Resolve(IEnumerable<string> patterns)
        {
            if (patterns == null || !patterns.Any())
            {
                return Enumerable.Empty<string>();
            }

            var matchingFiles = new HashSet<string>();

            foreach (var pattern in patterns)
            {
                string baseDirectory;
                string relativePattern;

                // パターンが絶対パスの場合、ベースディレクトリとMatcherに渡す相対パターンを特定する
                if (Path.IsPathRooted(pattern))
                {
                    // ワイルドカードの有無で処理を分ける
                    int firstWildcardIndex = pattern.IndexOfAny(new char[] { '*', '?' });
                    if (firstWildcardIndex != -1)
                    {
                        // ワイルドカードの直前までをベースディレクトリ候補とする
                        string pathUntilWildcard = pattern.Substring(0, firstWildcardIndex);
                        baseDirectory = Path.GetDirectoryName(pathUntilWildcard) ?? Directory.GetCurrentDirectory();
                        if (string.IsNullOrEmpty(baseDirectory))
                        {
                            // ワイルドカードがパスの最初にある場合など
                            baseDirectory = Directory.GetCurrentDirectory(); // またはエラー
                        }
                        relativePattern = Path.GetRelativePath(baseDirectory, pattern);
                    }
                    else
                    {
                        // ワイルドカードがない絶対パスの場合、ファイル単体を指すかディレクトリを指す
                        // ここでは、Matcherに渡すためにファイル名部分をrelativePatternとする
                        baseDirectory = Path.GetDirectoryName(pattern) ?? Directory.GetCurrentDirectory();
                        relativePattern = Path.GetFileName(pattern);
                    }
                }
                else
                {
                    // 相対パスの場合、カレントディレクトリをベースとする
                    baseDirectory = Directory.GetCurrentDirectory();
                    relativePattern = pattern;
                }

                // ベースディレクトリが存在しない場合や、適切に解決できない場合はスキップ
                if (string.IsNullOrEmpty(baseDirectory) || !Directory.Exists(baseDirectory))
                {
                    continue;
                }

                var matcher = new Matcher();
                // Matcherはスラッシュ区切りを想定しているので変換
                matcher.AddInclude(relativePattern.Replace('\\', '/'));

                // DirectoryInfoWrapper を使用して、Matcher がファイルシステムにアクセスできるようにする
                var result = matcher.Execute(new DirectoryInfoWrapper(new DirectoryInfo(baseDirectory)));
                foreach (var fileMatch in result.Files)
                {
                    // マッチしたファイルの相対パスを絶対パスに変換して追加
                    matchingFiles.Add(Path.GetFullPath(Path.Combine(baseDirectory, fileMatch.Path)));
                }
            }

            return matchingFiles.OrderBy(f => f); // 結果をソートして返す
        }
    }
}
