using System;
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
    [SerializeField] public float targetSpeed;
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
    
    private Vector3 defaultCameraPosition;
    private float defaultHeight;
    private Vector3 defaultCenter;
    [SerializeField] private float crouchingHeight = 1.5f;
    private bool isCrouching = false;

    public bool IsCrouching
    {
        get => isCrouching;
    }

    private Rigidbody rb;
    
    public Vector2 moveDirection = Vector2.zero;
    private wallRun wall;
    [SerializeField] private Canvas uiCanvas;
    private PlayerSlowdown slowdownScript;

    [SerializeField] private float fadeIconTime;

    private GameObject postureIcon;
    private GameObject crouchIcon;
    float fadeTimer;

    [SerializeField] private MeshRenderer levelBoundary;

    public Bounds LevelBoundary
    {
        get => levelBoundary.bounds;
    }

    public UIComponents UI
    {
        get => uiCanvas.GetComponent<UIComponents>();
    }

    private bool holdCrouch = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        wall = gameObject.GetComponent<wallRun>();
        
        defaultHeight = playerCollider.height;
        defaultCenter = playerCollider.center;
        defaultCameraPosition = playerCamera.localPosition;
        targetSpeed = walkSpeed;
        Application.targetFrameRate = 60;
        postureIcon = UI.PostureIcon;
        crouchIcon = UI.CrouchIcon;
        holdCrouch = Convert.ToBoolean(PlayerPrefs.GetInt("CrouchMode"));

        slowdownScript = GetComponent<PlayerSlowdown>();

    }

    private void FixedUpdate()
    {
        airborne = !Physics.Raycast(transform.position, -transform.up, out hitInfo, transform.localScale.y + 0.3f, groundLayer);

        if (isSprinting)
        {
            targetSpeed = sprintSpeed;
            targetSpeed = sprintSpeed * slowdownScript.GetSpeedMultiplier();
        }
        else 
        { 
            targetSpeed = walkSpeed;
        }

        Acceleration();

        Vector3 velocity = transform.right * moveDirection.x;
        velocity += transform.forward * moveDirection.y;
        velocity *= currentSpeed * Time.fixedDeltaTime;

        Vector3 newPosition = rb.position + velocity;

        if(LevelBoundary.Contains(newPosition))
        {
            rb.MovePosition(newPosition);
        }

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
            if (currentSpeed < targetSpeed)
            {
                currentSpeed += acceleration * Time.deltaTime;
            }
            // are we slower than the top possible speed, but faster than we are supposed to be by a significant amount? if so, slow down
            else if (currentSpeed < topSpeed && currentSpeed > targetSpeed + 0.2f)
            {
                currentSpeed -= acceleration * 1.5f * Time.deltaTime;
            }
            // if we're way too fast, or within our speed limits, go at the speed we can currently go at.
            else
            {
                currentSpeed = targetSpeed;
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

    public void ResetMovement()
    {
        currentSpeed = 0;
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        moveDirection = Vector2.zero;
        if (isCrouching)
        {
            Stand();
        }
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
            (airborne && !wall.Wallrunning) || 
            isCrouching)
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
        holdCrouch = Convert.ToBoolean(PlayerPrefs.GetInt("CrouchMode"));
        if (!holdCrouch && !context.performed)
        {
            return;
        }
        else if (holdCrouch && context.performed)
        {
            isCrouching = false;
        }
        else if (holdCrouch && context.canceled)
        {
            isCrouching = true;
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