using UnityEngine;

/// <summary>
/// Entered whenever the player is airborne but did NOT initiate a jump —
/// e.g. after a dash, after an air attack, or walking off a ledge.
/// Provides air-steering without applying any upward force.
/// </summary>
public class FallState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;

    public FallState(PlayerStateKey key, PlayerController player) : base(key)
    {
        this.player = player;
    }

    public override void EnterState()
    {
        player.anim?.SetBool("isFalling", true);
    }

    public override void UpdateState()
    {
        // Air steering only — no force applied
        float h = player.HorizontalInput;
        player.rb.linearVelocity = new Vector2(h * player.moveSpeed, player.rb.linearVelocity.y);
        player.FlipTowards(h);
    }

    public override void ExitState() 
    {
        player.anim?.SetBool("isFalling", false);
    }

    public override PlayerStateKey GetNextState()
    {
        if (player.attackPressed)  return PlayerStateKey.AttackAirborne;
        if (player.dashPressed)    return PlayerStateKey.Dash;

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
