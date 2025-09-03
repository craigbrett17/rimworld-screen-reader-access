using HarmonyLib;
using ScreenReaderAccess.Windows;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace ScreenReaderAccess.Patches
{
    static class WindowIntrospector
    {
        // margin per *instance*; cleared automatically when window is GC’d
        private static readonly ConditionalWeakTable<Window, StrongBox<float>> _margins = new ConditionalWeakTable<Window, StrongBox<float>>();

        public static float GetMargin(Window w)
        {
            if (_margins.TryGetValue(w, out var box)) return box.Value;

            float margin;
            try
            {
                // One-time reflection per window instance (fast enough + cached)
                margin = Traverse.Create(w).Property("Margin").GetValue<float>();
            }
            catch
            {
                // Vanilla default is 18f for most windows; safe fallback
                margin = 18f;
            }

            _margins.Add(w, new StrongBox<float>(margin));
            return margin;
        }
    }

    // a special patch that wraps Window.WindowOnGui to set up the window reading context
    // this allows other patches to query the context to decide whether to suppress announcements and leave it to window specific logic
    [HarmonyPatch(typeof(Window), nameof(Window.WindowOnGUI))]
    public static class Window_DoWindowContents_Patch
    {
        [HarmonyPrefix]
        static void Pre(Window __instance)
        {
            var windowRect = __instance.windowRect;
            var inRect = new Rect(0f, 0f, windowRect.width, windowRect.height).ContractedBy(WindowIntrospector.GetMargin(__instance));
            WindowReadingContext.Begin(__instance, inRect);
        }

        [HarmonyPostfix]
        static void Post(Window __instance)
        {
            WindowReadingContext.AnnounceHook();
            WindowReadingContext.End();
        }
    }
}