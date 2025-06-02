using HarmonyLib;

namespace ScreenReaderAccess
{
    // EventPatcher: applies Harmony patches
    public class EventPatcher
    {
        private readonly Harmony harmony;
        private const string HarmonyId = "com.screenreaderaccess.patch";

        public EventPatcher()
        {
            harmony = new Harmony(HarmonyId);
        }

        public void ApplyPatches()
        {
            // Attribute-based patching: Patch all classes in this assembly with HarmonyPatch attributes
            harmony.PatchAll();
        }

        public void RemovePatches()
        {
            harmony.UnpatchAll(HarmonyId);
        }
    }

}