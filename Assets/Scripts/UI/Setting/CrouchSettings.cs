
using UnityEngine;
using TMPro;

public class CrouchSettings : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown crouchDropdown;

    // Load the saved crouch mode when Settings opens.
    private void OnEnable()
    {
        int mode = PlayerPrefs.GetInt("CrouchMode", 0);

        crouchDropdown.SetValueWithoutNotify(mode);
        crouchDropdown.onValueChanged.AddListener(ChangeCrouchMode);
    }

    private void OnDisable()
    {
        crouchDropdown.onValueChanged.RemoveListener(ChangeCrouchMode);
        PlayerPrefs.Save();
    }

    private void ChangeCrouchMode(int mode)
    {
        // 0 = Hold, 1 = Toggle
        PlayerPrefs.SetInt("CrouchMode", mode);
    }
}
