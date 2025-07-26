using UnityEngine;
using TMPro;

public class WeaponUIManager : MonoBehaviour
{
    public static WeaponUIManager instance;
    public TMP_Text weaponNameText;
    public TMP_Text ammoText;
    public TMP_Text reloadText;
    

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void UpdateUI(IWeapon weapon)
    {
        if (weapon == null) return;

        weaponNameText.text = weapon.GetWeaponName();
        ammoText.text = $"{weapon.GetCurrentAmmo()} / {weapon.GetMaxAmmo()}";
        reloadText.text = ""; 
    }

    public void ShowReloading()
    {
        reloadText.text = "Reloading...";
    }

    public void HideReloading()
    {
        reloadText.text = "";
    }

    public void HideAmmo()
    {
        weaponNameText.text = "";
        ammoText.text = "";
        reloadText.text = "";
    }
}
