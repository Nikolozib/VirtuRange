using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    public GameObject arWeapon;
    public GameObject pistolWeapon;

    private void Start()
    {
        SetWeapon(arWeapon, pistolWeapon); // Start with AR
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetWeapon(arWeapon, pistolWeapon);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetWeapon(pistolWeapon, arWeapon);
        }
    }

    private void SetWeapon(GameObject enableObj, GameObject disableObj)
    {
        enableObj.SetActive(true);
        disableObj.SetActive(false);
    }
}
