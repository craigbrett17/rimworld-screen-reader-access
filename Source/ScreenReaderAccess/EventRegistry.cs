using ScreenReaderAccess.Observers;
using ScreenReaderAccess.Commands;

namespace ScreenReaderAccess
{
    public class EventRegistry
    {
        private readonly EventBus eventBus;

        public EventRegistry(EventBus eventBus)
        {
            this.eventBus = eventBus;
        }

        /// <summary>
        /// Registers all events with the EventBus pointing them to their corresponding observers
        /// Observers should be initialised with any necessary dependencies
        /// </summary>
        public void RegisterEvents()
        {
            // Register all events here
            eventBus.RegisterObserver(new PawnKilledObserver(new LogCommand()));
            eventBus.RegisterObserver(new NewMessageObserver(new LogCommand()));
        }
    }
}
