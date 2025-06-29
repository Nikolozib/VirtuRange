using UnityEngine;

public class SingleTargetSpawner : MonoBehaviour
{
    public GameObject targetPrefab;
    public Vector3 minBounds = new Vector3(-5f, 1f, 30f);
    public Vector3 maxBounds = new Vector3(5f, 3f, 30f);
    public float minRespawnTime = 1f;
    public float maxRespawnTime = 3f;

    private bool hasStarted = false;
    private GameObject currentTarget;
    private TargetSessionManager sessionManager;

    void Start()
    {
        sessionManager = FindObjectOfType<TargetSessionManager>();
    }

    void Update()
    {
        if (!hasStarted && Input.GetKeyDown(KeyCode.P))
        {
            hasStarted = true;
            sessionManager?.StartSession();
            SpawnTarget();
        }
    }

    public void SpawnTarget()
    {
        if (sessionManager == null || !sessionManager.IsSessionActive()) return;

        if (targetPrefab == null)
        {
            Debug.LogError("Target prefab not assigned!");
            return;
        }

        if (currentTarget != null)
            Destroy(currentTarget);

        Vector3 spawnPosition = new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y),
            minBounds.z
        );

        currentTarget = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);

        TargetSpawn ts = currentTarget.GetComponent<TargetSpawn>();
        if (ts != null)
        {
            ts.minBounds = minBounds;
            ts.maxBounds = maxBounds;
            ts.minRespawnTime = minRespawnTime;
            ts.maxRespawnTime = maxRespawnTime;
            ts.spawner = this;
        }

        sessionManager?.RegisterTargetSpawned(); // <-- Register respawn time
    }

    public void NotifyTargetHit()
    {
        if (sessionManager == null || !sessionManager.IsSessionActive()) return;

        float delay = Random.Range(minRespawnTime, maxRespawnTime);
        Invoke(nameof(SpawnTarget), delay);
    }

    public void ResetSpawner()
    {
        hasStarted = true;

        if (currentTarget != null)
        {
            Destroy(currentTarget);
            currentTarget = null;
        }

        SpawnTarget();
    }
}
