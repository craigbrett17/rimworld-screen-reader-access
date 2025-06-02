using ScreenReaderAccess.Patches;
using ScreenReaderAccess.Commands;

namespace ScreenReaderAccess.Observers
{
    public class PawnKilledObserver : IEventObserver<PawnKilledEvent>
    {
        private readonly ICommand<LogCommandArgs> logCommand;

        public PawnKilledObserver(ICommand<LogCommandArgs> logCommand)
        {
            this.logCommand = logCommand;
        }

        public void OnEvent(PawnKilledEvent evt)
        {
            var msg = $"Pawn killed: {evt.Pawn?.Name} ({evt.Pawn?.def?.label})";
            logCommand.Execute(new LogCommandArgs { Message = msg });
        }
    }
}
