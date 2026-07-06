using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private PlayerInput playerInput;

    private Rigidbody2D rb;
    private Vector2 moveInput;
    private int facing = 1;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        Flip();
    }

    private void FixedUpdate() {
        float targetSpeed = moveInput.x * speed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    private void OnMove(InputValue value) {
        moveInput = value.Get<Vector2>();
    }

    private void Flip() {
        if (moveInput.x > 0.1f) {
            facing = 1;
        } else if (moveInput.x < -0.1f) {
            facing = -1;
        }

        transform.localScale = new Vector3(facing, 1, 1);
    }
}
