using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private CapsuleCollider playerCollider;

    [SerializeField] private Transform playerCamera;

    
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float jumpForce = 6f;
    
    [SerializeField] private bool isSprinting = true;
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
    }

    private void FixedUpdate()
    {
        float currentSpeed;
        
        //moves player if moveDirection vector is updated
        if (moveDirection != Vector2.zero)
        {
            if (isSprinting)
            {
                currentSpeed = sprintSpeed;
            }
            else
            {
                currentSpeed = walkSpeed;
            }
            
            Vector3 velocity = transform.right * moveDirection.x + transform.forward * moveDirection.y;
            velocity *= currentSpeed * Time.fixedDeltaTime;

            Vector3 newPosition = rb.position + velocity;

            rb.MovePosition(newPosition);
        }
    }
    
    private void Jump()
    {
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
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
    
    //event function
    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded)
        {
            Jump();
        }
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
}