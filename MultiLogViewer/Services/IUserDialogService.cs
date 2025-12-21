namespace MultiLogViewer.Services
{
    public interface IUserDialogService
    {
        string? OpenFileDialog(string filter = "All files (*.*)|*.*");
        string? SaveFileDialog(string filter = "All files (*.*)|*.*", string defaultFileName = "");
        void ShowError(string title, string message);
    }
}
