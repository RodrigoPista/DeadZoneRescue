using UnityEngine;

using UnityEngine;

public class PlayerWeaponController : MonoBehaviour
{
    public Transform weaponSpawnPoint; // where to spawn held weapons
    public MonoBehaviour[] weaponStrategies; // drag your concrete weapons here

    public bool IsAiming { get; private set; }

    private IWeaponStrategy currentWeapon;

    void Start()
    {
        if (weaponStrategies.Length > 0)
            EquipWeapon(0); // default first
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1)) EquipWeapon(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) EquipWeapon(1);
        if (Input.GetKeyDown(KeyCode.Alpha3)) EquipWeapon(2);

        if (Input.GetButtonDown("Fire1"))
        {
            currentWeapon?.Use();
        }
        IsAiming = Input.GetMouseButton(1);
    }   

    public void EquipWeapon(int index)
    {
        if (index < 0 || index >= weaponStrategies.Length) return;

        currentWeapon?.Unequip();
        currentWeapon = weaponStrategies[index] as IWeaponStrategy;
        currentWeapon?.Equip(this);
    }
}
