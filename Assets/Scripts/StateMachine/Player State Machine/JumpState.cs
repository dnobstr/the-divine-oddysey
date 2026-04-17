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
    private readonly PlayerController _player;
    private MoveVariant _variant;
    private bool        _jumpApplied;

    // Tune per-variant modifiers
    private const float OrderHeightMult   = 1.35f;
    private const float ChaosHeightMult   = 0.75f;
    private const float DivineOrderGrav   = 0.4f;  // gravity scale during float
    private const float DivineChaosHeight = 2.2f;

    private float _originalGravity;

    public JumpState(PlayerStateKey key, PlayerController player) : base(key)
    {
        _player = player;
    }

    public override void EnterState()
    {
        _variant      = _player.GetCurrentVariant();
        _jumpApplied  = false;
        _originalGravity = _player.rb.gravityScale;
                
        ApplyJump();
        _player.anim?.SetTrigger($"jump");
        //player.anim?.SetTrigger($"Jump_{variant}");
    }

    private void ApplyJump()
    {
        float force = _player.jumpForce;

        switch (_variant)
        {
            case MoveVariant.Order:
                force *= OrderHeightMult;
                break;

            case MoveVariant.Chaos:
                force *= ChaosHeightMult;
                // Horizontal burst
                float burstDir = _player.FacingRight ? 1f : -1f;
                _player.rb.linearVelocity = new Vector2(burstDir * _player.moveSpeed * 1.5f, 0f);
                break;

            case MoveVariant.DivineOrder:
                _player.rb.gravityScale = DivineOrderGrav;
                break;

            case MoveVariant.DivineChaos:
                force *= DivineChaosHeight;
                break;
        }

        _player.rb.linearVelocityY = force;
        _jumpApplied = true;

        // Feed the meter
        if (_variant == MoveVariant.Normal || _variant == MoveVariant.Order || _variant == MoveVariant.DivineOrder)
            _player.stateMeter?.AddOrder(5f);
        else
            _player.stateMeter?.AddChaos(5f);
    }

    public override void UpdateState()
    {
        // Allow air-steering
        float h = _player.HorizontalInput;
        _player.rb.linearVelocity = new Vector2(h * _player.moveSpeed, _player.rb.linearVelocity.y);
        _player.FlipTowards(h);
    }

    public override void ExitState()
    {
        _player.rb.gravityScale = _originalGravity;
    }

    public override PlayerStateKey GetNextState()
    {
        if (!_jumpApplied) return StateKey;

        if (_player.attackPressed)  return PlayerStateKey.AttackAirborne;
        if (_player.dashPressed)    return PlayerStateKey.Dash;

        // Return to ground states once landed
        if (_player.isGrounded && _player.rb.linearVelocity.y <= 0f)
        {
            return Mathf.Abs(_player.HorizontalInput) > 0.01f
                ? PlayerStateKey.Move
                : PlayerStateKey.Idle;
        }

        return StateKey;
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }
}
