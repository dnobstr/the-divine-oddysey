using UnityEngine;

/// <summary>
/// Jump variants:
///   Normal      – standard jump force
///   Order       – extra height, slow horizontal drift
///   Chaos       – lower jump, high horizontal burst
///   DivineOrder – floaty, gravity-reduced arc
///   DivineChaos – double-height explosive launch
/// </summary>
public class JumpState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;
    private MoveVariant variant;
    private bool        jumpApplied;

    // Tune per-variant modifiers
    private const float OrderHeightMult   = 1.35f;
    private const float ChaosHeightMult   = 0.75f;
    private const float DivineOrderGrav   = 0.4f;  // gravity scale during float
    private const float DivineChaosHeight = 2.2f;

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
        player.anim?.SetTrigger($"jump");
        //player.anim?.SetTrigger($"Jump_{variant}");
    }

    private void ApplyJump()
    {
        float force = player.jumpForce;

        switch (variant)
        {
            case MoveVariant.Order:
                force *= OrderHeightMult;
                break;

            case MoveVariant.Chaos:
                force *= ChaosHeightMult;
                // Horizontal burst
                float burstDir = player.FacingRight ? 1f : -1f;
                player.rb.linearVelocity = new Vector2(burstDir * player.moveSpeed * 1.5f, 0f);
                break;

            case MoveVariant.DivineOrder:
                player.rb.gravityScale = DivineOrderGrav;
                break;

            case MoveVariant.DivineChaos:
                force *= DivineChaosHeight;
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
        float h = player.HorizontalInput;
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
            return Mathf.Abs(player.HorizontalInput) > 0.01f
                ? PlayerStateKey.Move
                : PlayerStateKey.Idle;
        }

        return StateKey;
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }
}
