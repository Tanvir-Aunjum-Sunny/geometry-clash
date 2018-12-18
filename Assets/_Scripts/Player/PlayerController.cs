using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerController : ExtendedMonoBehaviour
{
    [SerializeField] private CharacterMovementSettings Movement;
    [SerializeField] private CharacterMouseSettings Mouse;

    private Player player;
    private Vector3 velocity;
    private Vector3 mousePoint;

    void Start()
    {
        player = GetComponent<Player>();
    }

    void Update()
    {
        // Rotate player to match mouse target
        mousePoint = MouseUtils.GetMouseLookPoint(player.transform.position.z);
        player.LookAt(mousePoint);

        if (GameManager.Instance.DebugMode)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawLine(ray.origin, mousePoint, Color.yellow);
        }

        // Player movement
        Vector3 moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        velocity = Movement.GetTargetVelocity(moveInput.normalized);
        player.Move(velocity);
    }

    private void OnDrawGizmos()
    {
        if (GameManager.Instance == null || !GameManager.Instance.DebugMode) return;

        // Show mouse position
        Gizmos.color = new Color(239, 193, 186);
        Gizmos.DrawSphere(mousePoint, 0.15f);
    }
}

