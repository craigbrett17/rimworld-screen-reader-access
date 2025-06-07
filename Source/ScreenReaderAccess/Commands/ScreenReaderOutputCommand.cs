using CrossSpeak;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenReaderAccess.Commands
{
    public class ScreenReaderOutputCommandArgs
    {
        public string Message { get; set; }
        public bool Interrupt { get; set; }
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
            screenReader.Output(args.Message, args.Interrupt);
        }
    }
}
