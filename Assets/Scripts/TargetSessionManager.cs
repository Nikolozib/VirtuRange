using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections.Generic;

public class TargetSessionManager : MonoBehaviour
{
    public float sessionDuration = 60f;

    public TMP_Text timerText;
    public TMP_Text resultText;
    public TMP_Text recordText;

    private float timer;
    private bool sessionActive = false;
    private List<float> reactionTimes = new List<float>();
    private int targetsHit = 0;
    private float totalDowntime = 0f;
    private float activePlayTime = 0f;

    private void Awake()
    {
        TargetSessionManager[] managers = FindObjectsOfType<TargetSessionManager>();
        if (managers.Length > 1)
        {
            Destroy(gameObject);
            return;
        }

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
            {
                ResetSession();
            }
            return;
        }

        if (!PauseMenuManager.IsPaused)
        {
            timer -= Time.deltaTime;
            activePlayTime += Time.deltaTime;

            if (timerText != null)
                timerText.text = "Time: " + Mathf.CeilToInt(timer).ToString();
        }

        if (timer <= 0f)
        {
            EndSession();
        }
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

        TargetSpawn[] allTargets = FindObjectsOfType<TargetSpawn>();
        foreach (var target in allTargets)
        {
            target.StopSpawning();
        }

        float netActiveTime = activePlayTime - totalDowntime;
        float avgReaction = (targetsHit > 0) ? Mathf.Round((netActiveTime / targetsHit) * 1000f) / 1000f : 0f;

        if (resultText != null)
            resultText.text = $"Hits: {targetsHit}\nAvg Reaction: {avgReaction}s\nPress E to Restart";

        float best = PlayerPrefs.GetFloat("BestAvgReaction", float.MaxValue);
        if (avgReaction > 0f && avgReaction < best)
        {
            PlayerPrefs.SetFloat("BestAvgReaction", avgReaction);
            PlayerPrefs.Save();

            if (recordText != null)
                recordText.text = "Record: " + avgReaction.ToString("F3") + "s";
        }

        Debug.Log("Session ended.");
    }

    public void ResetSession()
    {
        timer = sessionDuration;
        sessionActive = true;
        reactionTimes.Clear();
        targetsHit = 0;
        totalDowntime = 0f;
        activePlayTime = 0f;

        if (resultText != null)
            resultText.text = "";

        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(timer).ToString();

        TargetSpawn[] allTargets = FindObjectsOfType<TargetSpawn>();
        foreach (var target in allTargets)
        {
            target.gameObject.SetActive(true);
            target.AllowRespawning(true);
            target.ResetSpawnTime();
        }

        SingleTargetSpawner spawner = FindObjectOfType<SingleTargetSpawner>();
        if (spawner != null)
        {
            spawner.ResetSpawner();
        }

        Debug.Log("Session restarted.");
    }

    public void StartSession()
    {
        timer = sessionDuration;
        sessionActive = true;
        reactionTimes.Clear();
        targetsHit = 0;
        totalDowntime = 0f;
        activePlayTime = 0f;

        if (resultText != null)
            resultText.text = "";

        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(timer).ToString();
    }

    public bool IsSessionActive()
    {
        return sessionActive;
    }

    public void RefreshUIReferences(TMP_Text timer, TMP_Text result, TMP_Text record)
    {
        timerText = timer;
        resultText = result;
        recordText = record;

        timerText.text = "Time: " + Mathf.CeilToInt(sessionDuration).ToString();
        resultText.text = "";

        float best = PlayerPrefs.GetFloat("BestAvgReaction", float.MaxValue);
        if (best != float.MaxValue)
            recordText.text = "Record: " + best.ToString("F3") + "s";
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        TMP_Text newTimer = GameObject.Find("TimerText")?.GetComponent<TMP_Text>();
        TMP_Text newResult = GameObject.Find("ResultText")?.GetComponent<TMP_Text>();
        TMP_Text newRecord = GameObject.Find("RecordText")?.GetComponent<TMP_Text>();

        if (newTimer && newResult && newRecord)
            RefreshUIReferences(newTimer, newResult, newRecord);

        sessionActive = false;
        timer = sessionDuration;

        if (timerText != null)
            timerText.text = "Time: " + Mathf.CeilToInt(timer).ToString();

        if (resultText != null)
            resultText.text = "";
    }
}
