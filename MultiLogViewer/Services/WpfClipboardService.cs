using System.Windows;

namespace MultiLogViewer.Services
{
    public class WpfClipboardService : IClipboardService
    {
        public void SetText(string text)
        {
            Clipboard.SetDataObject(text);
        }
    }
}
