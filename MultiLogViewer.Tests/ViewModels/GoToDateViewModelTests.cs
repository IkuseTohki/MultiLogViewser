using Microsoft.VisualStudio.TestTools.UnitTesting;
using MultiLogViewer.ViewModels;
using System;

namespace MultiLogViewer.Tests.ViewModels
{
    [TestClass]
    public class GoToDateViewModelTests
    {
        [TestMethod]
        public void JumpCommand_AbsoluteMode_FiresJumpRequested()
        {
            // Arrange
            var initial = new DateTime(2023, 1, 1, 12, 0, 0);
            var vm = new GoToDateViewModel(initial);
            vm.TargetDate = new DateTime(2023, 12, 25);
            vm.TargetTime = "10:30";
            DateTime? result = null;
            vm.JumpRequested += (dt) => result = dt;

            // Act
            vm.JumpCommand.Execute(null);

            // Assert
            Assert.AreEqual(new DateTime(2023, 12, 25, 10, 30, 0), result);
        }

        [TestMethod]
        public void NextCommand_UpdatesTargetAndFiresJumpRequested()
        {
            // Arrange
            var initial = new DateTime(2023, 1, 1, 12, 0, 0);
            var vm = new GoToDateViewModel(initial);
            vm.RelativeValue = 15;
            vm.RelativeUnit = "Minutes";
            DateTime? result = null;
            vm.JumpRequested += (dt) => result = dt;

            // Act
            vm.NextCommand.Execute(null);

            // Assert
            var expected = initial.AddMinutes(15);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NextCommand_Days_UpdatesTargetAndFiresJumpRequested()
        {
            // Arrange
            var initial = new DateTime(2023, 1, 1, 12, 0, 0);
            var vm = new GoToDateViewModel(initial);
            vm.RelativeValue = 2;
            vm.RelativeUnit = "Days";
            DateTime? result = null;
            vm.JumpRequested += (dt) => result = dt;

            // Act
            vm.NextCommand.Execute(null);

            // Assert
            var expected = initial.AddDays(2);
            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void NextCommand_Seconds_UpdatesTargetAndFiresJumpRequested()
        {
            // Arrange
            var initial = new DateTime(2023, 1, 1, 12, 0, 0);
            var vm = new GoToDateViewModel(initial, true); // Enable seconds
            vm.RelativeValue = 30;
            vm.RelativeUnit = "Seconds";
            DateTime? result = null;
            vm.JumpRequested += (dt) => result = dt;

            // Act
            vm.NextCommand.Execute(null);

            // Assert
            var expected = initial.AddSeconds(30);
            Assert.AreEqual(expected, result);
            Assert.AreEqual("12:00:30", vm.TargetTime);
        }

        [TestMethod]
        public void TargetTime_ReflectsIsSecondsEnabled()
        {
            // Arrange
            var initial = new DateTime(2023, 1, 1, 12, 34, 56);

            // Act
            var vmWithSec = new GoToDateViewModel(initial, true);
            var vmWithoutSec = new GoToDateViewModel(initial, false);

            // Assert
            Assert.AreEqual("12:34:56", vmWithSec.TargetTime);
            Assert.AreEqual("12:34", vmWithoutSec.TargetTime);
        }

        [TestMethod]
        public void PreviousCommand_UpdatesTargetAndFiresJumpRequested()
        {
            // Arrange
            var initial = new DateTime(2023, 1, 1, 12, 0, 0);
            var vm = new GoToDateViewModel(initial);
            vm.RelativeValue = 1;
            vm.RelativeUnit = "Hours";
            DateTime? result = null;
            vm.JumpRequested += (dt) => result = dt;

            // Act
            vm.PreviousCommand.Execute(null);

            // Assert
            var expected = initial.AddHours(-1);
            Assert.AreEqual(expected, result);
        }
    }
}
