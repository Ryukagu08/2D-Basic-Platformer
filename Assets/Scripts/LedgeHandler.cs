using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class LedgeHandler : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float ledgeCheckDistance = 0.2f;
    [SerializeField] private float ledgeClimbForce = 3f;
    [SerializeField] private float maxVerticalVelocity = 5f;
    [SerializeField] private float cooldown = 0.3f;
    [SerializeField] private LayerMask groundLayer;
    
    [Header("References")]
    [SerializeField] private PlayerController playerController;
    [SerializeField] private Transform ledgeCheckOrigin;
    
    private Rigidbody2D rb;
    private float lastClimbTime;
    private bool isFacingRight = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        if (!ledgeCheckOrigin) ledgeCheckOrigin = transform;
    }

    void Update()
    {
        if (Time.time - lastClimbTime > cooldown)
        {
            CheckAndClimbLedges();
        }
    }

    private void CheckAndClimbLedges()
    {
        if (playerController.IsGrounded || rb.linearVelocity.y > 0) return;

        float direction = isFacingRight ? 1 : -1;
        Vector2 checkPosition = new Vector2(
            ledgeCheckOrigin.position.x,
            ledgeCheckOrigin.position.y - 0.1f
        );

        // First check: Horizontal obstacle detection
        RaycastHit2D wallHit = Physics2D.Raycast(
            checkPosition,
            Vector2.right * direction,
            ledgeCheckDistance,
            groundLayer
        );

        // Second check: Vertical clearance check
        RaycastHit2D floorHit = Physics2D.Raycast(
            checkPosition + new Vector2(ledgeCheckDistance * direction, 0),
            Vector2.down,
            0.2f,
            groundLayer
        );

        // If there's a wall but no floor below it
        if (wallHit.collider != null && floorHit.collider == null)
        {
            ApplyLedgeClimb();
        }
    }

    private void ApplyLedgeClimb()
    {
        // Apply force only if we're moving downward
        if (rb.linearVelocity.y < 0)
        {
            rb.linearVelocityY = Mathf.Clamp(
                rb.linearVelocityY + ledgeClimbForce,
                -maxVerticalVelocity,
                maxVerticalVelocity
            );
            lastClimbTime = Time.time;
        }
    }

    void OnDrawGizmos()
    {
        if (ledgeCheckOrigin && Application.isPlaying)
        {
            float direction = isFacingRight ? 1 : -1;
            Vector2 start = new Vector2(
                ledgeCheckOrigin.position.x,
                ledgeCheckOrigin.position.y - 0.1f
            );

            // Draw wall check
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(start, start + Vector2.right * direction * ledgeCheckDistance);

            // Draw floor check
            Gizmos.color = Color.cyan;
            Vector2 floorStart = start + new Vector2(ledgeCheckDistance * direction, 0);
            Gizmos.DrawLine(floorStart, floorStart + Vector2.down * 0.2f);
        }
    }
}