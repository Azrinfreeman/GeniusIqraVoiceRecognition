using System;
using Gigadrillgames.AUP.Common;
using UnityEngine;

namespace Gigadrillgames.AUP.Tools
{
    public class FilePickerPlugin : MonoBehaviour
    {
        private static FilePickerPlugin instance;
        private static GameObject container;
        private const string TAG = "[FilePickerPlugin]: ";
        private static AUPHolder aupHolder;

        private Action<string,string> HandleGetFilePath;

        public event Action<string,string> OnHandleGetFilePath
        {
            add { HandleGetFilePath += value; }
            remove { HandleGetFilePath += value; }
        }

#if UNITY_ANDROID
        private static AndroidJavaObject jo;
#endif

        public bool isDebug = true;
        private bool isInit = false;

        public static FilePickerPlugin GetInstance()
        {
            if (instance == null)
            {
                container = new GameObject();
                container.name = "FilePickerPlugin";
                instance = container.AddComponent(typeof(FilePickerPlugin)) as FilePickerPlugin;
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
                jo = new AndroidJavaObject("com.gigadrillgames.androidplugin.filepicker.FilePickerPlugin");
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
        /// initialize the Image Picker Plugin
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
                AndroidJavaClass jc = new AndroidJavaClass("com.unity3d.player.UnityPlayer"); 
                AndroidJavaObject currentActivity = jc.GetStatic<AndroidJavaObject>("currentActivity");
                
                FilePickerCallback filePickerCallback = new FilePickerCallback();
                filePickerCallback.onHandleGetFilePath = onHandleGetFilePath;
                jo.CallStatic("init",currentActivity,filePickerCallback);
                isInit = true;
                Utils.Message(TAG, "init");
            }
            else
            {
                Utils.Message(TAG, "warning: must run in actual android device");
            }
#endif
        }

       
        
        /// <summary>
        /// Gets Image File path.
        /// start activity to pick audio filepath
        /// </summary>
        public void GetImageFile()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                jo.CallStatic("getImageFile");
                Utils.Message(TAG, "getImageFile");
            }
            else
            {
                Utils.Message(TAG, "warning: must run in actual android device");
            }
#endif
        }

        /// <summary>
        /// Gets Audio File path.
        /// start activity to pick audio filepath
        /// </summary>
        public void GetAudioFile()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                jo.CallStatic("getAudioFile");
                Utils.Message(TAG, "getAudioFile");
            }
            else
            {
                Utils.Message(TAG, "warning: must run in actual android device");
            }
#endif
        }
        
        /// <summary>
        /// Gets Video File path.
        /// start activity to pick audio filepath
        /// </summary>
        public void GetVideoFile()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                jo.CallStatic("getVideoFile");
                Utils.Message(TAG, "getVideoFile");
            }
            else
            {
                Utils.Message(TAG, "warning: must run in actual android device");
            }
#endif
        }

        /// <summary>
        /// dispatch when image successfully get crop image.
        /// </summary>
        /// <param name="imagePath">Image path.</param>
        private void onHandleGetFilePath(string message, string imagePath)
        {
            if (null != HandleGetFilePath)
            {
                HandleGetFilePath(message, imagePath);
            }
        }
    }
}
