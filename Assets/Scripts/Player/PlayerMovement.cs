using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 5f;

    private Rigidbody2D rb;
    private Vector2 movementInput;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnMove(InputValue value)
    {
        movementInput = value.Get<Vector2>();
    }

    private void FixedUpdate()
    {
        rb.linearVelocity = movementInput * moveSpeed;
    }
}