using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public PlayerController playerController;

    [Header("Offsets")]
    public Vector3 groundedOffset = new(0, 2f, -10f);
    public Vector3 flyingOffset = new(0, 0, -10f);

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float offsetTransitionSpeed = 5f;
    [SerializeField] public float groundApproachDistance = 3f;
    [SerializeField] private float fallingTransitionSpeed = 8f;
    [SerializeField] private LayerMask groundLayer;

    private Vector3 currentOffset;
    private Vector3 velocity = Vector3.zero;
    private bool isApproachingGround;

    void LateUpdate()
    {
        if (target == null || playerController == null) return;

        CheckGroundApproach();
        CalculateOffset();
        ApplyCameraMovement();
    }

    void CheckGroundApproach()
    {
        // Ground detecting raycast
        RaycastHit2D hit = Physics2D.Raycast(
            target.position,
            Vector2.down,
            groundApproachDistance,
            groundLayer
        );

        isApproachingGround = playerController.IsFalling && hit.collider != null;
    }

    void CalculateOffset()
    {
        Vector3 targetOffset;
        float transitionSpeed;

        if (playerController.isGrounded)
        {
            targetOffset = groundedOffset;
            transitionSpeed = offsetTransitionSpeed;
        }
        else if (isApproachingGround)
        {
            // Only transition when near ground
            targetOffset = groundedOffset;
            transitionSpeed = fallingTransitionSpeed;
        }
        else
        {
            targetOffset = flyingOffset;
            transitionSpeed = offsetTransitionSpeed;
        }

        currentOffset = Vector3.Lerp(
            currentOffset, 
            targetOffset, 
            transitionSpeed * Time.deltaTime
        );
    }

    void ApplyCameraMovement()
    {
        Vector3 targetPosition = target.position + currentOffset;
        
        float smoothedX = Mathf.SmoothDamp(
            transform.position.x, 
            targetPosition.x, 
            ref velocity.x, 
            smoothSpeed * Time.deltaTime
        );
        
        float smoothedY = Mathf.SmoothDamp(
            transform.position.y, 
            targetPosition.y, 
            ref velocity.y, 
            smoothSpeed * Time.deltaTime
        );
        
        transform.position = new Vector3(smoothedX, smoothedY, targetPosition.z);
    }

    void OnDrawGizmos()
    {
        if (target != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawLine(target.position, target.position + Vector3.down * groundApproachDistance);
        }
    }
}