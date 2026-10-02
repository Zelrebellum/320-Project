using UnityEngine;

public class DeathwallManager : MonoBehaviour
{
    /// <summary>
    /// Manager houses the player information
    /// </summary>
    [SerializeField] GameObject manager;
    [SerializeField] Transform startPos;
    [SerializeField] Transform endPos;
    [SerializeField, Range(0.0f, 1.0f)] float intensity;
    [SerializeField] bool isLooping;

    /// <summary>
    /// Becausing start position can be tied to the deathwalls position, we keep a separate reference
    /// </summary>
    private Vector3 startPosPure;

    void Start()
    {
        this.transform.position = startPos.position;
        startPosPure = startPos.position;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Movement();
    }

    /// <summary>
    /// Moves the wall in it's forward direction (assuming z axis)
    /// </summary>
    private void Movement()
    {
        if(this.gameObject.transform.position.z < endPos.position.z)
        {
            this.gameObject.transform.position += (this.gameObject.transform.forward*intensity);
        }
        else if (isLooping)
        {
            this.gameObject.transform.position = startPosPure;
        }
    }

    /// <summary>
    /// Respawns the player when collided with the death wall
    /// TODO
    /// NEEDS TO BE TESTED WITH PLAYER OBJECT
    /// </summary>
    /// <param name="collision">The collided object</param>
    public void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == manager.GetComponent<PlayerManager>().player)
        {
            manager.GetComponent<PlayerManager>().Respawn();
        }
    }
}
