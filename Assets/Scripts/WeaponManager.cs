using UnityEngine;
using UnityEngine.SceneManagement;

public class WeaponManager : MonoBehaviour
{
    public static WeaponManager instance;

    public GameObject arWeapon;
    public GameObject pistolWeapon;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        SetWeapon(arWeapon, pistolWeapon);
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().name == "StartRoom")
        {
            arWeapon.SetActive(false);
            pistolWeapon.SetActive(false);
        }
        else if (SceneManager.GetActiveScene().name == "CloseRange" || SceneManager.GetActiveScene().name == "MidRange" || SceneManager.GetActiveScene().name == "AdvancedRange")
        {

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                SetWeapon(arWeapon, pistolWeapon);
            }
            else if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                SetWeapon(pistolWeapon, arWeapon);
            }
        }
    }

    private void SetWeapon(GameObject enableObj, GameObject disableObj)
    {
        if (enableObj == null || disableObj == null) return;

        enableObj.SetActive(true);
        disableObj.SetActive(false);

        IWeapon weaponScript = enableObj.GetComponent<IWeapon>();
        if (weaponScript != null)
        {
            WeaponUIManager.instance?.UpdateUI(weaponScript);
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "StartRoom")
        {
            arWeapon.SetActive(false);
            pistolWeapon.SetActive(false);
            return;
        }

        // Reset ammo when entering new scene
        arWeapon?.GetComponent<IWeapon>()?.ResetAmmo();
        pistolWeapon?.GetComponent<IWeapon>()?.ResetAmmo();

        SetWeapon(arWeapon, pistolWeapon);
    }
}
