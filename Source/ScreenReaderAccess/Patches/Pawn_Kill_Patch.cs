using HarmonyLib;

namespace ScreenReaderAccess.Patches
{
    public class PawnKilledEvent
    {
        public Verse.Pawn Pawn { get; }
        public PawnKilledEvent(Verse.Pawn pawn) => Pawn = pawn;
    }

    // Attribute-based Harmony patch for Verse.Pawn.Kill
    [HarmonyPatch(typeof(Verse.Pawn), "Kill")]
    public static class Pawn_Kill_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Verse.Pawn __instance)
        {
            // Raise event via EventBus
            ScreenReaderAccess.EventBusInstance?.RaiseEvent(new PawnKilledEvent(__instance));
        }
    }
}