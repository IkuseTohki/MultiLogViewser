namespace MultiLogViewer.Services
{
    public interface ISearchWindowService
    {
        void Show(object viewModel);
        void Close();
        bool IsOpen { get; }
    }
}
