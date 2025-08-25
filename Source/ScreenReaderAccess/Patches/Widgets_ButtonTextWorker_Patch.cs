using HarmonyLib;
using ScreenReaderAccess.DTOs;
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
        private static string lastLabel = string.Empty;

        [HarmonyPostfix]
        public static void Postfix(Rect rect, string label, bool active)
        {
            if (string.IsNullOrWhiteSpace(label)) return;

            bool isSameLabel = lastLabel == label?.Trim();
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

            var text = label.Trim();
            lastLabel = text;

            ScreenReaderAccess.EventBusInstance?.RaiseEvent(new ButtonHoverEvent(new ButtonDto
            {
                Label = text,
                Active = active
            }));
        }
    }
}