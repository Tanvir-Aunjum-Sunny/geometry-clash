using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Instantiate guns on awake (rather than always destroying/creating)


/// <summary>
/// Gun controller state
/// </summary>
public enum GunControllerState
{
    EQUIPPING,
    READY
}


public class GunController : ExtendedMonoBehaviour
{
    public float GunEquipTime = 1f;

    [SerializeField] private KeyCode reloadKey = KeyCode.R;
    
    [Header("Guns")]
    [SerializeField] private Transform weaponTransform;
    [SerializeField] private Gun equippedGun;
    [SerializeField] private List<Gun> guns = new List<Gun>();

    private GunControllerState state = GunControllerState.READY;
    private int equippedGunIndex;

    private bool hasGunEquipped
    {
        get { return equippedGun != null; }
    }


    private void Start()
    {
        // Equip first gun if none is equipped
        if (!hasGunEquipped && guns.Count > 0)
        {
            EquipGun(guns[0], true);
        }
        else
        {
            state = GunControllerState.READY;
        }
    }

    private void Update()
    {
        // Equip a different gun
        if (Input.GetMouseButtonDown(1))
        {
            int newGunIndex = equippedGunIndex == 0 ? 1 : 0;

            EquipGun(guns[newGunIndex]);
        }
        // Other gun actions
        else if (hasGunEquipped && state == GunControllerState.READY)
        {
            // Fire weapon differently depending on type
            if (equippedGun.Data.Type == GunType.AUTOMATIC)
            {
                if (Input.GetMouseButton(0))
                {
                    equippedGun.Fire();
                }
            }
            else
            {
                if (Input.GetMouseButtonDown(0))
                {
                    equippedGun.Fire();
                }
            }

            // Reload gun
            if (Input.GetKeyDown(reloadKey))
            {
                equippedGun.Reload();
            }
        }
    }


    /// <summary>
    /// Equip a new gun
    /// </summary>
    /// <param name="gunToEquip">Gun to equip</param>
    /// <param name="skipEquipTime">Whether equip time is skipped</param>
    public void EquipGun(Gun gunToEquip, bool skipEquipTime = false)
    {
        // Can only equip gun when previous gun is ready
        if (state == GunControllerState.EQUIPPING) return;

        if (equippedGun != null)
        {
            Destroy(equippedGun.gameObject);
        }

        // Prevent switching to gun player does not have
        equippedGunIndex = guns.FindIndex(x => x == gunToEquip);
        if (equippedGunIndex < 0) return;

        // Equip weapon into player hand
        state = GunControllerState.EQUIPPING;
        float equipDelay = skipEquipTime ? 0 : GunEquipTime;
        Wait(equipDelay, () =>
        {
            equippedGun = Instantiate(gunToEquip, weaponTransform.position, weaponTransform.rotation);
            equippedGun.transform.parent = weaponTransform;

            state = GunControllerState.READY;
        });
    }
}
