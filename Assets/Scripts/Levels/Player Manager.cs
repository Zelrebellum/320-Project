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
        //gets relevant script references and spawns player at spawn point
        movement = player.GetComponent<PlayerMovement>();
        slowdown = player.GetComponent<PlayerSlowdown>();
        dash = player.GetComponent<dash>();
        playerInput = gameObject.GetComponent<PlayerInput>();

        playerRigidbody = player.GetComponent<Rigidbody>();
        spawnPosition = new Vector3(spawn.position.x, spawn.position.y, spawn.position.z);
        player.transform.position = spawnPosition;
        playerRigidbody.rotation = spawn.rotation;
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
    
    //used to start death sequence by other scripts
    public void Die()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;

        StartCoroutine(DeathSequence());
    }

    //a death sequence that plays, briefly disabling player input and lingering UI elements
    private IEnumerator DeathSequence()
    {
        //disables player movement
        movement.enabled = false;
        playerInput.DeactivateInput();

        //ensures overlay is set to max darkness and stops it from self-updating
        slowdownOverlay.UpdateOverlay(1);
        slowdownOverlay.isActive = false;

        yield return new WaitForSecondsRealtime(1f);


        //play blink transition and respawn player
        blinkTransition.PlayBlink();

        Respawn();

        slowdownOverlay.UpdateOverlay(0);

        //player regains controls after blink transition is complete
        yield return new WaitUntil(() => !BlinkTransition.IsPlaying);

        slowdownOverlay.isActive = true;

        movement.enabled = true;
        playerInput.ActivateInput();

        isDead = false;
    }

    /// <summary>
    /// Resets the Player to the spawn location and to default state
    /// </summary>
    public void Respawn()
    {
        movement.ResetMovement();
        dash.ResetDash();

        playerRigidbody.position = spawn.position;
        playerRigidbody.rotation = spawn.rotation;

        slowdown.ResetSlowdown();
    }

    public void OnReset(InputAction.CallbackContext context)
    {
        if (!context.performed)
        {
            return;
        }
        Respawn();
    }
}
