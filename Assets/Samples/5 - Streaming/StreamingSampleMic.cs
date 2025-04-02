using System.Collections.Generic;
using ArabicSupport;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using Whisper.Utils;

namespace Whisper.Samples
{
    /// <summary>
    /// Stream transcription from microphone input.
    /// </summary>
    public class StreamingSampleMic : MonoBehaviour
    {
        public List<double> comparisonList;
        public WhisperManager whisper;
        public MicrophoneRecord microphoneRecord;

        [Header("UI")]
        public Button button;
        public Text buttonText;
        public Text text;
        public ScrollRect scroll;
        private WhisperStream _stream;

        private async void Start()
        {
            _stream = await whisper.CreateStream(microphoneRecord);
            _stream.OnResultUpdated += OnResult;
            _stream.OnSegmentUpdated += OnSegmentUpdated;
            _stream.OnSegmentFinished += OnSegmentFinished;
            _stream.OnStreamFinished += OnFinished;

            microphoneRecord.OnRecordStop += OnRecordStop;
            button.onClick.AddListener(OnButtonPressed);
        }

        private void OnButtonPressed()
        {
            if (!microphoneRecord.IsRecording)
            {
                _stream.StartStream();

                microphoneRecord.StartRecord();
                text.text = "";
            }
            else
            {

                microphoneRecord.StopRecord();
                _stream.StopStream();
            }
            buttonText.text = microphoneRecord.IsRecording ? "Stop" : "Record";
        }

        private void OnRecordStop(AudioChunk recordedAudio)
        {
            buttonText.text = "Record";
        }

        private void OnResult(string result)
        {
            string arabicWord = ArabicFixer.Fix(result, true, true);
            text.text = arabicWord;
        }

        private void OnSegmentUpdated(WhisperResult segment)
        {
            print($"Segment updated:{segment.Result}");
            //
            Debug.Log("microphone stopped ");
            microphoneRecord.StopRecord();
            _stream.StopStream();
            buttonText.text = microphoneRecord.IsRecording ? "Stop" : "Record";

            MicrophoneController.instance.CheckMic(true);
            //CalculateProgress(segment.Result);
        }

        private void OnSegmentFinished(WhisperResult segment)
        {

            print($"Segment finished: {segment.Result}");





        }


        private void OnFinished(string finalResult)
        {
            print("Stream finished!");

        }
    }
}
