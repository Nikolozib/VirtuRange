using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CrosshairColorChanger : MonoBehaviour
{
    private Image crosshairImage;
    void Start()
    {
        // Try to find the crosshair GameObject in the scene
        GameObject crosshairGO = GameObject.Find("Crosshair"); // Make sure your UI object is actually named "Crosshair"

        if (crosshairGO != null)
        {
            crosshairImage = crosshairGO.GetComponent<Image>();

            if (crosshairImage != null)
            {
                Debug.Log("✅ Crosshair Image found on GameObject: " + crosshairGO.name);
                StartCoroutine(DelayedColorSet()); // Apply saved color after short delay
            }
            else
            {
                Debug.LogWarning("⚠️ Crosshair GameObject found, but it has NO Image component!");
            }
        }
        else
        {
            Debug.LogWarning("❌ Crosshair GameObject named 'Crosshair' was not found in this scene.");
        }
    }

    IEnumerator DelayedColorSet()
    {
        yield return new WaitForSeconds(0.1f); // Let scene fully initialize
        string savedColor = PlayerPrefs.GetString("CrosshairColor", "White");
        SetCrosshairColor(savedColor);
    }

    public void SetCrosshairColor(string colorName)
    {
        if (crosshairImage == null)
        {
            Debug.LogWarning("⚠️ No Image reference to set crosshair color.");
            return;
        }

        Color color = Color.white;

        switch (colorName)
        {
            case "Red": color = Color.red; break;
            case "Green": color = Color.green; break;
            case "Blue": color = Color.blue; break;
            case "White": color = Color.white; break;
            default: Debug.LogWarning("Unknown color: " + colorName); break;
        }

        color.a = 1f; // ensure fully visible
        crosshairImage.color = color;
        crosshairImage.canvasRenderer.SetColor(color); // force apply

        PlayerPrefs.SetString("CrosshairColor", colorName);
        Debug.Log("🎯 Crosshair color changed to " + colorName);
    }

    // 🧪 Optional: call this from Unity Inspector to test color change
    public void TestChangeToRed()
    {
        SetCrosshairColor("Red");
    }
}