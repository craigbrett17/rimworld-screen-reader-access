using Verse;

namespace ScreenReaderAccess
{
    public class ScreenReaderAccess : Mod
    {
        public static EventBus EventBusInstance { get; private set; }
        private EventPatcher EventPatcherInstance { get; set; }
        private EventRegistry EventRegistryInstance { get; set; }

        public ScreenReaderAccess(ModContentPack content) : base(content)
        {
            Log.Message("ScreenReaderAccess mod has been loaded successfully.");
            // Initialize EventBus and EventPatcher
            EventBusInstance = new EventBus();
            EventPatcherInstance = new EventPatcher();
            EventRegistryInstance = new EventRegistry(EventBusInstance);

            EventRegistryInstance.RegisterEvents();
            EventPatcherInstance.ApplyPatches();
        }
    }
}