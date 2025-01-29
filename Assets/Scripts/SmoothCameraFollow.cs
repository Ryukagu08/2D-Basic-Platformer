using UnityEngine;

public class SmoothCameraFollow : MonoBehaviour
{
    [Header("References")]
    public Transform target;
    public PlayerController playerController;

    [Header("Offsets")]
    public Vector3 groundedOffset = new Vector3(0, 2f, -10f);
    public Vector3 flyingOffset = new Vector3(0, 0, -10f);

    [Header("Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private float offsetTransitionSpeed = 5f;
    [SerializeField] private float groundStateBufferTime = 0.2f;

    private Vector3 currentOffset;
    private Vector3 velocity = Vector3.zero;
    private float lastGroundedTime;
    private bool bufferedGroundedState;

    void Start()
    {
        currentOffset = groundedOffset;
        bufferedGroundedState = true;
    }

    void LateUpdate()
    {
        if (target == null || playerController == null)
        {
            Debug.LogWarning("Camera follow references not set!");
            return;
        }

        UpdateGroundStateBuffer();
        CalculateOffset();
        ApplyCameraMovement();
    }

    void UpdateGroundStateBuffer()
    {
        // Update buffer based on actual ground state
        if (playerController.isGrounded)
        {
            lastGroundedTime = Time.time;
            bufferedGroundedState = true;
        }
        else if (Time.time - lastGroundedTime > groundStateBufferTime)
        {
            bufferedGroundedState = false;
        }
    }

    void CalculateOffset()
    {
        Vector3 targetOffset = bufferedGroundedState ? groundedOffset : flyingOffset;
        currentOffset = Vector3.Lerp(currentOffset, targetOffset, 
                                   offsetTransitionSpeed * Time.deltaTime);
    }

    void ApplyCameraMovement()
    {
        Vector3 targetPosition = target.position + currentOffset;
        
        // Separate smoothing for X and Y axes
        float smoothedX = Mathf.SmoothDamp(transform.position.x, targetPosition.x, 
                                         ref velocity.x, smoothSpeed * Time.deltaTime);
        float smoothedY = Mathf.SmoothDamp(transform.position.y, targetPosition.y, 
                                         ref velocity.y, smoothSpeed * Time.deltaTime);
        
        transform.position = new Vector3(smoothedX, smoothedY, targetPosition.z);
    }
}