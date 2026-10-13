using UnityEngine;
using TMPro;

public class CrouchSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown crouchDropdown;

    /// <summary>
    /// Load the saved crouch mode when Settings opens
    /// </summary>
    private void OnEnable()
    {
        int mode = PlayerPrefs.GetInt("CrouchMode", 0);

        crouchDropdown.SetValueWithoutNotify(mode);
        crouchDropdown.onValueChanged.AddListener(ChangeCrouchMode);
    }

    /// <summary>
    /// Save when Settings closes
    /// </summary>
    private void OnDisable()
    {
        crouchDropdown.onValueChanged.RemoveListener(ChangeCrouchMode);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// 0 = Hold, 1 = Toggle
    /// </summary>
    /// <param name="mode">user input</param>
    private void ChangeCrouchMode(int mode)
    {
        PlayerPrefs.SetInt("CrouchMode", mode);
    }
}