using UnityEngine;
using UnityEngine.SceneManagement;

public class TargetSpawn : MonoBehaviour
{
    public Vector3 minBounds;
    public Vector3 maxBounds;
    public float minRespawnTime = 1f;
    public float maxRespawnTime = 3f;

    // 🔁 Movement settings
    public bool enableMovement = true;
    public float moveXDistance = 2f;
    public float moveYDistance = 1f;
    public float moveSpeed = 2f;

    private Vector3 startPosition;
    private float timeOffset;
    private bool shouldMove = false;

    void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        shouldMove = sceneName == "AdvancedRange"; // only move in this scene

        startPosition = transform.position;
        timeOffset = Random.Range(0f, 100f); // vary movement between targets
    }

    void Update()
    {
        if (shouldMove && enableMovement)
        {
            float xOffset = Mathf.Sin((Time.time + timeOffset) * moveSpeed) * moveXDistance;
            float yOffset = Mathf.Cos((Time.time + timeOffset) * moveSpeed) * moveYDistance;

            transform.position = startPosition + new Vector3(xOffset, yOffset, 0f);
        }
    }

    public void Hit()
    {
        Debug.Log("TargetSpawn.Hit() called!");
        float respawnTime = Random.Range(minRespawnTime, maxRespawnTime);
        Invoke(nameof(Respawn), respawnTime);
        gameObject.SetActive(false);
    }

    private void Respawn()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        float zPosition;

        if (sceneName == "CloseRange")
            zPosition = 15f;
        else if (sceneName == "MidRange")
            zPosition = 30f;
        else if (sceneName == "AdvancedRange")
            zPosition = 30f;
        else
        {
            Debug.LogWarning($"Unknown scene '{sceneName}', using default z position.");
            zPosition = 30f;
        }

        Vector3 randomPosition = new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y),
            zPosition
        );

        transform.position = randomPosition;
        startPosition = transform.position; // reset base point for next movement
        gameObject.SetActive(true);
    }
}
