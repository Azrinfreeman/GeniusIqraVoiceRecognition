using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
using Whisper.Utils;
using Button = UnityEngine.UI.Button;
using Toggle = UnityEngine.UI.Toggle;

namespace Whisper.Samples
{
    /// <summary>
    /// Record audio clip from microphone and make a transcription.
    /// </summary>
    public class MicrophoneDemo : MonoBehaviour
    {
        public Transform loadingParent;
        public List<double> comparisonList;

        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;
        public bool streamSegments = true;
        public bool printLanguage = true;

        [Header("UI")]
        public Button button;
        public Text buttonText;
        public Text outputText;
        public Text timeText;
        public Dropdown languageDropdown;
        public Toggle translateToggle;
        public Toggle vadToggle;
        public ScrollRect scroll;

        private string _buffer;

        private void Awake()
        {
            loadingParent.gameObject.SetActive(false);
            whisper.OnNewSegment += OnNewSegment;
            whisper.OnProgress += OnProgressHandler;

            microphoneRecord.OnRecordStop += OnRecordStop;

            button.onClick.AddListener(OnButtonPressed);
            languageDropdown.value = languageDropdown.options
                .FindIndex(op => op.text == whisper.language);

            languageDropdown.onValueChanged.AddListener(OnLanguageChanged);

			languageDropdown.GetComponent<Transform>().gameObject.SetActive(false);
            //translateToggle.isOn = whisper.translateToEnglish;
            // translateToggle.onValueChanged.AddListener(OnTranslateChanged);

            // vadToggle.isOn = microphoneRecord.vadStop;
            //  vadToggle.onValueChanged.AddListener(OnVadChanged);
        }

        private void OnVadChanged(bool vadStop)
        {
            microphoneRecord.vadStop = vadStop;
        }

        private void OnButtonPressed()
        {
            if (!microphoneRecord.IsRecording)
            {
                microphoneRecord.StartRecord();
                buttonText.text = "Stop";
                loadingParent.gameObject.SetActive(false);
                loadingParent.parent.GetComponent<Button>().interactable = true;
            }
            else
            {
                microphoneRecord.StopRecord();
                buttonText.text = "Record";
                loadingParent.gameObject.SetActive(true);
                loadingParent.parent.GetComponent<Button>().interactable = false;
            }
        }

        private async void OnRecordStop(AudioChunk recordedAudio)
        {
            buttonText.text = "Record";
            _buffer = "";

            var sw = new Stopwatch();
            sw.Start();

            var res = await whisper.GetTextAsync(recordedAudio.Data, recordedAudio.Frequency, recordedAudio.Channels);
            if (res == null || !outputText)
                return;

            var time = sw.ElapsedMilliseconds;
            var rate = recordedAudio.Length / (time * 0.001f);
            timeText.text = $"Time: {time} ms\nRate: {rate:F1}x";

            var text = res.Result;
            string cleanText = text;
            if (printLanguage)
                // text += $"\n\nLanguage: {res.Language}";

                outputText.text = text;
            UiUtils.ScrollDown(scroll);
            UnityEngine.Debug.Log("Output: " + cleanText);
            CalculateProgress(cleanText);
            loadingParent.gameObject.SetActive(false);
            loadingParent.parent.GetComponent<Button>().interactable = true;
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
                UnityEngine.Debug.Log(arabicWord.ToLower() + " : " + QuestionController
                            .instance
                            .questionLists[QuestionController.instance.numQuestion]
                            .compare[i].ToLower() + " == " + SimilarityCalculator.instance.GetPercentage(arabicWord.ToLower(), QuestionController
                            .instance
                            .questionLists[QuestionController.instance.numQuestion]
                            .compare[i].ToLower()));

                UnityEngine.Debug.Log(arabicWord.ToLower() + " remove unwated" + RemoveUnwanted(arabicWord.ToLower()));
                string compareTo = QuestionController
                                            .instance
                                            .questionLists[QuestionController.instance.numQuestion]
                                            .compare[i].ToLower();

                compareTo = RemoveUnwanted(compareTo);
                compareTo = RemoveUnwantedDot(compareTo);
                comparisonList.Add(
                    SimilarityCalculator.instance.GetPercentage(
                        arabicWord.ToLower(),
                        compareTo
                    )
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

                    GameObject.Find("result").GetComponent<Text>().text = QuestionController
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

        private void OnLanguageChanged(int ind)
        {
            var opt = languageDropdown.options[ind];
            whisper.language = opt.text;
        }

        private void OnTranslateChanged(bool translate)
        {
            whisper.translateToEnglish = translate;
        }

        private void OnProgressHandler(int progress)
        {
            if (!timeText)
                return;
            timeText.text = $"Progress: {progress}%";
        }

        private void OnNewSegment(WhisperSegment segment)
        {
            if (!streamSegments || !outputText)
                return;

            _buffer += segment.Text;
            outputText.text = _buffer + "...";
            UiUtils.ScrollDown(scroll);
        }
    }
}