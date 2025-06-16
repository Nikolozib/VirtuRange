using UnityEngine;
using UnityEngine.SceneManagement;
public class TeleportationMidRange : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered teleportation area: off to MidRange scene");
            SceneManager.LoadScene("MidRange");
        }
    }
}
