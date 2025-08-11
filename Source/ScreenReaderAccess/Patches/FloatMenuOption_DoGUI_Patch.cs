using HarmonyLib;
using ScreenReaderAccess.DTOs;
using UnityEngine;
using Verse;

namespace ScreenReaderAccess.Patches
{
    public class FloatMenuMouseOverEvent
    {
        public FloatMenuItemDto FloatMenuItem { get; set; }
        public FloatMenuMouseOverEvent(FloatMenuItemDto floatMenuItem) => FloatMenuItem = floatMenuItem;
    }

    [HarmonyPatch(typeof(FloatMenuOption), "DoGUI")]
    public static class FloatMenuOption_DoGUI_Patch
    {
        private static string lastLabel = string.Empty;

        [HarmonyPostfix]
        public static void Postfix(FloatMenuOption __instance, Rect rect)
        {
            if (string.IsNullOrEmpty(__instance.Label))
                return;

            bool isSameLabel = lastLabel == __instance.Label?.Trim();
            bool isMouseOver = Mouse.IsOver(rect);
            if (isSameLabel && !isMouseOver)
            {
                // the user may have moved away from the label, so we should clear the lastLabel
                lastLabel = string.Empty;
                // and we should not raise an event
                return;
            }
            else if (isSameLabel && isMouseOver)
            {
                // the user is still hovering over the same label, so we should not raise an event
                return;
            }
            else if (!isMouseOver)
            {
                // the user is not hovering over the item, so we should not raise an event
                return;
            }

            var label = __instance.Label?.Trim();
            lastLabel = label;

            string tooltip = null;
            if (!string.IsNullOrEmpty(__instance.tooltip?.text))
            {
                tooltip = __instance.tooltip?.text.Trim();
            }

            var floatMenuItem = new FloatMenuItemDto
            {
                Label = label,
                Tooltip = tooltip,
            };

            var mouseOverEvent = new FloatMenuMouseOverEvent(floatMenuItem);
            ScreenReaderAccess.EventBusInstance?.RaiseEvent(mouseOverEvent);
        }
    }
}