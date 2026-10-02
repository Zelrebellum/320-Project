using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CapsuleCollider playerCollider;

    [SerializeField] private Transform playerCamera;

    
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 6f;

    [SerializeField] private float topSpeed;
    [SerializeField] public float speed;
    [SerializeField,Range(0,30)] private float acceleration;
    [SerializeField] private float currentSpeed = 0;
    [SerializeField] public bool airborne = false;
    
    [SerializeField] private bool isSprinting = false;

    [SerializeField] private LayerMask groundLayer;

    private RaycastHit hitInfo;
    private bool isGrounded = true;
    
    private Vector3 defaultCameraPosition;
    private float defaultHeight;
    private Vector3 defaultCenter;
    [SerializeField] private float crouchingHeight = 1.5f;
    private bool isCrouching = false;

    private Rigidbody rb;
    
    public Vector2 moveDirection = Vector2.zero;



    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        
        defaultHeight = playerCollider.height;
        defaultCenter = playerCollider.center;
        defaultCameraPosition = playerCamera.localPosition;
        speed = walkSpeed;
        Application.targetFrameRate = 60;
    }

    private void FixedUpdate()
    {
        airborne = !Physics.Raycast(transform.position, -transform.up, out hitInfo, 1.3f, groundLayer);

        Acceleration();

        if (isSprinting)
        {
            speed = sprintSpeed;
        }
        
        Vector3 velocity = transform.right * moveDirection.x + transform.forward * moveDirection.y;
        velocity *= currentSpeed * Time.fixedDeltaTime;

        Vector3 newPosition = rb.position + velocity;
        rb.MovePosition(newPosition);
    }
    
    private void Crouch()
    {
        float heightDifference = defaultHeight - crouchingHeight;

        playerCollider.height = crouchingHeight;

        playerCollider.center = new Vector3(
            defaultCenter.x,
            defaultCenter.y - heightDifference / 2f,
            defaultCenter.z
        );

        Vector3 cameraPosition = defaultCameraPosition;
        cameraPosition.y -= heightDifference;
        playerCamera.localPosition = cameraPosition;

        isCrouching = true;
    }
    
    private void Stand()
    {
        playerCollider.height = defaultHeight;
        playerCollider.center = defaultCenter;
        
        playerCamera.localPosition = defaultCameraPosition;

        isCrouching = false;
    }
    
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
        }
    }
    private void Acceleration()
    {
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
            }
            
        }
        
    }
    
    //event function
    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (airborne)
        {
            return;
        }

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            Debug.Log("Crouch pressed!");

            if (isCrouching)
            {
                Stand();
            }
            else
            {
                Crouch();
                isCrouching = true;
            }
        }
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
  
}