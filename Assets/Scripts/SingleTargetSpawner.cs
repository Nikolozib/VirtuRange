using UnityEngine;
using UnityEngine.SceneManagement;

public class SingleTargetSpawner : MonoBehaviour
{
    public static SingleTargetSpawner Instance;

    public GameObject targetPrefab;
    public Vector3 minBounds = new Vector3(-5f, 1f, 30f);
    public Vector3 maxBounds = new Vector3(5f, 3f, 30f);
    public float minRespawnTime = 1f;
    public float maxRespawnTime = 3f;

    private bool hasStarted = false;
    private GameObject currentTarget;
    private TargetSessionManager sessionManager;

    void Awake()
    {
        // Singleton protection
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject); // Optional if you want to persist it

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
        hasStarted = false;

        if (currentTarget != null)
        {
            Destroy(currentTarget);
            currentTarget = null;
        }

        SetZPerScene();
        FindManager();
    }

    void FindManager()
    {
        sessionManager = FindObjectOfType<TargetSessionManager>();
        if (sessionManager == null)
            Debug.LogWarning("TargetSessionManager not found in scene.");
    }

    void Update()
    {
        if (!hasStarted && Input.GetKeyDown(KeyCode.P))
        {
            if (sessionManager != null)
            {
                hasStarted = true;
                sessionManager.StartSession();
                SpawnTarget();
            }
        }
    }

    void SetZPerScene()
    {
        string scene = SceneManager.GetActiveScene().name;
        float z = scene == "CloseRange" ? 15f : 30f;
        minBounds.z = z;
        maxBounds.z = z;
    }

    public void SpawnTarget()
    {
        if (sessionManager == null || !sessionManager.IsSessionActive()) return;
        if (targetPrefab == null)
        {
            Debug.LogError("Target prefab not assigned!");
            return;
        }

        // Always destroy previous before new
        if (currentTarget != null)
        {
            Destroy(currentTarget);
            currentTarget = null;
        }

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

        sessionManager?.RegisterTargetSpawned();
    }

    public void NotifyTargetHit()
    {
        if (sessionManager == null || !sessionManager.IsSessionActive()) return;

        CancelInvoke(); // Stop stacked spawns
        Invoke(nameof(SpawnTarget), Random.Range(minRespawnTime, maxRespawnTime));
    }

    public void ResetSpawner()
    {
        CancelInvoke();
        hasStarted = true;

        if (currentTarget != null)
        {
            Destroy(currentTarget);
            currentTarget = null;
        }

        SpawnTarget();
    }
}
