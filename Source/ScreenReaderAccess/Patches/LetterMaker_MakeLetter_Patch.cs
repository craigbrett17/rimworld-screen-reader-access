using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using ScreenReaderAccess.DTOs;
using Verse;

namespace ScreenReaderAccess.Patches
{
    public class MakeLetterEvent
    {
        public MakeLetterEvent(LetterDto letter) => Letter = letter;
        public LetterDto Letter { get; }
    }

    // Attribute-based Harmony patch for LetterMaker.MakeLetter (with LetterDef parameter)
    [HarmonyPatch(typeof(Verse.LetterMaker), "MakeLetter", typeof(Verse.LetterDef))]
    public class LetterMaker_MakeLetter_LetterDef_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Verse.Letter __result)
        {
            if (__result != null)
            {
                var letterInfo = new LetterDto { Label = __result.Label.ToString() };
                ScreenReaderAccess.EventBusInstance?.RaiseEvent(new MakeLetterEvent(letterInfo));
            }
        }
    }
}