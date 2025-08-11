using Verse;

namespace ScreenReaderAccess
{
    public enum LoggingLevel
    {
        Debug,
        Info,
        Warning,
        Error
    }

    // used for debugging patches and things outside the normal event flow
    public static class DebugLog
    {
        public static void WriteLine(string message, LoggingLevel level = LoggingLevel.Info)
        {
            Log.Message($"[{level.ToString().ToUpper()}]: {message}");
        }
    }
}
