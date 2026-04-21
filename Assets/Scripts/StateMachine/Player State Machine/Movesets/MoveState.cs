using UnityEngine;
using static UnityEditor.UIElements.ToolbarMenu;

public class MoveState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;
    private MoveVariant variant;

    public MoveState(PlayerStateKey key, PlayerController player) : base(key)
    {
        this.player = player;
        variant = player.getCurrentVariant();
    }

    public override void EnterState()
    {
        if (variant == MoveVariant.DivineChaos || variant == MoveVariant.DivineOrder)
            player.anim.SetBool($"isMoving - {variant}", true);
        else
            player.anim.SetBool($"isMoving - Normal", true);
    }

    public override void UpdateState()
    {
        float h = player.HorizontalInput;
        player.rb.linearVelocityX = h * player.moveSpeed;
        player.FlipTowards(h);
    }

    public override void ExitState()
    {
        if (variant == MoveVariant.DivineChaos || variant == MoveVariant.DivineOrder)
            player.anim.SetBool($"isMoving - {variant}", true);
        else
            player.anim.SetBool($"isMoving - Normal", true);
    }

    public override PlayerStateKey GetNextState()
    {
        if (player.dashPressed)                          return PlayerStateKey.Dash;
        if (player.attackPressed)                        return PlayerStateKey.Attack;
        if (player.jumpPressed && player.isGrounded)     return PlayerStateKey.Jump;
        if (player.rb.linearVelocityY < 0 && !player.isGrounded) return PlayerStateKey.Fall;
        if (Mathf.Abs(player.HorizontalInput) < 0.01f)   return PlayerStateKey.Idle;

        return StateKey;
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }
}
