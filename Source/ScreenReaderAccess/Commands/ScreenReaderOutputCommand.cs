using System.Text.RegularExpressions;
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
        // Use a static readonly Regex for performance
        private static readonly Regex ColorTagRegex = new Regex(
            @"<color\s*=\s*""#\w{6}""\s*>(.*?)<\/color>",
            RegexOptions.IgnoreCase | RegexOptions.Compiled
        );

        public ScreenReaderOutputCommand(IScreenReader screenReader)
        {
            this.screenReader = screenReader;
        }

        public void Execute(ScreenReaderOutputCommandArgs args)
        {
            if (string.IsNullOrEmpty(args.Message))
            {
                return; // No message to output
            }

            var message = SanitizeMessage(args.Message);
            if (args.Delay.HasValue)
            {
                Task.Delay(args.Delay.Value).ContinueWith(_ => screenReader.Output(message, args.Interrupt));
            }
            else
            {
                screenReader.Output(message, args.Interrupt);
            }
        }

        private string SanitizeMessage(string message)
        {
            return ColorTagRegex.Replace(message, "$1");
        }
    }
}