using UnityEngine;
using UnityEngine.UI;

public class CrosshairUpdater : MonoBehaviour
{
    public Sprite newCrosshairSprite;

    void Start()
    {
        Image crosshairImage = GetComponent<Image>();
        if (crosshairImage != null && newCrosshairSprite != null)
        {
            crosshairImage.sprite = newCrosshairSprite;
        }
    }
}