using System.Windows;

namespace MultiLogViewer.Services
{
    public class WpfClipboardService : IClipboardService
    {
        public void SetText(string text)
        {
            Clipboard.SetDataObject(text);
        }

        public string? GetText()
        {
            if (Clipboard.ContainsText())
            {
                return Clipboard.GetText();
            }
            return null;
        }
    }
}
