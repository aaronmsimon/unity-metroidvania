using UnityEngine;

public class PlayerJumpState : PlayerState
{
    public PlayerJumpState(Player player) : base(player) { }

    public override void Enter() {
        base.Enter();

        animator.SetBool("isJumping", true);

        rb.linearVelocity = new Vector2(rb.linearVelocity.x, player.JumpForce);
        JumpPressed = false;
        JumpReleased = false;
    }

    public override void Update() {
        base.Update();

        if (player.IsGrounded && rb.linearVelocity.y < 0) {
            player.ChangeState(player.IdleState);
        }
    }

    public override void FixedUpdate() {
        base.FixedUpdate();

        player.ApplyVariableGravity();

        if (JumpReleased && rb.linearVelocity.y > 0) {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * player.JumpCutMultiplier);
            JumpReleased = false;
        }

        float currentSpeed = SprintPressed ? player.SprintSpeed : player.WalkSpeed;
        float targetSpeed = player.Facing * currentSpeed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    public override void Exit() {
        base.Exit();

        animator.SetBool("isJumping", false);
    }
}
