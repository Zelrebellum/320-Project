using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    /// <summary>
    /// Reference to the player object
    /// </summary>
    [SerializeField] public GameObject player;
    /// <summary>
    /// Reference of the level spawn position 
    /// </summary>
    [SerializeField] Transform spawn;

    private Vector2 spawnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        spawnPosition = new Vector3(spawn.position.x,player.transform.position.y, spawn.position.z);
        player.transform.position = spawnPosition;
    }

    /// <summary>
    /// Resets the Player to the spawn location
    /// </summary>
    public void Respawn()
    {
        player.transform.position = spawnPosition;
    }
}
