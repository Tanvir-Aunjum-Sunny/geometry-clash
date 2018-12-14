using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// TODO: Create "weapon" super class

public enum GunState
{
    READY,
    FIRING,
    EMPTY,
    RELOADING
}

/// <summary>
/// Basic gun
/// </summary>
public class Gun : ExtendedMonoBehaviour
{
    [Header("Bullet Settings")]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private int clipBullets;
    [SerializeField] private int clipSize = 10;
    [SerializeField] private float reloadTime = 1.5f;

    [Header("Effects")]
    [SerializeField] private AudioClip emptySound;
    [SerializeField] private AudioClip reloadSound;

    [Header("Miscellaneous")]
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firingTransform;

    private GunState state;

    void Start()
    {
        // Weapons start loaded
        clipBullets = clipSize;
        state = GunState.READY;

        // Weapon transform used as default firing transform (if no change is necessary)
        if (firingTransform == null) firingTransform = transform;
    }

    /// <summary>
    /// Fire a bullet
    /// </summary>
    public void Fire()
    {
        // Cannot fire empty gun (indicated with empty click)
        if (state == GunState.EMPTY)
        {
            AudioManager.Instance.PlayEffect(emptySound, transform.position);
            return;
        }
        // Cannot fire while reloading or immediately after firing
        else if (state == GunState.RELOADING || state == GunState.FIRING)
        {
            return;
        }

        // TODO: Figure out how to set a parent to avoid cluttering editor UI (although they are deleted...)
        GameObject bullet = Instantiate(
            projectilePrefab,
            firingTransform.position,
            firingTransform.rotation
        );

        Projectile projectile = projectilePrefab.GetComponent<Projectile>();
        AudioManager.Instance.PlayEffect(projectile.Data.FireSound, transform.position);

        clipBullets--;
        state = GunState.FIRING;

        // Fire rate prevents rapid-firing (unless empty)
        if (clipBullets > 0)
        {
            Wait(fireRate, () =>
            {
                state = GunState.READY;
            });
        }
        else
        {
            state = GunState.EMPTY;
        }
    }

    /// <summary>
    /// Reload the gun
    /// </summary>
    public void Reload()
    {
        // Can only reload while empty or not-firing
        if (state != GunState.EMPTY && state != GunState.READY)
        {
            return;
        }

        state = GunState.RELOADING;

        AudioManager.Instance.PlayEffect(reloadSound, transform.position, 20f);

        // Reset gun state after timeout
        Wait(reloadTime, () => {
            clipBullets = clipSize;
            state = GunState.READY;
        });
    }
}
