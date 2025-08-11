using ScreenReaderAccess.Commands;
using ScreenReaderAccess.Patches;

namespace ScreenReaderAccess.Observers
{
    public class GizmoMouseOverObserver : IEventObserver<GizmoMouseOverEvent>
    {
        private readonly ICommand<ScreenReaderOutputCommandArgs> outputCommand;
        private string lastGizmoLabel = string.Empty;

        public GizmoMouseOverObserver(ICommand<ScreenReaderOutputCommandArgs> outputCommand)
        {
            this.outputCommand = outputCommand;
        }

        public void OnEvent(GizmoMouseOverEvent evt)
        {
            if (evt.Gizmo.Label == lastGizmoLabel || string.IsNullOrEmpty(evt.Gizmo.Label))
                return;

            lastGizmoLabel = evt.Gizmo.Label;

            var args = new ScreenReaderOutputCommandArgs
            {
                Message = $"{evt.Gizmo.Label}. {evt.Gizmo.Description}",
                Interrupt = true
            };
            
            outputCommand.Execute(args);
        }
    }
}
