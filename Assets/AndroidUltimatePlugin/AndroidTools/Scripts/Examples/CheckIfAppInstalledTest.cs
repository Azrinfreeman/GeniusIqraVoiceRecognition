using Gigadrillgames.AUP.Common;
using TMPro;
using UnityEngine;

namespace Gigadrillgames.AUP.Tools
{
    public class CheckIfAppInstalledTest : MonoBehaviour
    {
        #region Fields
        private UtilsPlugin _utilsPlugin;
        public TextMeshProUGUI StatusText;
        public TMP_InputField packageInputField;

        #endregion Fields

        #region Methods

        private void Awake()
        {
            _utilsPlugin = UtilsPlugin.GetInstance();
            _utilsPlugin.Init();
            _utilsPlugin.SetDebug(0);
        }

        public void CheckAppInstalled()
        {
#if UNITY_EDITOR
            StatusText?.SetText("CheckAppInstalled, You must call this on actual android mobile device!");
#else
      if (!IsInputEmpty())
            {
                StatusText?.SetText("Checking for: " + packageInputField.text);
                if (_utilsPlugin.IsAppInstalled(packageInputField?.text))
                {
                    StatusText?.SetText("App is installed!");
                }
                else
                {
                    StatusText?.SetText("App is not installed!");
                }
            }
            else
            {
                StatusText?.SetText("please input package name! ex. com.companyname.appname");
            }      
#endif
        }

        public void OpenGooglePlayMarket()
        {
#if UNITY_EDITOR
            StatusText?.SetText("OpenGooglePlayMarket, You must call this on actual android mobile device!");
#else
            if (!IsInputEmpty())
            {
                StatusText?.SetText("Looking for: " + packageInputField.text);
                _utilsPlugin.OpenGooglePlayMarket((packageInputField?.text));
            }else
            {
                StatusText?.SetText("please input package name! ex. com.companyname.appname");
            }
#endif
        }

        private bool IsInputEmpty()
        {
            if (string.IsNullOrEmpty(packageInputField.text))
            {
                return true;
            }

            return false;
        }

        #endregion Methods
    }
}