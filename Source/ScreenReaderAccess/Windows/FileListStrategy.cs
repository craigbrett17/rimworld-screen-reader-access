using System;
using System.Collections.Generic;
using System.IO;
using HarmonyLib;
using RimWorld;
using ScreenReaderAccess.DTOs;
using ScreenReaderAccess.Patches;
using UnityEngine;
using Verse;

namespace ScreenReaderAccess.Windows
{
    public class FileListStrategy : WindowReadingStrategyBase<Dialog_FileList>
    {
        static string _lastKey;
        static int _lastFrame;

        protected override IEnumerable<Rect> SuppressionRegions(Dialog_FileList w, Rect inRect)
        {
            // compute outRect (list viewport) exactly like vanilla
            var tr = Traverse.Create(w);
            var bottomH = tr.Field("bottomAreaHeight").GetValue<float>();
            bool doTypeIn = tr.Property("ShouldDoTypeInField").GetValue<bool>();

            var searchRect = inRect.LeftHalf(); searchRect.height = 24f;

            var outRect = inRect;
            outRect.yMin = searchRect.yMax + 10f;
            outRect.yMax -= Window.CloseButSize.y + bottomH + 10f;
            if (doTypeIn) outRect.yMax -= 53f;

            yield return outRect; // suppress generic announcements over the list area
        }

        protected override void AnnounceHook(Dialog_FileList w, Rect inRect)
        {
            if (Event.current?.type != EventType.Repaint) return;

            var tr = Traverse.Create(w);
            var files = tr.Field("files").GetValue<List<SaveFileInfo>>();
            var search = tr.Field("search").GetValue<QuickSearchWidget>();
            var scrollPos = tr.Field("scrollPosition").GetValue<Vector2>();
            var bottomH = tr.Field("bottomAreaHeight").GetValue<float>();
            var label = tr.Field("interactButLabel").GetValue<string>();
            bool doTypeIn = tr.Property("ShouldDoTypeInField").GetValue<bool>();
            if (files == null) return;

            var rowSize = new Vector2(inRect.width - 16f, 40f);
            var searchRect = inRect.LeftHalf(); searchRect.height = 24f;

            var outRect = inRect;
            outRect.yMin = searchRect.yMax + 10f;
            outRect.yMax -= Window.CloseButSize.y + bottomH + 10f;
            if (doTypeIn) outRect.yMax -= 53f;

            float y = 0f;
            foreach (var f in files)
            {
                if (!search.filter.Matches(f.FileName)) { y += rowSize.y; continue; }

                if (y + rowSize.y >= scrollPos.y && y <= scrollPos.y + outRect.height)
                {
                    var row = new Rect(0f, y, rowSize.x, rowSize.y);
                    var del = new Rect(row.width - 36f, (row.height - 36f) / 2f, 36f, 36f);
                    var inter = new Rect(del.x - 100f, (row.height - 36f) / 2f, 100f, 36f);

                    Rect ToScreen(Rect r) => new Rect(outRect.x + r.x, outRect.y + (r.y - scrollPos.y), r.width, r.height);

                    var interScreen = ToScreen(new Rect(row.x + inter.x, row.y + inter.y, inter.width, inter.height));
                    var delScreen = ToScreen(new Rect(row.x + del.x, row.y + del.y, del.width, del.height));

                    var name = Path.GetFileNameWithoutExtension(f.FileName);
                    var when = f.LastWriteTime;
                    var ver = f.GameVersion;

                    if (Mouse.IsOver(interScreen))
                    {
                        SpeakOnce($"{Compose("Load", name, when, ver)}", $"act|{name}|{when:O}|{ver}");
                        return;
                    }
                    if (Mouse.IsOver(delScreen))
                    {
                        SpeakOnce($"Delete {name}", $"del|{name}|{when:O}");
                        return;
                    }
                }
                y += rowSize.y;
            }
        }

        static string Compose(string action, string name, DateTime when, string ver)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(action)) parts.Add(action);
            if (!string.IsNullOrWhiteSpace(name)) parts.Add(name);
            if (when != default) parts.Add(when.ToString("yyyy-MM-dd HH:mm"));
            if (!string.IsNullOrWhiteSpace(ver)) parts.Add(ver);
            return parts.Count > 0 ? string.Join(", ", parts) : "Save file";
        }
        
        private static void SpeakOnce(string text, string key)
        {
            if (_lastKey == key && Time.frameCount - _lastFrame <= 15)
                return; // already spoken recently
            _lastKey = key;
            _lastFrame = Time.frameCount;
            ScreenReaderAccess.EventBusInstance?.RaiseEvent(new ButtonHoverEvent(new ButtonDto
            {
                Label = text,
                Active = true // I believe these buttons can't be disabled anyway
            }));
        }
    }
}