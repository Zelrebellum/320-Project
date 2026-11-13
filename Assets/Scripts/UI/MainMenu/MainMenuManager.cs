using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject optionsPanel;
    [SerializeField] private GameObject mainMenuContent;

    [Header("Title Animation")]
    [Tooltip("The TextMeshPro title displayed in the main menu.")]
    [SerializeField] private TMP_Text titleText;

    [Tooltip("The parent containing the four UI fragments.")]
    [SerializeField] private RectTransform titleFragments;

    [Tooltip("Time spent scattering the title before fading to black.")]
    [SerializeField] private float scatterDuration = 1.5f;

    [Tooltip("How far the letters and fragments travel in UI units.")]
    [SerializeField] private float scatterDistance = 450f;

    [Header("Scene Transition")]
    [Tooltip("CanvasGroup used to fade the screen to black.")]
    [SerializeField] private CanvasGroup fadePanel;

    [SerializeField] private float fadeDuration = 0.8f;

    // Prevent repeated clicks from starting multiple transitions.
    private bool isTransitioning;

    private void Start()
    {
        // Keep the black overlay invisible while the menu is open.
        if (fadePanel != null)
        {
            fadePanel.alpha = 0f;
            fadePanel.blocksRaycasts = false;
            fadePanel.interactable = false;
        }
    }

    public void StartGame()
    {
        if (isTransitioning)
        {
            return;
        }

        isTransitioning = true;

        // Use the transparent overlay to block further menu clicks.
        if (fadePanel != null)
        {
            fadePanel.blocksRaycasts = true;
        }

        StartCoroutine(ScatterAndLoadGame());
    }

    private IEnumerator ScatterAndLoadGame()
    {
        // Finish the scatter animation before starting the black fade.
        yield return ScatterTitle();

        yield return FadeAndLoadGame();
    }

    private IEnumerator ScatterTitle()
    {
        CanvasGroup titleGroup = null;
        TMP_MeshInfo[] originalMesh = null;
        Vector3[] letterOffsets = null;
        float startingTitleAlpha = 1f;

        if (titleText != null)
        {
            // Stop the opening fade so it cannot compete with this animation.
            TitleSequence intro = titleText.GetComponent<TitleSequence>();

            if (intro != null)
            {
                intro.StopAllCoroutines();
            }

            titleGroup = titleText.GetComponent<CanvasGroup>();

            if (titleGroup != null)
            {
                startingTitleAlpha = titleGroup.alpha;
            }

            // Build the text mesh and keep an untouched copy of its vertices.
            titleText.ForceMeshUpdate();
            originalMesh = titleText.textInfo.CopyMeshInfoVertexData();

            int characterCount = titleText.textInfo.characterCount;
            letterOffsets = new Vector3[characterCount];

            for (int i = 0; i < characterCount; i++)
            {
                // Spread letters to either side with a slight upward motion.
                float side = i < characterCount * 0.5f ? -1f : 1f;

                letterOffsets[i] = new Vector3(
                    side * Random.Range(0.6f, 1.2f),
                    Random.Range(0.2f, 0.8f),
                    0f
                ) * scatterDistance;
            }
        }

        // Collect the existing fragments without creating extra objects.
        TitleFragment[] fragments = titleFragments != null
            ? titleFragments.GetComponentsInChildren<TitleFragment>()
            : new TitleFragment[0];

        RectTransform[] fragmentRects = new RectTransform[fragments.Length];
        CanvasGroup[] fragmentGroups = new CanvasGroup[fragments.Length];
        Vector2[] fragmentStarts = new Vector2[fragments.Length];
        Vector2[] fragmentOffsets = new Vector2[fragments.Length];
        Quaternion[] fragmentRotations = new Quaternion[fragments.Length];
        float[] fragmentAlphas = new float[fragments.Length];

        for (int i = 0; i < fragments.Length; i++)
        {
            // Stop the falling loop and continue from the current position.
            fragments[i].StopAllCoroutines();

            fragmentRects[i] = fragments[i].GetComponent<RectTransform>();
            fragmentGroups[i] = fragments[i].GetComponent<CanvasGroup>();

            fragmentStarts[i] = fragmentRects[i].anchoredPosition;
            fragmentRotations[i] = fragmentRects[i].localRotation;

            fragmentAlphas[i] = fragmentGroups[i] != null
                ? fragmentGroups[i].alpha
                : 1f;

            // Push fragments away from the center of the title.
            float side = fragmentStarts[i].x < 0f ? -1f : 1f;

            fragmentOffsets[i] = new Vector2(
                side * Random.Range(0.8f, 1.4f),
                Random.Range(0.3f, 1f)
            ) * scatterDistance;
        }

        float elapsed = 0f;
        float duration = Mathf.Max(0.01f, scatterDuration);

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress = Mathf.Clamp01(elapsed / duration);

            // Start slowly, then accelerate as the pieces fly away.
            float travel = progress * progress;

            // Keep the pieces visible at first, then gradually fade them out.
            float opacity = 1f - Mathf.SmoothStep(
                0f,
                1f,
                Mathf.InverseLerp(0.25f, 1f, progress)
            );

            if (titleText != null && originalMesh != null)
            {
                TMP_TextInfo textInfo = titleText.textInfo;

                for (int i = 0; i < textInfo.characterCount; i++)
                {
                    TMP_CharacterInfo character = textInfo.characterInfo[i];

                    // Spaces and other invisible characters have no visible quad.
                    if (!character.isVisible)
                    {
                        continue;
                    }

                    int meshIndex = character.materialReferenceIndex;
                    int vertexIndex = character.vertexIndex;

                    Vector3[] source = originalMesh[meshIndex].vertices;
                    Vector3[] destination = textInfo.meshInfo[meshIndex].vertices;

                    // Move all four corners together to preserve each letter.
                    for (int corner = 0; corner < 4; corner++)
                    {
                        int index = vertexIndex + corner;
                        destination[index] =
                            source[index] + letterOffsets[i] * travel;
                    }
                }

                // Send the modified letter positions to the rendered text.
                titleText.UpdateVertexData(TMP_VertexDataUpdateFlags.Vertices);

                if (titleGroup != null)
                {
                    titleGroup.alpha = startingTitleAlpha * opacity;
                }
            }

            for (int i = 0; i < fragmentRects.Length; i++)
            {
                fragmentRects[i].anchoredPosition =
                    fragmentStarts[i] + fragmentOffsets[i] * travel;

                // Add a gentle spin while each fragment moves outward.
                fragmentRects[i].localRotation =
                    fragmentRotations[i] *
                    Quaternion.Euler(0f, 0f, 180f * travel);

                if (fragmentGroups[i] != null)
                {
                    // Hidden fragments stay hidden instead of suddenly appearing.
                    fragmentGroups[i].alpha = fragmentAlphas[i] * opacity;
                }
            }

            yield return null;
        }
    }

    private IEnumerator FadeAndLoadGame()
    {
        if (fadePanel != null)
        {
            float elapsed = 0f;
            float duration = Mathf.Max(0.01f, fadeDuration);

            // Fade the screen to black using time unaffected by timeScale.
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                fadePanel.alpha = Mathf.Clamp01(elapsed / duration);

                yield return null;
            }

            fadePanel.alpha = 1f;
        }

        // Restore normal game speed before entering the level.
        Time.timeScale = 1f;
        SceneManager.LoadScene("Sloth");
    }

    public void OpenOptions()
    {
        // Ignore menu actions after the start transition has begun.
        if (isTransitioning)
        {
            return;
        }

        mainMenuContent.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        mainMenuContent.SetActive(true);
    }

    public void OpenCredits()
    {
        if (isTransitioning)
        {
            return;
        }

        Time.timeScale = 1f;
        SceneManager.LoadScene("EndingCredits");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }
}