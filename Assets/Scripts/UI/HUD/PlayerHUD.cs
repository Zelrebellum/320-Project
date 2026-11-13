using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;
using System;

public class PlayerHUD : MonoBehaviour
{
    [Header("HUD References")]
    [SerializeField] private GameObject hudCooldown;
    [SerializeField] private GameObject crouchIcon;

    [FormerlySerializedAs("postureIcon")]
    [SerializeField] private GameObject standIcon;

    [SerializeField] private TextMeshProUGUI timerText;

    [Header("Icon Settings")]
    [SerializeField] private float fadeIconTime = 2f;

    [Header("Times File Name")]
    [SerializeField] private string fileName = "testingData.txt";

    private int playerNumber;


    private float fadeTimer;
    private float timer;
    private Image standImage;

    private void Awake()
    {
        if (standIcon != null)
        {
            standImage = standIcon.GetComponent<Image>();
        }
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timer / 60f);
            int seconds = Mathf.FloorToInt(timer % 60f);

            timerText.text = $"{minutes:00}:{seconds:00}";
        }

        FadeIcon();
    }

    public void ShowCrouch()
    {
        if (crouchIcon != null)
        {
            crouchIcon.SetActive(true);
        }

        if (standIcon != null)
        {
            standIcon.SetActive(false);
        }
    }

    public void ShowStanding()
    {
        if (crouchIcon != null)
        {
            crouchIcon.SetActive(false);
        }

        if (standIcon == null)
        {
            return;
        }

        standIcon.SetActive(true);
        fadeTimer = Mathf.Max(0f, fadeIconTime);

        if (standImage != null)
        {
            Color color = standImage.color;
            color.a = 1f;
            standImage.color = color;
        }
    }

    // Fade out the standing icon
    private void FadeIcon()
    {
        if (standIcon == null || !standIcon.activeSelf)
        {
            return;
        }

        fadeTimer = Mathf.Max(0f, fadeTimer - Time.deltaTime);

        if (fadeTimer <= 0f)
        {
            standIcon.SetActive(false);
            return;
        }

        if (standImage != null)
        {
            Color color = standImage.color;
            color.a = Mathf.Clamp01(fadeTimer / fadeIconTime);
            standImage.color = color;
        }
    }

    // Update the dash cooldown bar
    public void UpdateDashCooldown(float progress)
    {
        if (hudCooldown == null)
        {
            return;
        }

        Vector3 scale = hudCooldown.transform.localScale;
        scale.x = Mathf.Clamp01(progress);
        hudCooldown.transform.localScale = scale;
    }

    public void TrackTimerEnd()
    {
        if (!File.Exists(fileName))
        {
            File.Create(fileName);
        }
        Scene scene = SceneManager.GetActiveScene();
        File.AppendAllText(fileName, String.Format("{0} {1},", scene.name, timer.ToString()));
    }

}