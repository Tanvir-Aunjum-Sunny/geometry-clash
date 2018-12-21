using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : ExtendedMonoBehaviour
{
    [Range(0f, 1f)]
    public float RotationSpeed = 1f;

    [SerializeField]
    private Transform source;
    [SerializeField]
    private LayerMask obscuringLayerMask;
    [SerializeField]
    private LayerMask targetLayerMask;

    [Header("Crosshair")]
    [SerializeField]
    private Color initialCrosshairColor = Color.black;
    [SerializeField]
    private Color invalidCrosshairColor = Color.grey;

    [Header("Dot")]
    [SerializeField]
    private GameObject dot;
    [SerializeField]
    private Color initialDotColor = Color.black;
    [SerializeField]
    private Color invalidDotColor = Color.yellow;
    [SerializeField]
    private Color targetDotColor = Color.red;

    private SpriteRenderer crosshairSprite;
    private SpriteRenderer dotSprite;
    private Vector3 originalDotScale;
    private bool isCrosshairsOnTarget = false;
    private bool isTargetReachable = false;

    const float ROTATION_MULTIPLIER = 100;
    const float COLOR_LERP_RATE = 10;
    const float DOT_SIZE_LERP_RATE = 10;


    private void Awake()
    {
        crosshairSprite = GetComponent<SpriteRenderer>();
        dotSprite = dot.GetComponent<SpriteRenderer>();
    }

    private void Start()
    {
        originalDotScale = dot.transform.localScale;

        GameManager.Instance.Player.Damageable.OnDeath += OnPlayerDeath;
    }

    void Update()
    {
        // Determine if target is reachable (not obscured by environment)
        CheckIfTargetIsReachable();

        // Determine if target is selected by crosshairs
        if (isTargetReachable)
        {
            CheckIfOnTarget();
        }

        // Rotating crosshairs is visually appealing
        transform.Rotate(Vector3.forward, RotationSpeed * ROTATION_MULTIPLIER * Time.deltaTime);
    }


    /// <summary>
    /// Determine whether target is reachable (not obscured by environment)
    /// </summary>
    private void CheckIfTargetIsReachable()
    {
        Ray ray = new Ray(source.position, source.forward);
        RaycastHit hit;
        float distanceToTarget = Vector3.Distance(source.position, transform.position);

        if (Physics.Raycast(ray, out hit, distanceToTarget, obscuringLayerMask))
        {
            isTargetReachable = false;
            isCrosshairsOnTarget = false;
            crosshairSprite.color = Color.Lerp(crosshairSprite.color, invalidCrosshairColor, COLOR_LERP_RATE * Time.deltaTime);
            dotSprite.color = Color.Lerp(dotSprite.color, invalidDotColor, COLOR_LERP_RATE * Time.deltaTime);
        }
        else
        {
            isTargetReachable = true;
            crosshairSprite.color = Color.Lerp(crosshairSprite.color, initialCrosshairColor, COLOR_LERP_RATE * 0.5f * Time.deltaTime);
            dotSprite.color = Color.Lerp(dotSprite.color, initialDotColor, COLOR_LERP_RATE * 0.5f * Time.deltaTime);
        }
    }

    /// <summary>
    /// Determine whether crosshair is over valid target
    /// </summary>
    private void CheckIfOnTarget()
    {
        // Move ray above mouse position and cast ray downwards
        Ray ray = new Ray(transform.position + new Vector3(0, 2f, 0), transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, 3, targetLayerMask))
        {
            isCrosshairsOnTarget = true;
            dotSprite.color = Color.Lerp(dotSprite.color, targetDotColor, COLOR_LERP_RATE * Time.deltaTime);
            dot.transform.localScale = Vector3.Lerp(dot.transform.localScale, originalDotScale * 1.5f, DOT_SIZE_LERP_RATE * Time.deltaTime);
        }
        else
        {
            isCrosshairsOnTarget = false;
            dotSprite.color = Color.Lerp(dotSprite.color, initialDotColor, COLOR_LERP_RATE * 0.5f * Time.deltaTime);
            dot.transform.localScale = Vector3.Lerp(dot.transform.localScale, originalDotScale, DOT_SIZE_LERP_RATE * Time.deltaTime);
        }
    }

    /// <summary>
    /// Destroy the crosshairs (player death)
    /// </summary>
    private void OnPlayerDeath(GameObject destroyTrigger)
    {
        Destroy(gameObject);
    }
}

