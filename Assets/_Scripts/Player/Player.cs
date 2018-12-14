using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Player : ExtendedMonoBehaviour
{
    private Rigidbody rb;
    private Vector3 velocity;
    private Vector3 lookPoint;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // Look towards mouse target (but keep player looking horizontally)
        Vector3 correctedLookPoint = new Vector3(lookPoint.x, transform.position.y, lookPoint.z);
        transform.LookAt(correctedLookPoint);
    }

    private void FixedUpdate()
    {
        // Move player
        rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
    }

    /// <summary>
    /// Update player velocity
    /// </summary>
    /// <param name="velocity">Target velocity</param>
    public void Move(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    /// <summary>
    /// Look towards point
    /// </summary>
    /// <param name="lookPoint">Target look point</param>
    public void LookAt(Vector3 lookPoint)
    {
        this.lookPoint = lookPoint;
    }
}
