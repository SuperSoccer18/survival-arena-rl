using UnityEngine;

[RequireComponent(typeof(Enemy))]
public class ChaseMovement : MonoBehaviour
{
    [Header("Steering")]
    [SerializeField] private float steeringSmooth = 8f;

    private Enemy enemy;
    private Vector2 currentDirection = Vector2.right;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        currentDirection = Vector2.right;
    }

    private void FixedUpdate()
    {
        if (enemy.Player == null)
        {
            enemy.Rigidbody.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 worldPos = enemy.Rigidbody.position;
        Vector2 flowDir = Vector2.zero;

        if (FlowFieldNavigator.Instance != null)
        {
            flowDir = FlowFieldNavigator.Instance.GetDirection(worldPos);
        }

        // fallback directly towards player if navigator not available or cell unreachable
        if (flowDir == Vector2.zero)
        {
            flowDir = ((Vector2)enemy.Player.position - worldPos).normalized;
        }

        // smooth steering
        currentDirection = Vector2.Lerp(
            currentDirection,
            flowDir,
            1f - Mathf.Exp(-steeringSmooth * Time.fixedDeltaTime)
        ).normalized;

        enemy.Rigidbody.linearVelocity = currentDirection * enemy.MoveSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        // optional quick visualization: draw current movement
        if (enemy == null) enemy = GetComponent<Enemy>();
        if (enemy == null) return;

        Vector3 pos = Application.isPlaying ? (Vector3)enemy.Rigidbody.position : transform.position;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pos, pos + (Vector3)(currentDirection * 1.0f));
    }
}