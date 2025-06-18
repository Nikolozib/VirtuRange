using UnityEngine;
using System.Collections;

public class ShootWithARRayCast : MonoBehaviour
{
    public float shootDistance = 40f;
    public float shootDelay = 0.3f;
    public int maxAmmo = 30;
    public float reloadTime = 2f;
    public LayerMask hitMask;
    public GameObject bulletPrefab;
    public float bulletSpeed = 200f;

    private int currentAmmo;
    private bool ableToShoot = true;
    private bool isReloading = false;

    private void Start()
    {
        currentAmmo = maxAmmo;
    }

    void Update()
    {
        if (isReloading)
            return;

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
            Debug.Log("Out of ammo!");
            StartCoroutine(Reload());
        }
    }

    private IEnumerator ShootDelay()
    {
        ableToShoot = false;
        FireBullet();
        ShootRaycast();
        currentAmmo--;
        Debug.Log("Current Ammo: " + currentAmmo);
        yield return new WaitForSeconds(shootDelay);
        ableToShoot = true;
    }

    private void ShootRaycast()
    {
        Camera cam = Camera.main;
        Ray ray = new Ray(cam.transform.position, cam.transform.forward);
        RaycastHit[] hits = Physics.RaycastAll(ray, shootDistance, hitMask);

        if (hits.Length == 0)
        {
            Debug.Log("Missed!");
        }
        else
        {
            foreach (var hit in hits)
            {
                Debug.Log("Hit Object: " + hit.transform.name);
                var targetScript = hit.transform.GetComponent<TargetSpawn>();
                if (targetScript != null)
                {
                    targetScript.Hit();
                }
            }
        }
        Debug.DrawRay(transform.position, transform.forward * shootDistance, Color.red, 1f);
    }

    private IEnumerator Reload()
    {
        isReloading = true;
        Debug.Log("Reloading...");
        yield return new WaitForSeconds(reloadTime);
        currentAmmo = maxAmmo;
        isReloading = false;
        Debug.Log("Reloaded");
    }

    private void FireBullet()
    {
        if (bulletPrefab == null)
        {
            Debug.LogWarning("Bullet prefab not assigned.");
            return;
        }


        Camera cam = Camera.main;
        Vector3 rayOrigin = cam.transform.position;
        Vector3 rayDirection = cam.transform.forward;

        Vector3 spawnPos = rayOrigin + rayDirection * 1f;
        Quaternion bulletRotation = Quaternion.LookRotation(rayDirection);

        GameObject bullet = Instantiate(bulletPrefab, spawnPos, bulletRotation);

        Collider bulletCollider = bullet.GetComponent<Collider>();
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (bulletCollider != null && playerObj != null)
        {
            Collider playerCollider = playerObj.GetComponent<Collider>();
            if (playerCollider != null)
            {
                Physics.IgnoreCollision(bulletCollider, playerCollider);
            }
        }

        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = rayDirection * bulletSpeed;
        }

        Debug.DrawRay(spawnPos, rayDirection * shootDistance, Color.yellow, 1f);
    }
}