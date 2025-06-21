using UnityEngine;
using UnityEngine.SceneManagement;

public class TargetSpawn : MonoBehaviour
{
    public Vector3 minBounds;
    public Vector3 maxBounds;
    public float minRespawnTime = 1f;
    public float maxRespawnTime = 3f;
    public float moveSpeed = 3f;

    public SingleTargetSpawner spawner;  // Assigned by spawner when instantiated

    private Vector3 targetPosition;
    private bool allowSpawning = true;
    private float spawnTime;

    void Start()
    {
        spawnTime = Time.time;
        PickNewTargetPosition();
    }

    void Update()
    {
        if (!allowSpawning) return;

        MoveTowardsTargetPosition();
    }

    private void MoveTowardsTargetPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            PickNewTargetPosition();
        }
    }

    private void PickNewTargetPosition()
    {
        float x = Random.Range(minBounds.x, maxBounds.x);
        float y = Random.Range(minBounds.y, maxBounds.y);
        float z = Random.Range(minBounds.z, maxBounds.z);
        targetPosition = new Vector3(x, y, z);
    }

    public void Hit()
    {
        if (!allowSpawning) return;

        float reactionTime = Time.time - spawnTime;

        var manager = FindObjectOfType<TargetSessionManager>();
        if (manager != null)
        {
            manager.RegisterHit(reactionTime);
        }

        allowSpawning = false;
        gameObject.SetActive(false);

        if (spawner != null)
            spawner.NotifyTargetHit();
        else
            Debug.LogWarning("Spawner reference missing!");
    }

    public void Respawn()
    {
        if (!allowSpawning) return;

        // Reset position immediately on respawn
        float zPos = 30f; // Default z-position

        string scene = SceneManager.GetActiveScene().name;
        if (scene == "CloseRange") zPos = 15f;

        Vector3 spawnPos = new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y),
            zPos
        );

        transform.position = spawnPos;
        spawnTime = Time.time;
        allowSpawning = true;
        gameObject.SetActive(true);

        PickNewTargetPosition();
    }

    public void StopSpawning()
    {
        allowSpawning = false;
        CancelInvoke();
        gameObject.SetActive(false);
    }

    public void AllowRespawning(bool allow)
    {
        allowSpawning = allow;
    }

    public void ResetSpawnTime()
    {
        spawnTime = Time.time;
    }
}
