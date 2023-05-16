using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] Text text;
    [SerializeField] bool maxScore;
    // Update is called once per frame
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
