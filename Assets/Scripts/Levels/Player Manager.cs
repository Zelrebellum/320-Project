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

    public bool IsDead {
        get { return isDead; }
    }

    [SerializeField] private SlowdownOverlay slowdownOverlay;

    [SerializeField] private BlinkTransition blinkTransition;
    Rigidbody playerRigidbody;
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

        playerRigidbody = player.GetComponent<Rigidbody>();
        spawnPosition = new Vector3(spawn.position.x, spawn.position.y, spawn.position.z);
        player.transform.position = spawnPosition;
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;

        StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        movement.enabled = false;
        playerInput.DeactivateInput();

        slowdownOverlay.UpdateOverlay(1);
        slowdownOverlay.isActive = false;

        yield return new WaitForSecondsRealtime(1f);


        blinkTransition.PlayBlink();

        Respawn();

        slowdownOverlay.UpdateOverlay(0);

        yield return new WaitUntil(() => !BlinkTransition.IsPlaying);

        slowdownOverlay.isActive = true;

        movement.enabled = true;
        playerInput.ActivateInput();

        isDead = false;
    }

    /// <summary>
    /// Resets the Player to the spawn location
    /// </summary>
    public void Respawn()
    {
        movement.ResetMovement();
        dash.ResetDash();

        playerRigidbody.position = spawn.position;
        playerRigidbody.rotation = spawn.rotation;

        slowdown.ResetSlowdown();
    }
}
