using ScreenReaderAccess.Windows;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ScreenReaderAccess.Windows
{
    public abstract class WindowReadingStrategyBase<T> : IWindowReadingStrategy
        where T : Window
    {
        public bool CanHandle(Window w) => w is T t && CanHandle(t);
        protected virtual bool CanHandle(T w) => true;

        public IEnumerable<Rect> SuppressionRegions(Window w, Rect inRect)
            => w is T t ? SuppressionRegions(t, inRect) : Array.Empty<Rect>();
        protected virtual IEnumerable<Rect> SuppressionRegions(T w, Rect inRect)
            => Array.Empty<Rect>();

        public void AnnounceHook(Window w, Rect inRect)
        {
            if (w is T t) AnnounceHook(t, inRect);
        }
        protected abstract void AnnounceHook(T w, Rect inRect);
    }

}