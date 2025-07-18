﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using CrossSpeak;
using Verse;

namespace ScreenReaderAccess
{
    public class ScreenReaderAccess : Mod
    {
        public static EventBus EventBusInstance { get; private set; }
        private EventPatcher eventPatcher { get; set; }
        private EventRegistry eventRegistry { get; set; }
        private IScreenReader screenReader { get; set; }

        public ScreenReaderAccess(ModContentPack content) : base(content)
        {
            Log.Message("ScreenReaderAccess mod has been loaded successfully.");

            EventBusInstance = new EventBus();
            eventPatcher = new EventPatcher();

            eventPatcher.ApplyPatches();

            try
            {
                PreloadNativeDlls();
                
                screenReader = CrossSpeakManager.Instance;
                screenReader.Initialize();
                screenReader.TrySAPI(true);
                Log.Message("ScreenReader has been initialized.");
            }
            catch (Exception e)
            {
                Log.Error($"ScreenReader initialization failed: {e.Message}");
                Log.Error(e.StackTrace);
            }

            eventRegistry = new EventRegistry(EventBusInstance, screenReader);
            eventRegistry.RegisterEvents();
        }

        ~ScreenReaderAccess()
        {
            Log.Message("ScreenReaderAccess mod is being unloaded.");
            // unsure if this ever gets called, but as a best attempt we try and close the screen reader if it exists
            if (screenReader?.IsLoaded() == true)
            {
                screenReader.Close();
            }
            Log.Message("ScreenReader has been closed.");
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr LoadLibrary(string lpFileName);

        /// <summary>
        /// Pre-loads the native DLLs we use for screen reader functionality.
        /// Pre-loading them means that when this is used in Rimworld and Unity, the DLLs are loaded into memory before the DLL path is changed
        /// </summary>
        private void PreloadNativeDlls()
        {
            string assemblyPath = Assembly.GetExecutingAssembly().Location;
            string basePath = Path.GetDirectoryName(assemblyPath);
            string nativePath = Path.Combine(basePath, "..", "lib", "screen-reader-libs", "windows");

            string[] dlls = new[]
            {
                "Tolk.dll",
                "nvdaControllerClient64.dll",
                "SAAPI64.dll"
            };

            foreach (var dll in dlls)
            {
                string fullPath = Path.Combine(nativePath, dll);
                IntPtr result = LoadLibrary(fullPath);
                Log.Message($"{dll} preload result: {(result != IntPtr.Zero ? "Success" : "Failed")}");
            }
        }
    }
}
