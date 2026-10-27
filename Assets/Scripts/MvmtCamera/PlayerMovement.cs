
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
    [SerializeField] private bool isSprinting = false;
    [SerializeField] private Vector2 moveDirection = Vector2.zero;


    [Header("Jumping")]
    [SerializeField] private float jumpForce = 6f;
    [SerializeField] private LayerMask groundLayer;

    [Header("Crouching")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private float crouchingHeight = 1.5f;

    [Header("HUD")]
    [SerializeField] private PlayerHUD playerHUD;


    // Runtime state
    private float currentSpeed;
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
    private PlayerSlowdown slowdown;
    public bool Airborne => airborne;
    public bool IsCrouching => isCrouching;
    public PlayerHUD UI => playerHUD;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        wall = GetComponent<WallRun>();
        slowdown = GetComponent<PlayerSlowdown>();

        defaultHeight = playerCollider.height;
        defaultCenter = playerCollider.center;
        defaultCameraPosition = playerCamera.localPosition;

        targetSpeed = walkSpeed;
        Application.targetFrameRate = 60;

        holdCrouch = Convert.ToBoolean(PlayerPrefs.GetInt("CrouchMode"));
    }

    private void FixedUpdate()
    {
        airborne = !Physics.Raycast(
            transform.position,
            -transform.up,
            out hitInfo,
            transform.localScale.y + 0.3f,
            groundLayer
        );

        if (isSprinting)
        {
            targetSpeed = sprintSpeed * slowdown.GetSpeedMultiplier();
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
        rb.AddForce(transform.forward * 20.0f, ForceMode.Impulse);
    }

    private void Acceleration()
    {
        if (moveDirection != Vector2.zero)
        {
            if (currentSpeed < targetSpeed)
            {
                currentSpeed += acceleration * Time.deltaTime;
            }
            else if (currentSpeed < topSpeed &&
                     currentSpeed > targetSpeed + 0.2f)
            {
                currentSpeed -= acceleration * 1.5f * Time.deltaTime;
            }
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

        if ((!holdCrouch && !context.performed) || wall.Wallrunning)
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
            Slide();
        }
    }

    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
}
