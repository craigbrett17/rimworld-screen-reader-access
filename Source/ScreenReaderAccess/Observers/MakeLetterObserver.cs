using ScreenReaderAccess.Commands;
using ScreenReaderAccess.Patches;
using System;

namespace ScreenReaderAccess.Observers
{
    public class MakeLetterObserver : IEventObserver<MakeLetterEvent>
    {
        private ICommand<ScreenReaderOutputCommandArgs> outputCommand;

        public MakeLetterObserver(ICommand<ScreenReaderOutputCommandArgs> outputCommand)
        {
            this.outputCommand = outputCommand;
        }

        public void OnEvent(MakeLetterEvent evt)
        {
            outputCommand.Execute(new ScreenReaderOutputCommandArgs
            {
                Message = evt.Letter.Label,
                Interrupt = true,
                Delay = 1000 // a little bit of time before reading so that sounds don't overlap
            });
        }
    }
}
