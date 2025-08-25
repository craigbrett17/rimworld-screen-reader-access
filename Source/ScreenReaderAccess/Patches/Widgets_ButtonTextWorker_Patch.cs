using HarmonyLib;
using RimWorld;
using ScreenReaderAccess.DTOs;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace ScreenReaderAccess.Patches
{
    public class ButtonHoverEvent
    {
        public ButtonHoverEvent(ButtonDto button) => Button = button;
        public ButtonDto Button { get; set; }
    }

    [HarmonyPatch(typeof(Widgets), "ButtonTextWorker")]
    public static class Widgets_ButtonTextWorker_Patch
    {
        private static string lastButtonKey = string.Empty;
        private static List<string> ignoredWindowTypes = new List<string>
        {
            typeof(Dialog_SaveFileList_Load).FullName,
            typeof(Dialog_SaveFileList_Save).FullName
        };

        [HarmonyPostfix]
        public static void Postfix(Rect rect, string label, bool active)
        {
            if (Event.current == null || Event.current.type != EventType.Repaint)
                return; // Only proceed during Repaint events
            if (string.IsNullOrWhiteSpace(label))
                return; // No label to process
            var currentWindow = Find.WindowStack?.currentlyDrawnWindow;
            if (currentWindow != null && ignoredWindowTypes.Contains(currentWindow.GetType().FullName))
                return; // Ignore certain window types

            string key = $"{label.Trim()} ({rect.x:F0},{rect.y:F0})";

            bool isSameButton = lastButtonKey == key;
            bool isMouseOver = Mouse.IsOver(rect);

            if (isSameButton && !isMouseOver)
            {
                // the user may have moved away from the button, so we should clear the lastButtonKey
                lastButtonKey = string.Empty;
                // and we should not raise an event
                return;
            }
            else if (isSameButton && isMouseOver)
            {
                // the user is still hovering over the same button, so we should not raise an event
                return;
            }
            else if (!isMouseOver)
            {
                // the user is not hovering over the item, so we should not raise an event
                return;
            }

            lastButtonKey = key;
            var text = label.Trim();

            ScreenReaderAccess.EventBusInstance?.RaiseEvent(new ButtonHoverEvent(new ButtonDto
            {
                Label = text,
                Active = active
            }));
        }
    }
}