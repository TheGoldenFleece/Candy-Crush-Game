using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { private set; get; }
    [SerializeField] private Score score;

    private void Awake()
    {
        if (Instance != null)
        {
            return;
        }

        Instance = this;
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
        score.SaveMaxScore();
    }
}
