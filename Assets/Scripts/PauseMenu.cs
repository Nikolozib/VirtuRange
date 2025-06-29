using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenu; // Assign in Inspector or auto-find
    public static bool isPaused = false;

    private Canvas parentCanvas;

    void Awake()
    {
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        if (pauseMenu != null)
            pauseMenu.SetActive(false);
    }

    void Update()
    {
        // Automatically reassign if reference lost
        if (pauseMenu == null)
        {
            pauseMenu = GameObject.FindWithTag("PauseMenu"); // Or find by name
            if (pauseMenu == null) return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            parentCanvas = pauseMenu.GetComponentInParent<Canvas>();
            if (parentCanvas != null)
            {
                parentCanvas.sortingOrder = 1000;

                // 🔻 Hide all siblings except pauseMenu
                foreach (Transform child in parentCanvas.transform)
                {
                    if (child.gameObject != pauseMenu)
                    {
                        child.gameObject.SetActive(false);
                    }
                }
            }

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            if (isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        isPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        pauseMenu.SetActive(false);
        isPaused = false;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        isPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game is quitting...");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Try to reassign pauseMenu when scene changes
        if (pauseMenu == null)
            pauseMenu = GameObject.FindWithTag("PauseMenu");
    }
}
