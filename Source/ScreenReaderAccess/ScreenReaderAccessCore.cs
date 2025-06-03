using Verse;

namespace ScreenReaderAccess
{
    public class ScreenReaderAccess : Mod
    {
        public static EventBus EventBusInstance { get; private set; }
        private EventPatcher eventPatcher { get; set; }
        private EventRegistry eventRegistry { get; set; }

        public ScreenReaderAccess(ModContentPack content) : base(content)
        {
            Log.Message("ScreenReaderAccess mod has been loaded successfully.");

            EventBusInstance = new EventBus();
            eventPatcher = new EventPatcher();
            eventRegistry = new EventRegistry(EventBusInstance);

            eventRegistry.RegisterEvents();
            eventPatcher.ApplyPatches();
        }
    }
}