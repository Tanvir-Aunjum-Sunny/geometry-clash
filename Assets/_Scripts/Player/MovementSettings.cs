using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Character movement settings
/// </summary>
[Serializable]
public class CharacterMovementSettings
{
    [Header("Base Speed Settings")]
    public float Speed = 5f;
    public float RunMultiplier = 2.0f;

    [Header("Key Bindings")]
    public KeyCode RunKey = KeyCode.LeftShift;

    private float TargetSpeed;


    /// <summary>
    /// Get target speed from parameters and constraints
    /// </summary>
    /// <param name="moveInput">Movement input (from keyboard)</param>
    /// <param name="isRunning">Whether target is running</param>
    /// <returns>Target speed</returns>
    public float GetTargetSpeed(Vector3 moveInput, bool isRunning = false)
    {
        TargetSpeed = Speed;

        if (moveInput == Vector3.zero)
        {
            return 0f;
        }

        // TODO: Only enable while player has stamina
        if (isRunning)
        {
            TargetSpeed = Speed *= RunMultiplier;
        }

        return TargetSpeed;
    }

    /// <summary>
    /// Get target velocity from parameters and constraints
    /// </summary>
    /// <param name="moveInput">Movement input (from keyboard)</param>
    /// <param name="isRunning">Whether target is running</param>
    /// <returns>Target velocity</returns>
    public Vector3 GetTargetVelocity(Vector3 moveInput, bool isRunning = false)
    {
        float targetSpeed = GetTargetSpeed(moveInput, isRunning);

        return moveInput * targetSpeed;
    }
}

