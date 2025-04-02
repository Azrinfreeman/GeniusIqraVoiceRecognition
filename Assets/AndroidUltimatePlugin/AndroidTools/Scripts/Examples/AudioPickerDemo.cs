using System;
using System.IO;
using Gigadrillgames.AUP.Common;
using Gigadrillgames.AUP.Tools;
using UnityEngine;
using UnityEngine.UI;

namespace Gigadrillgames.AUP.Tools
{
    public class AudioPickerDemo : MonoBehaviour
    {
        private const string TAG = "[AudioPickerDemo]: ";
        public Text statusText;
        public AudioSource audioSource;

        private FilePickerPlugin _filePickerPlugin;
        private UtilsPlugin _utilsPlugin;
        private string _filepath;
        private AudioClip _audioClip;
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

        public void GetAudioFile()
        {
            audioSource.Stop();
            _filePickerPlugin.GetAudioFile();
        }

        public void PlayAudio()
        {
            if (_audioClip != null)
            {
                audioSource.PlayOneShot(_audioClip);
                statusText.text = $"PlayAudio filepath: {_filepath}";
                Debug.Log("play audio");
            }
            else
            {
                Debug.Log("play audio failed audio clip is null");
                statusText.text = "play audio failed audio clip is null";
            }
        }

        public void StopAudio()
        {
            audioSource.Stop();
        }

        private void LoadAudio(String audioFilepath)
        {
            statusText.text = $"load audio path: {audioFilepath}";
            string extension = Path.GetExtension(audioFilepath);
            AudioType audioType = Utils.GetAudioType(extension);
            if (audioType != AudioType.UNKNOWN)
            {
                statusText.text = $"trying to load audio clip path: {audioFilepath} extension: {extension}";
                StartCoroutine(Utils.LoadAudio2(audioFilepath, audioType, LoadAudioClipHandler,
                    LoadAudioClipFailedHandler));
            }
            else
            {
                statusText.text =
                    $"failed to load audioClip file format not supported path: {audioFilepath} extension: {extension}";
            }
        }

        private void LoadAudioClipFailedHandler()
        {
            Debug.Log($"{TAG} Failed to load AudioClip filepath: {_filepath}");
            statusText.text = $"Failed to load AudioClip filepath: {_filepath}";
        }

        private void LoadAudioClipHandler(AudioClip audioClip)
        {
            statusText.text = $"Successfully Load AudioClip filepath: {_filepath}";
            _audioClip = audioClip;
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
                            LoadAudio(_filepath);
                        }
                    }
                }
            );
        }
    }
}

