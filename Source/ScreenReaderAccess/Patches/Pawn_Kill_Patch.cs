using HarmonyLib;
using ScreenReaderAccess.DTOs;

namespace ScreenReaderAccess.Patches
{
    public class PawnKilledEvent
    {
        public PawnInfoDto Pawn { get; }
        public PawnKilledEvent(PawnInfoDto pawn) => Pawn = pawn;
    }

    // Attribute-based Harmony patch for Verse.Pawn.Kill
    [HarmonyPatch(typeof(Verse.Pawn), "Kill")]
    public static class Pawn_Kill_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Verse.Pawn __instance)
        {
            // Convert Verse.Pawn to PawnInfoDto
            var pawnInfo = new PawnInfoDto
            {
                Name = __instance.Name?.ToString(),
                Label = __instance.def?.label
            };
            // Raise event via EventBus
            ScreenReaderAccess.EventBusInstance?.RaiseEvent(new PawnKilledEvent(pawnInfo));
        }
    }
}