using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerInput playerInput;

    [Header("Movement Variables")]
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
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
    private Animator animator;

    private Vector2 moveInput;
    private bool sprintPressed;
    private bool jumpPressed;
    private bool jumpReleased;

    private int facing = 1;
    private bool isGrounded;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = normalGravity;

        animator = GetComponentInChildren<Animator>();
    }

    private void Update() {
        Flip();
        HandleAnimations();
    }

    private void FixedUpdate() {
        ApplyVariableGravity();
        CheckGrounded();
        HandleMovement();
        HandleJump();
    }

    private void HandleMovement() {
        float currentSpeed = sprintPressed ? sprintSpeed : walkSpeed;
        float targetSpeed = moveInput.x * currentSpeed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    private void HandleJump() {
        if (jumpPressed && isGrounded) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            jumpPressed = false;
            jumpReleased = false;
        }
        if (jumpReleased) {
            if (rb.linearVelocity.y > 0) {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            }
            jumpReleased = false;
        }
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

    private void HandleAnimations() {
        animator.SetBool("isJumping", rb.linearVelocity.y > 0.1f);
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        
        bool isMoving = Mathf.Abs(moveInput.x) > 0.1f && isGrounded;
        animator.SetBool("isIdle", !isMoving && isGrounded);
        animator.SetBool("isWalking", isMoving && !sprintPressed);
        animator.SetBool("isSprinting", isMoving && sprintPressed);
    }

    private void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    private void OnSprint(InputValue value) {
        sprintPressed = value.isPressed;
    }

    private void OnJump(InputValue value) {
        if (value.isPressed) {
            jumpPressed = true;
            jumpReleased = false;
        } else {
            jumpReleased = true;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
