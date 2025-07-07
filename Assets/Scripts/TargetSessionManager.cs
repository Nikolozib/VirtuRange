using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class TargetSessionManager : MonoBehaviour
{
    public float sessionDuration = 60f;
    public TMP_Text timerText, resultText, recordText;

    private float timer;
    private bool sessionActive = false;
    private List<float> reactionTimes = new();
    private int targetsHit = 0;
    private float totalDowntime = 0f;
    private float activePlayTime = 0f;

    void Awake()
    {
        var managers = FindObjectsOfType<TargetSessionManager>();
        if (managers.Length > 1) { Destroy(gameObject); return; }
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (!sessionActive)
        {
            if (Input.GetKeyDown(KeyCode.E))
                ResetSession();
            return;
        }

        if (!PauseMenuManager.IsPaused)
        {
            timer -= Time.deltaTime;
            activePlayTime += Time.deltaTime;
            if (timerText != null)
                timerText.text = "Time: " + Mathf.CeilToInt(timer);
        }

        if (timer <= 0f)
            EndSession();
    }

    public void RegisterHit(float reactionTime)
    {
        if (!sessionActive) return;
        reactionTimes.Add(reactionTime);
        targetsHit++;
    }

    public void RegisterTargetSpawned()
    {
        if (!sessionActive) return;
        totalDowntime += Random.Range(1f, 3f);
    }

    private void EndSession()
    {
        sessionActive = false;

        foreach (var target in FindObjectsOfType<TargetSpawn>())
            target.StopSpawning();

        float netActiveTime = activePlayTime - totalDowntime;
        float avgReaction = targetsHit > 0 ? Mathf.Round((netActiveTime / targetsHit) * 1000f) / 1000f : 0f;

        resultText.text = $"Hits: {targetsHit}\nAvg Reaction: {avgReaction}s\nPress E to Restart";

        // 🔑 Use scene-specific key
        string sceneKey = SceneManager.GetActiveScene().name + "_BestAvgReaction";
        float best = PlayerPrefs.GetFloat(sceneKey, float.MaxValue);

        if (avgReaction > 0f && avgReaction < best)
        {
            PlayerPrefs.SetFloat(sceneKey, avgReaction);
            PlayerPrefs.Save();
            recordText.text = "Record: " + avgReaction.ToString("F3") + "s";
        }
    }


    public void ResetSession()
    {
        timer = sessionDuration;
        sessionActive = true;
        reactionTimes.Clear();
        targetsHit = 0;
        totalDowntime = 0f;
        activePlayTime = 0f;

        resultText.text = "";
        timerText.text = "Time: " + Mathf.CeilToInt(timer);

        foreach (var target in FindObjectsOfType<TargetSpawn>())
        {
            target.gameObject.SetActive(true);
            target.AllowRespawning(true);
            target.ResetSpawnTime();
        }

        var spawner = FindObjectOfType<SingleTargetSpawner>();
        spawner?.ResetSpawner();
    }

    public void StartSession()
    {
        timer = sessionDuration;
        sessionActive = true;
        reactionTimes.Clear();
        targetsHit = 0;
        totalDowntime = 0f;
        activePlayTime = 0f;

        resultText.text = "";
        timerText.text = "Time: " + Mathf.CeilToInt(timer);
    }

    public bool IsSessionActive() => sessionActive;

    public void RefreshUIReferences(TMP_Text timer, TMP_Text result, TMP_Text record)
    {
        timerText = timer;
        resultText = result;
        recordText = record;

        timerText.text = "Time: " + Mathf.CeilToInt(sessionDuration);
        resultText.text = "";

        // 🔑 Load record from scene-specific key
        string sceneKey = SceneManager.GetActiveScene().name + "_BestAvgReaction";
        float best = PlayerPrefs.GetFloat(sceneKey, float.MaxValue);
        if (best != float.MaxValue)
            recordText.text = "Record: " + best.ToString("F3") + "s";
    }


    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshUIReferences(
            GameObject.Find("TimerText")?.GetComponent<TMP_Text>(),
            GameObject.Find("ResultText")?.GetComponent<TMP_Text>(),
            GameObject.Find("RecordText")?.GetComponent<TMP_Text>()
        );

        sessionActive = false;
        timer = sessionDuration;

        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(timer);

        if (resultText != null)
            resultText.text = "";
    }
}
