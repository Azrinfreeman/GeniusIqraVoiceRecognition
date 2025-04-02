using System;
using Gigadrillgames.AUP.Common;
using Gigadrillgames.AUP.Tools;
using UnityEngine;
using UnityEngine.UI;


namespace Gigadrillgames.AUP.Tools
{
    public class FilePickerDemo : MonoBehaviour
    {
        private const string TAG = "[AudioPickerDemo]: ";
        private FilePickerPlugin filePickerPlugin;
        private UtilsPlugin utilsPlugin;
        public Text statusText;
        private string _filepath;
        private Dispatcher dispatcher;

        // Start is called before the first frame update
        void Start()
        {
            // needed to run the callback on the main thread
            dispatcher = Dispatcher.GetInstance();

            filePickerPlugin = FilePickerPlugin.GetInstance();
            filePickerPlugin.Init();
            filePickerPlugin.SetDebug(0);
            filePickerPlugin.OnHandleGetFilePath += OnHandleFilePickerFilePath;
        }

        public void GetImageFile()
        {
            filePickerPlugin.GetImageFile();
        }

        public void GetAudioFile()
        {
            filePickerPlugin.GetAudioFile();
        }

        public void GetVideoFile()
        {
            filePickerPlugin.GetVideoFile();
        }

        private void OnHandleFilePickerFilePath(string message, string filepath)
        {
            Debug.Log($"{TAG} filepath: {filepath}");
            dispatcher.InvokeAction(
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
                        }
                    }
                }
            );
        }
    }
}
