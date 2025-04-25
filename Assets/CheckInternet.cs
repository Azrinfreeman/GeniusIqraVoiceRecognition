using System.Collections;
using System.Collections.Generic;
using Gigadrillgames.AUP.Information;
using TMPro;
using UnityEngine;

public class CheckInternet : MonoBehaviour
{
    public static CheckInternet instance;

    void Awake()
    {
        instance = this;
    }
    private bool isChecking = false;

    public bool isMobileConnected = false;
    private bool isMobileFast = false;

    public bool isWifiConnected = false;
    private bool isWifiFast = false;

    public InternetPlugin internetPlugin;
    public TextMeshProUGUI wifiConnectionText;


    // Start is called before the first frame update
    void Start()
    {
        internetPlugin = InternetPlugin.GetInstance();
        internetPlugin.SetDebug(0);
        internetPlugin.Init();
        internetPlugin.setInternetCallbackListener(
            OnWifiConnect,
            OnWifiDisconnect,
            OnWifiSignalStrengthChange
        );
    }


    void OnWifiConnect()
    {
        Debug.Log("[InternetInfoDemo] OnWifiConnect");
        wifiConnectionText.text = "wifi is connected";
    }

    void OnWifiDisconnect()
    {
        Debug.Log("[InternetInfoDemo] OnWifiDisconnect");
        wifiConnectionText.text = "wifi is not connected";
    }

    void OnWifiSignalStrengthChange(int signalStrength, int signalDifference)
    {
        Debug.Log(
            "[InternetInfoDemo] OnWifiSignalStrengthChange signalStrength "
                + signalStrength
                + " signalDifference "
                + signalDifference
        );

        // this is a good signal
        if (signalStrength > 2)
        {
            // do something here
            UpdateWifiSpeed("wifi signal is fast!");
        }
        else
        {
            UpdateWifiSpeed("wifi signal is slow!");
            // do something here
        }
    }

    private void UpdateWifiSpeed(string val) { }

    void FixedUpdate()
    {

    }


    public void CheckingNetwork()
    {

        if (internetPlugin.IsWifiConnected() || internetPlugin.IsMobileConnected())
        {
            isWifiConnected = true;
            if (wifiConnectionText != null)
            {
                wifiConnectionText.text = "wifi disambung";
            }

            internetPlugin.ScanWifi();
        }
        else
        {
            isWifiConnected = false;
            if (wifiConnectionText != null)
            {
                wifiConnectionText.text = "wifi tidak disambung, mengunakan accuracy biasa.";
            }

            //isChecking = false;
            //UpdateStatus("Done Checking.");
            //FinalCheck();
        }
    }

    // Update is called once per frame
    void Update() { }
}
