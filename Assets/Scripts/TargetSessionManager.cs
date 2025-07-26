using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class TargetSessionManager : MonoBehaviour
{
    public static TargetSessionManager Instance;

    [Header("Session Settings")]
    public float sessionDuration = 60f;

    [Header("UI References")]
    public TMP_Text timerText;
    public TMP_Text resultText;
    public TMP_Text recordText;

    private float timer = 0f;
    private bool sessionActive = false;
    private bool sessionEnded = false;

    private int hitCount = 0;
    private float totalReactionTime = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (sessionActive)
        {
            timer -= Time.deltaTime;
            UpdateTimerUI();

            if (timer <= 0f)
            {
                EndSession();
            }
        }
        else if (!sessionEnded && Input.GetKeyDown(KeyCode.P)) // Start session with P
        {
            StartSession();
        }
        else if (sessionEnded && Input.GetKeyDown(KeyCode.E)) // Restart session with E
        {
            RestartSession();
        }
    }

    public void StartSession()
    {
        timer = sessionDuration;
        hitCount = 0;
        totalReactionTime = 0f;
        sessionActive = true;
        sessionEnded = false;

        if (resultText != null)
            resultText.text = "";

        UpdateTimerUI();

        // ✅ Spawn target when session starts
        SingleTargetSpawner.Instance?.ResetSpawner();

        Debug.Log("Target session started.");
    }

    public void EndSession()
    {
        sessionActive = false;
        sessionEnded = true;

        float avgTime = hitCount > 0 ? totalReactionTime / hitCount : 0f;
        string sceneName = SceneManager.GetActiveScene().name;

        if (resultText != null)
        {
            resultText.text = hitCount > 0
                ? $"Avg Hit Time: {avgTime:F2}s\nPress E to restart."
                : "No targets hit.\nPress E to restart.";
        }

        if (recordText != null)
        {
            float bestAvg = PlayerPrefs.GetFloat($"{sceneName}_BestAvgTime", float.MaxValue);
            if (avgTime > 0 && avgTime < bestAvg)
            {
                PlayerPrefs.SetFloat($"{sceneName}_BestAvgTime", avgTime);
                recordText.text = $"New Best Avg: {avgTime:F2}s";
            }
            else
            {
                recordText.text = bestAvg < float.MaxValue
                    ? $"Best Avg: {bestAvg:F2}s"
                    : "Best Avg: --";
            }
        }

        Debug.Log("Target session ended.");
    }

    public void RestartSession()
    {
        StartSession();
        SingleTargetSpawner.Instance?.ResetSpawner();
        Debug.Log("Session restarted.");
    }

    public void RegisterHit(float reactionTime)
    {
        if (!sessionActive) return;

        totalReactionTime += reactionTime;
        hitCount++;

        Debug.Log($"Target hit! Reaction time: {reactionTime:F2} s");
    }

    public bool IsSessionActive() => sessionActive;

    private void UpdateTimerUI()
    {
        if (timerText != null)
        {
            timerText.text = Mathf.CeilToInt(timer).ToString();
        }
    }

    public void RefreshUIReferences(TMP_Text timer, TMP_Text result, TMP_Text record)
    {
        timerText = timer;
        resultText = result;
        recordText = record;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (sessionActive)
        {
            EndSession();
        }

        TMP_Text timer = GameObject.Find("TimerText")?.GetComponent<TMP_Text>();
        TMP_Text result = GameObject.Find("ResultText")?.GetComponent<TMP_Text>();
        TMP_Text record = GameObject.Find("RecordText")?.GetComponent<TMP_Text>();

        RefreshUIReferences(timer, result, record);

        if (timerText != null) timerText.text = "";
        if (resultText != null) resultText.text = "Press P to start session.";
        if (recordText != null)
        {
            float bestAvg = PlayerPrefs.GetFloat($"{scene.name}_BestAvgTime", float.MaxValue);
            recordText.text = bestAvg != float.MaxValue
                ? $"Best Avg: {bestAvg:F2}s"
                : "Best Avg: --";
        }

        sessionActive = false;
        sessionEnded = false;
    }
}