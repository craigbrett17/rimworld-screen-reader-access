using System;
using HarmonyLib;
using RimWorld;
using ScreenReaderAccess.DTOs;
using UnityEngine;
using Verse;

namespace ScreenReaderAccess.Patches
{
    public class InspectPanelUpdatedEvent
    {
        public InspectPanelUpdatedEvent(InspectPaneDto inspectPane) => InspectPane = inspectPane;
        public InspectPaneDto InspectPane { get; }
    }

    // Attribute-based Harmony patch for InspectPaneFiller.DoPaneContentsFor (individual selectables)
    [HarmonyPatch(typeof(InspectPaneFiller), "DoPaneContentsFor")]
    public static class InspectPaneFiller_DoPaneContentsFor_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(ISelectable sel, Rect rect)
        {
            if (sel == null) return;

            string label = "Inspect";
            string contents = string.Empty;

            try
            {
                if (sel is Thing thing)
                {
                    var thingLabel = thing.LabelCap?.ToString();
                    label = "Inspect: " + (string.IsNullOrEmpty(thingLabel) ? "Unknown Item" : thingLabel);
                }
                else
                {
                    // use the Rimworld code to get the correct adjusted label
                    label = InspectPaneUtility.AdjustedLabelFor(Find.Selector.SelectedObjects, rect);
                }
            }
            catch (Exception)
            {
                label = "Inspect: Unknown";
                // output this exception to the log when we set this up
            }

            try
            {
                contents = sel.GetInspectString() ?? string.Empty;
                
                if (sel is Thing thingForInspect)
                {
                    try
                    {
                        string inspectStringLowPriority = thingForInspect.GetInspectStringLowPriority();
                        if (!inspectStringLowPriority.NullOrEmpty())
                        {
                            if (!contents.NullOrEmpty())
                            {
                                contents = contents.TrimEndNewlines() + "\n";
                            }
                            contents += inspectStringLowPriority;
                        }
                    }
                    catch (Exception)
                    {
                        // If low priority string fails, keep the main inspect string
                        // Don't overwrite contents, just log that low priority failed
                        // we will eventually add a logging event that will log these warnings
                    }
                }
            }
            catch (Exception ex)
            {
                contents = $"Inspect information unavailable: {ex.GetType().Name}";
            }

            // Ensure contents is never null
            contents = contents ?? string.Empty;

            // Only raise event if we have meaningful data
            if (!string.IsNullOrEmpty(label))
            {
                var inspectPaneInfo = new InspectPaneDto 
                { 
                    Label = label,
                    Contents = contents
                };
                ScreenReaderAccess.EventBusInstance?.RaiseEvent(new InspectPanelUpdatedEvent(inspectPaneInfo));
            }
        }
    }

    // Attribute-based Harmony patch for InspectPaneFiller.DoPaneContentsForStorageGroup (storage groups)
    [HarmonyPatch(typeof(InspectPaneFiller), "DoPaneContentsForStorageGroup")]
    public static class InspectPaneFiller_DoPaneContentsForStorageGroup_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(StorageGroup group)
        {
            if (group == null) return;

            string label = "Inspect: Storage Group";
            string contents = "Storage group with multiple items";

            try
            {
                var renamableLabel = group.RenamableLabel?.ToString();
                if (!string.IsNullOrEmpty(renamableLabel))
                {
                    label = $"Inspect: {renamableLabel.CapitalizeFirst()}";
                }

                int memberCount = group.MemberCount;
                string storageGroupLabelText = "StorageGroupLabel".Translate();
                string memberCountText;
                
                if (memberCount <= 1)
                {
                    memberCountText = $"({("OneBuilding".Translate())})";
                }
                else
                {
                    memberCountText = $"({("NumBuildings".Translate(memberCount))})";
                }

                contents = $"{storageGroupLabelText}: {renamableLabel?.CapitalizeFirst() ?? "Storage Group"} {memberCountText}";
            }
            catch (Exception)
            {
                // If anything fails, don't output - just return silently
                return;
            }

            var inspectPaneInfo = new InspectPaneDto 
            { 
                Label = label, 
                Contents = contents 
            };
            ScreenReaderAccess.EventBusInstance?.RaiseEvent(new InspectPanelUpdatedEvent(inspectPaneInfo));
        }
    }
}