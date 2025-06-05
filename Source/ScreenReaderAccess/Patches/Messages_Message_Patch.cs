using HarmonyLib;
using ScreenReaderAccess.DTOs;

namespace ScreenReaderAccess.Patches
{
    public class MessageEvent
    {
        public MessageEvent(MessageDto message) => Message = message;
        public MessageDto Message { get; }
    }

    // Attribute-based Harmony patch for Messages.Message
    [HarmonyPatch(typeof(Verse.Messages), "Message", typeof(Verse.Message), typeof(bool))]
    public static class Messages_Message_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(Verse.Message msg, bool historical)
        {
            var messageInfo = new MessageDto { Text = msg.text };
            ScreenReaderAccess.EventBusInstance?.RaiseEvent(new MessageEvent(messageInfo));
        }
    }
}