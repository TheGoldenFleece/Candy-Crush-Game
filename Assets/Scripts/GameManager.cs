using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public static int Score;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        Score = 0;
    }

    public void Restart()
    {
        Exit();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void Menu()
    {
        Exit();
        SceneManager.LoadScene("Menu");
    }

    void Exit()
    {
        int maxScore = PlayerPrefs.GetInt("Score", 0);
        if (maxScore < Score)
        {
            PlayerPrefs.SetInt("Score", Score);
        }
    }
}
