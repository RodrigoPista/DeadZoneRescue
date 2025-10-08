using UnityEngine;
using System.Collections;

public class PistolWeapon : MonoBehaviour, IWeaponStrategy
{
    [Header("Projectile")]
    public GameObject bulletPrefab;
    public float bulletSpeed = 50f;
    public Transform muzzleOverride;

    [Header("Ammo")]
    public int magazineSize = 12;
    public int totalAmmo = 60;
    public float reloadTime = 1.5f;

    [Header("Fire Settings")]
    public float fireRate = 0.2f;
    public AudioClip fireSound;
    public AudioClip reloadSound;

    [Header("Aiming")]
    public Camera playerCamera; // assign your player camera here
    public float maxAimDistance = 200f;

    private PlayerWeaponController controller;
    private bool isReloading;
    private float nextFireTime;
    private int currentAmmo;
    private AudioSource audioSource;

    public void Equip(PlayerWeaponController controller)
    {
        this.controller = controller;
        enabled = true;
        gameObject.SetActive(true);

        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        currentAmmo = Mathf.Min(magazineSize, totalAmmo + magazineSize);
    }

    public void Unequip()
    {
        enabled = false;
        gameObject.SetActive(false);
    }

    public void Use()
    {
        if (isReloading) return;

        if (Time.time >= nextFireTime)
        {
            if (currentAmmo > 0)
            {
                FireBullet();
            }
            else if (totalAmmo > 0)
            {
                controller.StartCoroutine(Reload());
            }
        }
    }

    private void FireBullet()
    {
        nextFireTime = Time.time + fireRate;
        currentAmmo--;

        Transform spawnPoint = muzzleOverride != null ? muzzleOverride : controller.weaponSpawnPoint;

        Vector3 targetPoint;
        if (controller.IsAiming && playerCamera != null)
        {
            Ray ray = playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance))
            {
                targetPoint = hit.point;
            }
            else
            {
                targetPoint = ray.origin + ray.direction * maxAimDistance;
            }
        }
        else
        {
            targetPoint = spawnPoint.position + spawnPoint.forward * maxAimDistance;
        }

        Vector3 direction = (targetPoint - spawnPoint.position).normalized;

        GameObject bullet = Object.Instantiate(bulletPrefab, spawnPoint.position, Quaternion.LookRotation(direction));
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = direction * bulletSpeed;
        }

        if (fireSound) audioSource.PlayOneShot(fireSound);

        if (currentAmmo <= 0 && totalAmmo > 0)
        {
            controller.StartCoroutine(Reload());
        }
    }

    public IEnumerator Reload()
    {
        if (isReloading) yield break;
        isReloading = true;
        if (reloadSound) audioSource.PlayOneShot(reloadSound);
        yield return new WaitForSeconds(reloadTime);
        int ammoNeeded = magazineSize - currentAmmo;
        int ammoToLoad = Mathf.Min(ammoNeeded, totalAmmo);
        currentAmmo += ammoToLoad;
        totalAmmo -= ammoToLoad;
        isReloading = false;

    }

    public int CurrentAmmo => currentAmmo;
    public int ReserveAmmo => totalAmmo;
    public bool IsReloading => isReloading;
}
