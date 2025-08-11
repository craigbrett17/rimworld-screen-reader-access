using HarmonyLib;
using ScreenReaderAccess.DTOs;
using UnityEngine;
using Verse;

namespace ScreenReaderAccess.Patches
{
    public class GizmoMouseOverEvent
    {
        public GizmoMouseOverEvent(GizmoDto gizmo) => Gizmo = gizmo;
        public GizmoDto Gizmo { get; private set; }
    }
    
    // attribute based Harmony patch for Command.GizmoOnGUI
    // Command tiles (incl. many architect tools via Designator->Command)
    [HarmonyPatch(typeof(Command), "GizmoOnGUI")]
    public static class Command_GizmoOnGUI_Patch
    {
        private static string lastLabel = string.Empty;

        [HarmonyPostfix]
        public static void Postfix(Command __instance, GizmoResult __result)
        {
            if (__instance == null || __instance.Label == lastLabel)
                return; // Avoid duplicate events for the same label
            if (__result.State != GizmoState.Mouseover)
                return; // Only handle mouseover state

            lastLabel = __instance.Label; // Update lastLabel to prevent duplicate events
            string label = (__instance.LabelCap ?? __instance.defaultLabel)?.Trim();
            string desc = __instance.Desc?.Trim();

            if (__instance.Disabled)
            {
                var reason = __instance.disabledReason?.Trim();
                if (!string.IsNullOrEmpty(reason))
                    label = string.IsNullOrEmpty(label) ? reason : $"{label}. {reason}";
            }

            if (string.IsNullOrEmpty(label) && string.IsNullOrEmpty(desc)) return;

            var dto = new GizmoDto
            {
                Label = label,
                Description = desc
            };

            DebugLog.WriteLine($"GizmoMouseOverEvent from Command GizmoOnGUI: Label: {label}, Desc: {desc}");
            ScreenReaderAccess.EventBusInstance?.RaiseEvent(new GizmoMouseOverEvent(dto));
        }
    }
}
