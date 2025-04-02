using System;
using System.Collections.Generic;
using ArabicSupport;
using Gigadrillgames.AUP.Common;
using Gigadrillgames.AUP.Helpers;
using Gigadrillgames.AUP.Information;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Gigadrillgames.AUP.SpeechTTS
{
    public class SpeechRecognizerDemo2 : MonoBehaviour
    {
        private const string TAG = "[SpeechRecognizerDemo]: ";

        public TextMeshProUGUI compareText;
        private bool isToggle;
        public Animator anim;
        public List<double> comparisonList;
        public Image MicButton;

        [SerializeField]
        private SpeechPlugin speechPlugin;
        public Text resultText;
        public Text partialResultText;
        public Text statusText;

        public SpeechExtraLocale currentExtraLocale = SpeechExtraLocale.MY;

        public Text speechExtraLocaleText;
        public Slider speechExtaLocaleSlider;

        private Dispatcher dispatcher;
        private UtilsPlugin utilsPlugin;

        //network

        public void toggleLanguage()
        {
            if (!isToggle)
            {
                isToggle = true;
                anim.Play("accurateAnim");
                currentExtraLocale = SpeechExtraLocale.AE;
            }
            else if (isToggle)
            {
                isToggle = false;
                anim.Play("normalAnim");
                currentExtraLocale = SpeechExtraLocale.MY;
            }
        }

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
            //input user
            string arabicWord = result.Trim();
            arabicWord = RemoveUnwanted(arabicWord);
            arabicWord = RemoveUnwantedDot(arabicWord);
            arabicWord = ArabicFixer.Fix(arabicWord, true, true);
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
                compareText.text =
                    arabicWord
                    + " | "
                    + QuestionController
                        .instance
                        .questionLists[QuestionController.instance.numQuestion]
                        .compare[i]
                    + " = "
                    + SimilarityCalculator.instance.GetPercentage(
                        QuestionController
                            .instance
                            .questionLists[QuestionController.instance.numQuestion]
                            .compare[i],
                        QuestionController
                            .instance
                            .questionLists[QuestionController.instance.numQuestion]
                            .compare[QuestionController.instance.numQuestion]
                    );
                //Debug.Log(SimilarityCalculator.instance.GetPercentage("ba", "ba."));
                UnityEngine.Debug.Log(
                    arabicWord.ToLower()
                        + " : "
                        + QuestionController
                            .instance
                            .questionLists[QuestionController.instance.numQuestion]
                            .compare[i]
                        + " == "
                        + SimilarityCalculator.instance.GetPercentage(
                            arabicWord,
                            QuestionController
                                .instance
                                .questionLists[QuestionController.instance.numQuestion]
                                .compare[i]
                        )
                );


                string compareTo = QuestionController
                    .instance
                    .questionLists[QuestionController.instance.numQuestion]
                    .compare[i];

                compareTo = RemoveUnwanted(compareTo);
                compareTo = RemoveUnwantedDot(compareTo);
                ArabicFixer.Fix(compareTo, false, false);
                comparisonList.Add(
                    SimilarityCalculator.instance.GetPercentage(arabicWord, compareTo)
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

                    GameObject.Find("result").GetComponent<TextMeshProUGUI>().text =
                        QuestionController
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

        // Use this for initialization
        void Start()
        {
            MicButton = transform.GetChild(0).transform.GetChild(3).GetComponent<Image>();
            MicButton.color = Color.red;

            dispatcher = Dispatcher.GetInstance();
            // for accessing audio
            utilsPlugin = UtilsPlugin.GetInstance();
            utilsPlugin.Init();
            utilsPlugin.SetDebug(0);

            speechPlugin = SpeechPlugin.GetInstance();
            speechPlugin.Init();
            speechPlugin.SetDebug(0);

            AddSpeechPluginListener();
        }

        private void OnEnable()
        {
            AddSpeechPluginListener();
        }

        private void OnDisable()
        {
            RemoveSpeechPluginListener();
        }

        private void AddSpeechPluginListener()
        {
            if (speechPlugin != null)
            {
                //add speech recognizer listener
                speechPlugin.onReadyForSpeech += onReadyForSpeech;
                speechPlugin.onBeginningOfSpeech += onBeginningOfSpeech;
                speechPlugin.onEndOfSpeech += onEndOfSpeech;
                speechPlugin.onError += onError;
                speechPlugin.onResults += onResults;
                speechPlugin.onPartialResults += onPartialResults;
            }
        }

        private void RemoveSpeechPluginListener()
        {
            if (speechPlugin != null)
            {
                //remove speech recognizer listener
                speechPlugin.onReadyForSpeech -= onReadyForSpeech;
                speechPlugin.onBeginningOfSpeech -= onBeginningOfSpeech;
                speechPlugin.onEndOfSpeech -= onEndOfSpeech;
                speechPlugin.onError -= onError;
                speechPlugin.onResults -= onResults;
                speechPlugin.onPartialResults -= onPartialResults;
            }
        }

        private void OnApplicationPause(bool val)
        {
            if (speechPlugin != null)
            {
                if (val)
                {
                    RemoveSpeechPluginListener();
                }
                else
                {
                    AddSpeechPluginListener();
                }
            }
        }

        //this is for debug or test purpose only to log available extra locale on adb using  "adb logcat -s Unity" comand on command prompt or terminal
        public void CheckSpeechRecognizerExtraLanguage()
        {
            string[] extraLanguageAvailable = speechPlugin.GetExtraLanguage();
            foreach (string extraLocale in extraLanguageAvailable)
            {
                Debug.Log(TAG + extraLocale);
            }
        }

        public void StartListeningWithExtraLanguage()
        {
            MicButton.color = Color.green;
            transform
                .GetChild(0)
                .transform.GetChild(3)
                .GetComponent<Transform>()
                .gameObject.SetActive(false);
            transform
                .GetChild(0)
                .transform.GetChild(4)
                .GetComponent<Transform>()
                .gameObject.SetActive(true);

            bool isSupported = speechPlugin.CheckSpeechRecognizerSupport();

            if (isSupported)
            {
                // unmute beep
                utilsPlugin.UnMuteBeep();
                //MicrophoneController.instance.CheckMic(true);
                // enable offline
                //speechPlugin.EnableOffline(true);

                // enable partial Results
                speechPlugin.EnablePartialResult(true);

                int numberOfResults = 5;
                speechPlugin.StartListeningWithHackExtraLanguage(
                    numberOfResults,
                    currentExtraLocale.GetDescription()
                );
            }
            else
            {
                Debug.Log("Speech Recognizer not supported by this Android device ");
            }
        }

        //cancel speech
        public void CancelSpeech()
        {
            if (speechPlugin != null)
            {
                bool isSupported = speechPlugin.CheckSpeechRecognizerSupport();

                if (isSupported)
                {
                    speechPlugin.Cancel();
                    //MicrophoneController.instance.CheckMic(false);

                    transform
                        .GetChild(0)
                        .transform.GetChild(3)
                        .GetComponent<Transform>()
                        .gameObject.SetActive(true);
                    transform
                        .GetChild(0)
                        .transform.GetChild(4)
                        .GetComponent<Transform>()
                        .gameObject.SetActive(false);
                }
            }

            Debug.Log(TAG + " call CancelSpeech..  ");
        }

        public void StopListening()
        {
            if (speechPlugin != null)
            {
                speechPlugin.StopListening();
            }

            Debug.Log(TAG + " StopListening...  ");
        }

        public void StopCancel()
        {
            if (speechPlugin != null)
            {
                speechPlugin.StopCancel();
            }

            Debug.Log(TAG + " StopCancel...  ");
        }

        public void OnSpeechExtraLocaleSliderChange()
        {
            Debug.Log("[TextToSpeechDemo] OnExtraLocaleSliderChange");
            if (speechExtaLocaleSlider != null)
            {
                //update current extra locale here
                currentExtraLocale = (SpeechExtraLocale)speechExtaLocaleSlider.value;

                //update the status to notify user
                UpdateSpeechExtraLocale(currentExtraLocale);
            }
        }

        private void UpdateSpeechExtraLocale(SpeechExtraLocale ttsLocaleCountry)
        {
            if (speechExtraLocaleText != null)
            {
                speechExtraLocaleText.text = String.Format("Extra Locale: {0}", ttsLocaleCountry);
            }
        }

        private void UpdateStatus(string status)
        {
            if (statusText != null)
            {
                statusText.text = String.Format("Status: {0}", status);
            }
        }

        public void NextExtraLocale()
        {
            if (speechExtaLocaleSlider != null)
            {
                if (speechExtaLocaleSlider.value < speechExtaLocaleSlider.maxValue)
                {
                    speechExtaLocaleSlider.value++;
                }
            }
        }

        public void PrevExtraLocale()
        {
            if (speechExtaLocaleSlider != null)
            {
                if (speechExtaLocaleSlider.value > 1)
                {
                    speechExtaLocaleSlider.value--;
                }
            }
        }

        private void OnDestroy()
        {
            // RemoveSpeechPluginListener();
            //speechPlugin.StopListening();
            // speechPlugin.DestroySpeechController();
        }

        //SpeechRecognizer Events
        private void onReadyForSpeech(string data)
        {
            dispatcher.InvokeAction(() =>
            {
                if (speechPlugin != null)
                {
                    //Disables modal
                    speechPlugin.EnableModal(false);
                }

                if (statusText != null)
                {
                    statusText.text = String.Format("Status: {0}", data.ToString());
                }
            });
        }

        private void onBeginningOfSpeech(string data)
        {
            dispatcher.InvokeAction(() =>
            {
                if (statusText != null)
                {
                    statusText.text = String.Format("Status: {0}", data.ToString());
                }
            });
        }

        private void onEndOfSpeech(string data)
        {
            dispatcher.InvokeAction(() =>
            {
                if (statusText != null)
                {
                    statusText.text = String.Format("Status: {0}", data.ToString());
                }
            });
            MicButton.color = Color.red;
            transform
                .GetChild(0)
                .transform.GetChild(3)
                .GetComponent<Transform>()
                .gameObject.SetActive(true);
            transform
                .GetChild(0)
                .transform.GetChild(4)
                .GetComponent<Transform>()
                .gameObject.SetActive(false);
        }

        private void onError(int data)
        {
            dispatcher.InvokeAction(() =>
            {
                if (statusText != null)
                {
                    SpeechRecognizerError error = (SpeechRecognizerError)data;
                    statusText.text = String.Format("Status: {0}", error.ToString());
                }

                if (resultText != null)
                {
                    resultText.text = "Result: Waiting for result...";
                }
                MicButton.color = Color.red;
                transform
                    .GetChild(0)
                    .transform.GetChild(3)
                    .GetComponent<Transform>()
                    .gameObject.SetActive(true);
                transform
                    .GetChild(0)
                    .transform.GetChild(4)
                    .GetComponent<Transform>()
                    .gameObject.SetActive(false);
            });
        }

        private void onResults(string data)
        {
            dispatcher.InvokeAction(() =>
            {
                if (resultText != null)
                {
                    string[] results = data.Split(',');
                    Debug.Log(TAG + " result length " + results.Length);

                    //when you set morethan 1 results index zero is always the closest to the words the you said
                    //but it's not always the case so if you are not happy with index zero result you can always
                    //check the other index

                    //sample on checking other results
                    foreach (string possibleResults in results)
                    {
                        Debug.Log(TAG + " possibleResults " + possibleResults);
                    }

                    //sample showing the nearest result
                    string whatToSay = results.GetValue(0).ToString();
                    if (currentExtraLocale == SpeechExtraLocale.AE)
                    {
                        ArabicFixer.Fix(whatToSay, true, true);
                        resultText.text = string.Format("Result: {0}", whatToSay);
                    }
                    else
                    {
                        resultText.text = string.Format("Result: {0}", whatToSay);
                    }

                    CalculateProgress(whatToSay);
                }
            });
        }

        private void onPartialResults(string data)
        {
            dispatcher.InvokeAction(() =>
            {
                if (partialResultText != null)
                {
                    string[] results = data.Split(',');
                    Debug.Log(TAG + " partial result length " + results.Length);

                    //when you set morethan 1 results index zero is always the closest to the words the you said
                    //but it's not always the case so if you are not happy with index zero result you can always
                    //check the other index

                    //sample on checking other results
                    foreach (string possibleResults in results)
                    {
                        Debug.Log(TAG + " partial possibleResults " + possibleResults);
                    }

                    //sample showing the nearest result
                    string whatToSay = results.GetValue(0).ToString();
                    partialResultText.text = string.Format("Partial Result: {0}", whatToSay);
                }
            });
        }

        //SpeechRecognizer Events
    }
}
