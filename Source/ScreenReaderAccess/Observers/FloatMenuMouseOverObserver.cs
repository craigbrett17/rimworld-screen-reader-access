using ScreenReaderAccess.Commands;
using ScreenReaderAccess.Patches;

namespace ScreenReaderAccess.Observers
{
    public class FloatMenuMouseOverObserver : IEventObserver<FloatMenuMouseOverEvent>
    {
        private ICommand<ScreenReaderOutputCommandArgs> outputCommand;

        public FloatMenuMouseOverObserver(ICommand<ScreenReaderOutputCommandArgs> outputCommand)
        {
            this.outputCommand = outputCommand;
        }

        public void OnEvent(FloatMenuMouseOverEvent evt)
        {
            var args = new ScreenReaderOutputCommandArgs
            {
                Message = evt.FloatMenuItem.Label,
                Interrupt = true,
            };
            outputCommand?.Execute(args);
        }
    }
}
