using UnityEngine;
using UnityEngine.SceneManagement;

public class TeleportationChooser : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Time.timeScale = 1f;
            Debug.Log("Player entered teleportation area: off to DifficultyChooser scene");
            SceneManager.LoadScene("DifficultyChooser");
        }
    }
}
