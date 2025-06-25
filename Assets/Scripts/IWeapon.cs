public interface IWeapon
{
    string GetWeaponName();
    int GetCurrentAmmo();
    int GetMaxAmmo();
    void ResetAmmo();
}