using UnityEngine;

public class PlayerSlideState : PlayerState
{
    private float slideTimer;
    private float slideStopTimer;

    public PlayerSlideState(Player player) : base(player) { }

    public override void Enter() {
        base.Enter();

        slideTimer = player.SlideDuration;
        slideStopTimer = 0;

        player.SetColliderSlide();
        animator.SetBool("isSliding", true);
    }

    public override void Update() {
        base.Update();

        if (slideTimer > 0) {
            slideTimer -= Time.deltaTime;
        } else if (slideTimer <= 0) {
            slideStopTimer = player.SlideStopDuration;
        } else {
            slideStopTimer -= Time.deltaTime;
            
            if (slideStopTimer <= 0)
            {
                if (player.CheckForCeiling() || MoveInput.y <= -0.1f) {
                    player.ChangeState(player.CrouchState);
                } else {
                    player.ChangeState(player.IdleState);
                }
            }
        }

    }

    public override void FixedUpdate() {
        base.FixedUpdate();
        
        if (slideTimer > 0) {
            rb.linearVelocity = new Vector2(player.SlideSpeed * player.Facing, rb.linearVelocity.y);
        } else {
            rb.linearVelocity = new Vector2(0, rb.linearVelocity.y);
        }
    }

    public override void Exit() {
        base.Exit();

        player.SetColliderNormal();
        animator.SetBool("isSliding", false);
    }
}
