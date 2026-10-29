using UnityEngine;

public class GroundSnapping : MonoBehaviour
{
    private LayerMask groundLayerMask;

    [SerializeField]
    private float yOffset = 1;

    [SerializeField] private float snapDistance = 1.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    private void Update()
    {
        if (Physics.Raycast(
                transform.position,
                Vector3.down,
                out RaycastHit groundHit,
                snapDistance,
                groundLayerMask))
        {
            Vector3 alignedForward =
                Vector3.Cross(transform.right, groundHit.normal);

            transform.LookAt(
                transform.position + alignedForward,
                groundHit.normal
            );

            Vector3 snapPosition = transform.position;
            snapPosition.y = groundHit.point.y + yOffset;

            transform.position = snapPosition;
        }
        else if (Physics.Raycast(
                     transform.position,
                     Vector3.down,
                     Mathf.Infinity,
                     groundLayerMask))
        {
            Vector3 alignedForward =
                Vector3.Cross(transform.right, Vector3.up);

            transform.LookAt(
                transform.position + alignedForward,
                Vector3.up
            );
        }
    }
}
