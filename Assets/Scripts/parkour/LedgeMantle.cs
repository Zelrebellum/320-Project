
using UnityEngine;

public class LedgeMantle : MonoBehaviour
{
    [Header("Ledge Detection")]
    [SerializeField] private LayerMask groundLayer;

    [Tooltip("detection distance")]
    [SerializeField] private float ledgeDetectionDistance = 2f;
    [Tooltip("maximum ledge height from player (max height player can mantle")]
    [SerializeField] private float maxLedgeHeight = 10f;
    [Tooltip("minimum ledge height (height difference between player and ledge must be at least this value)")]
    [SerializeField] private float minLedgeHeight = 0f;
    [SerializeField]
    private float maximumMantleFallSpeed = 3f;

    [Header("Mantling")]
    [SerializeField, Range(0, 20)]
    private float climbSpeed;
    private Vector3 approachVelocity;
    [SerializeField, Range(0f, 1f)]
    private float momentumRetention = 0.8f;

    [SerializeField] private float scaleCooldown = 0.5f;

    // Component references
    private PlayerMovement movement;
    private Rigidbody rBody;
    
    private bool climbing;
    public bool IsMantling => climbing;
    private RaycastHit hitInfo;
    private float waitTime;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
        rBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (climbing)
        {
            rBody.useGravity = false;
            Vector3 position = transform.position;

            float bottom = transform.position.y - transform.localScale.y;

            if (bottom < hitInfo.collider.bounds.center.y +
                         hitInfo.collider.bounds.size.y / 2f)
            {
                position.y += climbSpeed * Time.deltaTime;
                rBody.MovePosition(position);
            }
            else
            {
                Vector3 exitVelocity = approachVelocity * momentumRetention;

                rBody.linearVelocity = new Vector3(
                    exitVelocity.x,
                    rBody.linearVelocity.y,
                    exitVelocity.z
                );

                waitTime = scaleCooldown;
                climbing = false;
                rBody.useGravity = true;
                Debug.Log("Mantling finished");
            }
            return;
        }
        
        if (waitTime > 0f)
        {
            waitTime -= Time.deltaTime;
            return;
        }
        
        if (!movement.Airborne)
        {
            return;
        }
        
        if (rBody.linearVelocity.y < -maximumMantleFallSpeed)
        {
            return;
        }
        
        bool hit = Physics.Raycast(
            transform.position,
            transform.forward,
            out hitInfo,
            ledgeDetectionDistance,
            groundLayer,
            QueryTriggerInteraction.Ignore
        );

        if (!hit)
        {
            return;
        }
        if (!hitInfo.collider.CompareTag("Mantle") && !hitInfo.collider.CompareTag("WallrunMantle"))
        {
            return;
        }

        float ledge = Mathf.Abs(
            (hitInfo.collider.bounds.center.y +
             hitInfo.collider.bounds.size.y / 2f) -
            transform.position.y
        );
        
        if (ledge <= maxLedgeHeight && ledge > minLedgeHeight)
        {
            Debug.Log("Climbing");
            climbing = true;
            rBody.useGravity = false;
            Vector3 velocity = rBody.linearVelocity;
            approachVelocity = new  Vector3(velocity.x, 0, velocity.z);
            rBody.linearVelocity = Vector3.zero;
            Debug.Log("Mantling started");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Vector3 position = transform.position;
        //position.y += transform.localScale.y * 3/4;

        Gizmos.DrawRay(position, transform.forward);
    }
}
