using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class ShooterEnemy : MonoBehaviour
{
    [Header("Positioning")]
    [SerializeField] private float preferredDistance = 5f;
    [SerializeField] private float distanceTolerance = 0.5f;

    [Header("Shooting")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] private float fireInterval = 1.5f;

    private Enemy enemy;
    private float fireTimer;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
    }

    private void FixedUpdate()
    {
        if (enemy.Player == null)
        {
            enemy.Rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        UpdateMovement();
    }

    private void Update()
    {
        if (enemy.Player == null)
        {
            return;
        }

        AimAtPlayer();

        fireTimer += Time.deltaTime;

        if (fireTimer >= fireInterval)
        {
            Fire();
            fireTimer = 0f;
        }
    }

    private void UpdateMovement()
    {
        Vector2 toPlayer =
            (Vector2)enemy.Player.position - enemy.Rigidbody.position;

        float distance = toPlayer.magnitude;
        Vector2 direction = toPlayer.normalized;

        if (distance > preferredDistance + distanceTolerance)
        {
            enemy.Rigidbody.linearVelocity =
                direction * enemy.MoveSpeed;
        }
        else if (distance < preferredDistance - distanceTolerance)
        {
            enemy.Rigidbody.linearVelocity =
                -direction * enemy.MoveSpeed;
        }
        else
        {
            enemy.Rigidbody.linearVelocity = Vector2.zero;
        }
    }

    private void AimAtPlayer()
    {
        Vector2 direction =
            enemy.Player.position - firePoint.position;

        float angle =
            Mathf.Atan2(direction.y, direction.x)
            * Mathf.Rad2Deg;

        firePoint.rotation =
            Quaternion.Euler(0f, 0f, angle);
    }

    private void Fire()
    {
        Instantiate(
            projectilePrefab,
            firePoint.position,
            firePoint.rotation
        );
    }
}