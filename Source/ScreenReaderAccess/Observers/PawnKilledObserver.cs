using ScreenReaderAccess.Patches;
using Verse;

namespace ScreenReaderAccess.Observers
{
    public class PawnKilledObserver : IEventObserver<PawnKilledEvent>
    {
        public void OnEvent(PawnKilledEvent evt)
        {
            // simply log the event
            Log.Message($"Pawn killed: {evt.Pawn?.Name} ({evt.Pawn?.def?.label})");
        }
    }
}
