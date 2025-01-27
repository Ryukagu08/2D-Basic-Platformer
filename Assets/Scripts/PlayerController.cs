using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    
    public float speed;
    public float jumpForce;
    public InputAction MoveAction;
    private bool isGrounded;
    private Rigidbody2D rb;

    void Start() {
        MoveAction.Enable();
        rb.GetComponent<Rigidbody2D>();
    }

    void Update() {

        Vector2 move = MoveAction.ReadValue<Vector2>(); // Movement
        Vector2 position = (Vector2)transform.position + speed * Time.deltaTime * move;
        transform.position = position;

        if (Input.GetButtonDown("Jump") && isGrounded) { // Jump
            rb.linearVelocity = new Vector2(rb.linearVelocityX, jumpForce);
        }
    }

    void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = true;
        }
    }
    void OnCollisionExit2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isGrounded = false;
        }
    }

}
