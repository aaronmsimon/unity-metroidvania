using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player) : base(player) { }

    public override void Enter() {
        animator.SetBool("isIdle", true);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    public override void Update() {
        base.Update();

        if (AttackPressed && combat.CanAttack) {
            player.ChangeState(player.AttackState);
        } else if (JumpPressed) {
            JumpPressed = false;
            player.ChangeState(player.JumpState);
        } else if (Mathf.Abs(MoveInput.x) > 0.1f) {
            player.ChangeState(player.MoveState);
        } else if (MoveInput.y < -0.1f) {
            player.ChangeState(player.CrouchState);
        }
    }

    public override void Exit() {
        animator.SetBool("isIdle", false);
    }
}
