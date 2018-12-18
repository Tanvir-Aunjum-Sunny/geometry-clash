using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    [Range(0f, 1f)]
    public float RotationSpeed = 1f;

    const float ROTATION_MULTIPLIER = 100;

    void Update()
    {
        transform.Rotate(Vector3.forward, RotationSpeed * ROTATION_MULTIPLIER * Time.deltaTime);
    }
}
