using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    // Movement
    public float speed = 8f;
    public float airControl = 0.8f;
    private float horizontalinput;

    // Jump System
    public float initialJumpForce = 12f;
    public float maxJumpHoldTime = 0.3f;
    public float holdJumpForce = 5f;
    public float fallGravity = 2.5f;
    public float releaseGravity = 4f;
    public float lowJumpGravity = 3f;

    public InputAction MoveAction;
    public InputAction JumpAction;

    private Rigidbody2D rb;
    public bool isGrounded;
    private bool isJumping;
    private float jumpTimeCounter;
    private float defaultGravity;

    // Ledge Handler
    public bool IsGrounded => isGrounded;
    public bool IsFacingRight { get; private set; } = true;

    void Start()
    {
        MoveAction.Enable();
        JumpAction.Enable();
        rb = GetComponent<Rigidbody2D>();
        defaultGravity = rb.gravityScale;
    }

    void Update()
    {
        HandleMovement();
        HandleJump();
        HandleGravity();
        UpdateFacingDirection();
    }

    void HandleMovement()
    {
        float moveInput = MoveAction.ReadValue<Vector2>().x;
        float currentSpeed = isGrounded ? speed : speed * airControl;
        rb.linearVelocityX = moveInput * currentSpeed;
    }

    void HandleJump()
    {
        float jumpInput = JumpAction.ReadValue<float>();

        // Start jump
        if (jumpInput > 0.1f && isGrounded)
        {
            isJumping = true;
            jumpTimeCounter = maxJumpHoldTime;
            rb.linearVelocityY = initialJumpForce;
        }

        // Sustained jump force while button held
        if (isJumping && jumpInput > 0.1f)
        {
            if (jumpTimeCounter > 0)
            {
                rb.linearVelocityY += holdJumpForce * Time.deltaTime;
                jumpTimeCounter -= Time.deltaTime;
            }
            else
            {
                isJumping = false;
            }
        }

        // Early release cut-off
        if (jumpInput < 0.1f)
        {
            isJumping = false;
        }
    }

    void HandleGravity()
    {
        // Increased gravity when falling
        if (rb.linearVelocityY < 0)
        {
            rb.gravityScale = fallGravity;
        }
        // Reduced gravity at jump peak
        else if (rb.linearVelocityY > 0 && !isJumping)
        {
            rb.gravityScale = lowJumpGravity;
        }
        // Immediate gravity on release
        else if (!isJumping && rb.linearVelocityY > 0)
        {
            rb.gravityScale = releaseGravity;
        }
        else
        {
            rb.gravityScale = defaultGravity;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            rb.gravityScale = defaultGravity;
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }
    }

    void UpdateFacingDirection() // Ledge Handler
    {
        if (horizontalinput > 0) IsFacingRight = true;
        if (horizontalinput < 0) IsFacingRight = false;
    }

}