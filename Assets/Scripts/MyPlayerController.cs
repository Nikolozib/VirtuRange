using System.Collections;
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

    private static MyPlayerController Instance;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // persist player across scenes

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
        StartCoroutine(MoveToSpawnPoint());
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
        HandleFootsteps();

        if (footstepSource == null)
        {
            Debug.LogWarning("❌ footstepSource is missing! Recreating...");
            footstepSource = gameObject.AddComponent<AudioSource>();
            footstepSource.clip = footstepClip;
            footstepSource.playOnAwake = false;
            footstepSource.spatialBlend = 1f;
            footstepSource.volume = 0.3f;
        }

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
        if (transform.position.y < -5f && !isRespawning)
        {
            StartCoroutine(RespawnPlayer());
        }
    }

    private void HandleFootsteps()
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
        StartCoroutine(MoveToSpawnPoint());
        EnableCamera();
    }

    private static bool isRespawning = false;

    private IEnumerator MoveToSpawnPoint()
    {
        if (hasMovedToSpawn) yield break;

        GameObject spawn = null;
        float timeout = 2f;
        float timer = 0f;

        // ⏳ Wait for spawn to load in scene
        while (spawn == null && timer < timeout)
        {
            spawn = GameObject.FindGameObjectWithTag("Respawn");
            timer += Time.deltaTime;
            yield return null;
        }

        if (spawn != null)
        {
            yield return null; // Allow one frame to stabilize
            MoveToPosition(spawn.transform.position, spawn.transform.rotation);
            hasMovedToSpawn = true;
        }
        else
        {
            Debug.LogWarning("⚠️ No spawn point found with tag 'Respawn'.");
        }
    }

    private IEnumerator RespawnPlayer()
    {
        isRespawning = true;

        // Wait a frame to avoid physics conflicts
        yield return new WaitForSeconds(0.1f);

        GameObject spawn = GameObject.FindGameObjectWithTag("Respawn");

        if (spawn != null)
        {
            MoveToPosition(spawn.transform.position, spawn.transform.rotation);
            Debug.Log("🔁 Respawned player.");
        }
        else
        {
            Debug.LogWarning("⚠️ Could not respawn — no object with tag 'Respawn'.");
        }

        yield return null;
        isRespawning = false;
    }

    private void MoveToPosition(Vector3 position, Quaternion rotation)
    {
        // Disable controller to teleport cleanly
        if (controller != null) controller.enabled = false;

        transform.position = position;
        transform.rotation = rotation;

        if (controller != null) controller.enabled = true;
    }

    private void EnableCamera()
    {
        Camera cam = GetComponentInChildren<Camera>(true);

        AudioListener[] listeners = FindObjectsOfType<AudioListener>();
        foreach (AudioListener l in listeners)
        {
            if (l.gameObject != cam?.gameObject)
                Destroy(l);
        }

        if (cam != null)
        {
            cam.enabled = true;
            cam.gameObject.SetActive(true);

            if (cam.GetComponent<AudioListener>() == null)
                cam.gameObject.AddComponent<AudioListener>();

            Debug.Log("✅ Camera and AudioListener enabled.");
        }
        else
        {
            Debug.LogWarning("⚠️ No camera found under player.");
        }
    }
}