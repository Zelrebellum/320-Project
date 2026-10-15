using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    /// <summary>
    /// Reference to the player object
    /// </summary>
    [SerializeField] public GameObject player;

    private PlayerMovement movement;
    /// <summary>
    /// Reference of the level spawn position 
    /// </summary>
    [SerializeField] Transform spawn;

    private Vector3 spawnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPosition = new Vector3(spawn.position.x,player.transform.position.y, spawn.position.z);
        player.transform.position = spawnPosition;
        movement = player.GetComponent<PlayerMovement>();
    }

    void Update()
    {
        if (!movement.LevelBoundary.Contains(player.transform.position))
        {
            Debug.Log("works");
            Respawn();
        }
    }

    /// <summary>
    /// Resets the Player to the spawn location
    /// </summary>
    public void Respawn()
    {
        player.transform.position = spawnPosition;
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        player.transform.position = spawnPosition;
    }
}
