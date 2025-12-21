using Microsoft.Win32;
using System.IO;
using System.Windows; // MessageBox

namespace MultiLogViewer.Services
{
    public class WpfUserDialogService : IUserDialogService
    {
        public string? OpenFileDialog(string filter = "All files (*.*)|*.*")
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = filter,
                InitialDirectory = Directory.GetCurrentDirectory()
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;
        }

        public string? SaveFileDialog(string filter = "All files (*.*)|*.*", string defaultFileName = "")
        {
            var saveFileDialog = new SaveFileDialog
            {
                Filter = filter,
                FileName = defaultFileName,
                InitialDirectory = Directory.GetCurrentDirectory()
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }

            return null;
        }

        public void ShowError(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
