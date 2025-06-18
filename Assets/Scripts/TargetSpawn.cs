using UnityEngine;
using UnityEngine.SceneManagement;


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
        Invoke(nameof(Respawn), respawnTime);
        gameObject.SetActive(false);
    }

    private void Respawn()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        float zPosition;

        if (sceneName == "CloseRange")
        {
            zPosition = 15f;
        }
        else if (sceneName == "MidRange")
        {
            zPosition = 30f;
        }
        else
        {
            zPosition = 15f;
        }

        Debug.Log($"Respawning in scene {sceneName} at z = {zPosition}");

        Vector3 randomPosition = new Vector3(
            Random.Range(minBounds.x, maxBounds.x),
            Random.Range(minBounds.y, maxBounds.y),
            zPosition
        );

        transform.position = randomPosition;
        gameObject.SetActive(true);
    }
}