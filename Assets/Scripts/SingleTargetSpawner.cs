using UnityEngine;
using UnityEngine.SceneManagement;

public class SingleTargetSpawner : MonoBehaviour
{
    public static SingleTargetSpawner Instance;

    [Header("Target Prefab")]
    public GameObject targetPrefab;

    [Header("Spawn Bounds")]
    public Vector3 minBounds = new Vector3(-5f, 1f, 30f);
    public Vector3 maxBounds = new Vector3(5f, 3f, 30f);

    [Header("Timing")]
    public float minRespawnTime = 1f;
    public float maxRespawnTime = 3f;

    private GameObject currentTarget;
    private TargetSessionManager sessionManager;

    void Awake()
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

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Start()
    {
        CancelInvoke();
        SetZPerScene();
        FindManager();
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        CancelInvoke();

        if (currentTarget != null)
        {
            Destroy(currentTarget);
            currentTarget = null;
        }

        SetZPerScene();
        FindManager();

        if (TargetSessionManager.Instance?.IsSessionActive() == true)
        {
            SpawnTarget();
        }
    }

    void SetZPerScene()
    {
        float z = SceneManager.GetActiveScene().name == "CloseRange" ? 15f : 30f;
        minBounds.z = z;
        maxBounds.z = z;
    }

    void FindManager()
    {
        sessionManager = FindObjectOfType<TargetSessionManager>();
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
        {
            Destroy(currentTarget);
            currentTarget = null;
        }

        Vector3 spawnPos = new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y),
            minBounds.z
        );

        currentTarget = Instantiate(targetPrefab, spawnPos, Quaternion.identity);

        TargetSpawn ts = currentTarget.GetComponent<TargetSpawn>();
        if (ts != null)
        {
            ts.minBounds = minBounds;
            ts.maxBounds = maxBounds;
            ts.minRespawnTime = minRespawnTime;
            ts.maxRespawnTime = maxRespawnTime;
            ts.spawner = this;
        }
    }

    public void NotifyTargetHit()
    {
        if (sessionManager == null || !sessionManager.IsSessionActive()) return;
        CancelInvoke();
        Invoke(nameof(SpawnTarget), Random.Range(minRespawnTime, maxRespawnTime));
    }

    public void ResetSpawner()
    {
        CancelInvoke();

        if (currentTarget != null)
        {
            Destroy(currentTarget);
            currentTarget = null;
        }

        SpawnTarget();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Vector3 center = (minBounds + maxBounds) / 2f;
        Vector3 size = maxBounds - minBounds;
        Gizmos.DrawWireCube(center, size);
    }
}