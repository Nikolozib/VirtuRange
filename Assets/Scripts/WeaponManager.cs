using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;

    public GameObject arWeapon;
    public GameObject pistolWeapon;

    private void Awake()
    {
        // Ensure only one instance exists
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // Keep across scenes
    }

    private void Start()
    {
        SetWeapon(arWeapon, pistolWeapon); // Start with AR

        if (SceneManager.GetActiveScene().name != "StartRoom")
        {
            Debug.Log("Hiding both weapons for safety...");
            arWeapon.SetActive(false);
            pistolWeapon.SetActive(false);
        }
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "StartRoom") return;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetWeapon(arWeapon, pistolWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetWeapon(pistolWeapon, arWeapon);
        }
    }

    private void SetWeapon(GameObject enableObj, GameObject disableObj)
    {
        enableObj.SetActive(true);
        disableObj.SetActive(false);
    }
}
