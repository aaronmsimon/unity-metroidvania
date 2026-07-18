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

    [Header("Attack Settings")]
    [SerializeField] private int damage;
    [SerializeField] private float attackRadius = 0.5f;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private LayerMask enemyLayer;

    public PlayerState CurrentState;
    public PlayerIdleState IdleState;
    public PlayerJumpState JumpState;
    public PlayerMoveState MoveState;
    public PlayerCrouchState CrouchState;
    public PlayerSlideState SlideState;

    private Rigidbody2D rb;
    private CapsuleCollider2D playerCollider;
    private Animator animator;

    private Vector2 moveInput;
    private bool sprintPressed;
    private bool jumpPressed;
    private bool jumpReleased;

    private int facing = 1;
    private bool isGrounded;

    private bool isSliding;
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
        MoveState = new PlayerMoveState(this);
        CrouchState = new PlayerCrouchState(this);
        SlideState = new PlayerSlideState(this);
    }

    private void Start() {
        ChangeState(IdleState);
    }

    private void Update() {
        CurrentState.Update();

        if (!isSliding) {
            Flip();            
        }

        HandleAnimations();
    }

    private void FixedUpdate() {
        CurrentState.FixedUpdate();

        CheckGrounded();
    }

    public void ChangeState(PlayerState newState) {
        if (CurrentState != null) {
            CurrentState.Exit();
        }
            
        CurrentState = newState;
        CurrentState.Enter();
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

    public bool CheckForCeiling() {
        return Physics2D.OverlapCircle(headCheck.position, headCheckRadius, groundLayer);
    }

    private void Flip() {
        if (MoveInput.x > 0.1f) {
            facing = 1;
        } else if (MoveInput.x < -0.1f) {
            facing = -1;
        }

        transform.localScale = new Vector3(facing, 1, 1);
    }

    private void HandleAnimations() {
        animator.SetBool("isGrounded", isGrounded);
        animator.SetFloat("yVelocity", rb.linearVelocity.y);
    }

    public void SetColliderNormal() {
        playerCollider.size = new Vector2(playerCollider.size.x, normalHeight);
        playerCollider.offset = normalOffset;
    }

    public void SetColliderSlide() {
        playerCollider.size = new Vector2(playerCollider.size.x, slideHeight);
        playerCollider.offset = slideOffset;
    }

    private void OnMove(InputValue value) {
        MoveInput = value.Get<Vector2>();
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

    private void OnAttack(InputValue value) {
        Collider2D enemy = Physics2D.OverlapCircle(attackPoint.position, attackRadius, enemyLayer);

        if (enemy != null) {
            enemy.gameObject.GetComponent<Health>().ChangeHealth(-damage);
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(headCheck.position, headCheckRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
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
    public float SlideDuration => slideDuration;
    public float SlideSpeed => slideSpeed;
    public float SlideStopDuration => slideStopDuration;

    public Vector2 MoveInput { get { return moveInput; } set { moveInput = value; } }
    public bool JumpPressed { get { return jumpPressed; } set { jumpPressed = value; } }
    public bool JumpReleased { get { return jumpReleased; } set { jumpReleased = value; } }
}
