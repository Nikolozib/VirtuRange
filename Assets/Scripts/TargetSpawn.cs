using UnityEngine;
using UnityEngine.SceneManagement;

public class TargetSpawn : MonoBehaviour
{
    public Vector3 minBounds;
    public Vector3 maxBounds;
    public float minRespawnTime = 1f;
    public float maxRespawnTime = 3f;
    public float moveSpeed = 3f;

    public SingleTargetSpawner spawner;

    private Vector3 targetPosition;
    private bool allowSpawning = true;
    private float spawnTime;
    private bool enableMovement = false;
    private TargetSessionManager sessionManager;

    void Start()
    {
        spawnTime = Time.time;
        enableMovement = SceneManager.GetActiveScene().name == "AdvancedRange";
        PickNewTargetPosition();
        sessionManager = FindObjectOfType<TargetSessionManager>();
    }

    void Update()
    {
        if (!allowSpawning || !enableMovement) return;
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
        float z = minBounds.z; // always fixed z per scene
        targetPosition = new Vector3(x, y, z);
    }

    public void Hit()
    {
        if (!allowSpawning || sessionManager == null || !sessionManager.IsSessionActive()) return;

        float reactionTime = Time.time - spawnTime;
        sessionManager.RegisterHit(reactionTime);

        allowSpawning = false;
        gameObject.SetActive(false);

        if (spawner != null)
            spawner.NotifyTargetHit();
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
