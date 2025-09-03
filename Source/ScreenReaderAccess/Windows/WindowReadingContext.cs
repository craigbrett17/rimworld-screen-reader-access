using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace ScreenReaderAccess.Windows
{
    public static class WindowReadingContext
    {
        [ThreadStatic] static Stack<FrameCtx> _windowStack; // stack to support nested windows
        [ThreadStatic] static int _frameId; // to detect new frames so we can clear stale context

        // we cannot use records in .NET framework
        struct FrameCtx
        {
            public Window Window;
            public Rect InRect;
            public IWindowReadingStrategy Strategy;
            public IReadOnlyList<Rect> SuppressedRects;
        }

        public static void Begin(Window w, Rect inRect)
        {
            // when a window is having its OnGUI called, we push context for it
            if (_windowStack == null)
                _windowStack = new Stack<FrameCtx>();
                
            if (Time.frameCount != _frameId)
            {
                // new frame; clear any leftover context if a Postfix was skipped
                _windowStack.Clear();
                _frameId = Time.frameCount;
            }

            var strategy = WindowReadingStrategies.For(w);
            var suppressed = strategy?.SuppressionRegions(w, inRect)?.ToList() ?? new List<Rect>();
            _windowStack.Push(new FrameCtx
            {
                Window = w,
                InRect = inRect,
                Strategy = strategy,
                SuppressedRects = suppressed
            });
        }

        public static void End()
        {
            if (_windowStack != null && _windowStack.Count > 0) 
                _windowStack.Pop();
        }

        public static bool ShouldSuppressHandling(Rect r)
        {
            return _windowStack != null && _windowStack.Count > 0 && 
                   _windowStack.Peek().SuppressedRects.Any(s => s.Overlaps(r));
        }

        public static void AnnounceHook()
        {
            if (_windowStack != null && _windowStack.Count > 0)
            {
                var top = _windowStack.Peek();
                top.Strategy?.AnnounceHook(top.Window, top.InRect);
            }
        }
    }
}