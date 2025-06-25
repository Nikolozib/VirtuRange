using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultyChooser : MonoBehaviour
{
    public void CloseRange()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("CloseRange");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void MidRange()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("MidRange");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void AdvancedRange()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        SceneManager.LoadScene("AdvancedRange");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}
