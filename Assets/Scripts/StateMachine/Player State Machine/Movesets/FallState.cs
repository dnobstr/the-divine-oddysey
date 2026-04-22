using UnityEngine;

/// <summary>
/// Entered whenever the player is airborne but did NOT initiate a orderJump —
/// e.g. after a orderDash, after an air chaosAttack, or walking off a ledge.
/// Provides air-steering without applying any upward force.
/// </summary>
public class FallState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;
    private MoveVariant variant;

    public FallState(PlayerStateKey key, PlayerController player) : base(key)
    {
        this.player = player;
        variant = player.getCurrentVariant();
    }

    public override void EnterState()
    {
        if (variant == MoveVariant.DivineChaos || variant == MoveVariant.DivineOrder)
            player.anim?.SetBool($"isFalling - {variant}", true);
        else
            player.anim?.SetBool($"isFalling - Normal", true);
    }

    public override void UpdateState()
    {
        // Air steering only — no force applied
        float h = player.horizontalInput;
        player.rb.linearVelocity = new Vector2(h * player.moveSpeed, player.rb.linearVelocity.y);
        player.FlipTowards(h);
    }

    public override void ExitState() 
    {
        if (variant == MoveVariant.DivineChaos || variant == MoveVariant.DivineOrder)
            player.anim?.SetBool($"isFalling - {variant}", false);
        else
            player.anim?.SetBool($"isFalling - Normal", false);
    }

    public override PlayerStateKey GetNextState()
    {
        if (player.attackPressed && !player.isGrounded)  return PlayerStateKey.AttackAirborne;
        if (player.attackPressed && player.isGrounded)  return PlayerStateKey.Attack;
        if (player.dashPressed)    return PlayerStateKey.Dash;

        if (player.isGrounded && player.rb.linearVelocity.y <= 0f)
        {
            return Mathf.Abs(player.horizontalInput) > 0.01f
                ? PlayerStateKey.Move
                : PlayerStateKey.Idle;
        }

        return StateKey;
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }
}
