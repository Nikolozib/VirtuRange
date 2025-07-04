using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ShootWithPistolRayCast : MonoBehaviour, IWeapon
{
    public float shootDistance = 35f;
    public float shootDelay = 0.7f;
    public int maxAmmo = 10;
    public float reloadTime = 3f;
    public LayerMask hitMask;
    public GameObject bulletPrefab;
    public float bulletSpeed = 200f;

    private int currentAmmo;
    private bool ableToShoot = true;
    private bool isReloading = false;

    private void Awake()
    {
        currentAmmo = maxAmmo;
    }

    private void OnEnable()
    {
        ableToShoot = true;
        isReloading = false;
        WeaponUIManager.instance?.UpdateUI(this);
    }
    private void OnDisable()
    {
        WeaponUIManager.instance?.HideAmmo();
    }


    void Update()
    {
        if (!PauseMenuManager.IsPaused && gameObject.activeInHierarchy && SceneManager.GetActiveScene().name != "StartRoom")
        {
            if (isReloading) return;

            if (Input.GetKeyDown(KeyCode.R) && currentAmmo < maxAmmo)
            {
                StartCoroutine(Reload());
                return;
            }

            if (Input.GetKey(KeyCode.Mouse0) && ableToShoot && currentAmmo > 0)
            {
                StartCoroutine(ShootDelay());
            }

            if (currentAmmo == 0 && !isReloading)
            {
                StartCoroutine(Reload());
            }
        }
    }

    private IEnumerator ShootDelay()
    {
        ableToShoot = false;
        FireBullet();
        ShootRaycast();
        currentAmmo--;
        WeaponUIManager.instance?.UpdateUI(this);
        yield return new WaitForSeconds(shootDelay);
        ableToShoot = true;
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        WeaponUIManager.instance?.ShowReloading();
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        WeaponUIManager.instance?.HideReloading();
        WeaponUIManager.instance?.UpdateUI(this);
    }


    private void ShootRaycast()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, shootDistance, hitMask))
        {
            hit.collider.GetComponent<TargetSpawn>()?.Hit();
        }
    }

    private void FireBullet()
    {
        if (!bulletPrefab) return;

        Vector3 spawnPos = Camera.main.transform.position + Camera.main.transform.forward * 1f;
        GameObject bullet = Instantiate(bulletPrefab, spawnPos, Quaternion.LookRotation(Camera.main.transform.forward));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
            rb.linearVelocity = Camera.main.transform.forward * bulletSpeed;
    }

    // IWeapon interface implementation
    public string GetWeaponName() => "Pistol";
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
    public void ResetAmmo() => currentAmmo = maxAmmo;
}
