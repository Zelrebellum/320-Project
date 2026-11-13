using UnityEngine;
using UnityEngine.SceneManagement;

public enum LevelType
{
    Sloth,
    Wrath
}

public class LevelManager : MonoBehaviour
{
    [Header("Level Configuration")]
    [SerializeField] private LevelType levelType;
    [SerializeField] private string nextSceneName;

    [Header("References")]
    [SerializeField] private GameObject slothOverlay;
    [SerializeField] private GameObject wrathHUD;
    
    private bool isTransitioning;
    private SlothManager slothManager;
    private WrathManager wrathManager;
    private SlothDetector  slothDetector;

    private void Awake()
    {
        slothDetector = gameObject.GetComponent<PlayerManager>().Player.GetComponent<SlothDetector>();
        wrathManager = GetComponent<WrathManager>();
        slothManager = GetComponent<SlothManager>();
        
        if (levelType == LevelType.Sloth && slothOverlay == null)
        {
            Debug.LogError("LevelManager: Sloth overlay reference is missing!", this);
            enabled = false;
            return;
        }
        
        if (levelType == LevelType.Wrath && wrathHUD == null)
        {
            Debug.LogError("LevelManager: Wrath HUD reference is missing!", this);
            enabled = false;
            return;
        }

        ConfigureLevel();
    }

    private void ConfigureLevel()
    {
        DisableLevelMechanics();

        switch (levelType)
        {
            case LevelType.Sloth:
                EnableSloth();
                break;

            case LevelType.Wrath:
                EnableWrath();
                break;
        }
    }

    private void DisableLevelMechanics()
    {
        slothManager.enabled = false;
        slothDetector.enabled = false;
        wrathManager.enabled = false;
        slothOverlay?.SetActive(false);
        wrathHUD?.SetActive(false);
    }

    private void EnableSloth()
    {
        slothManager.enabled = true;
        slothDetector.enabled = true;
        slothOverlay.SetActive(true);
    }

    private void EnableWrath()
    {
        wrathManager.enabled = true;
        wrathHUD.SetActive(true);
    }
    
    public void CompleteLevel()
    {
        if (isTransitioning)
        {
            return;
        }
        
        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogError("LevelManager: Next scene name is missing!", this);
            return;
        }

        if (!Application.CanStreamedLevelBeLoaded(nextSceneName))
        {
            Debug.LogError($"LevelManager: Next scene '{nextSceneName}' cannot be loaded!", this);
            return;
        }
        
        isTransitioning = true;
        SceneManager.LoadScene(nextSceneName);
    }
}
