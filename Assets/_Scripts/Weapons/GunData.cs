using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Gun data
/// </summary>
[CreateAssetMenu(fileName = "Create Gun", menuName = "Items/Gun")]
public class GunData : ScriptableObject
{
    [Header("Properties")]
    public float FireRate = 0.5f;
    public float ReloadTime = 1.5f;
    public int ClipSize = 10;
    [Range(0, 10f)]
    public float SpreadAngle = 0f;
    public GunType Type;

    [Header("Effects")]
    public GameObject FireEffect;
    public AudioClip FireSound;
    public AudioClip EmptySound;
    public AudioClip ReloadSound;
}
