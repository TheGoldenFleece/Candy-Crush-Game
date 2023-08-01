using TMPro;
using UnityEngine;

public class Score : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] bool maxScore;

    private void Start()
    {
        if (maxScore) DisplayMaxScore();
    }
    void Update()
    {
        Display();
    }

    void Display()
    {
        text.text = $"Score: {GameManager.Score}";
    }

    void DisplayMaxScore()
    {
        text.text = $"Score: {PlayerPrefs.GetInt("Score", 0)}";
    }
}
