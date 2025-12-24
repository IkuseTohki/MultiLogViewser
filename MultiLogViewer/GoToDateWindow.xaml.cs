using System.Windows;
using System.Windows.Input;

namespace MultiLogViewer
{
    public partial class GoToDateWindow : Window
    {
        public GoToDateWindow()
        {
            InitializeComponent();

            // タイトルバーがないため、背景のドラッグでウィンドウを移動できるようにする
            this.MouseLeftButtonDown += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    this.DragMove();
                }
            };
        }
    }
}
