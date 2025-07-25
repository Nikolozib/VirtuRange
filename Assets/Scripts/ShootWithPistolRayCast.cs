using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class ShootWithPistolRayCast : MonoBehaviour, IWeapon
{
    public float shootDistance = 35f;
    public float shootDelay = 0.7f;
    public int maxAmmo = 10;
    public float reloadTime = 1.7f;
    public LayerMask hitMask;
    public GameObject bulletPrefab;
    public float bulletSpeed = 200f;
    public ParticleSystem muzzleFlash;
    public ParticleSystem explosionEffect;
    public AudioClip explosionClip;
    public AudioClip shootClip;
    public AudioClip reloadClip;

    private int currentAmmo;
    private bool ableToShoot = true;
    private bool isReloading = false;

    private AudioSource oneShotAudioSource;

    private void Awake()
    {
        currentAmmo = maxAmmo;

        oneShotAudioSource = gameObject.AddComponent<AudioSource>();
        oneShotAudioSource.playOnAwake = false;
        oneShotAudioSource.spatialBlend = 1f;
        oneShotAudioSource.volume = 0.7f;
        oneShotAudioSource.hideFlags = HideFlags.HideInInspector;
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
        if (shootClip != null)
        {
            oneShotAudioSource.PlayOneShot(shootClip);
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
        WeaponUIManager.instance?.ShowReloading();
        if (reloadClip != null)
        {
            oneShotAudioSource.PlayOneShot(reloadClip);
        }
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
                if (explosionClip != null)
                {
                    AudioSource.PlayClipAtPoint(explosionClip, hit.point);
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

    public string GetWeaponName() => "Pistol";
    public int GetCurrentAmmo() => currentAmmo;
    public int GetMaxAmmo() => maxAmmo;
    public void ResetAmmo() => currentAmmo = maxAmmo;
}