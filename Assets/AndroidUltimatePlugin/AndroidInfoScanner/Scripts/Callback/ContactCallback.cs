using System;
using UnityEngine;

namespace Gigadrillgames.AUP.Information
{
    public class ContactCallback : AndroidJavaProxy
    {
        public Action<string, string> onGetContactComplete;
        public Action onGetContactFail;

        public ContactCallback() : base("com.gigadrillgames.androidplugin.contactinfo.IContactCallback")
        {
        }


        void GetContactComplete(String contactName, String contacPhoneNumber)
        {
            onGetContactComplete(contactName, contacPhoneNumber);
        }

        void GetContactFail()
        {
            onGetContactFail();
        }
    }
}