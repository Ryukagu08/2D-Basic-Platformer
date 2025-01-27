using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    public float speed = 3.0f;
    public float jumpForce = 10.0f;
    public float maxJumpTime = 0.5f;
    public InputAction MoveAction;
    public InputAction JumpAction;
    private bool isGrounded;
    private Rigidbody2D rb;
    private float jumpTime;

    void Start() {
        MoveAction.Enable();
        JumpAction.Enable();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {

        Vector2 move = MoveAction.ReadValue<Vector2>(); // Movement
        Vector2 position = (Vector2)transform.position + speed * Time.deltaTime * move;
        transform.position = position;

        float jumpInput = JumpAction.ReadValue<float>(); // Jump

        if (jumpInput > 0.01f && isGrounded) {
            jumpTime += Time.deltaTime; // Increase Jump while pressed
        } else {
            jumpTime = 0f; // Reset Jump when released
        }

        if (jumpTime > 0 && isGrounded) {
            float currentJumpForce = Mathf.Lerp(0, jumpForce, jumpTime / maxJumpTime); // Greadual increase
            rb.linearVelocity = new Vector2 (rb.linearVelocityX, currentJumpForce); // Apply jump force
        } else if (jumpInput == 0f && rb.linearVelocityY > 0f) {
            rb.linearVelocity = new Vector2(rb.linearVelocityX, rb.linearVelocityY * 0.9f); // Smooth decrease
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;
            jumpTime = 0f;
        }
    }

    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = false;
        }
    }


}
