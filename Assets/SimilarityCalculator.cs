using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SimilarityCalculator : MonoBehaviour
{
    public static SimilarityCalculator instance;

    private void Awake()
    {
        instance = this;
    }

    public string name1;
    public string name2;

    public double percentage { get; set; }
    [SerializeField] private Slider slider;
    [SerializeField] private Image imageFill;
    // Start is called before the first frame update
    void Start()
    {
        name1.ToLower();
        name2.ToLower();
        Debug.Log(GetPercentage(name1, name2));
        slider = GameObject.Find("Slider").GetComponent<Slider>();
        imageFill = slider.transform.GetChild(1).transform.GetChild(0).GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //double percentage = ComputeSimilarity.CalculateSimilarity(name1, name2);
        GetComponent<TextMeshProUGUI>().text = (percentage * 100).ToString("00") + "% ";
        slider.value = (float)(percentage * 100);
        if (slider.value > 0 && slider.value < 40)
        {
            imageFill.color = new Color32(255, 0, 0, 255);
        }
        else if (slider.value > 40 && slider.value < 62)
        {
            imageFill.color = new Color32(255, 228, 0, 255);
        }
        else
        {
            imageFill.color = new Color32(61, 255, 0, 255);
        }
    }

    public double GetPercentage(string name1, string name2)
    {


        return percentage = ComputeSimilarity.CalculateSimilarity(name1, name2);
    }
}
public static class ComputeSimilarity
{
    /// <summary>
    /// Calculate percentage similarity of two strings
    /// <param name="source">Source String to Compare with</param>
    /// <param name="target">Targeted String to Compare</param>
    /// <returns>Return Similarity between two strings from 0 to 1.0</returns>
    /// </summary>
    public static double CalculateSimilarity(this string source, string target)
    {
        if ((source == null) || (target == null)) return 0.0;
        if ((source.Length == 0) || (target.Length == 0)) return 0.0;
        if (source == target) return 1.0;

        int stepsToSame = ComputeLevenshteinDistance(source, target);
        return 1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length));
    }
    /// <summary>
    /// Returns the number of steps required to transform the source string
    /// into the target string.
    /// </summary>
    static int ComputeLevenshteinDistance(string source, string target)
    {
        if ((source == null) || (target == null)) return 0;
        if ((source.Length == 0) || (target.Length == 0)) return 0;
        if (source == target) return source.Length;

        int sourceWordCount = source.Length;
        int targetWordCount = target.Length;

        // Step 1
        if (sourceWordCount == 0)
            return targetWordCount;

        if (targetWordCount == 0)
            return sourceWordCount;

        int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

        // Step 2
        for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
        for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

        for (int i = 1; i <= sourceWordCount; i++)
        {
            for (int j = 1; j <= targetWordCount; j++)
            {
                // Step 3
                int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                // Step 4
                distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
            }
        }

        return distance[sourceWordCount, targetWordCount];
    }
}