
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [Header("Player References")]
    [Tooltip("Player GameObject reference")]
    [SerializeField] private GameObject player;
    public GameObject Player => player;
    [SerializeField] private Transform spawn;

    [Header("Level Boundary")]
    [SerializeField] private MeshRenderer levelBoundary;

    [Header("Transitions and UI")]
    [SerializeField] private BlinkTransition blinkTransition;
    [SerializeField] private SlowdownOverlay slowdownOverlay;
    
    // Player components
    private PlayerMovement movement;
    private SlothManager slowdown;
    private WrathManager wrathManager;
    private WallRun wallRun;
    private Dash dashAbility;
    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;
    private CameraController camControl;
    private SlothDetector slothDetector;

    private CameraController camera;

    // Runtime state
    private bool isDead;

    public bool IsDead => isDead;

    private void Awake()
    {
        if (player == null || spawn == null ||
            levelBoundary == null || blinkTransition == null)
        {
            Debug.LogError("PlayerManager: One or more required references are missing!", this);
            enabled = false;
            return;
        }
        
        movement = player.GetComponent<PlayerMovement>();
        slowdown = GetComponent<SlothManager>();
        wrathManager = gameObject.GetComponent<WrathManager>();
        wallRun = player.GetComponent<WallRun>();
        dashAbility = player.GetComponent<Dash>();
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = player.GetComponent<Rigidbody>();
        camControl = player.GetComponent<CameraController>();
        slothDetector = player.GetComponent<SlothDetector>();
    }

    void Start()
    {
        if (slowdown.isActiveAndEnabled && slowdownOverlay == null)
        {
            Debug.LogError(
                "PlayerManager: Sloth is enabled, but SlowdownOverlay is not assigned!",
                this
            );
            enabled = false;
            return;
        }
        
        player.transform.position = spawn.position;
        playerRigidbody.rotation = spawn.rotation;  
    }

    private void Update()
    {
        if (!isDead && !levelBoundary.bounds.Contains(player.transform.position))
        {
            Respawn();
        }
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
        // Temporarily disable player controls.
        movement.enabled = false;
        playerInput.DeactivateInput();

        // Display maximum slowdown darkness during death.
        if (slowdown.isActiveAndEnabled)
        {
            slowdownOverlay.UpdateOverlay(1f);
            slowdownOverlay.isActive = false;
        }

        yield return new WaitForSecondsRealtime(1f);

        blinkTransition.PlayBlink();
        Respawn();

        if (slowdown.isActiveAndEnabled)
        {
            slowdownOverlay.UpdateOverlay(0f);
        }

        yield return new WaitUntil(() => !BlinkTransition.IsPlaying);

        if (slowdown.isActiveAndEnabled)
        {
            slowdownOverlay.isActive = true;
        }

        movement.enabled = true;
        playerInput.ActivateInput();

        isDead = false;
    }

    public void Respawn()
    {
        if (wallRun.Wallrunning)
        {
            wallRun.StopWallrun();
        }
        movement.ResetMovement();
        dashAbility.ResetDash();

        playerRigidbody.position = spawn.position;
        playerRigidbody.rotation = spawn.rotation;

        if (slowdown.isActiveAndEnabled)
        {
            slothDetector.ResetContacts();
            slowdown.ResetSlowdown();
        }

        if (wrathManager.isActiveAndEnabled)
        {
            wrathManager.ResetWrath();
        }
        
        camControl.ResetFOV();
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
