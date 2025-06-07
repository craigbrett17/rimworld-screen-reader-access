using ScreenReaderAccess.Patches;
using ScreenReaderAccess.Commands;
using ScreenReaderAccess.DTOs;

namespace ScreenReaderAccess.Observers
{
    public class NewMessageObserver : IEventObserver<MessageEvent>
    {
        private readonly ICommand<ScreenReaderOutputCommandArgs> outputCommand;

        public NewMessageObserver(ICommand<ScreenReaderOutputCommandArgs> outputCommand)
        {
            this.outputCommand = outputCommand;
        }

        public void OnEvent(MessageEvent evt)
        {
            outputCommand.Execute(new ScreenReaderOutputCommandArgs
            {
                Message = evt.Message.Text,
                Interrupt = true
            });
        }
    }
}
