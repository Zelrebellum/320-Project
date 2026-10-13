using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonHover : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private GameObject hoverCrack;

    /// <summary>
    /// Hide the crack effect when the menu starts
    /// </summary>
    private void Start()
    {
        hoverCrack.SetActive(false);
    }

    /// <summary>
    /// Show the crack when the mouse hover button
    /// </summary>
    /// <param name="eventData">Mouse pointer event data</param>
    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverCrack.SetActive(true);
    }

    /// <summary>
    /// Hide the crackwhen the mouse leaves button
    /// </summary>
    /// <param name="eventData">Mouse pointer event data</param>
    public void OnPointerExit(PointerEventData eventData)
    {
        hoverCrack.SetActive(false);
    }
}