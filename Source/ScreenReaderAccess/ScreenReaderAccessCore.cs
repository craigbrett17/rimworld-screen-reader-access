using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RimWorld;
using Verse;

namespace ScreenReaderAccess
{
    public class ScreenReaderAccess : Mod
    {
        public ScreenReaderAccess(ModContentPack content) : base(content)
        {
            // Initialization code can go here if needed
            // log out to the dev console that the mod has been loaded
            Log.Message("ScreenReaderAccess mod has been loaded successfully.");
        }
    }
}