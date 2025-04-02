using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraCanvasController : MonoBehaviour
{
    public Transform CanvasQuestion;
    public Transform Camera;
    // Start is called before the first frame update
    void Start()
    {
        //Camera.GetComponent<Camera>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        /*
        if(CanvasQuestion.gameObject.activeSelf){
			Camera.GetComponent<Camera>().enabled = false;
		CanvasQuestion.gameObject.SetActive(true);
		}else{
			CanvasQuestion.gameObject.SetActive(false);
			Camera.GetComponent<Camera>().enabled = true;
		}
        */
    }
}
