using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicrophoneController : MonoBehaviour
{
    public static MicrophoneController instance;

    private void Awake()
    {
        instance = this;
    }

    public bool isToggleMic;

    // Start is called before the first frame update
    void Start()
    {
        //AudioRecorder.instance.OnStop();
        GetComponent<Button>().onClick.AddListener(() => CheckMic(isToggleMic));

        transform.GetChild(0).GetComponent<Image>().color = new Color32(169, 169, 169, 255);
    }

    // Update is called once per frame
    void Update() { }

    private void FixedUpdate()
    {
        /*
        if (!SpeechRecognizer.instance._init)
        {
            isToggleMic = false;
        }
        else
        {
            isToggleMic = true;
        }\
        */
    }

    public void CheckMic(bool condition)
    {
        if (!condition)
        {
            transform.GetChild(0).GetComponent<Image>().color = Color.green;
            isToggleMic = true;
            //AudioRecorder.instance.OnStart();
        }
        else
        {
            transform.GetChild(0).GetComponent<Image>().color = new Color32(169, 169, 169, 255);
            isToggleMic = false;
            //AudioRecorder.instance.OnStop();
        }
    }

    public void ToggleMic()
    {
        //isToggleMic = AudioRecorder.instance.IsRecording();
        if (isToggleMic)
        {
            isToggleMic = false;
            //            SpeechRecognizer.instance._init = isToggleMic;
            transform.GetChild(0).GetComponent<Image>().color = new Color32(169, 169, 169, 255);
            //  AudioRecorder.instance.OnStop();
        }
        else
        {
            isToggleMic = true;
            //          SpeechRecognizer.instance._init = isToggleMic;
            transform.GetChild(0).GetComponent<Image>().color = Color.green;
            //AudioRecorder.instance.OnStart();
        }
    }
}
