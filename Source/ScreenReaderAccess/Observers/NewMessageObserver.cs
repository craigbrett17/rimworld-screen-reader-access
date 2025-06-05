using ScreenReaderAccess.Patches;
using ScreenReaderAccess.Commands;
using ScreenReaderAccess.DTOs;

namespace ScreenReaderAccess.Observers
{
    public class NewMessageObserver : IEventObserver<MessageEvent>
    {
        private readonly ICommand<LogCommandArgs> logCommand;

        public NewMessageObserver(ICommand<LogCommandArgs> logCommand)
        {
            this.logCommand = logCommand;
        }

        public void OnEvent(MessageEvent evt)
        {
            var message = $"Message from game: {evt.Message.Text}";
            logCommand.Execute(new LogCommandArgs { Message = message });
        }
    }
}
