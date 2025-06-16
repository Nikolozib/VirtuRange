using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportationAdvancedRange : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered teleportation area: off to AdvancedRange scene");
            SceneManager.LoadScene("AdvancedRange");
        }
    }
}
