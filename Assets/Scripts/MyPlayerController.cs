using UnityEngine;
using UnityEngine.SceneManagement;

public class MyPlayerController : MonoBehaviour
{
    private static bool hasMovedToSpawn = false;

    void Awake()
    {
        Debug.Log("✅ MyPlayerController Awake called.");

        gameObject.SetActive(true); // Ensure player is active

        EnableCamera(); // Ensure camera is enabled
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        MoveToSpawnPointIfNeeded(); // Try placing player at spawn point
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hasMovedToSpawn = false; // Reset so player moves again on scene load
        MoveToSpawnPointIfNeeded();
    }

    private void MoveToSpawnPointIfNeeded()
    {
        if (hasMovedToSpawn) return;

        GameObject spawn = GameObject.FindGameObjectWithTag("Respawn");
        if (spawn != null)
        {
            transform.position = spawn.transform.position;
            transform.rotation = spawn.transform.rotation;
            Debug.Log("✅ Player moved to spawn point: " + spawn.name);
        }
        else
        {
            Debug.LogWarning("⚠️ No spawn point found with tag 'Respawn'.");
        }

        hasMovedToSpawn = true;
    }

    private void EnableCamera()
    {
        Camera cam = GetComponentInChildren<Camera>(true); // even if inactive
        if (cam != null)
        {
            cam.enabled = true;
            cam.gameObject.SetActive(true);
            Debug.Log("✅ Camera enabled.");
        }
        else
        {
            Debug.LogWarning("⚠️ No camera found under player!");
        }
    }
}