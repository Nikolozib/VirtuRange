using UnityEngine;
using UnityEngine.SceneManagement;

public class CrooshairManager : MonoBehaviour
{
    public GameObject crosshairPanel;
    void Start()
    {

    }

    void Update()
    {
        crosshairPanel = GameObject.Find("CrossairPanel");
        if (SceneManager.GetActiveScene().name == "MainMenu" || SceneManager.GetActiveScene().name == "DifficultyChooser" || SceneManager.GetActiveScene().name == "StartRoom")
        {
            if (crosshairPanel != null)
                crosshairPanel.SetActive(false);
            return;
        }
        else if (crosshairPanel != null && !crosshairPanel.activeSelf)
        {
            crosshairPanel.SetActive(true);
            return;
        }
    }
}
