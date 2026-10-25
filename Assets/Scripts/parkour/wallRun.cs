using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class wallRun : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private LayerMask wallJumpLayer;

    [SerializeField] private float wallJumpDistance = 1.3f;

    [SerializeField] private bool wallrunning;

    public bool Wallrunning
    {
        get => wallrunning;
    }

    [SerializeField] private float increaseSpeed;

    [SerializeField] private float speedMinimum;

    [SerializeField] private float wallRunTime;
    
    [SerializeField] private float timer;
    private PlayerMovement movement;
    private Rigidbody rBody;

    private CameraController camControl;
    private RaycastHit hitInfo;
    private float currentAngle = 0;

    private bool leftWall = false;
    private bool rightWall = false;

    private bool onWall = true;

    [SerializeField] private int wallRunFOV;
    [SerializeField] private float FOVfade;
    Vector3 positionCheck = Vector3.zero;
    Vector3 rayDirection = Vector3.zero;

    [SerializeField, Range(0, 5)] private float maximumHeight;

    public bool atHeight = false;

    private float initialHeight = 0;

    void Start()
    {
        movement = gameObject.GetComponent<PlayerMovement>();
        rBody = gameObject.GetComponent<Rigidbody>();
        camControl = gameObject.GetComponent<CameraController>();
    }

    // Update is called once per frame
    void Update()
    {
        // if we're not wall running, skip everything
        if (!wallrunning)
        {
            rBody.constraints = (RigidbodyConstraints) 112;
            currentAngle = 0;
            rBody.useGravity = true;
            return;
        }

        if (
            timer < 0 || 
            movement.GetCurrentSpeed() < speedMinimum || 
            !onWall )
        {
            StopWallrun();
            return;
        }

        timer -= Time.deltaTime;
        rBody.useGravity = false;
        // use hitInfo.normal to do what u nee
        Quaternion rotation = Quaternion.Euler(
            new Vector3(transform.rotation.eulerAngles.x,
                        transform.rotation.eulerAngles.y,
                        currentAngle));
        
        transform.rotation = rotation;

        movement.targetSpeed = increaseSpeed;

        positionCheck = transform.position;
        positionCheck.y -= transform.localScale.y;

        int side = leftWall ? -1: 1;

        rayDirection = -transform.up;
        rayDirection = new Vector3((rayDirection.x + transform.right.x * side)/ 2.0f, (rayDirection.y + transform.right.y * side)/ 2.0f, (rayDirection.z + transform.right.z * side)/ 2.0f);
        onWall = Physics.Raycast(positionCheck, rayDirection, out hitInfo, wallJumpDistance, wallJumpLayer);

        if (transform.position.y < initialHeight + maximumHeight)
        {
            rBody.constraints = (RigidbodyConstraints) 112;
            atHeight = false;
            return;
        }
        atHeight = true;

        // I HATE C# WITH EVERY FIBER IN MY BEING WHAT DO YOU MEAN I CANT ADD ENUMS THEY ARE INTS 
        // OH MY GOD WHY DO I HAVE TO CAST IT ITS AN INT OHHHHHHHHHH MY GODDDDDDDDDDDDDDDDDDDDDDDDDDD
        rBody.constraints = (RigidbodyConstraints) 116;
    }

    // list for what i need to do to implement this 4 now
    // input check for jump
    // check angles ~45 degrees to the left and right of player hull for a runnable wall layer game object
    // if hit, get player on the wall + at an angle, stop checking for gravity
    // if the player stops holding w, check for gravity and right them
    // at the end of the run (abt ~2 seconds) drop them
    // if at any time during the run player presses space, have them jump
    // increase player speed a lil 2 make them feel awesome and amazing
    // roll when landing? maybe? if i can do that ?

    public void WallJump(InputAction.CallbackContext context)
    {
        if (!context.performed 
        || wallrunning 
        || movement.Airborne
        || movement.IsCrouching)
        {
            return;
        } 

        Vector3 left = Quaternion.AngleAxis(-75.0f, Vector3.up) * transform.forward * 1.5f;
        Vector3 right = Quaternion.AngleAxis(75.0f, Vector3.up) * transform.forward * 1.5f;

        leftWall = Physics.Raycast(transform.position, left, out hitInfo, wallJumpDistance, wallJumpLayer);
        rightWall = Physics.Raycast(transform.position, right, out hitInfo, wallJumpDistance, wallJumpLayer);

        currentAngle = 15 * (leftWall ? -1 : 1);

        if (!leftWall && !rightWall)
        {
            return;
        }
        initialHeight = transform.position.y;
        wallrunning = true;
        rBody.useGravity = false;
        timer = wallRunTime;
        camControl.EnableFOV(wallRunFOV, 75f);
    }

    public void StopWallrun()
    {
        timer = wallRunTime;
        wallrunning = false;
        Quaternion rotation = Quaternion.Euler(
        new Vector3(0f,
                    transform.rotation.eulerAngles.y,
                    0f));
        transform.rotation = rotation;
        onWall = true;
        camControl.EnableFOV(camControl.StartFOV, FOVfade);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Vector3 left = Quaternion.AngleAxis(-75.0f, Vector3.up) * transform.forward * 1.5f;

        Gizmos.DrawRay(transform.position, left);
        
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, Quaternion.AngleAxis(75.0f, Vector3.up) * transform.forward * 1.5f);

        Gizmos.DrawRay(positionCheck, rayDirection * wallJumpDistance);

    }

}
