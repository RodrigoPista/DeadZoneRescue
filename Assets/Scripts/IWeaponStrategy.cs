using UnityEngine;

public interface IWeaponStrategy
{
    void Equip(PlayerWeaponController controller);
    void Unequip();
    void Use();
}
