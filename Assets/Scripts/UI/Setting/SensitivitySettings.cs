using UnityEngine;
using UnityEngine.UI;

public class SensitivitySettings : MonoBehaviour
{
    [SerializeField] private Slider sensitivitySlider;

    /// <summary>
    /// Load the saved sensitivity when Settings opens
    /// </summary>
    private void OnEnable()
    {
        float sensitivity =
            PlayerPrefs.GetFloat("MouseSensitivity", 1f);

        sensitivitySlider.SetValueWithoutNotify(sensitivity);
        sensitivitySlider.onValueChanged.AddListener(ChangeSensitivity);
    }

    /// <summary>
    /// Save when Settings closes
    /// </summary>
    private void OnDisable()
    {
        sensitivitySlider.onValueChanged.RemoveListener(ChangeSensitivity);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Store the slider value
    /// </summary>
    /// <param name="sensitivity">sensitivity value</param>
    private void ChangeSensitivity(float sensitivity)
    {
        PlayerPrefs.SetFloat("MouseSensitivity", sensitivity);
    }
}