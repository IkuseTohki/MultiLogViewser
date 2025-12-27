namespace MultiLogViewer.Services
{
    public interface ITailModeWarningDialogService
    {
        /// <summary>
        /// Tailモードの警告ダイアログを表示します。
        /// </summary>
        /// <returns>ユーザーが続行（OK）を選択した場合は true。キャンセルした場合は false。
        /// 引数の skipNextTime は「次回から表示しない」のチェック状態を受け取ります。</returns>
        bool ShowWarning(out bool skipNextTime);
    }
}
