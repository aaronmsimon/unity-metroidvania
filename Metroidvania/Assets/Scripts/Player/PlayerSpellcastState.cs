using UnityEngine;

public class PlayerSpellcastState : PlayerState
{
    public PlayerSpellcastState(Player player) : base(player) { }

    public override void Enter() {
        base.Enter();

        animator.SetBool("isCasting", true);
    }

    public override void Update() {
        base.Update();
    }

    public override void FixedUpdate() {
        base.FixedUpdate();
    }

    public override void AnimationFinished() {
        base.AnimationFinished();

        if (Mathf.Abs(MoveInput.x) > 0.1f) {
            player.ChangeState(player.MoveState);
        } else {
            player.ChangeState(player.IdleState);
        }
    }

    public override void Exit() {
        base.Exit();

        animator.SetBool("isCasting", false);
    }
}
