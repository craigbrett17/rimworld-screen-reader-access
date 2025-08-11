using ScreenReaderAccess.Observers;
using ScreenReaderAccess.Commands;
using CrossSpeak;

namespace ScreenReaderAccess
{
    public class EventRegistry
    {
        private readonly EventBus eventBus;
        private readonly IScreenReader screenReader;

        public EventRegistry(EventBus eventBus, CrossSpeak.IScreenReader screenReader)
        {
            this.eventBus = eventBus;
            this.screenReader = screenReader;
        }

        /// <summary>
        /// Registers all events with the EventBus pointing them to their corresponding observers
        /// Observers should be initialised with any necessary dependencies
        /// </summary>
        public void RegisterEvents()
        {
            // Register all events here
            eventBus.RegisterObserver(new PawnKilledObserver(new LogCommand()));

            var screenReaderOutputCommand = new ScreenReaderOutputCommand(screenReader);
            eventBus.RegisterObserver(new NewMessageObserver(screenReaderOutputCommand));
            eventBus.RegisterObserver(new MakeLetterObserver(screenReaderOutputCommand));
            eventBus.RegisterObserver(new TooltipDrawnObserver(screenReaderOutputCommand));
            eventBus.RegisterObserver(new InspectPanelUpdatedObserver(screenReaderOutputCommand));
            eventBus.RegisterObserver(new GizmoMouseOverObserver(screenReaderOutputCommand));
        }
    }
}
