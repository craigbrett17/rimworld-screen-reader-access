using CrossSpeak;
using Verse;

namespace ScreenReaderAccess
{
    public class ScreenReaderAccess : Mod
    {
        public static EventBus EventBusInstance { get; private set; }
        private EventPatcher eventPatcher { get; set; }
        private EventRegistry eventRegistry { get; set; }
        private IScreenReader screenReader { get; set; }

        public ScreenReaderAccess(ModContentPack content) : base(content)
        {
            Log.Message("ScreenReaderAccess mod has been loaded successfully.");

            EventBusInstance = new EventBus();
            eventPatcher = new EventPatcher();
            screenReader = CrossSpeakManager.Instance;

            eventRegistry.RegisterEvents();
            eventPatcher.ApplyPatches();
            screenReader.Initialize();
            Log.Message("ScreenReader has been initialized.");

            eventRegistry = new EventRegistry(EventBusInstance, screenReader);
        }

        ~ScreenReaderAccess()
        {
            Log.Message("ScreenReaderAccess mod is being unloaded.");
            // unsure if this ever gets called, but as a best attempt we try and close the screen reader if it exists
            if (screenReader?.IsLoaded() == true)
            {
                screenReader.Close();
            }
            Log.Message("ScreenReader has been closed.");
        }
    }
}