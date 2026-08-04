using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyChase : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Transform player;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject == null)
        {
            Debug.LogError("No GameObject with the Player tag was found.");
            enabled = false;
            return;
        }

        player = playerObject.transform;
    }

    private void FixedUpdate()
    {
        Vector2 direction =
            ((Vector2)player.position - rb.position).normalized;

        rb.linearVelocity = direction * moveSpeed;
    }
}