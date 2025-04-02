using System.Collections;
using System.Collections.Generic;
using ArabicSupport;
using ClawbearGames;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class QuestionController : MonoBehaviour
{
    public static QuestionController instance;


    public Transform buttonNext;
    public int numQuestion;

    public int numCount;

    [System.Serializable]
    public class QuestionList
    {
        public string name = null;

        public List<string> compare = null;

        public int level = 1;
    }

    public QuestionList[] questionLists;

    void Awake()
    {
        if (!instance)
        {
            instance = this;
        }
        else
        {
            Destroy(instance);
            instance = this;
        }
    }

    public List<string> questionList;
    public List<string> comparingString;

    public TextMeshProUGUI questionLabel;
    public List<Transform> questionImage;

    // Start is called before the first frame update
    void Start()
    {
        numCount = 0;
        if (PlayerPrefs.GetInt("numQuestion") == 0)
        {
            numQuestion = 0;
        }
        else
        {
            numQuestion = PlayerPrefs.GetInt("numQuestion");
        }
        //buttonNext = GameObject.Find("NextButton").GetComponent<Transform>();

        questionLabel = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        questionImage.Add(transform.GetChild(0).transform.GetChild(0));
        questionImage.Add(transform.GetChild(0).transform.GetChild(1));
        questionImage.Add(transform.GetChild(0).transform.GetChild(2));

        LoadQuestion();
    }

    // Update is called once per frame
    void Update()
    {
        if (questionLists.Length == 0)
        {
            questionLabel.text = "";
        }
    }

    public void NextQuestion()
    {
        SimilarityCalculator.instance.percentage = 0f;
        buttonNext.gameObject.SetActive(false);
        numQuestion++;
        // Debug.Log(questionLists[0].name);

        if (numQuestion > questionLists.Length)
        {
            questionLabel.text = "";
        }
        else if (numQuestion < questionLists.Length)
        {
            questionLabel.text = questionLists[numQuestion].name;
            // Traffic light
            questionImage[0].gameObject.SetActive(false);
            questionImage[1].gameObject.SetActive(true);
            questionImage[2].gameObject.SetActive(false);
        }
    }

    public void NextQuestion2(int numQuestion2)
    {
        SimilarityCalculator.instance.percentage = 0f;
        buttonNext.gameObject.SetActive(false);
        numQuestion2++;
        // Debug.Log(questionLists[0].name);

        if (numQuestion2 > questionLists.Length)
        {
            questionLabel.text = "";
        }
        else if (numQuestion2 < questionLists.Length)
        {
            questionLabel.text = questionLists[numQuestion2].name;
            // Traffic light
            questionImage[0].gameObject.SetActive(false);
            questionImage[1].gameObject.SetActive(true);
            questionImage[2].gameObject.SetActive(false);
        }
    }

    public void LoadQuestion()
    {
        string fixString = questionLists[numQuestion].name;
        buttonNext.gameObject.SetActive(false);


        questionLabel.text = fixString;
        Debug.Log(fixString);
        // Traffic light
        questionImage[0].gameObject.SetActive(false);
        questionImage[1].gameObject.SetActive(true);
        questionImage[2].gameObject.SetActive(false);
    }

    public void NextRound()
    {
        numQuestion++;
        if (numQuestion > questionLists.Length - 1)
        {
            numQuestion = 0;
        }
        PlayerPrefs.SetInt("numQuestion", numQuestion);
        IngameManager.Instance.CompletedLevel();
        StartCoroutine(PlayerController.Instance.CRHandleActionsReachedFinishLine());
        StartCoroutine(PlayerController.Instance.EndingTheRound());
    }

    public void CheckingCorrect(bool isCorrect)
    {
        if (isCorrect)
        {
            buttonNext.GetComponent<Button>().onClick.RemoveAllListeners();
            if (numQuestion == questionLists.Length - 1)
            {
                buttonNext.GetChild(0).GetComponent<TextMeshProUGUI>().text = "TAMAT SOALAN";
                buttonNext.GetComponent<Button>().onClick.AddListener(NextRound);
                buttonNext.gameObject.SetActive(true);
            }
            else
            {
                numCount++;
                // Traffic light
                questionImage[0].gameObject.SetActive(false);
                questionImage[1].gameObject.SetActive(false);
                questionImage[2].gameObject.SetActive(true);

                //checking if reach 5 queestion
                if (numCount > 4)
                {
                    buttonNext.GetChild(0).GetComponent<TextMeshProUGUI>().text = "TAMAT SOALAN";
                    buttonNext.GetComponent<Button>().onClick.AddListener(NextRound);
                    buttonNext.gameObject.SetActive(true);
                }
                else
                {
                    buttonNext.GetComponent<Button>().onClick.AddListener(NextQuestion);
                    buttonNext.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            // Traffic light
            questionImage[0].gameObject.SetActive(true);
            questionImage[1].gameObject.SetActive(false);
            questionImage[2].gameObject.SetActive(false);
            //buttonNext.gameObject.SetActive(false);
        }
    }
}
