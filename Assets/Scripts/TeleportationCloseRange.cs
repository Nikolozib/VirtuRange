using UnityEngine;
using UnityEngine.SceneManagement;
public class TeleportationCloseRange : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered teleportation area: off to CloseRange scene");
            SceneManager.LoadScene("CloseRange");
        }
    }
}


