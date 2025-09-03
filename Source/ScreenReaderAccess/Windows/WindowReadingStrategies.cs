using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace ScreenReaderAccess.Windows
{
    public static class WindowReadingStrategies
    {
        private static readonly List<IWindowReadingStrategy> _all = new List<IWindowReadingStrategy>
        {
            new FileListStrategy()
        };

        public static IWindowReadingStrategy For(Window w)
            => _all.FirstOrDefault(s => s.CanHandle(w));
    }
}
