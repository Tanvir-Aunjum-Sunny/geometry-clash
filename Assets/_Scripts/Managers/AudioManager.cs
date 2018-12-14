using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : GameSingleton<AudioManager>
{
    [Header("Volume Settings")]
    [Range(0, 1)]
    public float MasterVolume = 1;
    [Range(0, 1)]
    public float EffectVolume = 1;
    [Range(0, 1)]
    public float MusicVolume = 1;


    /// <summary>
    /// Play a sound effect at a point in the world
    /// </summary>
    /// <param name="clip">Sound effect</param>
    /// <param name="position">World position</param>
    /// <param name="volume">Volume multiplier</param>
    public void PlayEffect(AudioClip clip, Vector3 position, float volume = 1f)
    {
        if (clip == null) return;

        AudioSource.PlayClipAtPoint(clip, position, EffectVolume * MasterVolume * volume);
    }
}

