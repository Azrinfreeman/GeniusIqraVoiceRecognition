using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class togglePlugin : MonoBehaviour
{
    public static togglePlugin instance;

    void Awake()
    {
        instance = this;
    }
    public bool isToggle;
    public Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        anim = transform.GetChild(2).GetComponent<Animator>();
        anim.Play("normalAnim");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Toggling()
    {
        if (CheckInternet.instance.isWifiConnected)
        {

            if (!isToggle)
            {
                isToggle = true;
                anim.Play("accurateAnim");
                CheckInternet.instance.wifiConnectionText.text = "wifi is connected, switching to accurate";
            }
            else
            {
                isToggle = false;
                anim.Play("normalAnim");
                CheckInternet.instance.wifiConnectionText.text = "wifi is connected, switching to normal accuracy";
            }
        }
    }
}
