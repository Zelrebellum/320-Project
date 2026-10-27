
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    [Header("Player References")]
    [Tooltip("Player GameObject reference")]
    [SerializeField] private GameObject player;
    [SerializeField] private Transform spawn;

    [Header("Level Boundary")]
    [SerializeField] private MeshRenderer levelBoundary;

    [Header("Transitions and UI")]
    [SerializeField] private BlinkTransition blinkTransition;
    [SerializeField] private SlowdownOverlay slowdownOverlay;

    // Player components
    private PlayerMovement movement;
    private PlayerSlowdown slowdown;
    private WallRun wallRun;
    private Dash dashAbility;
    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;

    // Runtime state
    private bool isDead;

    public bool IsDead => isDead;

    private void Start()
    {
        movement = player.GetComponent<PlayerMovement>();
        slowdown = player.GetComponent<PlayerSlowdown>();
        wallRun = player.GetComponent<WallRun>();
        dashAbility = player.GetComponent<Dash>();
        playerInput = GetComponent<PlayerInput>();
        playerRigidbody = player.GetComponent<Rigidbody>();

        player.transform.position = spawn.position;
    }

    private void Update()
    {
        if (!levelBoundary.bounds.Contains(player.transform.position))
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
        slowdownOverlay.UpdateOverlay(1f);
        slowdownOverlay.isActive = false;

        yield return new WaitForSecondsRealtime(1f);

        blinkTransition.PlayBlink();
        Respawn();

        slowdownOverlay.UpdateOverlay(0f);

        yield return new WaitUntil(() => !BlinkTransition.IsPlaying);

        slowdownOverlay.isActive = true;

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
