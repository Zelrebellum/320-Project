using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    /// <summary>
    /// Reference to the player object
    /// </summary>
    [SerializeField] public GameObject player;
    private PlayerMovement movement;
    private PlayerSlowdown slowdown;
    private dash dash;
    private PlayerInput playerInput;
    private bool isDead = false;

    // public bool IsDead {

    // }
    /// <summary>
    /// Reference of the level spawn position 
    /// </summary>
    [SerializeField] Transform spawn;

    private Vector3 spawnPosition;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    
    
    void Start()
    {
        movement = player.GetComponent<PlayerMovement>();
        slowdown = player.GetComponent<PlayerSlowdown>();
        dash = player.GetComponent<dash>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        spawnPosition = new Vector3(spawn.position.x, spawn.position.y, spawn.position.z);
        player.transform.position = spawnPosition;
    }

    void Die()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;
    }

    private IEnumerator DeathSequence()
    {
        movement.enabled = false;
        playerInput.DeactivateInput();
        //slowdownOverlay.IsActive = false
        Respawn();
        //ResetOverlay()

        yield return new WaitUntil(() => !BlinkTransition.IsPlaying);
        playerInput.ActivateInput();
        movement.enabled = true;
    }

    /// <summary>
    /// Resets the Player to the spawn location
    /// </summary>
    public void Respawn()
    {
        player.transform.position = spawnPosition;
    }
}
