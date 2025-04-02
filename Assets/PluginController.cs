using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PluginController : MonoBehaviour
{
    public Transform[] voicePlugins;

    public Transform[] micButtons;

    // Start is called before the first frame update
    void Start() { }

    void FixedUpdate()
    {
        if (CheckInternet.instance.isWifiConnected || CheckInternet.instance.isMobileConnected)
        {
            if (togglePlugin.instance.isToggle)
            {
                //    Debug.Log("internet ");
                voicePlugins[0].gameObject.SetActive(true);
                voicePlugins[1].gameObject.SetActive(false);
                micButtons[0].gameObject.SetActive(true);
                micButtons[1].gameObject.SetActive(false);
            }
            else
            {
                //    Debug.Log("accurate  ");
                voicePlugins[0].gameObject.SetActive(true);
                voicePlugins[1].gameObject.SetActive(false);
                micButtons[0].gameObject.SetActive(true);
                micButtons[1].gameObject.SetActive(false);
            }
        }
        else
        {
            //    Debug.Log("accurate  ");
            voicePlugins[0].gameObject.SetActive(true);
            voicePlugins[1].gameObject.SetActive(false);
            micButtons[0].gameObject.SetActive(true);
            micButtons[1].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update() { }
}
