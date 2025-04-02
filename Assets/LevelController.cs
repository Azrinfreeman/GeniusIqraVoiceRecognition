using System.Collections;
using System.Collections.Generic;
using ClawbearGames;
using UnityEngine;
using UnityEngine.EventSystems;

public class LevelController : MonoBehaviour
{
    public List<Transform> QuestionList;



    // Start is called before the first frame update
    void Start()
    {
        // setQuestions();
    }

    public void setQuestions()
    {
        if (PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL) == 1)
        {
            QuestionList[0].transform.gameObject.SetActive(true);
            QuestionList[1].transform.gameObject.SetActive(false);
            QuestionList[2].transform.gameObject.SetActive(false);
        }
        else if (PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL) == 2)
        {
            QuestionList[0].transform.gameObject.SetActive(false);
            QuestionList[1].transform.gameObject.SetActive(true);
            QuestionList[2].transform.gameObject.SetActive(false);
        }
        else if (PlayerPrefs.GetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL) == 3)
        {
            QuestionList[0].transform.gameObject.SetActive(false);
            QuestionList[1].transform.gameObject.SetActive(false);
            QuestionList[2].transform.gameObject.SetActive(true);
        }
    }

    public void ChooseLevel(int level)
    {
        PlayerPrefs.SetInt(PlayerPrefsKeys.PPK_SAVED_LEVEL, level);
        //hide the buttons
        EventSystem.current.currentSelectedGameObject.transform.parent.transform.gameObject.SetActive(
            false
        );
        //setactive questions
        transform.parent.transform.GetChild(2).transform.gameObject.SetActive(true);
        //display questions

        setQuestions();
        QuestionSelect.instance.buttonsNextLevel[level - 1].gameObject.SetActive(true);
    }

    // Update is called once per frame
    void Update() { }
}
