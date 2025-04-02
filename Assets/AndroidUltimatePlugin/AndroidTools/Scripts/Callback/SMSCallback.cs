using System;
using UnityEngine;

namespace Gigadrillgames.AUP.Tools
{
    public class SMSCallback : AndroidJavaProxy
    {
        public Action onSMSComplete;
        public Action onSMSFail;
        public Action<string, string, string> onReceivedSMS;

        public SMSCallback() : base("com.gigadrillgames.androidplugin.sms.ISMSCallback")
        {
        }

        void SendSMSComplete()
        {
            onSMSComplete();
        }

        void SendSMSFail()
        {
            onSMSFail();
        }

        void ReceivedSMS(String sender, String message, String all)
        {
            onReceivedSMS(sender, message, all);
        }
    }
}