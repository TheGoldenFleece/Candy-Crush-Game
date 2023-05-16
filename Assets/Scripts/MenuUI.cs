using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour
{
    public void Game()
    {
        SceneManager.LoadScene("Game");
    }

    public void Data()
    {
        SceneManager.LoadScene("Data");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
