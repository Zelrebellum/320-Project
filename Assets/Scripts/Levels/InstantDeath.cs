using UnityEngine;



public class InstantDeath : MonoBehaviour
{
    /// <summary>
    /// Manager houses the player information
    /// </summary>
    [SerializeField] GameObject manager;

    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {

    }



    /// <summary>
    /// Respawns the player when collided with the death wall
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
