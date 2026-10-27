
using UnityEngine;
using UnityEngine.EventSystems;

public class MenuButtonHover : MonoBehaviour,
    IPointerEnterHandler,
    IPointerExitHandler
{
    [SerializeField] private GameObject hoverEffect;

    private void OnEnable()
    {
        hoverEffect.SetActive(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hoverEffect.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hoverEffect.SetActive(false);
    }
}
