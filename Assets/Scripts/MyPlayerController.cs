using UnityEngine;
using UnityEngine.SceneManagement;

public class MyPlayerController : MonoBehaviour
{
    private static bool hasMovedToSpawn = false;

    [Header("Footstep Settings")]
    public AudioClip footstepClip;
    public float stepInterval = 0.5f;
    private float stepTimer;
    private AudioSource footstepSource;

    private CharacterController controller;

    void Awake()
    {
        Debug.Log("✅ MyPlayerController Awake called.");
        EnableCamera();

        footstepSource = gameObject.AddComponent<AudioSource>();
        footstepSource.playOnAwake = false;
        footstepSource.spatialBlend = 1f;
        footstepSource.volume = 0.3f;

        controller = GetComponent<CharacterController>();
        if (controller == null)
            Debug.LogWarning("⚠️ CharacterController not found — footstep grounded check will fail.");
    }

    void Start()
    {
        MoveToSpawnPointIfNeeded();
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (IsMoving() && IsGrounded())
        {
            stepTimer -= Time.deltaTime;
            if (stepTimer <= 0f)
            {
                PlayFootstep();
                stepTimer = stepInterval;
            }
        }
        else
        {
            stepTimer = 0f;
        }
    }

    private bool IsMoving()
    {
        return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
    }

    private bool IsGrounded()
    {
        if (controller != null)
            return controller.isGrounded;

        // Optional: fallback raycast if no CharacterController
        return Physics.Raycast(transform.position, Vector3.down, 1.1f);
    }

    private void PlayFootstep()
    {
        if (footstepClip != null)
        {
            footstepSource.PlayOneShot(footstepClip);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        hasMovedToSpawn = false;
        MoveToSpawnPointIfNeeded();
    }

    private void MoveToSpawnPointIfNeeded()
    {
        if (hasMovedToSpawn) return;

        GameObject spawn = GameObject.FindGameObjectWithTag("Respawn");
        if (spawn != null)
        {
            transform.position = spawn.transform.position;
            transform.rotation = spawn.transform.rotation;
            Debug.Log("✅ Player moved to spawn point: " + spawn.name);
        }
        else
        {
            Debug.LogWarning("⚠️ No spawn point found with tag 'Respawn'.");
        }

        hasMovedToSpawn = true;
    }

    private void EnableCamera()
    {
        Camera cam = GetComponentInChildren<Camera>(true);

        // Destroy other AudioListeners to avoid conflict
        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        foreach (AudioListener l in listeners)
        {
            if (l.gameObject != cam.gameObject)
                Destroy(l);
        }

        if (cam != null)
        {
            cam.enabled = true;
            cam.gameObject.SetActive(true);

            if (cam.GetComponent<AudioListener>() == null)
                cam.gameObject.AddComponent<AudioListener>();

            Debug.Log("✅ Camera enabled with AudioListener.");
        }
        else
        {
            Debug.LogWarning("⚠️ No camera found under player.");
        }
    }
}