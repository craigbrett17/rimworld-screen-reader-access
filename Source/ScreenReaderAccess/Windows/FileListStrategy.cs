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
        private static string lastAnnouncedKey;
        private static readonly FileListCoordinateCalculator coordinateCalculator = new FileListCoordinateCalculator();
        private static readonly FileListMouseDetector mouseDetector = new FileListMouseDetector();

        protected override IEnumerable<Rect> SuppressionRegions(Dialog_FileList window, Rect inRect)
        {
            var layout = FileListLayout.Calculate(window, inRect);
            yield return layout.ListViewport;
        }

        protected override void AnnounceHook(Dialog_FileList window, Rect inRect)
        {
            if (Event.current?.type != EventType.Repaint) return;

            var context = FileListContext.Create(window, inRect);
            if (context.Files == null) return;

            var layout = FileListLayout.Calculate(window, inRect);
            var mousePos = Event.current.mousePosition;

            foreach (var file in context.Files)
            {
                if (!ShouldProcessFile(file, context, layout))
                    continue;
                if (!IsFileVisible(file, context, layout))
                    continue;

                var buttons = coordinateCalculator.CalculateButtonPositions(file, context, layout, window);
                var mouseOverResult = mouseDetector.DetectMouseOver(buttons, mousePos);

                if (mouseOverResult.IsMouseOver)
                {
                    AnnounceFileAction(file, mouseOverResult.ButtonType);
                    return;
                }
            }

            // Mouse not over any actionable item - reset to allow re-announcement
            lastAnnouncedKey = null;
        }

        private static bool ShouldProcessFile(SaveFileInfo file, FileListContext context, FileListLayout layout)
        {
            // QuickSearchWidget has a 'filter' property that can match filenames
            return context.SearchFilter.filter.Matches(file.FileName);
        }

        private static bool IsFileVisible(SaveFileInfo file, FileListContext context, FileListLayout layout)
        {
            var fileIndex = context.Files.IndexOf(file);
            var fileY = fileIndex * layout.RowHeight;
            return fileY + layout.RowHeight >= context.ScrollPosition.y &&
                   fileY <= context.ScrollPosition.y + layout.ListViewport.height;
        }

        private static void AnnounceFileAction(SaveFileInfo file, FileButtonType buttonType)
        {
            var fileName = Path.GetFileNameWithoutExtension(file.FileName);
            var announcement = CreateAnnouncement(buttonType, fileName, file.LastWriteTime, file.GameVersion);
            var announcementKey = CreateAnnouncementKey(buttonType, fileName, file.LastWriteTime);

            SpeakOnce(announcement, announcementKey);
        }

        private static string CreateAnnouncement(FileButtonType buttonType, string fileName, DateTime lastWriteTime, string gameVersion)
        {
            switch (buttonType)
            {
                case FileButtonType.Load:
                    return ComposeLoadAnnouncement("Load", fileName, lastWriteTime, gameVersion);
                case FileButtonType.Delete:
                    return $"Delete {fileName}";
                default:
                    return fileName;
            }
        }

        private static string ComposeLoadAnnouncement(string action, string name, DateTime when, string version)
        {
            var parts = new List<string>();
            if (!string.IsNullOrWhiteSpace(action)) parts.Add(action);
            if (!string.IsNullOrWhiteSpace(name)) parts.Add(name);
            if (when != default) parts.Add(when.ToString("yyyy-MM-dd HH:mm"));
            if (!string.IsNullOrWhiteSpace(version)) parts.Add(version);
            return parts.Count > 0 ? string.Join(", ", parts) : "Save file";
        }

        private static string CreateAnnouncementKey(FileButtonType buttonType, string fileName, DateTime lastWriteTime)
        {
            return $"{buttonType}|{fileName}|{lastWriteTime:O}";
        }

        private static void SpeakOnce(string text, string key)
        {
            if (lastAnnouncedKey == key) return;

            lastAnnouncedKey = key;
            ScreenReaderAccess.EventBusInstance?.RaiseEvent(new ButtonHoverEvent(new ButtonDto
            {
                Label = text,
                Active = true
            }));
        }

        private enum FileButtonType
        {
            None,
            Load,
            Delete
        }

        private class FileListContext
        {
            public List<SaveFileInfo> Files { get; set; }
            public QuickSearchWidget SearchFilter { get; set; }
            public Vector2 ScrollPosition { get; set; }

            public static FileListContext Create(Dialog_FileList window, Rect inRect)
            {
                var traverse = Traverse.Create(window);
                return new FileListContext
                {
                    Files = traverse.Field("files").GetValue<List<SaveFileInfo>>(),
                    SearchFilter = traverse.Field("search").GetValue<QuickSearchWidget>(),
                    ScrollPosition = traverse.Field("scrollPosition").GetValue<Vector2>()
                };
            }
        }

        private class FileListLayout
        {
            public Rect ListViewport { get; set; }
            public Rect SearchRect { get; set; }
            public float RowHeight { get; set; }
            public Vector2 RowSize { get; set; }

            public static FileListLayout Calculate(Dialog_FileList window, Rect inRect)
            {
                var traverse = Traverse.Create(window);
                var bottomHeight = traverse.Field("bottomAreaHeight").GetValue<float>();
                var doTypeIn = traverse.Property("ShouldDoTypeInField").GetValue<bool>();

                var searchRect = inRect.LeftHalf();
                searchRect.height = 24f;

                var listViewport = inRect;
                listViewport.yMin = searchRect.yMax + 10f;
                listViewport.yMax -= Window.CloseButSize.y + bottomHeight + 10f;
                if (doTypeIn) listViewport.yMax -= 53f;

                var rowSize = new Vector2(inRect.width - 16f, 40f);

                return new FileListLayout
                {
                    ListViewport = listViewport,
                    SearchRect = searchRect,
                    RowHeight = rowSize.y,
                    RowSize = rowSize
                };
            }
        }

        private class FileButtonPositions
        {
            public Rect LoadButton { get; set; }
            public Rect DeleteButton { get; set; }
        }

        private class MouseOverResult
        {
            public bool IsMouseOver { get; set; }
            public FileButtonType ButtonType { get; set; }
        }

        private class FileListCoordinateCalculator
        {
            public FileButtonPositions CalculateButtonPositions(SaveFileInfo file, FileListContext context, FileListLayout layout, Dialog_FileList window)
            {
                var fileIndex = context.Files.IndexOf(file);
                var fileY = fileIndex * layout.RowHeight;

                var deleteButtonRect = new Rect(layout.RowSize.x - 36f, (layout.RowHeight - 36f) / 2f, 36f, 36f);
                var loadButtonRect = new Rect(deleteButtonRect.x - 100f, (layout.RowHeight - 36f) / 2f, 100f, 36f);

                return new FileButtonPositions
                {
                    LoadButton = ConvertToAbsoluteGUICoordinates(loadButtonRect, fileY, layout, window, context),
                    DeleteButton = ConvertToAbsoluteGUICoordinates(deleteButtonRect, fileY, layout, window, context)
                };
            }

            private Rect ConvertToAbsoluteGUICoordinates(Rect buttonRect, float fileY, FileListLayout layout, Dialog_FileList window, FileListContext context)
            {
                // Convert to window-relative coordinates, accounting for scroll position
                var windowRelativeRect = new Rect(
                    layout.ListViewport.x + buttonRect.x,
                    layout.ListViewport.y + (fileY - context.ScrollPosition.y),
                    buttonRect.width,
                    buttonRect.height
                );

                // Convert to absolute GUI coordinates
                var screenCoordinates = new Rect(
                    window.windowRect.x + windowRelativeRect.x,
                    window.windowRect.y + windowRelativeRect.y,
                    windowRelativeRect.width,
                    windowRelativeRect.height
                );

                return GUIUtility.ScreenToGUIRect(screenCoordinates);
            }
        }

        private class FileListMouseDetector
        {
            public MouseOverResult DetectMouseOver(FileButtonPositions buttons, Vector2 mousePosition)
            {
                if (buttons.LoadButton.Contains(mousePosition))
                {
                    return new MouseOverResult { IsMouseOver = true, ButtonType = FileButtonType.Load };
                }

                if (buttons.DeleteButton.Contains(mousePosition))
                {
                    return new MouseOverResult { IsMouseOver = true, ButtonType = FileButtonType.Delete };
                }

                return new MouseOverResult { IsMouseOver = false, ButtonType = FileButtonType.None };
            }
        }
    }
}