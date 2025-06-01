using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace ScreenReaderAccess
{
    public class ScreenReaderAccess : Mod
    {
        public static EventBus EventBusInstance { get; private set; }
        public static EventPatcher EventPatcherInstance { get; private set; }

        public ScreenReaderAccess(ModContentPack content) : base(content)
        {
            Log.Message("ScreenReaderAccess mod has been loaded successfully.");
            // Initialize EventBus and EventPatcher
            EventBusInstance = new EventBus();
            EventPatcherInstance = new EventPatcher();
            EventPatcherInstance.ApplyPatches();
        }
    }
}