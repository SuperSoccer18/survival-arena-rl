using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Enemy : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float maxHealth = 1f;
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private bool shouldChase = true;
    [SerializeField] private int contactDamage = 1;

    private Rigidbody2D rb;
    private Transform player;
    private float currentHealth;

    public Transform Player => player;
    public Rigidbody2D Rigidbody => rb;
    public float MoveSpeed => moveSpeed;
    public int ContactDamage => contactDamage;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    private void Start()
    {
        GameObject playerObject =
            GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError(
                $"{name} could not find a GameObject tagged Player."
            );

            enabled = false;
            return;
        }

        player = playerObject.transform;
    }

    private void FixedUpdate()
    {
        if (player == null)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        if (shouldChase)
        {
            ChasePlayer();
        }
    }

    private void ChasePlayer()
    {
        Vector2 direction =
            ((Vector2)player.position - rb.position).normalized;

        rb.linearVelocity = direction * moveSpeed;
    }

    public void TakeDamage(float damage)
    {
        currentHealth -= damage;

        if (currentHealth <= 0f)
        {
            Destroy(gameObject);
        }
    }
}