using Verse;

namespace ScreenReaderAccess.Commands
{
    public class LogCommandArgs
    {
        public string Message { get; set; }
    }

    public class LogCommand : ICommand<LogCommandArgs>
    {
        public void Execute(LogCommandArgs args)
        {
            Log.Message(args.Message);
        }
    }
}
