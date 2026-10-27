using UnityEngine;
using UnityEngine.UI;


public class OverlayManager : MonoBehaviour
{
    [SerializeField] Image mudImage;
    [SerializeField] Image handsImage;
    [SerializeField] Image darknessImage;
    [SerializeField] SlowdownOverlay slowdown;
    [SerializeField] GameObject player;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //if (mudImage == null)
        //{
        //    Debug.LogError("OverlayManager: Mud Image is missing!", this);
        //}

        //if (handsImage == null)
        //{
        //    Debug.LogError("OverlayManager: Hands Image is missing!", this);
        //}

        if (darknessImage == null)
        {
            Debug.LogError("OverlayManager: Darkness Image is missing!", this);
        }

        if (slowdown == null)
        {
            Debug.LogError("OverlayManager: Slowdown Overlay Script is missing!", this);
        }

        if (player == null)
        {
            Debug.LogError("OverlayManager: Player reference is missing!", this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Image GetMudImage()
    {
        return mudImage;
    }

    public Image GetHandsImage()
    {
        return handsImage;
    }

    public Image GetDarknessImage()
    {
        return darknessImage;
    }

    public GameObject GetPlayer()
    {
        return player;
    }
}
