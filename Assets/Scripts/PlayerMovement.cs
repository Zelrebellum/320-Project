using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float speed = 5f;

    private Rigidbody rb;

    // Input
    public Vector2 moveDirection = Vector2.zero;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        if (moveDirection != Vector2.zero)
        {
            Vector3 velocity = transform.right * moveDirection.x + transform.forward * moveDirection.y;
            velocity *= speed * Time.fixedDeltaTime;

            Vector3 newPosition = rb.position + velocity;

            rb.MovePosition(newPosition);
        }
    }


    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }
}