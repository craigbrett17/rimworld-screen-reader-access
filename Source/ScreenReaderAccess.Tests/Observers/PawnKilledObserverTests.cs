using ScreenReaderAccess.Commands;
using ScreenReaderAccess.Observers;
using ScreenReaderAccess.DTOs;
using ScreenReaderAccess.Patches;
using FluentAssertions;
using Xunit;

namespace ScreenReaderAccess.Tests.Observers
{
    public class PawnKilledObserverTests
    {
        private class TestCommand : ICommand<LogCommandArgs>
        {
            public LogCommandArgs ReceivedArgs { get; private set; }
            public void Execute(LogCommandArgs args)
            {
                ReceivedArgs = args;
            }
        }

        [Fact]
        public void OnEvent_ExecutesLogCommandWithCorrectMessage()
        {
            // Arrange
            var testCommand = new TestCommand();
            var observer = new PawnKilledObserver(testCommand);

            // Use DTO instead of Verse.Pawn
            var pawnInfo = new PawnInfoDto { Name = "Testy", Label = "Colonist" };
            var evt = new PawnKilledEvent(pawnInfo);

            // Act
            observer.OnEvent(evt);

            // Assert
            testCommand.ReceivedArgs.Should().NotBeNull();
            testCommand.ReceivedArgs.Message.Should().Contain("Pawn killed: Testy (Colonist)");
        }
    }
}
