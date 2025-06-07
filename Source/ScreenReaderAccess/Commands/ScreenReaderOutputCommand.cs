using System.Threading.Tasks;
using CrossSpeak;

namespace ScreenReaderAccess.Commands
{
    public class ScreenReaderOutputCommandArgs
    {
        public string Message { get; set; }
        public bool Interrupt { get; set; }
        public int? Delay { get; set; }
    }

    public class ScreenReaderOutputCommand : ICommand<ScreenReaderOutputCommandArgs>
    {
        private readonly IScreenReader screenReader;

        public ScreenReaderOutputCommand(IScreenReader screenReader)
        {
            this.screenReader = screenReader;
        }

        public void Execute(ScreenReaderOutputCommandArgs args)
        {
            if (args.Delay.HasValue)
            {
                Task.Delay(args.Delay.Value).ContinueWith(_ => screenReader.Output(args.Message, args.Interrupt));
            }
            else
            {
                screenReader.Output(args.Message, args.Interrupt);
            }
        }
    }
}