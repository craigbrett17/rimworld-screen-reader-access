using RimWorld;
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
        [ThreadStatic] static HashSet<Window> _windowsThisFrame; // track which windows we've already pushed context for this frame
        [ThreadStatic] static int _debugShouldSuppressLogCount = 0;
        [ThreadStatic] static int _debugBeginEndLogCount = 0;

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
            if (_windowsThisFrame == null)
                _windowsThisFrame = new HashSet<Window>();
                
            if (Time.frameCount != _frameId)
            {
                // new frame; clear any leftover context from previous frame
                if (_windowStack.Count > 0)
                {
                    if (_debugBeginEndLogCount < 5)
                    {
                        DebugLog.WriteLine($"WindowReadingContext - New frame detected, clearing {_windowStack.Count} leftover contexts", LoggingLevel.Debug); 
                    }
                    _windowStack.Clear();
                }
                _windowsThisFrame.Clear();
                _frameId = Time.frameCount;
            }

            // Only push context once per window per frame to avoid multiple GUI passes stacking
            if (_windowsThisFrame.Contains(w))
            {
                if (_debugBeginEndLogCount < 5)
                {
                    DebugLog.WriteLine($"WindowReadingContext - Skipping duplicate context push for window: {w}", LoggingLevel.Debug); 
                }
                return;
            }
            
            _windowsThisFrame.Add(w);
            var strategy = WindowReadingStrategies.For(w);
            var suppressed = strategy?.SuppressionRegions(w, inRect)?.ToList() ?? new List<Rect>();
            _windowStack.Push(new FrameCtx
            {
                Window = w,
                InRect = inRect,
                Strategy = strategy,
                SuppressedRects = suppressed
            });
            if (_debugBeginEndLogCount < 5)
            {
                DebugLog.WriteLine($"WindowReadingContext - Pushed context for window: {w}. Suppression regions: {string.Join(", ", suppressed)}", LoggingLevel.Debug); 
                _debugBeginEndLogCount++;
            }
        }

        public static void End()
        {
            // Don't pop immediately - defer cleanup to happen after repaint events
            // The stack will be cleared on next frame or when a new window context begins
            if (_debugBeginEndLogCount < 5)
            {
                DebugLog.WriteLine($"WindowReadingContext - End() called but deferring cleanup. Stack depth: {(_windowStack?.Count ?? 0)}", LoggingLevel.Debug); 
                _debugBeginEndLogCount++;
            }
        }

        public static bool ShouldSuppressHandling(Rect r)
        {
            if (_windowStack == null || _windowStack.Count == 0)
                return false;

            var ctx = _windowStack.Peek();
            
            // Transform button rect to screen coordinates to match suppression regions
            var screenRect = GUIUtility.GUIToScreenRect(r);
            var windowRect = ctx.Window.windowRect;
            
            // Convert screen coordinates back to window-relative coordinates
            var windowRelativeRect = new Rect(
                screenRect.x - windowRect.x,
                screenRect.y - windowRect.y,
                screenRect.width,
                screenRect.height
            );
            
            bool shouldSuppress = ctx.SuppressedRects.Any(s => s.Overlaps(windowRelativeRect));
            
            // Debug logging for first few instances, focusing on Dialog_FileList windows
            if (_debugShouldSuppressLogCount < 5 && ctx.Window is Dialog_FileList)
            {
                DebugLog.WriteLine($"WindowReadingContext - Original rect: {r}, Screen rect: {screenRect}, Window-relative: {windowRelativeRect}", LoggingLevel.Debug);
                DebugLog.WriteLine($"  - Window rect: {windowRect}", LoggingLevel.Debug);
                foreach (var suppressedRect in ctx.SuppressedRects)
                {
                    bool overlaps = suppressedRect.Overlaps(windowRelativeRect);
                    DebugLog.WriteLine($"  - Suppressed region: {suppressedRect}, Overlaps with window-relative rect: {overlaps}", LoggingLevel.Debug);
                }
                DebugLog.WriteLine($"  - Final result: ShouldSuppress = {shouldSuppress}", LoggingLevel.Debug);
                _debugShouldSuppressLogCount++;
            }
            else if (!(ctx.Window is Dialog_FileList))
            {
                // Additional logging for non-Dialog_FileList windows
                DebugLog.WriteLine($"WindowReadingContext - Non-Dialog_FileList window: {ctx.Window}. Of type {ctx.Window.GetType().Name}", LoggingLevel.Debug);
            }

                return shouldSuppress;
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