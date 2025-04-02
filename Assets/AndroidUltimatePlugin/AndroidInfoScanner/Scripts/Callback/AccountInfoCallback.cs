using System;
using UnityEngine;

namespace Gigadrillgames.AUP.Information
{
    public class AccountInfoCallback : AndroidJavaProxy
    {
        public Action<string, string> onGetAccountComplete;
        public Action onGetAccountFail;

        public AccountInfoCallback() : base("com.gigadrillgames.androidplugin.accountinfo.IAccountCallback")
        {
        }

        void GetAccountComplete(String emailSet, String accountInfo)
        {
            onGetAccountComplete(emailSet, accountInfo);
        }

        void GetAccountFail()
        {
            onGetAccountFail();
        }
    }
}