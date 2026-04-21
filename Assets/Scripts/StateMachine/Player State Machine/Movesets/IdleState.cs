using UnityEngine;
using static UnityEditor.UIElements.ToolbarMenu;

public class IdleState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;

    public IdleState(PlayerStateKey key, PlayerController player) : base(key)
    {
        this.player = player;
    }

    public override void EnterState()
    {
        // Zero horizontal velocity so the player doesn't slide
        var v = player.rb.linearVelocity;
        player.rb.linearVelocity = new Vector2(0f, v.y);

        MoveVariant variant = player.getCurrentVariant();

        if (variant == MoveVariant.DivineChaos || variant == MoveVariant.DivineOrder)
            player.anim.SetBool($"isMoving - {variant}", false);
        else
            player.anim.SetBool($"isMoving - Normal", false);
    }

    public override void UpdateState() { /* nothing – just stand there */ }

    public override void ExitState() { }

    public override PlayerStateKey GetNextState()
    {
        // Priority: Dash > Attack > Jump > Move
        if (player.dashPressed)                          return PlayerStateKey.Dash;
        if (player.attackPressed)                        return PlayerStateKey.Attack;
        if (player.jumpPressed && player.isGrounded)     return PlayerStateKey.Jump;
        if (player.rb.linearVelocityY < 0 && !player.isGrounded) return PlayerStateKey.Fall;
        if (Mathf.Abs(player.HorizontalInput) > 0.01f)   return PlayerStateKey.Move;


        return StateKey; // stay Idle
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }
}
