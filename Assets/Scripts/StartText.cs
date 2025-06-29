using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class StartText : MonoBehaviour
{
    public GameObject startText;
    private bool triggered = false;

    void Start()
    {
        if (SceneManager.GetActiveScene().name == "StartRoom")
        {
            startText.SetActive(true);
        }
    }

    void Update()
    {
        if (!triggered && Input.anyKeyDown)
        {
            triggered = true;
            StartCoroutine(HideStartText());
        }
        else if (SceneManager.GetActiveScene().name != "StartRoom")
        {
            startText.SetActive(false);
        }

    }

    private IEnumerator HideStartText()
    {
        yield return new WaitForSeconds(1.5f);
        startText.SetActive(false);
    }
}
