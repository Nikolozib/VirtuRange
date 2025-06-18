using UnityEngine;

public class TargetSpawnAtStartMidRange : MonoBehaviour
{
    public GameObject targetPrefab;
    public Vector3 minBounds = new Vector3(-5f, 1f, 30f);
    public Vector3 maxBounds = new Vector3(5f, 3f, 30f);
    public float minRespawnTime = 1f;
    public float maxRespawnTime = 3f;

    void Start()
    {
        SpawnTarget();
    }

    private void SpawnTarget()
    {
        if (targetPrefab == null)
        {
            Debug.LogError("Target prefab not assigned!");
            return;
        }

        Vector3 spawnPosition = new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y),
            30f
        );

        GameObject newTarget = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);

        TargetSpawn ts = newTarget.GetComponent<TargetSpawn>();
        if (ts != null)
        {
            ts.minBounds = minBounds;
            ts.maxBounds = maxBounds;
            ts.minRespawnTime = minRespawnTime;
            ts.maxRespawnTime = maxRespawnTime;
        }
    }
}
