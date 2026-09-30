using UnityEngine;
using UnityEngine.InputSystem;

public class MovementController : MonoBehaviour
{
    // measures how fast the player can go right now
    [SerializeField]
    public float speed;

    // measures how fast the player can ever go
    [SerializeField]
    private float topSpeed;

    private float airborneSpeed;
    // public for debug purposes
    public Vector2 moveDirection = Vector2.zero;
    public Vector3 velocity = Vector3.zero;

    [SerializeField]
    private Rigidbody rBody;
    [SerializeField]
    public float jumpForce;
    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField, Range(0,10)]
    private float acceleration;

    // measures how fast the player is currently going
    [SerializeField]
    private float currentSpeed = 0;

    [SerializeField]
    public bool airborne = false;

    private RaycastHit hitInfo;


    void Start()
    {
        airborneSpeed /= speed;
    }
    // Update is called once per frame
    void Update()
    {
        airborne = !Physics.Raycast(transform.position, -transform.up, out hitInfo, 1.3f, groundLayer);
        if (moveDirection != Vector2.zero)
        {
            // are we slower than the speed we can currently go? if we are, go faster
            if (currentSpeed < speed)
            {
                currentSpeed += acceleration * Time.deltaTime;
            }
            // are we slower than the top possible speed, but faster than we are supposed to be by a significant amount? if so, slow down
            else if (currentSpeed < topSpeed && currentSpeed > speed + 0.2f)
            {
                currentSpeed -= acceleration * 1.5f * Time.deltaTime;
            }
            // if we're way too fast, or within our speed limits, go at the speed we can currently go at.
            else
            {
                currentSpeed = speed;
            }
            // Determine the velocity based on player direction & speed
            // axis by axis

            // left/right -- along player's right vector
            velocity = transform.right * moveDirection.x;

            // fwd/back
            velocity += transform.forward * moveDirection.y;
        }
        else
        {
            if (currentSpeed > 0)
            {
                currentSpeed -= (acceleration * 2) * Time.deltaTime;
            }
            else
            {
                currentSpeed = 0;
                velocity = Vector3.zero;
            }
            
        }

        

        transform.position += velocity.normalized * currentSpeed * Time.deltaTime;

    }



    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }


    public void OnJump(InputAction.CallbackContext context)
    {
        if (!context.canceled)
        {
            return;
        }

        if (airborne)
        {
            return;
        }

        rBody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        rBody.useGravity = true;
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
  
}
