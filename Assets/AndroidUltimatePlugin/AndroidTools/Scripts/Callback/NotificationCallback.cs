using System;
using UnityEngine;

namespace Gigadrillgames.AUP.Tools
{
    public class NotificationCallback : AndroidJavaProxy
    {
        public NotificationCallback() : base("com.gigadrillgames.androidplugin.notification.INotificationCallback")
        {
        }

        public Action onNotificationReady;
        public Action<string> onNotificationLoadComplete;
        public Action onNotificationLoadFail;


        void NotificationLoadComplete(string notifications)
        {
            onNotificationLoadComplete(notifications);
        }

        void NotificationLoadFail()
        {
            onNotificationLoadFail();
        }

        void NotificationReady()
        {
            onNotificationReady();
        }
    }
}