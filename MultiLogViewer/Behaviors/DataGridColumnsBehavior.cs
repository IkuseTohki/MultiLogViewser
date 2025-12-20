using MultiLogViewer.Models;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace MultiLogViewer.Behaviors
{
    public static class DataGridColumnsBehavior
    {
        public static readonly DependencyProperty BindableColumnsProperty =
            DependencyProperty.RegisterAttached(
                "BindableColumns",
                typeof(ObservableCollection<DisplayColumnConfig>),
                typeof(DataGridColumnsBehavior),
                new PropertyMetadata(null, OnBindableColumnsChanged));

        public static ObservableCollection<DisplayColumnConfig> GetBindableColumns(DependencyObject obj)
        {
            return (ObservableCollection<DisplayColumnConfig>)obj.GetValue(BindableColumnsProperty);
        }

        public static void SetBindableColumns(DependencyObject obj, ObservableCollection<DisplayColumnConfig> value)
        {
            obj.SetValue(BindableColumnsProperty, value);
        }

        private static void OnBindableColumnsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is DataGrid dataGrid)
            {
                if (e.OldValue is INotifyCollectionChanged oldCollection)
                {
                    oldCollection.CollectionChanged -= (sender, args) => OnCollectionChanged(sender, args, dataGrid);
                }

                if (e.NewValue is INotifyCollectionChanged newCollection)
                {
                    newCollection.CollectionChanged += (sender, args) => OnCollectionChanged(sender, args, dataGrid);
                }

                // Initial population of columns
                GenerateColumns(dataGrid, e.NewValue as ObservableCollection<DisplayColumnConfig>);
            }
        }

        private static void OnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e, DataGrid dataGrid)
        {
            // Regenerate columns when the collection changes
            GenerateColumns(dataGrid, sender as ObservableCollection<DisplayColumnConfig>);
        }

        private static void GenerateColumns(DataGrid dataGrid, ObservableCollection<DisplayColumnConfig>? columns)
        {
            dataGrid.Columns.Clear();
            if (columns == null)
            {
                return;
            }

            foreach (var columnConfig in columns)
            {
                DataGridColumn newColumn;

                if (columnConfig.BindingPath == "Message")
                {
                    // Message列の場合はテンプレートを使用（1行表示 + アイコン）
                    // 検索範囲をDataGrid自身に広げることで、Windowのリソースも見つけられるようにする
                    var template = dataGrid.TryFindResource("MultilineMessageTemplate") as DataTemplate;
                    if (template != null)
                    {
                        newColumn = new DataGridTemplateColumn
                        {
                            Header = columnConfig.Header,
                            Width = new DataGridLength(columnConfig.Width),
                            MinWidth = columnConfig.Width,
                            CellTemplate = template,
                            SortMemberPath = columnConfig.BindingPath
                        };
                    }
                    else
                    {
                        newColumn = CreateTextColumn(columnConfig);
                    }
                }
                else
                {
                    newColumn = CreateTextColumn(columnConfig);
                }

                dataGrid.Columns.Add(newColumn);
            }
        }

        private static DataGridTextColumn CreateTextColumn(DisplayColumnConfig columnConfig)
        {
            var binding = new Binding(columnConfig.BindingPath)
            {
                Mode = BindingMode.OneWay
            };

            if (!string.IsNullOrEmpty(columnConfig.StringFormat))
            {
                binding.StringFormat = columnConfig.StringFormat;
            }

            return new DataGridTextColumn
            {
                Header = columnConfig.Header,
                Width = new DataGridLength(columnConfig.Width),
                MinWidth = columnConfig.Width,
                Binding = binding,
                SortMemberPath = columnConfig.BindingPath
            };
        }
    }
}
