using UnityEngine;

public class TeleportationBack : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered teleportation area: off to StartRoom scene");
            UnityEngine.SceneManagement.SceneManager.LoadScene("StartRoom");
        }
    }
}
