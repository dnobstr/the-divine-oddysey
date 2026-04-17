using System.Collections;
using UnityEngine;

/// <summary>
/// Dash variants:
///   Normal      – straight horizontal dash
///   Order       – slower but leaves a brief shield window
///   Chaos       – faster, passes through enemies (set layer ignore in EnterState)
///   DivineOrder – dash + brief time-slow effect (Time.timeScale)
///   DivineChaos – multi-dash: fires 3 short dashes in rapid succession
/// </summary>
public class DashState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;
    private MoveVariant _variant;

    private float _dashTimer;
    private bool  _dashComplete;
    private int   _divineDashCount;

    // Variant speed multipliers
    private const float OrderSpeedMult      = 0.7f;
    private const float ChaosSpeedMult      = 1.5f;
    private const float DivineOrderTimeSlow = 0.35f;
    private const int   DivineChaosRepeats  = 3;

    public DashState(PlayerStateKey key, PlayerController player) : base(key)
    {
        this.player = player;
    }

    public override void EnterState()
    {
        _variant      = player.GetCurrentVariant();
        _dashTimer    = 0f;
        _dashComplete = false;
        _divineDashCount = 0;

        // Disable gravity during dash so arc doesn't curve
        player.rb.gravityScale = 0f;
        player.rb.linearVelocity = Vector2.zero;

        StartDash();
        player.anim?.SetTrigger($"dash");
        //player.anim?.SetTrigger($"Dash_{variant}");

        // Meter cost
        if (_variant == MoveVariant.Chaos || _variant == MoveVariant.DivineChaos)
            player.stateMeter?.AddChaos(8f);
        else
            player.stateMeter?.AddOrder(8f);
    }

    private void StartDash()
    {
        float dir   = player.FacingRight ? 1f : -1f;
        float speed = player.dashSpeed;

        switch (_variant)
        {
            case MoveVariant.Order:       speed *= OrderSpeedMult;  break;
            case MoveVariant.Chaos:       speed *= ChaosSpeedMult;  break;
            case MoveVariant.DivineOrder:
                Time.timeScale = DivineOrderTimeSlow;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                break;
            case MoveVariant.DivineChaos: speed *= ChaosSpeedMult;  break;
        }

        player.rb.linearVelocity = new Vector2(dir * speed, 0f);
        _dashTimer = player.dashDuration;
    }

    public override void UpdateState()
    {
        _dashTimer -= Time.deltaTime;

        if (_dashTimer <= 0f)
        {
            if (_variant == MoveVariant.DivineChaos && _divineDashCount < DivineChaosRepeats - 1)
            {
                _divineDashCount++;
                StartDash();
            }
            else
            {
                _dashComplete = true;
            }
        }
    }

    public override void ExitState()
    {
        player.rb.gravityScale = 1f;
        player.rb.linearVelocity = new Vector2(0f, player.rb.linearVelocity.y);

        // Restore time if DivineOrder dash was used
        if (_variant == MoveVariant.DivineOrder)
        {
            Time.timeScale      = 1f;
            Time.fixedDeltaTime = 0.02f;
        }
    }

    public override PlayerStateKey GetNextState()
    {
        if (!_dashComplete) return StateKey;

        return player.isGrounded
            ? (Mathf.Abs(player.HorizontalInput) > 0.01f ? PlayerStateKey.Move : PlayerStateKey.Idle)
            : PlayerStateKey.Idle; // treat as mid-air after airborne dash
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }
}
