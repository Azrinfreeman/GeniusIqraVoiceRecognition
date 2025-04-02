using System;
using Gigadrillgames.AUP.Common;
using Gigadrillgames.AUP.Tools;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;


namespace Gigadrillgames.AUP.Tools
{
    public class VideoPickerDemo : MonoBehaviour
    {
        private const string TAG = "[VideoPickerDemo]: ";
        public Text statusText;
        public VideoPlayer videoPlayer;

        private FilePickerPlugin _filePickerPlugin;
        private UtilsPlugin _utilsPlugin;
        private string _filepath;
        private Dispatcher _dispatcher;

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

        public void GetVideoFile()
        {
            if (videoPlayer != null)
            {
                videoPlayer.Stop();
            }

            Debug.Log("getting video file path");
            statusText.text = "getting video file path";
            _filePickerPlugin.GetVideoFile();
        }

        public void PlayVideo()
        {
            if (videoPlayer != null)
            {
                if (!String.IsNullOrEmpty(_filepath))
                {
                    Debug.Log($"preparing video filepath {_filepath}");
                    statusText.text = $"preparing video filepath {_filepath}";
                    videoPlayer.renderMode = VideoRenderMode.CameraNearPlane;
                    videoPlayer.url = _filepath;
                    videoPlayer.isLooping = true;
                    videoPlayer.Prepare();
                    videoPlayer.prepareCompleted += PrepareCompleted;
                }
                else
                {
                    Debug.Log("play video failed filepath is null or empty");
                    statusText.text = "play video failed filepath is null or empty";
                }
            }
            else
            {
                Debug.Log("play video failed video player is null");
            }
        }

        public void StopVideo()
        {
            if (videoPlayer != null)
            {
                Debug.Log("stop video");
                statusText.text = "stop video";
                videoPlayer.Stop();
            }
            else
            {
                Debug.Log("stop video failed video player is null");
            }
        }

        private void PrepareCompleted(VideoPlayer vidPlayer)
        {
            if (videoPlayer != null)
            {
                Debug.Log($"play video filepath {_filepath}");
                statusText.text = $"play video filepath {_filepath}";
                vidPlayer.Play();
            }
            else
            {
                Debug.Log("play video failed video player is null");
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
                        }
                    }
                }
            );
        }
    }
}

