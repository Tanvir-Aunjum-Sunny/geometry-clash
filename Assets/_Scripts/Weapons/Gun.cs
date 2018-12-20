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

public enum GunType
{
    AUTOMATIC,
    SHOTGUN,
    SINGLESHOT,
    PISTOL
}


/// <summary>
/// Basic gun
/// </summary>
public class Gun : ExtendedMonoBehaviour
{
    public GunData Data;

    [SerializeField]
    private int bulletsInClip;

    [Header("Miscellaneous")]
    [SerializeField]
    private GameObject projectilePrefab;
    [SerializeField]
    private Transform firingTransform;
    [SerializeField]
    private ParticleSystem shellParticle;

    private GunState state;


    void Start()
    {
        if (Data == null) Data = ScriptableObject.CreateInstance<GunData>();

        // Weapons start loaded
        bulletsInClip = Data.ClipSize;
        state = GunState.READY;

        // Weapon transform used as default firing transform (if no change is necessary)
        if (firingTransform == null) firingTransform = transform;
    }

    private void Update()
    {
        if (GameManager.Instance.DebugMode)
        {
            // Show direction gun is facing
            Debug.DrawRay(firingTransform.position, firingTransform.forward * 10, Color.blue);
        }
    }


    /// <summary>
    /// Fire a bullet
    /// </summary>
    public void Fire()
    {
        // Cannot fire empty gun (indicated with empty click)
        if (state == GunState.EMPTY)
        {
            AudioManager.Instance.PlayEffect(Data.EmptySound, transform.position);
            return;
        }
        // Cannot fire while reloading or immediately after firing
        else if (state == GunState.RELOADING || state == GunState.FIRING)
        {
            return;
        }

        // Some weapons have a spread angle range while firing (typically automatics)
        float spreadAngle = Random.Range(-Data.SpreadAngle, Data.SpreadAngle);
        Quaternion projectileSpreadRotation = firingTransform.rotation * Quaternion.Euler(0, spreadAngle, 0);

        GameObject bullet = Instantiate(
            projectilePrefab,
            firingTransform.position,
            projectileSpreadRotation,
            TemporaryManager.Instance.TemporaryChildren
        );

        Projectile projectile = projectilePrefab.GetComponent<Projectile>();
        AudioManager.Instance.PlayEffect(Data.FireSound, transform.position);

        // shellParticle.Emit(1);
        if (shellParticle != null) shellParticle.Play();

        bulletsInClip--;
        state = GunState.FIRING;

        // Fire rate prevents rapid-firing (unless empty)
        if (bulletsInClip > 0)
        {
            Wait(Data.FireRate, () =>
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


        AudioManager.Instance.PlayEffect(Data.ReloadSound, transform.position, 20f);

        // Reset gun state after timeout
        state = GunState.RELOADING;
        Wait(Data.ReloadTime, () => {
            bulletsInClip = Data.ClipSize;
            state = GunState.READY;
        });
    }
}
