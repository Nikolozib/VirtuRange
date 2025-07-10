using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ShootWithARRayCast : MonoBehaviour, IWeapon
{
    public float shootDistance = 40f;
    public float shootDelay = 0.3f;
    public int maxAmmo = 30;
    public float reloadTime = 2f;
    public LayerMask hitMask;
    public GameObject bulletPrefab;
    public float bulletSpeed = 200f;
    public ParticleSystem muzzleFlash;
    public ParticleSystem explosionEffect;
    public AudioSource explosionSound; 
    public AudioSource reloadSound;
    public AudioSource shootSound;

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
        muzzleFlash?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }




    private void OnDisable()
    {
        WeaponUIManager.instance?.HideAmmo();
        muzzleFlash?.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void Update()
    {
        if (!PauseMenuManager.IsPaused && gameObject.activeInHierarchy && SceneManager.GetActiveScene().name != "StartRoom" && SceneManager.GetActiveScene().name != "MainMenu" && SceneManager.GetActiveScene().name != "DifficultyChooser")
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
        muzzleFlash?.Play();
        if (shootSound != null)
        {
            AudioSource.PlayClipAtPoint(shootSound.clip, Camera.main.transform.position);
        }
        ShootRaycast();
        currentAmmo--;
        WeaponUIManager.instance?.UpdateUI(this);
        yield return new WaitForSeconds(shootDelay);
        ableToShoot = true;
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        WeaponUIManager.instance?.ShowReloading(); // <-- Show reload message
        if (reloadSound != null)
        {
            AudioSource.PlayClipAtPoint(reloadSound.clip, Camera.main.transform.position);
        }
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        WeaponUIManager.instance?.HideReloading(); // <-- Hide reload message
        WeaponUIManager.instance?.UpdateUI(this);  // <-- Update ammo
    }



    private void ShootRaycast()
    {
        Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);
        if (Physics.Raycast(ray, out RaycastHit hit, shootDistance, hitMask))
        {
            TargetSpawn target = hit.collider.GetComponent<TargetSpawn>();
            if (target != null)
            {
                target.Hit();

                if (explosionEffect != null)
                {
                    explosionEffect.transform.position = hit.point;
                    explosionEffect.transform.rotation = Quaternion.LookRotation(hit.normal);
                    ParticleSystem explosion = Instantiate(explosionEffect, hit.point, Quaternion.LookRotation(hit.normal));
                    Destroy(explosion.gameObject, explosion.main.duration);
                }
                if (explosionSound != null)
                {
                    AudioSource.PlayClipAtPoint(explosionSound.clip, hit.point);
                }
            }
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

    public string GetWeaponName() => "AR";
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
    public void ResetAmmo() => currentAmmo = maxAmmo;
}
