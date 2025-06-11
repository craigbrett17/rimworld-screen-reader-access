using HarmonyLib;
using ScreenReaderAccess.DTOs;
using Verse;

namespace ScreenReaderAccess.Patches
{
    public class ToolTipDrawnEvent
    {
        public ToolTipDrawnEvent(TooltipDto tooltip) => Tooltip = tooltip;
        public TooltipDto Tooltip { get; private set;  }
    }

    // Attribute-based Harmony patch for ActiveTip.DrawTooltip
    [HarmonyPatch(typeof(ActiveTip), "DrawTooltip")]
    public class ActiveTip_DrawTooltip_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ActiveTip __instance)
        {
            var tooltipText = __instance.signal.text;
            var tooltipInfo = new TooltipDto { Text = tooltipText.ToString() };
            ScreenReaderAccess.EventBusInstance?.RaiseEvent(new ToolTipDrawnEvent(tooltipInfo));
        }
    }
}
