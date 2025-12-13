using Microsoft.Win32;
using System.IO;

namespace MultiLogViewer.Services
{
    public class WpfUserDialogService : IUserDialogService
    {
        public string? OpenFileDialog()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Log files (*.log)|*.log|All files (*.*)|*.*",
                InitialDirectory = Directory.GetCurrentDirectory()
            };

            if (openFileDialog.ShowDialog() == true)
            {
                return openFileDialog.FileName;
            }

            return null;
        }
    }
}
