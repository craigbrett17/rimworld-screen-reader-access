using ScreenReaderAccess.Commands;
using ScreenReaderAccess.Observers;
using ScreenReaderAccess.DTOs;
using ScreenReaderAccess.Patches;
using FluentAssertions;
using Xunit;

namespace ScreenReaderAccess.Tests.Observers
{
    public class NewMessageObserverTests
    {
        private class TestCommand : ICommand<ScreenReaderOutputCommandArgs>
        {
            public ScreenReaderOutputCommandArgs ReceivedArgs { get; private set; }
            public void Execute(ScreenReaderOutputCommandArgs args)
            {
                ReceivedArgs = args;
            }
        }

        [Fact]
        public void OnEvent_ExecutesLogCommandWithCorrectMessage()
        {
            // Arrange
            var testCommand = new TestCommand();
            var observer = new NewMessageObserver(testCommand);
            var messageDto = new MessageDto { Text = "Hello world!" };
            var evt = new MessageEvent(messageDto);

            // Act
            observer.OnEvent(evt);

            // Assert
            testCommand.ReceivedArgs.Should().NotBeNull();
            testCommand.ReceivedArgs.Message.Should().Be("Hello world!");
            testCommand.ReceivedArgs.Interrupt.Should().BeTrue();
        }
    }
}
