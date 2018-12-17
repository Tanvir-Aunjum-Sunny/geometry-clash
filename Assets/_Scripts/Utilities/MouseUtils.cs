using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MouseUtils
{
    public static float GetHorizontalAngleToMouse(Vector3 sourcePosition)
    {
        Vector3 mousePoint = GetMouseLookPoint();

        return GetHorizontalAngleToTarget(sourcePosition, mousePoint);
    }

    /// <summary>
    /// Get the mouse look point (on z=0 plane)
    /// </summary>
    /// <returns>Mouse look point</returns>
    public static Vector3 GetMouseLookPoint()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.up, Vector3.zero);
        float rayDistance;

        // QUESTION: What happens if there is no raycast (should always be with plane)?
        ground.Raycast(ray, out rayDistance);
        return ray.GetPoint(rayDistance);
    }

    public static float GetHorizontalAngleToTarget(Vector3 sourcePosition, Vector3 targetPosition)
    {
        Vector3 offset = new Vector3(targetPosition.x - sourcePosition.x, 0, targetPosition.z - sourcePosition.z);

        return Mathf.Atan2(offset.x, offset.z) * Mathf.Rad2Deg;
    }
}
