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

    [Header("Crouch Check")]
    [SerializeField] private Transform headCheck;
    [SerializeField] private float headCheckRadius = 0.2f;

    [Header("Slide Settings")]
    [SerializeField] private float slideDuration = 0.6f;
    [SerializeField] private float slideSpeed = 12;
    [SerializeField] private float slideStopDuration = 0.15f;
    [SerializeField] private float slideHeight;
    [SerializeField] private Vector2 slideOffset;

    public PlayerState CurrentState;
    public PlayerIdleState IdleState;
    public PlayerJumpState JumpState;

    private Rigidbody2D rb;
    private CapsuleCollider2D playerCollider;
    private Animator animator;

    private Vector2 moveInput;
    private bool sprintPressed;
    public bool JumpPressed;
    public bool JumpReleased;

    private int facing = 1;
    private bool isGrounded;

    private bool isSliding;
    private bool slideInputLocked;
    private float slideTimer;
    private float slideStopTimer;
    private float normalHeight;
    private Vector2 normalOffset;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = normalGravity;

        animator = GetComponentInChildren<Animator>();

        playerCollider = GetComponent<CapsuleCollider2D>();
        normalHeight = playerCollider.size.y;
        normalOffset = playerCollider.offset;

        IdleState = new PlayerIdleState(this);
        JumpState = new PlayerJumpState(this);
    }

    private void Start() {
        ChangeState(IdleState);
    }

    private void Update() {
        CurrentState.Update();

        TryStandUp();

        if (!isSliding) {
            Flip();            
        }

        HandleAnimations();
        HandleSlide();
    }

    private void FixedUpdate() {
        CurrentState.FixedUpdate();

        CheckGrounded();

        if (!isSliding) {
            HandleMovement();
        }
    }

    public void ChangeState(PlayerState newState) {
        if (CurrentState != null) {
            CurrentState.Exit();
        }
            
        CurrentState = newState;
        CurrentState.Enter();
    }

    private void HandleMovement() {
        float currentSpeed = sprintPressed ? sprintSpeed : walkSpeed;
        float targetSpeed = moveInput.x * currentSpeed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    public void ApplyVariableGravity() {
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
        bool isCrouching = animator.GetBool("isCrouching");

        animator.SetBool("isGrounded", isGrounded);
        animator.SetBool("isSliding", isSliding);

        animator.SetFloat("yVelocity", rb.linearVelocity.y);
        
        bool isMoving = Mathf.Abs(moveInput.x) > 0.1f && isGrounded;
        animator.SetBool("isIdle", !isMoving && isGrounded && !isSliding && !isCrouching);
        animator.SetBool("isWalking", isMoving && !sprintPressed && !isSliding && !isCrouching);
        animator.SetBool("isSprinting", isMoving && sprintPressed && !isSliding && !isCrouching);
    }

    private void HandleSlide() {
        if (isSliding) {
            slideTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(slideSpeed * facing, rb.linearVelocity.y);

            // Done sliding
            if (slideTimer <= 0) {
                isSliding = false;
                slideStopTimer = slideStopDuration;
                TryStandUp();
            }
        }

        if (slideStopTimer > 0) {
            slideStopTimer -= Time.deltaTime;
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }

        // Start sliding
        if (isGrounded && sprintPressed && moveInput.y < -0.1f && !isSliding && !slideInputLocked) {
            isSliding = true;
            slideInputLocked = true;
            slideTimer = slideDuration;
            SetColliderSlide();
        }

        if (slideStopTimer < 0 && moveInput.y >= -0.1f) {
            slideInputLocked = false;
        }
    }

    private void SetColliderNormal() {
        playerCollider.size = new Vector2(playerCollider.size.x, normalHeight);
        playerCollider.offset = normalOffset;
    }

    private void SetColliderSlide() {
        playerCollider.size = new Vector2(playerCollider.size.x, slideHeight);
        playerCollider.offset = slideOffset;
    }

    private void TryStandUp() {
        if (isSliding) {
            animator.SetBool("isCrouching", false);
            return;
        }

        bool shouldCrouch = moveInput.y < -0.1f || Physics2D.OverlapCircle(headCheck.position, headCheckRadius, groundLayer);

        if(!shouldCrouch) {
            SetColliderNormal();
            animator.SetBool("isCrouching", false);
        } else {
            SetColliderSlide();
            animator.SetBool("isCrouching", true);
        }
    }

    private void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    private void OnSprint(InputValue value) {
        sprintPressed = value.isPressed;
    }

    private void OnJump(InputValue value) {
        if (value.isPressed) {
            JumpPressed = true;
            JumpReleased = false;
        } else {
            JumpReleased = true;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(headCheck.position, headCheckRadius);
    }

    public Animator Animator => animator;
    public Rigidbody2D Rb => rb;
    public float JumpForce => jumpForce;
    public bool IsGrounded => isGrounded;
    public float JumpCutMultiplier => jumpCutMultiplier;
    public bool SprintPressed => sprintPressed;
    public float WalkSpeed => walkSpeed;
    public float SprintSpeed => sprintSpeed;
    public float Facing => facing;
}
