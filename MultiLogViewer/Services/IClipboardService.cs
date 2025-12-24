namespace MultiLogViewer.Services
{
    public interface IClipboardService
    {
        void SetText(string text);
        string? GetText();
    }
}
