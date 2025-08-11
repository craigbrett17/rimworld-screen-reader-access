using System;
using System.Linq;
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

            bool isSameLabel = evt.InspectPane.Label == lastLabel;
            string previousLastContents = lastContents;
            lastLabel = evt.InspectPane.Label;
            lastContents = evt.InspectPane.Contents;

            string message = string.Empty;
            if (isSameLabel)
            {
                // only announce lines of the contents that have changed
                var previousLines = previousLastContents.Split('\n') ?? Array.Empty<string>();
                var newLines = evt.InspectPane.Contents.Split('\n') ?? Array.Empty<string>();
                var changedLines = newLines.Where((line, index) => !string.IsNullOrEmpty(line)
                        && line != previousLines.ElementAtOrDefault(index)).ToArray();
                
                message = string.Join("\n", changedLines);
            }
            else
            {
                message = evt.InspectPane.Label;
                if (!string.IsNullOrEmpty(evt.InspectPane.Contents))
                {
                    message += ". " + evt.InspectPane.Contents;
                }
            }

            if (string.IsNullOrEmpty(message))
                return;

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