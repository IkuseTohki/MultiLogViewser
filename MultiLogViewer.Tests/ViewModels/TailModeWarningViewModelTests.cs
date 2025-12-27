using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.ViewModels;

namespace MultiLogViewer.Tests.ViewModels
{
    [TestClass]
    public class TailModeWarningViewModelTests
    {
        [TestMethod]
        public void SkipNextTime_PropertyChange_RaisesNotification()
        {
            var vm = new TailModeWarningViewModel();
            bool raised = false;
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(TailModeWarningViewModel.SkipNextTime)) raised = true;
            };

            vm.SkipNextTime = true;

            Assert.IsTrue(raised);
            Assert.IsTrue(vm.SkipNextTime);
        }

        [TestMethod]
        public void EnableCommand_SetsDialogResultToTrue()
        {
            var vm = new TailModeWarningViewModel();
            Assert.IsNull(vm.DialogResult);

            vm.EnableCommand.Execute(null);

            Assert.AreEqual(true, vm.DialogResult);
        }
    }
}
