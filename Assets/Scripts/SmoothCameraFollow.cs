using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public PlayerController playerController; // Reference to your player controller

    [Header("Offsets")]
    public Vector3 groundedOffset = new Vector3(0, 2f, -10f); // Slightly above player
    public Vector3 flyingOffset = new Vector3(0, 0, -10f);    // Centered when airborne

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float offsetTransitionSpeed = 5f;

    private Vector3 currentOffset;
    private Vector3 velocity = Vector3.zero;

    void Start()
    {
        currentOffset = groundedOffset;
    }

    void LateUpdate()
    {
        if (target == null || playerController == null)
        {
            Debug.LogWarning("Camera follow references not set!");
            return;
        }

        // Determine target offset based on grounded state
        Vector3 targetOffset = playerController.isGrounded ? groundedOffset : flyingOffset;

        // Smoothly transition between offsets
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, 
                                   offsetTransitionSpeed * Time.deltaTime);

        // Calculate target position
        Vector3 targetPosition = target.position + currentOffset;

        // Apply smooth follow
        transform.position = Vector3.SmoothDamp(transform.position, 
                                              targetPosition, 
                                              ref velocity, 
                                              smoothSpeed * Time.deltaTime);
    }
}