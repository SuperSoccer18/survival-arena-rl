using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAim : MonoBehaviour
{
    [SerializeField] private Transform aimPivot;

    private Camera mainCamera;
    private Vector2 pointerScreenPosition;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnAim(InputValue value)
    {
        pointerScreenPosition = value.Get<Vector2>();
    }

    private void Update()
    {
        Vector3 pointerWorldPosition =
            mainCamera.ScreenToWorldPoint(pointerScreenPosition);

        Vector2 aimDirection =
            pointerWorldPosition - transform.position;

        float angle =
            Mathf.Atan2(aimDirection.y, aimDirection.x)
            * Mathf.Rad2Deg;

        aimPivot.rotation = Quaternion.Euler(0f, 0f, angle);
    }
}