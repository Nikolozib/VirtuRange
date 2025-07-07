using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public Button resumeButton;
    public Button mainMenuButton;
    public Button quitButton;

    public static bool IsPaused { get; private set; } = false;

    private static PauseMenuManager instance;

    private readonly string[] scenesWherePauseIsDisabled = { "MainMenu", "DifficultyChooser" };

    void Awake()
    {
        // Singleton pattern
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        // ❌ Don't allow pause in specific scenes
        string currentScene = SceneManager.GetActiveScene().name;
        if (System.Array.Exists(scenesWherePauseIsDisabled, scene => scene == currentScene))
            return;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (IsPaused)
                ResumeGame();
            else
                PauseGame();
        }
    }

    public void PauseGame()
    {
        Time.timeScale = 0f;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(true);

        IsPaused = true;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1f;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        if (pauseMenuUI != null)
            pauseMenuUI.SetActive(false);

        IsPaused = false;
    }

    public void LoadMainMenu()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene("MainMenu");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Quitting game...");
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RehookUIReferences();
    }

    private void RehookUIReferences()
    {
        pauseMenuUI = GameObject.Find("PausePanel");

        if (pauseMenuUI == null)
        {
            Debug.LogWarning("PausePanel not found in scene.");
            return;
        }

        resumeButton = GameObject.Find("ResumeButton")?.GetComponent<Button>();
        mainMenuButton = GameObject.Find("MainMenuButton")?.GetComponent<Button>();
        quitButton = GameObject.Find("QuitButton")?.GetComponent<Button>();

        if (resumeButton != null)
        {
            resumeButton.onClick.RemoveAllListeners();
            resumeButton.onClick.AddListener(ResumeGame);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.RemoveAllListeners();
            mainMenuButton.onClick.AddListener(LoadMainMenu);
        }

        if (quitButton != null)
        {
            quitButton.onClick.RemoveAllListeners();
            quitButton.onClick.AddListener(QuitGame);
        }

        pauseMenuUI.SetActive(false);
    }
}
