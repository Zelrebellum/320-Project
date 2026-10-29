using UnityEngine;
using UnityEngine.UI;

public class SensitivitySettings : MonoBehaviour
{
    [SerializeField] private Slider sensitivitySlider;

    private CameraController cameraController;

    /// <summary>
    /// Load the saved sensitivity when Settings opens
    /// </summary>
    private void OnEnable()
    {
        float sensitivity = PlayerPrefs.GetFloat("MouseSensitivity", 5f);

        sensitivitySlider.SetValueWithoutNotify(sensitivity);
        sensitivitySlider.onValueChanged.AddListener(ChangeSensitivity);

        cameraController = FindAnyObjectByType<CameraController>();
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

        if (cameraController == null)
        {
            cameraController = FindAnyObjectByType<CameraController>();
        }

        if (cameraController != null)
        {
            cameraController.ChangeSensitivity(sensitivity);
        }
    }
}