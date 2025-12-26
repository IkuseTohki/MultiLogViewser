using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Data;
using MultiLogViewer.Models;

namespace MultiLogViewer.ViewModels.Converters
{
    /// <summary>
    /// AdditionalDataのキー（またはLogFilterオブジェクト）とDisplayColumns設定を受け取り、
    /// 適切な表示名（ヘッダー名）を返すコンバーター。
    /// </summary>
    public class KeyToHeaderConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // values[0]: Key (string) または LogFilter オブジェクト
            // values[1]: DisplayColumns (IEnumerable<DisplayColumnConfig>)

            if (values.Length < 2 || !(values[1] is IEnumerable<DisplayColumnConfig> columns))
            {
                return Binding.DoNothing;
            }

            string key;
            LogFilter? filter = null;

            if (values[0] is string k)
            {
                key = k;
            }
            else if (values[0] is LogFilter f)
            {
                if (f.Type != FilterType.ColumnEmpty)
                {
                    // 日時フィルターなどは、保持している DisplayText をそのまま返す
                    return f.DisplayText;
                }
                filter = f;
                key = f.Key;
            }
            else
            {
                return Binding.DoNothing;
            }

            // AdditionalData[key] というバインディングパスを持つカラムを探す
            var targetPath = $"AdditionalData[{key}]";
            var col = columns.FirstOrDefault(c => c.BindingPath == targetPath);

            if (col != null && !string.IsNullOrEmpty(col.Header))
            {
                return col.Header;
            }

            // 見つからない、またはヘッダーが空の場合はキーをそのまま返す
            return key;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
