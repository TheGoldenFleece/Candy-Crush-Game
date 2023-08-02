using Newtonsoft.Json;
using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] bool maxScore;

    public static int ScoreAmount { private set; get; }
    public static string ScoreDataPath { get => Application.persistentDataPath + "/" + "ScoreData.json"; }

    private void Start()
    {
        ScoreAmount = 0;

        if (maxScore) DisplayMaxScore();
    }

    public void UpdateScore(int amount)
    {
        ScoreAmount += amount; 
        text.text = $"Score: {ScoreAmount}";
    }

    void DisplayMaxScore()
    {
        int maxScore = GetMaxScore();

        text.text = $"Score: {maxScore}";
    }

    public void SaveMaxScore()
    {
        int maxScore = GetMaxScore();

        if (maxScore < ScoreAmount)
        {
            System.IO.File.WriteAllText(ScoreDataPath, ScoreAmount.ToString());
        }
    }

    public int GetMaxScore()
    {
        int maxScore = 0;

        if (!System.IO.File.Exists(ScoreDataPath))
        {
            return maxScore;
        }

        string scoreData = System.IO.File.ReadAllText(ScoreDataPath);

        if (!string.IsNullOrEmpty(scoreData))
        {
            maxScore = JsonConvert.DeserializeObject<int>(scoreData);
        }

        return maxScore;
    }
}
