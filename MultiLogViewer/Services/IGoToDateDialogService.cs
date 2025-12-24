using MultiLogViewer.ViewModels;

namespace MultiLogViewer.Services
{
    public interface IGoToDateDialogService
    {
        void Show(GoToDateViewModel viewModel, System.Action<System.DateTime> onJump);
        void Close();
        bool IsOpen { get; }
    }
}
