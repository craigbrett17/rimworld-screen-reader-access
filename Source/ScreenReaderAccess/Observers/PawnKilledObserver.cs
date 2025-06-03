using ScreenReaderAccess.Patches;
using ScreenReaderAccess.Commands;
using ScreenReaderAccess.DTOs;

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
            var msg = $"Pawn killed: {evt.Pawn?.Name} ({evt.Pawn?.Label})";
            logCommand.Execute(new LogCommandArgs { Message = msg });
        }
    }
}
