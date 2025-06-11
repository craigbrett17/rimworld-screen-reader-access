using ScreenReaderAccess.Commands;
using ScreenReaderAccess.Patches;

namespace ScreenReaderAccess.Observers
{
    public class TooltipDrawnObserver : IEventObserver<ToolTipDrawnEvent>
    {
        private ICommand<ScreenReaderOutputCommandArgs> outputCommand;
        // to prevent double announcements of the same tooltip text
        private string lastTooltipText = string.Empty;

        public TooltipDrawnObserver(ICommand<ScreenReaderOutputCommandArgs> outputCommand)
        {
            this.outputCommand = outputCommand;
        }

        public void OnEvent(ToolTipDrawnEvent evt)
        {
            

            if (evt.Tooltip.Text == lastTooltipText)
                return;

            lastTooltipText = evt.Tooltip.Text;

            var args = new ScreenReaderOutputCommandArgs
            {
                Message = evt.Tooltip.Text,
                Interrupt = false,
                Delay = 500 // Optional delay in milliseconds
            };
            outputCommand?.Execute(args);
        }
    }
}
