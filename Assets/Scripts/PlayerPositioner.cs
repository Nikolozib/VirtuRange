using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerPositioner : MonoBehaviour
{
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject spawnPoint = GameObject.FindWithTag("Respawn");
        if (spawnPoint != null)
        {
            transform.position = spawnPoint.transform.position;
            transform.rotation = spawnPoint.transform.rotation;
        }
        else
        {
            Debug.LogWarning("Spawn point not found in scene " + scene.name);
        }
    }
}
