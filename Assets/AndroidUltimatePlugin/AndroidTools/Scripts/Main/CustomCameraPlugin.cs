using System;
using Gigadrillgames.AUP.Common;
using UnityEngine;

namespace Gigadrillgames.AUP.Tools
{
    public class CustomCameraPlugin : MonoBehaviour
    {
        private static CustomCameraPlugin instance;
        private static GameObject container;
        private const string TAG = "[CustomCameraPlugin]: ";
        private static AUPHolder aupHolder;

#if UNITY_ANDROID
        private static AndroidJavaObject jo;
#endif

        public bool isDebug = true;
        private bool isInit = false;

        public static CustomCameraPlugin GetInstance()
        {
            if (instance == null)
            {
                container = new GameObject();
                container.name = "CustomCameraPlugin";
                instance = container.AddComponent(typeof(CustomCameraPlugin)) as CustomCameraPlugin;
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
                jo = new AndroidJavaObject("com.gigadrillgames.androidplugin.camera.CustomCameraPlugin");
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
        public void Init(string folderName, string imageFileName, bool useBackCamera)
        {
            if (isInit)
            {
                return;
            }

#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                jo.CallStatic("init", folderName, imageFileName, useBackCamera);
                isInit = true;
                Utils.Message(TAG, "init");
            }
            else
            {
                Utils.Message(TAG, "warning: must run in actual android device");
            }
#endif
        }


        public void SetCameraCallbackListener(Action<string> onCaptureImageComplete, Action onCaptureImageCancel,
            Action onCaptureImageFail)
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                CustomCameraCallback customCameraCallback = new CustomCameraCallback();
                customCameraCallback.onCaptureImageComplete = onCaptureImageComplete;
                customCameraCallback.onCaptureImageCancel = onCaptureImageCancel;
                customCameraCallback.onCaptureImageFail = onCaptureImageFail;

                jo.CallStatic("setCameraCallbackListener", customCameraCallback);
                Utils.Message(TAG, "setCameraCallbackListener");
            }
            else
            {
                Utils.Message(TAG, "warning: must run in actual android device");
            }
#endif
        }


        public void OpenCamera()
        {
#if UNITY_ANDROID
            if (Application.platform == RuntimePlatform.Android)
            {
                jo.CallStatic("openCamera");
                Utils.Message(TAG, "openCamera");
            }
            else
            {
                Utils.Message(TAG, "warning: must run in actual android device");
            }
#endif
        }
    }
}