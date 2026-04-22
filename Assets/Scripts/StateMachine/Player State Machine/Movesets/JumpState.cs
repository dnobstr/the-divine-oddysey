using UnityEngine;

/// <summary>
/// Jump variants:
///   Normal      – standard orderJump force
///   Order       – extra height, slow horizontal drift
///   Chaos       – lower orderJump, high horizontal burst
///   DivineOrder – floaty, gravity-reduced arc
///   DivineChaos – double-height explosive launch
/// </summary>
public class JumpState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;
    private MoveVariant variant;
    private bool jumpApplied;

    private float originalGravity;

    public JumpState(PlayerStateKey key, PlayerController player) : base(key)
    {
        this.player = player;
    }

    public override void EnterState()
    {
        variant      = player.getCurrentVariant();
        jumpApplied  = false;
        originalGravity = player.rb.gravityScale;
                
        ApplyJump();
        
        if (variant == MoveVariant.DivineChaos || variant == MoveVariant.DivineOrder)
            player.anim?.SetTrigger($"jump - {variant}");
        else
            player.anim?.SetTrigger($"jump - Normal");

    }

    private void ApplyJump()
    {
        float force = player.stats.normal.jump.force;
        switch (variant)
        {
            case MoveVariant.Order:
                force = player.stats.order.orderJump.force;
                break;

            case MoveVariant.Chaos:
                force = player.stats.chaos.chaosJump.force;
                // Horizontal burst
                float burstDir = player.facingRight ? 1f : -1f;
                player.rb.linearVelocity = new Vector2(burstDir * player.moveSpeed * 1.5f, 0f);
                break;

            case MoveVariant.DivineOrder:
                player.rb.gravityScale = player.stats.divineOrder.divineJump.gravityScale;
                break;

            case MoveVariant.DivineChaos:
                force *= player.stats.divineChaos.divineJump.force;
                break;
        }

        player.rb.linearVelocityY = force;
        jumpApplied = true;

        // Feed the meter
        if (variant == MoveVariant.Normal || variant == MoveVariant.Order || variant == MoveVariant.DivineOrder)
            player.stateMeter?.addOrder(5f);
        else
            player.stateMeter?.addChaos(5f);
    }

    public override void UpdateState()
    {
        // Allow air-steering
        float h = player.horizontalInput;
        player.rb.linearVelocity = new Vector2(h * player.moveSpeed, player.rb.linearVelocity.y);
        player.FlipTowards(h);
    }

    public override void ExitState()
    {
        player.rb.gravityScale = originalGravity;
    }

    public override PlayerStateKey GetNextState()
    {
        if (!jumpApplied) return StateKey;

        if (player.attackPressed)  return PlayerStateKey.AttackAirborne;
        if (player.dashPressed)    return PlayerStateKey.Dash;
        if (player.rb.linearVelocityY < 0) return PlayerStateKey.Fall;

        // Return to ground states once landed
        if (player.isGrounded && player.rb.linearVelocity.y <= 0f)
        {
            return Mathf.Abs(player.horizontalInput) > 0.01f
                ? PlayerStateKey.Move
                : PlayerStateKey.Idle;
        }

        if (!player.isGrounded && player.rb.linearVelocity.y < 0f) return PlayerStateKey.Fall;

        return StateKey;
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }
}
