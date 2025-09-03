using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ScreenReaderAccess.Windows
{
    public interface IWindowReadingStrategy
    {
        bool CanHandle(Window w);

        IEnumerable<Rect> SuppressionRegions(Window w, Rect inRect);

        void AnnounceHook(Window w, Rect inRect);
    }
}
