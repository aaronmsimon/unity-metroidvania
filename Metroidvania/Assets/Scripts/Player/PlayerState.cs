using UnityEngine;

public abstract class PlayerState
{
    protected Player player;
    protected Animator animator;
    protected Rigidbody2D rb;

    protected bool JumpPressed {
        get => player.JumpPressed;
        set => player.JumpPressed = value;
    }
    protected bool JumpReleased {
        get => player.JumpReleased;
        set => player.JumpReleased = value;
    }
    protected bool SprintPressed => player.SprintPressed;
    protected Vector2 MoveInput => player.MoveInput;

    public PlayerState(Player player) {
        this.player = player;
        animator = player.Animator;
        rb = player.Rb;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }

    public virtual void Update() { }
    public virtual void FixedUpdate() { }
}
