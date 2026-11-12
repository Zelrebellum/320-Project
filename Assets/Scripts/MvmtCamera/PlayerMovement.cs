
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 8f;
    [SerializeField] private float topSpeed;
    [SerializeField, Range(0, 30)] private float acceleration;
    public float targetSpeed;
    [SerializeField] private bool isSprinting = true;
    [SerializeField] private Vector2 moveDirection = Vector2.zero;


    [Header("Movement Acceleration")]
    [SerializeField] private float groundAcceleration = 25f;
    [SerializeField] private float groundDeceleration = 40f;
    [SerializeField] private float groundTurnAcceleration = 35f;
    [SerializeField] private float airAcceleration = 10f;
    
    [Header("Ground Detection")]
    [SerializeField] private float sphereCastCheckRadius = 0.45f;
    [Tooltip("Distance the sphere cast travels")]
    [SerializeField] private float groundCheckDistance = 0.3f;
    [Tooltip("Offset from player capsule bottom and sphere bottom")]
    [SerializeField] private float groundCheckOffset = 0.15f;

    [SerializeField, Range(0f, 1f)]
    [Tooltip("Acceptable angle for surface to be counted as ground (THIS IS THE COSX VALUE OF DOT PRODUCT)")]
    private float minimumGroundNormal = 0.5f;
    
    [Header("Jumping")]
    [SerializeField] private float jumpForce = 8.5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField, Min(1f)]
    private float risingGravityMultiplier = 1.2f;
    [SerializeField, Min(1f)]
    private float fallingGravityMultiplier = 1.6f;
    private bool wasAirborne;
    private float previousHorizontalSpeed;
    
    [SerializeField, Range(0f, 1f)]
    private float endZoneJumpMultiplier = 0.5f;
    private float normalJumpForce;
    private bool jumpReducedByEndZone = false;

    [Header("Crouching")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float crouchingHeight = 1.5f;

    [Header("HUD")]
    [SerializeField] private PlayerHUD playerHUD;


    // Runtime state
    [SerializeField] private float currentSpeed;
    private bool airborne;
    private bool isCrouching;
    private bool holdCrouch;

    private Vector3 defaultCameraPosition;
    private float defaultHeight;
    private Vector3 defaultCenter;

    private RaycastHit hitInfo;

    private Rigidbody rb;
    private CapsuleCollider playerCollider;
    private WallRun wall;
    private Dash dash;
    private LedgeMantle ledgeMantle;
    private SlothManager slowdown;
    public bool Airborne => airborne;
    public bool IsCrouching => isCrouching;
    public PlayerHUD UI => playerHUD;
    
    private float wrathSpeedMultiplier = 1f;
    private float wrathJumpMultiplier = 1f;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        wall = GetComponent<WallRun>();
        //EDIT BEFORE PUSH
        //slowdown = GetComponent<SlothManager>();
        ledgeMantle = GetComponent<LedgeMantle>();
        dash = GetComponent<Dash>();

        defaultHeight = playerCollider.height;
        defaultCenter = playerCollider.center;
        defaultCameraPosition = playerCamera.localPosition;

        targetSpeed = walkSpeed;
        Application.targetFrameRate = 60;

        holdCrouch = Convert.ToBoolean(PlayerPrefs.GetInt("CrouchMode"));
    }

    private void FixedUpdate()
    {
        Bounds colliderBounds = playerCollider.bounds;

        Vector3 groundCheckOrigin = new Vector3(
            colliderBounds.center.x,
            colliderBounds.min.y + sphereCastCheckRadius + groundCheckOffset,
            colliderBounds.center.z
        );

        bool groundHit = Physics.SphereCast(
            groundCheckOrigin,
            sphereCastCheckRadius,
            Vector3.down,
            out hitInfo,
            groundCheckDistance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        airborne = !groundHit ||
                   Vector3.Dot(hitInfo.normal, Vector3.up) < minimumGroundNormal;
        
        Vector3 velocity = rb.linearVelocity;

        float horizontalSpeed = new Vector3(
            velocity.x,
            0f,
            velocity.z
        ).magnitude;
        
        previousHorizontalSpeed = horizontalSpeed;
        wasAirborne = airborne;
        
        ApplyJumpGravity();

        if (wall.Wallrunning)
        {
            targetSpeed = wall.WallRunSpeed;
        }
        else if (isSprinting)
        {
            targetSpeed = sprintSpeed * (slowdown != null
                ? slowdown.GetSpeedMultiplier()
                : 1f);
        }
        else
        {
            targetSpeed = walkSpeed;
        }

        targetSpeed *= wrathSpeedMultiplier;

        ApplyMovement();
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
        UI.ShowCrouch();
    }

    private void Stand()
    {
        playerCollider.height = defaultHeight;
        playerCollider.center = defaultCenter;

        playerCamera.localPosition = defaultCameraPosition;

        isCrouching = false;
        UI.ShowStanding();
    }

    private void Slide()
    {
        //rb.AddForce(transform.forward * 20.0f, ForceMode.Impulse);
    }

    private void ApplyMovement()
    {
        if (dash.HasDashMomentum)
        {
            return;
        }
        
        Vector3 inputDirection =
            transform.right * moveDirection.x +
            transform.forward * moveDirection.y;

        inputDirection = Vector3.ClampMagnitude(inputDirection, 1f);

        Vector3 velocity = rb.linearVelocity;

        Vector3 horizontalVelocity = new Vector3(
            velocity.x,
            0f,
            velocity.z
        );

        Vector3 desiredVelocity = inputDirection * targetSpeed;

        float accelerationRate;

        if (airborne)
        {
            if (moveDirection.sqrMagnitude < 0.01f)
            {
                accelerationRate = 0f;
            }
            else
            {
                accelerationRate = airAcceleration;
            }
        }
        else if (moveDirection.sqrMagnitude < 0.01f)
        {
            accelerationRate = groundDeceleration;
        }
        else if (horizontalVelocity.sqrMagnitude > 0.01f &&
                 Vector3.Dot(horizontalVelocity, desiredVelocity) < 0f)
        {
            accelerationRate = groundTurnAcceleration;
        }
        else
        {
            accelerationRate = groundAcceleration;
        }

        horizontalVelocity = Vector3.MoveTowards(
            horizontalVelocity,
            desiredVelocity,
            accelerationRate * Time.fixedDeltaTime
        );

        rb.linearVelocity = new Vector3(
            horizontalVelocity.x,
            velocity.y,
            horizontalVelocity.z
        );

        currentSpeed = horizontalVelocity.magnitude;
    }
    
    private void ApplyJumpGravity()
    {
        if (!airborne || wall.Wallrunning || ledgeMantle.IsMantling)
        {
            return;
        }
        
        float verticalVelocity = rb.linearVelocity.y;

        float gravityMultiplier = verticalVelocity > 0f
            ? risingGravityMultiplier
            : fallingGravityMultiplier;

        Vector3 additionalGravity =
            Physics.gravity * (gravityMultiplier - 1f);

        rb.AddForce(additionalGravity, ForceMode.Acceleration);
    }
    
    public void SetEndZoneJump(bool reduced)
    {
        if (jumpReducedByEndZone == reduced)
        {
            return;
        }

        if (reduced)
        {
            normalJumpForce = jumpForce;
            jumpForce *= endZoneJumpMultiplier;
        }
        else
        {
            jumpForce = normalJumpForce;
        }

        jumpReducedByEndZone = reduced;
    }

    public void SetWrathBonuses(float speedMultiplier, float jumpMultiplier)
    {
        wrathSpeedMultiplier = speedMultiplier;
        wrathJumpMultiplier = jumpMultiplier;
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

        float effectiveJumpForce = jumpForce * wrathJumpMultiplier;
        rb.AddForce(transform.up * effectiveJumpForce, ForceMode.Impulse);

        if (wall.Wallrunning)
        {
            wall.StopWallrun();
        }
    }

    public void OnCrouch(InputAction.CallbackContext context)
    {
        // Prevent crouching while wallrunning.
        if (wall.Wallrunning)
        {
            return;
        }

        // 0 = Hold, 1 = Toggle.
        bool toggleCrouch =
            PlayerPrefs.GetInt("CrouchMode", 0) == 1;

        if (toggleCrouch)
        {
            // Toggle only when the button is pressed.
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
                //Slide();
            }
        }
        else
        {
            // Hold to crouch, release to stand.
            if (context.performed && !isCrouching)
            {
                Crouch();
                Slide();
            }
            else if (context.canceled && isCrouching)
            {
                Stand();
            }
        }
    }
    
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}
