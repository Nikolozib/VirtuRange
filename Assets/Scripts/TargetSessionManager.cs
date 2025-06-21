using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TargetSessionManager : MonoBehaviour
{
    public float sessionDuration = 60f;
    public TMP_Text timerText;
    public TMP_Text resultText;
    public GameObject resetButton;  // You can hide or remove if you want since we're using R key

    private float timer;
    private bool sessionActive = false;
    private List<float> reactionTimes = new List<float>();
    private int targetsHit = 0;

    private void Start()
    {
        timerText.text = "Time: " + Mathf.CeilToInt(sessionDuration).ToString();
        resultText.text = "";
        if (resetButton != null)
            resetButton.SetActive(false);
    }

    private void Update()
    {
        if (!sessionActive) return;

        timer -= Time.deltaTime;
        timerText.text = "Time: " + Mathf.CeilToInt(timer).ToString();

        if (timer <= 0f)
        {
            EndSession();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            ResetSession();
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

        float avgReaction = reactionTimes.Count > 0 ? Mathf.Round((Sum(reactionTimes) / reactionTimes.Count) * 1000f) / 1000f : 0f;

        resultText.text = $"Hits: {targetsHit}\nAvg Reaction: {avgReaction}s";
        if (resetButton != null)
            resetButton.SetActive(true);
    }

    public void ResetSession()
    {
        // Instead of reloading scene, reset session variables and restart targets

        // Reset timer and states
        timer = sessionDuration;
        sessionActive = true;
        resultText.text = "";
        if (resetButton != null)
            resetButton.SetActive(false);

        reactionTimes.Clear();
        targetsHit = 0;
        timerText.text = "Time: " + Mathf.CeilToInt(timer).ToString();

        // Restart all targets - you need to make sure targets are ready to respawn
        TargetSpawn[] allTargets = FindObjectsOfType<TargetSpawn>();
        foreach (var target in allTargets)
        {
            target.gameObject.SetActive(true);
            target.AllowRespawning(true);  // Add this method in TargetSpawn script to allow spawning again
            target.ResetSpawnTime();       // Optional: reset spawn timer so reaction time works correctly
        }
    }

    private float Sum(List<float> values)
    {
        float total = 0f;
        foreach (float v in values) total += v;
        return total;
    }

    public void StartSession()
    {
        timer = sessionDuration;
        sessionActive = true;
        resultText.text = "";
        if (resetButton != null)
            resetButton.SetActive(false);

        reactionTimes.Clear();
        targetsHit = 0;
        timerText.text = "Time: " + Mathf.CeilToInt(timer).ToString();
    }
}
