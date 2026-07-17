public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player player) : base(player) { }

    public override void Enter() {
        animator.SetBool("isIdle", true);
    }

    public override void Update() {
        base.Update();

        if (JumpPressed) {
            JumpPressed = false;
            player.ChangeState(player.JumpState);
        }
    }

    public override void Exit() {
        animator.SetBool("isIdle", false);
    }
}
