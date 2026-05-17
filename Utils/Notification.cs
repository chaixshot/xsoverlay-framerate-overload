using BepInEx;
using XSOverlay;
using XSOverlay.Websockets.API;

namespace xsoverlay_tweak.Utils
{
    internal class Notification
    {
        public static void Send(string title, string content = "", float timeout = 5f)
        {
            var notif = new Objects.NotificationObject
            {
                title = title,
                content = content,
                messageType = 1,
                timeout = timeout,
                height = CalculateHeight(content),
                sourceApp = MyPluginInfo.PLUGIN_NAME,
                volume = 0.5f
            };

            ThreadingHelper.Instance.StartSyncInvoke(() => XSOEventSystem.Current.EventQueueNotification(notif));
        }

        private static int CalculateHeight(string content)
        {
            return content.Length switch
            {
                <= 100 => 100,
                <= 200 => 150,
                <= 300 => 200,
                _ => 250
            };
        }
    }
}
