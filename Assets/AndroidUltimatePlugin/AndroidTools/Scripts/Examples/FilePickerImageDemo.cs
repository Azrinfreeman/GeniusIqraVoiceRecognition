using System;
using Gigadrillgames.AUP.Common;
using Gigadrillgames.AUP.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Gigadrillgames.AUP.Tools
{
    public class FilePickerImageDemo : MonoBehaviour
    {
        private const string TAG = "[FilePickerImageDemo]: ";
        private FilePickerPlugin _filePickerPlugin;
        private UtilsPlugin _utilsPlugin;
        private Dispatcher _dispatcher;
        private string _filepath;

        public Text statusText;
        public RawImage rawImage;

        // Start is called before the first frame update
        void Start()
        {
            // needed to run the callback on the main thread
            _dispatcher = Dispatcher.GetInstance();

            _filePickerPlugin = FilePickerPlugin.GetInstance();
            _filePickerPlugin.Init();
            _filePickerPlugin.SetDebug(0);
            _filePickerPlugin.OnHandleGetFilePath += OnHandleGetFilePath;
        }

        public void GetImageFilePath()
        {
            _filePickerPlugin.GetImageFile();
        }

        private void LoadImage(String imageFilePath)
        {
            if (!String.IsNullOrEmpty(imageFilePath))
            {
                StartCoroutine(Utils.LoadImage(imageFilePath, LoadTextureHandler, LoadTexureFailedHandler));
            }
        }

        private void LoadTexureFailedHandler()
        {
            Debug.Log($"{TAG} File Picker Image load failed filepath: {_filepath}");
            statusText.text = $"File Picker Image load failed filepath: {_filepath}";
        }

        private void LoadTextureHandler(Texture texture)
        {
            if (rawImage != null)
            {
                if (texture != null)
                {
                    Debug.Log($"{TAG} File Picker Image load Successfully filepath: {_filepath}");
                    statusText.text = $"File Picker Image load Successfully filepath: {_filepath}";
                    rawImage.texture = texture;
                }
                else
                {
                    Debug.Log($"{TAG} File Picker Image load failed texture is null filepath: {_filepath}");
                    statusText.text = $"File Picker Image load failed texture is null filepath: {_filepath}";
                }
            }
            else
            {
                Debug.Log($"{TAG} File Picker Image load failed rawimage object is null");
                statusText.text = $"File Picker Image load failed rawimage object is null";
            }
        }

        private void OnHandleGetFilePath(string message, string filepath)
        {
            Debug.Log($"{TAG} filepath: {_filepath}");
            _dispatcher.InvokeAction(
                () =>
                {
#if UNITY_ANDROID && !UNITY_EDITOR
        if (!String.IsNullOrEmpty(filepath))
        {
            _filepath = $"file://{filepath}";
        }
        else
        {
            _filepath = filepath;
        }
#else
                    _filepath = filepath;
#endif
                    if (statusText != null)
                    {
                        if (String.IsNullOrEmpty(_filepath))
                        {
                            statusText.text = $"filepath: {message}";
                            Debug.Log($"{TAG} update message: {message}");
                        }
                        else
                        {
                            statusText.text = $"filepath: {_filepath}";
                            Debug.Log($"{TAG} update filepath text: {_filepath}");
                            LoadImage(_filepath);
                        }
                    }
                }
            );
        }
    }
}