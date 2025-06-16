using UnityEngine;
using UnityEngine.SceneManagement;
public class TeleportationStartingRoom : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered teleportation area: off to StartingRoom scene");
            SceneManager.LoadScene("StartingRoom");
        }
    }
}
