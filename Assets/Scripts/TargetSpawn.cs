using System.Collections;
using UnityEngine;

public class TargetSpawn : MonoBehaviour
{
    public Vector3 minBounds;
    public Vector3 maxBounds;
    public float minRespawnTime = 1f;
    public float maxRespawnTime = 3f;

    public void Hit()
    {
        Debug.Log("TargetSpawn.Hit() called!");
        float respawnTime = Random.Range(minRespawnTime, maxRespawnTime);
        Invoke(nameof(Respawn), respawnTime); // This still works even if GameObject is inactive
        gameObject.SetActive(false);
    }

    private void Respawn()
    {
        Debug.Log("TargetSpawn.Respawn() called!");
        Vector3 randomPosition = new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y),
            15f
        );

        transform.position = randomPosition;
        gameObject.SetActive(true);
    }
}
