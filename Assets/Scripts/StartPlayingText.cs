using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartPlayingText : MonoBehaviour
{
    public GameObject startText;
    private bool triggered = false;

    private readonly string[] allowedScenes = { "CloseRange", "MidRange", "AdvancedRange" };

    void Start()
    {
        startText.SetActive(false);
    }

    void Update()
    {
        string currentScene = SceneManager.GetActiveScene().name;

        // If you're in one of the allowed scenes
        if (System.Array.Exists(allowedScenes, scene => scene == currentScene))
        {
            // If game is paused, hide the start text
            if (PauseMenu.isPaused)
            {
                if (startText.activeSelf)
                    startText.SetActive(false);
                return;
            }

            // Show startText only if not triggered and not already visible
            if (!startText.activeSelf && !triggered)
                startText.SetActive(true);

            // If P is pressed, start session
            if (!triggered && Input.GetKeyDown(KeyCode.P))
            {
                triggered = true;
                startText.SetActive(false);
            }
        }
        else
        {
            // If you're not in an allowed scene, hide it always
            if (startText.activeSelf)
                startText.SetActive(false);
        }
    }



}
