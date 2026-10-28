
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using System.Collections.Generic;

public class CustomCursor : MonoBehaviour
{
    [SerializeField] private Texture2D normalCursor;
    [SerializeField] private Texture2D hoverCursor;

    private Vector2 hotspot = new Vector2(5, 3);
    private bool isHovering = false;

    private void Start()
    {
        SetCursor(false);
    }

    private void Update()
    {
        if (EventSystem.current == null || Mouse.current == null)
            return;

        // Get mouse position using New Input System
        Vector2 mousePosition = Mouse.current.position.ReadValue();

        PointerEventData pointerData =
            new PointerEventData(EventSystem.current);

        pointerData.position = mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();

        EventSystem.current.RaycastAll(pointerData, results);

        bool hovering = false;

        // Check if the mouse is over any UI Button
        foreach (RaycastResult result in results)
        {
            Button button =
                result.gameObject.GetComponentInParent<Button>();

            if (button != null && button.IsActive() && button.IsInteractable())
            {
                hovering = true;
                break;
            }
        }

        // Only change cursor when hover state changes
        if (hovering != isHovering)
        {
            isHovering = hovering;
            SetCursor(isHovering);
        }
    }

    private void SetCursor(bool hover)
    {
        Texture2D texture = hover ? hoverCursor : normalCursor;

        if (texture != null)
        {
            Cursor.SetCursor(texture, hotspot, CursorMode.Auto);
        }
    }
}
