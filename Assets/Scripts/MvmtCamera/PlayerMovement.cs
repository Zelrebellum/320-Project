using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] private bool airborne = false;

    public bool Airborne
    {
        get => airborne;
    }
    
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

    private wallRun wall;
    [SerializeField] private Canvas uiCanvas;

    [SerializeField] private float fadeIconTime;

    private GameObject postureIcon;
    private GameObject crouchIcon;
    float fadeTimer;

    public UIComponents UI
    {
        get => uiCanvas.GetComponent<UIComponents>();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        wall = gameObject.GetComponent<wallRun>();
        
        defaultHeight = playerCollider.height;
        defaultCenter = playerCollider.center;
        defaultCameraPosition = playerCamera.localPosition;
        speed = walkSpeed;
        Application.targetFrameRate = 60;
        postureIcon = UI.PostureIcon;
        crouchIcon = UI.CrouchIcon;
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

        FadeIcon();
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
        crouchIcon.SetActive(true);
    }
    
    private void Stand()
    {
        playerCollider.height = defaultHeight;
        playerCollider.center = defaultCenter;
        
        playerCamera.localPosition = defaultCameraPosition;

        isCrouching = false;
        fadeTimer = fadeIconTime;
        crouchIcon.SetActive(false);
        postureIcon.SetActive(true);
        Vector4 color = postureIcon.GetComponent<Image>().color;
        color.w = 1.0f;
        postureIcon.GetComponent<Image>().color = color;
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

    private void FadeIcon()
    {
        if (isCrouching || fadeTimer <= 0)
        {
            postureIcon.SetActive(false);
            return;
        }

        fadeTimer -= Time.fixedDeltaTime;
        
        Vector4 color = postureIcon.GetComponent<Image>().color;
        color.w = (fadeTimer) / fadeIconTime;
        postureIcon.GetComponent<Image>().color = color;

    }
    
    //event function
    public void OnMove(InputAction.CallbackContext context)
    {
        moveDirection = context.ReadValue<Vector2>();

        if (wall.Wallrunning)
        {
            moveDirection.x = 0f;
        }

        moveDirection.Normalize();
    }
    
    public void OnJump(InputAction.CallbackContext context)
    {

        if (!context.performed || 
            (airborne &&
            !wall.Wallrunning))
        {
            return;
        }

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        if (wall.Wallrunning)
        {
            wall.StopWallrun();
        }
    }
    
    public void OnCrouch(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }

        if (isCrouching)
        {
            Stand();
        }
        else
        {
            Crouch();
        }
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
  
}