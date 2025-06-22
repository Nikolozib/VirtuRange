using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetSessionManager : MonoBehaviour
{
    public float sessionDuration = 60f;
    public TMP_Text timerText;
    public TMP_Text resultText;

    private float timer;
    private bool sessionActive = false;
    private List<float> reactionTimes = new List<float>();
    private int targetsHit = 0;

    private void Start()
    {
        timerText.text = "Time: " + Mathf.CeilToInt(sessionDuration).ToString();
        resultText.text = "";
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

        timer -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.CeilToInt(timer).ToString();

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

    private void EndSession()
    {
        sessionActive = false;

        TargetSpawn[] allTargets = FindObjectsOfType<TargetSpawn>();
        foreach (var target in allTargets)
        {
            target.StopSpawning();
        }

        float avgReaction = reactionTimes.Count > 0
            ? Mathf.Round((Sum(reactionTimes) / reactionTimes.Count) * 1000f) / 1000f
            : 0f;

        resultText.text = $"Hits: {targetsHit}\nAvg Reaction: {avgReaction}s\nPress E to Restart";
        Debug.Log("Session ended.");
    }

    public void ResetSession()
    {
        timer = sessionDuration;
        sessionActive = true;
        resultText.text = "";

        reactionTimes.Clear();
        targetsHit = 0;
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
            spawner.ResetSpawner();  // Spawn target immediately after reset
        }

        Debug.Log("Session restarted.");
    }

    private float Sum(List<float> values)
    {
        float total = 0f;
        foreach (float v in values)
            total += v;
        return total;
    }

    public void StartSession()
    {
        timer = sessionDuration;
        sessionActive = true;
        resultText.text = "";
        reactionTimes.Clear();
        targetsHit = 0;
        timerText.text = "Time: " + Mathf.CeilToInt(timer).ToString();
    }

    public bool IsSessionActive()
    {
        return sessionActive;
    }
}
