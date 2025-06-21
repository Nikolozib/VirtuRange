using UnityEngine;

public class TeleportationBack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 1f;
            Debug.Log("Player entered teleportation area: off to StartRoom scene");
            UnityEngine.SceneManagement.SceneManager.LoadScene("StartRoom");
        }
    }
}
