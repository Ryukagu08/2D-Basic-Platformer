using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public float speed = 3.0f;
    public float initialJumpForce = 8.0f;
    public float maxJumpForce = 15.0f;
    public float jumpChargeRate = 5f;
    public float fallGravityMultiplier = 2f;
    public float jumpReleaseGravityMultiplier = 3f;
    public InputAction MoveAction;
    public InputAction JumpAction;
    
    private Rigidbody2D rb;
    private bool isGrounded;
    private bool isJumping;
    private float defaultGravityScale;
    private float currentJumpForce;

    void Start()
    {
        MoveAction.Enable();
        JumpAction.Enable();
        rb = GetComponent<Rigidbody2D>();
        defaultGravityScale = rb.gravityScale;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleGravity();
    }

    void HandleMovement()
    {
        Vector2 move = MoveAction.ReadValue<Vector2>();
        rb.linearVelocity = new Vector2(move.x * speed, rb.linearVelocityY);
    }

    void HandleJump()
    {
        float jumpInput = JumpAction.ReadValue<float>();

        // Start jump
        if (jumpInput > 0.1f && isGrounded && !isJumping)
        {
            isJumping = true;
            currentJumpForce = initialJumpForce;
            rb.linearVelocity = new Vector2(rb.linearVelocityX, currentJumpForce);
        }

        // Charging jump while button held
        if (isJumping && jumpInput > 0.1f)
        {
            if (currentJumpForce < maxJumpForce)
            {
                currentJumpForce += jumpChargeRate * Time.deltaTime;
                rb.linearVelocity = new Vector2(rb.linearVelocityX, currentJumpForce);
            }
        }

        // Release jump button
        if (jumpInput < 0.1f && isJumping)
        {
            isJumping = false;
            rb.gravityScale = jumpReleaseGravityMultiplier;
        }
    }

    void HandleGravity()
    {
        // Increase gravity when falling
        if (rb.linearVelocityY < 0)
        {
            rb.gravityScale = fallGravityMultiplier;
        }
        else if (!isJumping && rb.linearVelocityY > 0)
        {
            rb.gravityScale = defaultGravityScale;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            rb.gravityScale = defaultGravityScale;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }
}