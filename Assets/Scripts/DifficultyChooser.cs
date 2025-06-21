using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyChooser : MonoBehaviour
{
    public void CloseRange()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("CloseRange");
    }
    public void MidRange()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MidRange");
    }
    public void AdvancedRange()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("AdvancedRange");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
