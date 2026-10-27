
using UnityEngine;
using UnityEngine.InputSystem;

public class WallRun : MonoBehaviour
{
    [Header("Wall Detection")]
    [SerializeField] private LayerMask wallJumpLayer;
    [SerializeField] private float wallDetectionDistance = 1.3f;

    [Header("Wall Run Movement")]
    [SerializeField] private float wallRunSpeed;
    [SerializeField] private float speedMinimum;
    [SerializeField] private float maxWallRunTime;

    [Tooltip("Maximum vertical distance the player can travel during a wall run.")]
    [SerializeField, Range(0, 5)] private float maximumHeight;

    [Header("Camera Effects")]
    [SerializeField] private int wallRunFOV;
    [SerializeField] private float FOVfade;

    // Component references
    private PlayerMovement movement;
    private Rigidbody rBody;
    private CameraController camControl;

    // Runtime state
    private bool wallrunning;
    private float timer;
    private RaycastHit hitInfo;
    private float currentAngle;

    private bool leftWall;
    private bool rightWall;
    private bool onWall = true;

    private Vector3 positionCheck = Vector3.zero;
    private Vector3 rayDirection = Vector3.zero;

    private float initialHeight;
    public bool atHeight = false;

    public bool Wallrunning => wallrunning;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
        rBody = GetComponent<Rigidbody>();
        camControl = GetComponent<CameraController>();
    }

    private void Update()
    {
        if (!wallrunning)
        {
            rBody.constraints = (RigidbodyConstraints)112;
            currentAngle = 0;
            rBody.useGravity = true;
            return;
        }

        if (timer < 0 ||
            movement.GetCurrentSpeed() < speedMinimum ||
            !onWall)
        {
            StopWallrun();
            return;
        }

        timer -= Time.deltaTime;
        rBody.useGravity = false;

        Quaternion rotation = Quaternion.Euler(
            new Vector3(
                transform.rotation.eulerAngles.x,
                transform.rotation.eulerAngles.y,
                currentAngle
            )
        );

        transform.rotation = rotation;

        movement.targetSpeed = wallRunSpeed;

        positionCheck = transform.position;
        positionCheck.y -= transform.localScale.y;

        int side = leftWall ? -1 : 1;

        rayDirection = -transform.up;
        rayDirection = new Vector3(
            (rayDirection.x + transform.right.x * side) / 2.0f,
            (rayDirection.y + transform.right.y * side) / 2.0f,
            (rayDirection.z + transform.right.z * side) / 2.0f
        );

        onWall = Physics.Raycast(
            positionCheck,
            rayDirection,
            out hitInfo,
            wallDetectionDistance,
            wallJumpLayer
        );

        if (transform.position.y < initialHeight + maximumHeight)
        {
            rBody.constraints = (RigidbodyConstraints)112;
            atHeight = false;
            return;
        }

        atHeight = true;
        rBody.constraints = (RigidbodyConstraints)116;
    }

    public void WallJump(InputAction.CallbackContext context)
    {
        if (!context.performed ||
            wallrunning ||
            movement.Airborne ||
            movement.IsCrouching)
        {
            return;
        }

        Vector3 left =
            Quaternion.AngleAxis(-75.0f, Vector3.up) *
            transform.forward * 1.5f;

        Vector3 right =
            Quaternion.AngleAxis(75.0f, Vector3.up) *
            transform.forward * 1.5f;

        leftWall = Physics.Raycast(
            transform.position,
            left,
            out hitInfo,
            wallDetectionDistance,
            wallJumpLayer
        );

        rightWall = Physics.Raycast(
            transform.position,
            right,
            out hitInfo,
            wallDetectionDistance,
            wallJumpLayer
        );

        currentAngle = 15 * (leftWall ? -1 : 1);

        if (!leftWall && !rightWall)
        {
            return;
        }

        initialHeight = transform.position.y;
        wallrunning = true;
        rBody.useGravity = false;
        timer = maxWallRunTime;

        camControl.EnableFOV(wallRunFOV, 75f);
    }

    public void StopWallrun()
    {
        timer = maxWallRunTime;
        wallrunning = false;

        Quaternion rotation = Quaternion.Euler(
            new Vector3(
                0f,
                transform.rotation.eulerAngles.y,
                0f
            )
        );

        transform.rotation = rotation;
        onWall = true;

        camControl.EnableFOV(camControl.StartFOV, FOVfade);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 left =
            Quaternion.AngleAxis(-75.0f, Vector3.up) *
            transform.forward * 1.5f;

        Gizmos.DrawRay(transform.position, left);

        Gizmos.color = Color.green;

        Gizmos.DrawRay(
            transform.position,
            Quaternion.AngleAxis(75.0f, Vector3.up) *
            transform.forward * 1.5f
        );

        Gizmos.DrawRay(positionCheck, rayDirection * wallDetectionDistance);
    }
}
