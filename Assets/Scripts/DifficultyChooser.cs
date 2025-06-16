using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyChooser : MonoBehaviour
{
    public void CloseRange()
    {
        SceneManager.LoadScene("CloseRange");
    }
    public void MidRange()
    {
        SceneManager.LoadScene("MidRange");
    }
    public void AdvancedRange()
    {
        SceneManager.LoadScene("AdvancedRange");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
