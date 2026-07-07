using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    [Header("Movement Variables")]
    [SerializeField] private float speed;
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCutMultiplier = 0.5f;
    [SerializeField] private float normalGravity;
    [SerializeField] private float fallGravity;
    [SerializeField] private float jumpGravity;    

    [Header("Ground Check")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float groundCheckRadius;
    [SerializeField] private LayerMask groundLayer;

    private Rigidbody2D rb;

    private Vector2 moveInput;
    private int facing = 1;
    private bool isGrounded;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = normalGravity;
    }

    private void Update() {
        ApplyVariableGravity();
        CheckGrounded();
        Flip();
    }

    private void FixedUpdate() {
        float targetSpeed = moveInput.x * speed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    private void ApplyVariableGravity() {
        if (rb.linearVelocity.y < -0.1f) {
            rb.gravityScale = fallGravity;
        } else if (rb.linearVelocity.y > 0.1f) {
            rb.gravityScale = jumpGravity;
        } else {
            rb.gravityScale = normalGravity;
        }
    }

    private void CheckGrounded() {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void Flip() {
        if (moveInput.x > 0.1f) {
            facing = 1;
        } else if (moveInput.x < -0.1f) {
            facing = -1;
        }

        transform.localScale = new Vector3(facing, 1, 1);
    }

    private void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    private void OnJump(InputValue value) {
        if (value.isPressed && isGrounded) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        } else {
            if (rb.linearVelocity.y > 0) {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            }
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
