
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

    [Header("Mantling")]
    [SerializeField, Range(0, 20)]
    private float climbSpeed;

    [SerializeField] private float scaleTime;

    // Component references
    private PlayerMovement movement;
    private Rigidbody rBody;

    // Runtime state
    private bool climbing;
    private RaycastHit hitInfo;
    private float waitTime;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
        rBody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!movement.Airborne)
        {
            return;
        }

        if (climbing)
        {
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
                rBody.AddForce(
                    transform.forward * 2f,
                    ForceMode.Impulse
                );

                waitTime = scaleTime;
                climbing = false;
            }

            return;
        }

        if (waitTime > 0f)
        {
            waitTime -= Time.deltaTime;
            return;
        }

        bool hit = Physics.Raycast(
            transform.position,
            transform.forward,
            out hitInfo,
            ledgeDetectionDistance,
            groundLayer
        );

        if (!hit)
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
            climbing = true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;

        Vector3 position = transform.position;
        position.y += transform.localScale.y;

        Gizmos.DrawRay(position, transform.forward * ledgeDetectionDistance);
    }
}
