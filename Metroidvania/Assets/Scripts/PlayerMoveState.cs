using UnityEngine;

public class PlayerMoveState : PlayerState
{
    public PlayerMoveState(Player player) : base(player) { }

    public override void Enter() {
        base.Enter();
    }

    public override void Update() {
        base.Update();

        if (JumpPressed) {
            player.ChangeState(player.JumpState);
        } else if (Mathf.Abs(MoveInput.x) < 0.1f) {
            player.ChangeState(player.IdleState);
        } else {
            animator.SetBool("isWalking", !SprintPressed);
            animator.SetBool("isSprinting", SprintPressed);
        }

    }

    public override void FixedUpdate() {
        base.FixedUpdate();

        float currentSpeed = SprintPressed ? player.SprintSpeed : player.WalkSpeed;
        float targetSpeed = player.Facing * currentSpeed;
        rb.linearVelocity = new Vector2(targetSpeed, rb.linearVelocity.y);
    }

    public override void Exit() {
        base.Exit();

        animator.SetBool("isWalking", false);
        animator.SetBool("isSprinting", false);
    }
}
