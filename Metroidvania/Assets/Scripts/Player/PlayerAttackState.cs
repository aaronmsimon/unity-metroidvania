using UnityEngine;

public class PlayerAttackState : PlayerState
{
    public PlayerAttackState(Player player) : base(player) { }

    public override void Enter() {
        base.Enter();

        player.Animator.SetBool("isAttacking", true);
        rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
    }

    public override void AnimationFinished() {
        if (Mathf.Abs(MoveInput.x) > 0.1f) {
            player.ChangeState(player.MoveState);
        } else {
            player.ChangeState(player.IdleState);
        }
    }

    public override void FixedUpdate() {
        base.FixedUpdate();
    }

    public override void Exit() {
        base.Exit();

        player.Animator.SetBool("isAttacking", false);
    }
}
