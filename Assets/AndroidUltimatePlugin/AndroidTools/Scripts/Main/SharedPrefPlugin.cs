using System;
using System.Collections.Generic;
using Gigadrillgames.AUP.Common;
using UnityEngine;

namespace Gigadrillgames.AUP.Tools
{
    public class SharedPrefPlugin : MonoBehaviour
    {
        private static SharedPrefPlugin instance;
        private static GameObject container;
        private const string TAG = "[CustomCameraPlugin]: ";
        private static AUPHolder aupHolder;

#if UNITY_ANDROID
        private static AndroidJavaObject jo;
#endif

        public bool isDebug = true;
        private bool isInit = false;

        public static SharedPrefPlugin GetInstance()
        {
            if (instance == null)
            {
                container = new GameObject();
                container.name = "SharedPrefPlugin";
                instance = container.AddComponent(typeof(SharedPrefPlugin)) as SharedPrefPlugin;
                DontDestroyOnLoad(instance.gameObject);
                aupHolder = AUPHolder.GetInstance();
                instance.gameObject.transform.SetParent(aupHolder.gameObject.transform);
            }

            return instance;
        }

        private void Awake()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                jo = new AndroidJavaObject("com.gigadrillgames.androidplugin.sharedpref.SharedPrefPlugin");
            }
#endif
        }

        /// <summary>
        /// Sets the debug.
        /// 0 - false, 1 - true
        /// </summary>
        /// <param name="debug">Debug.</param>
        public void SetDebug(int debug)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                jo.CallStatic("SetDebug", debug);
                Utils.Message(TAG, "SetDebug");
            }
            else
            {
                Utils.Message(TAG, "warning: must run in actual android device");
            }
#endif
        }

        /// <summary>
        /// initialize the camera plugin
        /// </summary>
        public void Init()
        {
            if (isInit)
            {
                return;
            }

#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                jo.CallStatic("init");
                isInit = true;
                Utils.Message(TAG, "init");
            }
            else
            {
                Utils.Message(TAG, "warning: must run in actual android device");
            }
#endif
        }


        public void SaveString(string sharedPrefname, string dataKey, string value)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                jo.CallStatic("saveString", sharedPrefname, dataKey, value);
                Utils.Message(TAG, "SaveString");
            }
            else
            {
                Utils.Message(TAG, "warning: must run in actual android device");
            }
#endif
        }

        public void SaveInt(string sharedPrefname, string dataKey, int value)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                jo.CallStatic("saveInt", sharedPrefname, dataKey, value);
                Utils.Message(TAG, "SaveInt");
            }
            else
            {
                Utils.Message(TAG, "warning: must run in actual android device");
            }
#endif
        }

        public void SaveArrayString(string sharedPrefname, string dataKey, List<string> value)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                jo.CallStatic("saveArrayString", sharedPrefname, dataKey, value);
                Utils.Message(TAG, "SaveArrayString");
            }
            else
            {
                Utils.Message(TAG, "warning: must run in actual android device");
            }
#endif
        }

        public String LoadString(string sharedPrefname, string dataKey)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                return jo.CallStatic<String>("loadString", sharedPrefname, dataKey);
            }
            else
            {
                Utils.Message(TAG, "warning: must run in actual android device");
            }
#endif

            return "";
        }

        public int LoadInt(string sharedPrefname, string dataKey)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                return jo.CallStatic<int>("loadInt", sharedPrefname, dataKey);
            }
            else
            {
                Utils.Message(TAG, "warning: must run in actual android device");
            }
#endif

            return 0;
        }

        public List<string> loadArrayString(string sharedPrefname, string dataKey)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                return jo.CallStatic<List<string>>("loadArrayString", sharedPrefname, dataKey);
            }
            else
            {
                Utils.Message(TAG, "warning: must run in actual android device");
            }
#endif

            return null;
        }
    }
}