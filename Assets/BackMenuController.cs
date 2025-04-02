using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackMenuController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }
    void OnEnable()
    {
        Time.timeScale = 0f;
    }

    public void hide()
    {
        gameObject.SetActive(false);
        Time.timeScale = 1f;
    }

    public void BackScene()
    {
        SceneManager.LoadScene("Home");
    }
    // Update is called once per frame
    void Update() { }
}
