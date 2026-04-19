using UnityEngine;

public class MoveState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;

    public MoveState(PlayerStateKey key, PlayerController player) : base(key)
    {
        this.player = player;
    }

    public override void EnterState()
    {
        player.anim?.SetBool("isMoving", true);
    }

    public override void UpdateState()
    {
        float h = player.HorizontalInput;
        player.rb.linearVelocityX = h * player.moveSpeed;
        player.FlipTowards(h);
    }

    public override void ExitState()
    {
        player.anim?.SetBool("isMoving", false);
    }

    public override PlayerStateKey GetNextState()
    {
        if (player.dashPressed)                          return PlayerStateKey.Dash;
        if (player.attackPressed)                        return PlayerStateKey.Attack;
        if (player.jumpPressed && player.isGrounded)     return PlayerStateKey.Jump;
        if (player.rb.linearVelocityY < 0)               return PlayerStateKey.Fall;
        if (Mathf.Abs(player.HorizontalInput) < 0.01f)   return PlayerStateKey.Idle;

        return StateKey;
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }
}
