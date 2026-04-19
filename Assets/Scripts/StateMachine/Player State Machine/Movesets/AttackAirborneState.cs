using UnityEngine;

/// <summary>
/// Airborne Attack variants:
///   Normal      – quick aerial slash, slight downward push
///   Order       – upward arc slash (anti-air), pauses fall briefly
///   Chaos       – downward slam, launches player upward on land
///   DivineOrder – holy radial burst, freezes vertical velocity
///   DivineChaos – spinning chaos drill downward
/// </summary>
public class AttackAirborneState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;
    private MoveVariant variant;

    private float attackTimer;
    private bool  attackDone;

    private float originalGravity;

    private static readonly float[] Durations =
    {
        0.30f, // Normal
        0.40f, // Order
        0.35f, // Chaos
        0.55f, // DivineOrder
        0.45f  // DivineChaos
    };

    public AttackAirborneState(PlayerStateKey key, PlayerController player) : base(key)
    {
        this.player = player;
    }

    public override void EnterState()
    {
        variant         = player.getCurrentVariant();
        attackDone      = false;
        attackTimer     = Durations[(int)variant];
        originalGravity = player.rb.gravityScale;

        //player.anim?.SetTrigger($"AttackAir_{variant}");
        ApplyVariantPhysics();
        PerformAttackLogic();
    }

    private void ApplyVariantPhysics()
    {
        switch (variant)
        {
            case MoveVariant.Normal:
                // Slight downward nudge to give weight
                player.rb.linearVelocityY = -2f;
                break;

            case MoveVariant.Order:
                // Pause the fall – anti-air feel
                player.rb.gravityScale = 0f;
                player.rb.linearVelocityY =  2f;
                break;

            case MoveVariant.Chaos:
                // Slam downward fast
                player.rb.linearVelocityY = - player.jumpForce * 0.9f;
                break;

            case MoveVariant.DivineOrder:
                // Freeze in air during holy burst
                player.rb.gravityScale   = 0f;
                player.rb.linearVelocity = Vector2.zero;
                break;

            case MoveVariant.DivineChaos:
                // Spinning drill – fast downward
                player.rb.linearVelocityY =  - player.jumpForce * 1.2f;
                break;
        }
    }

    private void PerformAttackLogic()
    {
        switch (variant)
        {
            case MoveVariant.Normal:                                            break;
            case MoveVariant.Order:       player.stateMeter?.addOrder(10f);     break;
            case MoveVariant.Chaos:       player.stateMeter?.addChaos(10f);     break;
        }

        // TODO: activate your aerial hitbox here
    }

    public override void UpdateState()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
            attackDone = true;
    }

    public override void ExitState()
    {
        player.rb.gravityScale = originalGravity;

        // Chaos slam bounce: launch up when landing
        if (variant == MoveVariant.Chaos && player.isGrounded)
            player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, player.jumpForce * 0.6f);
    }

    public override PlayerStateKey GetNextState()
    {
        if (!attackDone) return StateKey;

        if (player.attackPressed && !player.isGrounded) return PlayerStateKey.AttackAirborne;
        if (player.isGrounded)
        {
            return Mathf.Abs(player.HorizontalInput) > 0.01f
                ? PlayerStateKey.Move
                : PlayerStateKey.Idle;
        }

        return PlayerStateKey.Fall;
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }
}
