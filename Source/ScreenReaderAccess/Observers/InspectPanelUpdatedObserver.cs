using ScreenReaderAccess.Commands;
using ScreenReaderAccess.Patches;

namespace ScreenReaderAccess.Observers
{
    public class InspectPanelUpdatedObserver : IEventObserver<InspectPanelUpdatedEvent>
    {
        private readonly ICommand<ScreenReaderOutputCommandArgs> outputCommand;
        private string lastLabel = string.Empty;
        private string lastContents = string.Empty;

        public InspectPanelUpdatedObserver(ICommand<ScreenReaderOutputCommandArgs> outputCommand)
        {
            this.outputCommand = outputCommand;
        }

        public void OnEvent(InspectPanelUpdatedEvent evt)
        {
            // Prevent duplicate announcements
            if (evt.InspectPane.Label == lastLabel && evt.InspectPane.Contents == lastContents)
                return;

            lastLabel = evt.InspectPane.Label;
            lastContents = evt.InspectPane.Contents;

            // Build the message with label first, then contents
            string message = evt.InspectPane.Label;
            if (!string.IsNullOrEmpty(evt.InspectPane.Contents))
            {
                message += ". " + evt.InspectPane.Contents;
            }

            var args = new ScreenReaderOutputCommandArgs
            {
                Message = message,
                Interrupt = false,
                Delay = 300 // Small delay to prevent too frequent updates
            };
            outputCommand?.Execute(args);
        }
    }
}