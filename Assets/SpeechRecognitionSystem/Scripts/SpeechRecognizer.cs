using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using SpeechRecognitionSystem;
using TMPro;
using UnityEngine;
using UnityEngine.Android;
using UnityEngine.Events;
using UnityEngine.UI;

internal class SpeechRecognizer : MonoBehaviour
{
    public string LanguageModelDirPath = "SpeechRecognitionSystem/model/english_small";

    public void OnDataProviderReady(IAudioProvider audioProvider)
    {
        _audioProvider = audioProvider;
    }

    [System.Serializable]
    public class MessageEvent : UnityEvent<string> { }

    public MessageEvent LogMessageReceived = new MessageEvent();
    public MessageEvent PartialResultReceived = new MessageEvent();
    public MessageEvent ResultReceived = new MessageEvent();

    private void onLanguageModelCopyComplete(string modelDirPath)
    {
        if (!String.IsNullOrEmpty(modelDirPath))
        {
            _languageModelWasCopied = Directory.Exists(modelDirPath);
            _absoluteLanguageModelDirPath = modelDirPath;
        }
        else
        {
            LogMessageReceived?.Invoke("Error on copying streaming assets");
        }
    }

    #region initialization management
    private void tryDeinitSpeechRecognizer()
    {
        var languageModelNeed2Update = !_absoluteLanguageModelDirPath.Contains(
            LanguageModelDirPath
        );
        var frequencyNeed2Update =
            _audioProvider != null && _sr != null && _sr.Frequency != _audioProvider.Frequency;

        if (languageModelNeed2Update)
        {
            _languageModelWasCopied = false;
            _copyRequested = false;
        }
        if (languageModelNeed2Update || frequencyNeed2Update)
        {
            _init = false;
            _running = false;

            if (_sr != null)
            {
                _sr.Dispose();
                _sr = null;
            }
        }
    }

    private void tryToInitLanguageModel()
    {
        if (!_languageModelWasCopied)
        {
            if (Application.platform == RuntimePlatform.Android)
            {
                if (!_copyRequested)
                {
                    copyAssets2ExternalStorage(LanguageModelDirPath);
                    _copyRequested = true;
                }
            }
            else
            {
                onLanguageModelCopyComplete(
                    Application.streamingAssetsPath + "/" + LanguageModelDirPath
                );
            }
        }
    }

    private void tryToInitSpeechRecognizer()
    {
        if (!_init && _languageModelWasCopied && _audioProvider != null)
        {
            if (_sr == null)
            {
                _sr = new SpeechRecognitionSystem.SpeechRecognizer();
            }
            _sr.Frequency = _audioProvider.Frequency;

            _init = _sr.Init(_absoluteLanguageModelDirPath);

            if (_init)
            {
                LogMessageReceived?.Invoke("Mode Offline...");

                _running = true;
                Task.Run(processing).ConfigureAwait(false);
            }
            else
            {
                LogMessageReceived?.Invoke(
                    "Error on init SRS plugin. Check 'Language model dir path'\n"
                        + _absoluteLanguageModelDirPath
                );
            }
        }
    }
    #endregion

    private void onReceiveLogMess(string message)
    {
        LogMessageReceived?.Invoke(message);
    }

    private void Update()
    {
        tryDeinitSpeechRecognizer();

        tryToInitLanguageModel();

        tryToInitSpeechRecognizer();

        if (_audioProvider is AudioRecorder mic)
        {
            micIsRecording = mic.IsRecording();
        }
        else if (_audioProvider is AudioPlayer player)
        {
            micIsRecording = true;
        }

        if (_init && _audioProvider != null)
        {
            var audioData = _audioProvider.GetData();
            if (audioData != null)
            {
                _threadedBufferQueue.Enqueue(audioData);
            }

            if (_recognitionPartialResultsQueue.TryDequeue(out string part))
            {
                if (part != string.Empty)
                    PartialResultReceived?.Invoke(part);
            }
            if (_recognitionFinalResultsQueue.TryDequeue(out string result))
            {
                if (result != string.Empty)
                {
                    ResultReceived?.Invoke(result);
                    AudioRecorder.instance.OnStop();
                    microphone.GetComponent<Image>().color = Color.red;
                    CalculateProgress(result);
                }
            }
        }
    }

    bool micIsRecording = false;

    private async Task processing()
    {
        while (_running)
        {
            if (micIsRecording)
            {
                float[] audioData;
                var isOk = _threadedBufferQueue.TryDequeue(out audioData);
                if (isOk)
                {
                    int resultReady = _sr.AppendAudioData(audioData);
                    if (resultReady == 0)
                    {
                        _recognitionPartialResultsQueue.Enqueue(_sr.GetPartialResult()?.partial);
                    }
                    else
                    {
                        _recognitionFinalResultsQueue.Enqueue(_sr.GetResult()?.text);
                    }
                }
                else
                {
                    await Task.Delay(10);
                }
            }
            else
            {
                _sr.GetPartialResult();
                _sr.GetResult();
            }
        }
    }

    private void OnDestroy()
    {
        tryDeinitSpeechRecognizer();
    }

    private void copyAssets2ExternalStorage(string modelDirPath)
    {
        if (Application.platform == RuntimePlatform.Android)
        {
            var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            var currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            var recognizerActivity = new AndroidJavaObject(
                "com.sss.unity_asset_manager.MainActivity",
                currentActivity
            );
            recognizerActivity.CallStatic("setReceiverObjectName", this.gameObject.name);
            recognizerActivity.CallStatic("setLogReceiverMethodName", "onReceiveLogMess");
            recognizerActivity.CallStatic(
                "setOnCopyingCompleteMethod",
                "onLanguageModelCopyComplete"
            );

            LogMessageReceived?.Invoke(
                ""
            );
            recognizerActivity.Call("tryCopyStreamingAssets2ExternalStorage", modelDirPath);
        }
    }

    public List<double> comparisonList;

    public Transform microphone;
    private SpeechRecognitionSystem.SpeechRecognizer _sr = null;
    private IAudioProvider _audioProvider = null;
    private bool _init = false;
    private bool _copyRequested = false;

    private readonly ConcurrentQueue<float[]> _threadedBufferQueue = new ConcurrentQueue<float[]>();
    private readonly ConcurrentQueue<string> _recognitionPartialResultsQueue =
        new ConcurrentQueue<string>();
    private readonly ConcurrentQueue<string> _recognitionFinalResultsQueue =
        new ConcurrentQueue<string>();

    private bool _languageModelWasCopied = false;
    private string _absoluteLanguageModelDirPath = string.Empty;

    private bool _running = false;

    public static string RemoveUnwanted(string s)
    {
        return s.EndsWith("!") ? s.Remove(s.Length - 1) : s;
    }

    public static string RemoveUnwantedDot(string s)
    {
        return s.EndsWith(".") ? s.Remove(s.Length - 1) : s;
    }

    public void CalculateProgress(string result)
    {
        //MicrophoneController.instance.CheckMic(true);
        //text.text = result;
        comparisonList.Clear();
        if (QuestionController.instance.buttonNext.gameObject.activeSelf)
        {
            return;
        }
        string arabicWord = result.Trim();
        arabicWord = RemoveUnwanted(arabicWord);
        arabicWord = RemoveUnwantedDot(arabicWord);
        for (
            int i = 0;
            i
                < QuestionController
                    .instance
                    .questionLists[QuestionController.instance.numQuestion]
                    .compare
                    .Count;
            i++
        )
        {
            //Debug.Log(SimilarityCalculator.instance.GetPercentage("ba", "ba."));
            UnityEngine.Debug.Log(
                arabicWord.ToLower()
                    + " : "
                    + QuestionController
                        .instance.questionLists[QuestionController.instance.numQuestion]
                        .compare[i]
                        .ToLower()
                    + " == "
                    + SimilarityCalculator.instance.GetPercentage(
                        arabicWord.ToLower(),
                        QuestionController
                            .instance.questionLists[QuestionController.instance.numQuestion]
                            .compare[i]
                            .ToLower()
                    )
            );

            UnityEngine.Debug.Log(
                arabicWord.ToLower() + " remove unwated" + RemoveUnwanted(arabicWord.ToLower())
            );
            string compareTo = QuestionController
                .instance.questionLists[QuestionController.instance.numQuestion]
                .compare[i]
                .ToLower();

            compareTo = RemoveUnwanted(compareTo);
            compareTo = RemoveUnwantedDot(compareTo);
            comparisonList.Add(
                SimilarityCalculator.instance.GetPercentage(arabicWord.ToLower(), compareTo)
            );
            /*        Debug.Log(
                        "Comparison between : "
                            + arabicWord.ToLower()
                            + " AND "
                            + " "
                            + QuestionController
                                .instance
                                .questionLists[QuestionController.instance.numQuestion]
                                .compare[i]
                    );
    */
            if (comparisonList[i] > 0.62 && comparisonList[i] < 0.73f)
            {
                UnityEngine.Debug.Log("detecting");

                //GameObject.Find("sin").transform.GetChild(0).GetComponent<Image>().color = Color.green;
                UnityEngine.Debug.Log("Betul");
                QuestionController.instance.CheckingCorrect(true);
                if (!GameObject.Find("correctSfx").GetComponent<AudioSource>().isPlaying)
                {
                    GameObject.Find("correctSfx").GetComponent<AudioSource>().Play();
                }
                float timeRemaining = 3f;
                while (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                }

                break;
            }
            else if (comparisonList[i] > 0.73f)
            {
                UnityEngine.Debug.Log("detectingBetter");

                GameObject.Find("result").GetComponent<TextMeshProUGUI>().text = QuestionController
                    .instance.questionLists[QuestionController.instance.numQuestion]
                    .name.ToString();

                //GameObject.Find("sin").transform.GetChild(0).GetComponent<Image>().color = Color.green;
                UnityEngine.Debug.Log("Betul");
                QuestionController.instance.CheckingCorrect(true);
                if (!GameObject.Find("correctSfx").GetComponent<AudioSource>().isPlaying)
                {
                    GameObject.Find("correctSfx").GetComponent<AudioSource>().Play();
                }
                float timeRemaining = 3f;
                while (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                }

                break;
            }
            else
            {
                QuestionController.instance.CheckingCorrect(false);
                // Debug.Log("Salah");
            }
        }
    }

    public string removeTag(string tagname)
    {
        int startIndex = 0;
        int lastIndex = 0;
        if (tagname.Contains("<color=\"red\">"))
        {
            for (int i = 0; i < tagname.Length; i++)
            {
                if (tagname[i].Equals("<"))
                {
                    startIndex = i;
                    lastIndex = 13 + i;
                }
            }

            tagname = tagname.Remove(startIndex, lastIndex);
            return tagname;
        }
        else
        {
            return tagname;
        }
    }
}
