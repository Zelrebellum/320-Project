using UnityEngine;
using UnityEngine.UI;

public class OverlayManager : MonoBehaviour
{
    [Header("Overlay Images")]
    [SerializeField] private Image mudImage;
    [SerializeField] private Image handsImage;
    [SerializeField] private Image darknessImage;

    [Header("Overlay Controller")]
    [SerializeField] private SlowdownOverlay slowdown;

    [Header("Scene References")]
    [SerializeField] private GameObject player;

    /// <summary>
    /// Finds local references and connects the current scene's player.
    /// </summary>
    private void Awake()
    {
        if (slowdown == null)
        {
            slowdown = GetComponentInChildren<SlowdownOverlay>(true);
        }

        if (player == null)
        {
            FindScenePlayer();
        }

        if (slowdown != null)
        {
            slowdown.BindPlayer(player);
        }

        ValidateReferences();
    }

    /// <summary>
    /// Finds the player through a PlayerManager in the same scene.
    /// This allows the UI prefab to avoid storing a scene-specific player.
    /// </summary>
    private void FindScenePlayer()
    {
        PlayerManager[] managers =
            FindObjectsByType<PlayerManager>(FindObjectsSortMode.None);

        foreach (PlayerManager manager in managers)
        {
            if (manager.gameObject.scene != gameObject.scene)
            {
                continue;
            }

            if (manager.player != null)
            {
                player = manager.player;
                return;
            }
        }
    }

    /// <summary>
    /// Connects a player explicitly, such as after replacing the player.
    /// </summary>
    public void BindPlayer(GameObject newPlayer)
    {
        player = newPlayer;

        if (slowdown != null)
        {
            slowdown.BindPlayer(player);
        }
    }

    /// <summary>
    /// Reports missing Inspector references.
    /// </summary>
    private void ValidateReferences()
    {
        if (mudImage == null)
        {
            Debug.LogWarning(
                "OverlayManager: Mud Image is not assigned.", this);
        }

        if (handsImage == null)
        {
            Debug.LogWarning(
                "OverlayManager: Hands Image is not assigned.", this);
        }

        if (darknessImage == null)
        {
            Debug.LogError(
                "OverlayManager: Darkness Image is not assigned.", this);
        }

        if (slowdown == null)
        {
            Debug.LogError(
                "OverlayManager: SlowdownOverlay is missing.", this);
        }

        if (player == null)
        {
            Debug.LogWarning(
                "OverlayManager: No player was found in this scene.", this);
        }
    }

    public Image GetMudImage()
    {
        return mudImage;
    }

    public Image GetHandsImage()
    {
        return handsImage;
    }

    public Image GetDarknessImage()
    {
        return darknessImage;
    }

    public GameObject GetPlayer()
    {
        if (player == null)
        {
            FindScenePlayer();
        }

        return player;
    }
}