using ScreenReaderAccess.Commands;
using ScreenReaderAccess.Patches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenReaderAccess.Observers
{
    public class ButtonHoverObserver : IEventObserver<ButtonHoverEvent>
    {
        private ICommand<ScreenReaderOutputCommandArgs> outputCommand;

        public ButtonHoverObserver(ICommand<ScreenReaderOutputCommandArgs> outputCommand)
        {
            this.outputCommand = outputCommand;
        }

        public void OnEvent(ButtonHoverEvent evt)
        {
            if (string.IsNullOrEmpty(evt.Button?.Label))
                return;

            var text = evt.Button.Label;
            if (!evt.Button.Active)
                text = $"{text}, disabled";

            var args = new ScreenReaderOutputCommandArgs
            {
                Message = text,
                Interrupt = true,
            };
            outputCommand?.Execute(args);
        }
    }
}