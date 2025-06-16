using UnityEngine;

public class MyPlayerController : MonoBehaviour
{
    public GameObject player;

    void Awake()
    {
        Debug.Log("PlayerController Awake called.");
        if (player != null)
        {
            player.SetActive(true);
            Debug.Log("PlayerController Awake called, player set to active.");
        }
        else
        {
            Debug.LogWarning("Player GameObject is not assigned in the inspector.");
        }
    }
}
