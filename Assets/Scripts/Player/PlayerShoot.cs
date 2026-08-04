using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerShoot : MonoBehaviour
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireCooldown = 0.2f;

    private float nextFireTime;

    private void OnFire(InputValue value)
    {
        if (!value.isPressed)
        {
            return;
        }

        if (Time.time < nextFireTime)
        {
            return;
        }

        Shoot();
        nextFireTime = Time.time + fireCooldown;
    }

    private void Shoot()
    {
        Instantiate(
            bulletPrefab,
            firePoint.position,
            firePoint.rotation
        );
    }
}