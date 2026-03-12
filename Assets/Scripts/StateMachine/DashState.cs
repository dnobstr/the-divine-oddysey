using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class DashState : BaseState<PlayerStateEnum>
{
    readonly PlayerController _player;

    float _dashTimer;
    float _dashDirection;

    public DashState(PlayerStateEnum key, PlayerController player) : base(key)
    {
        _player = player;
    }

    public override void EnterState()
    {
        _player.anim.SetTrigger("dash");

        // Record time and direction on entry
        _player.lastDashTime = Time.time;
        _dashTimer           = _player.dashDuration;
        _dashDirection       = _player.isFacingRight ? 1f : -1f;

        // Zero out gravity effect during dash for a snappy, horizontal feel
        _player.rb.gravityScale = 0f;
        _player.rb.linearVelocity = new Vector2(_dashDirection * _player.dashSpeed, 0f);
    }

    public override void UpdateState()
    {
        _dashTimer -= Time.deltaTime;
    }

    public override PlayerStateEnum GetNextState()
    {
        if (_dashTimer > 0f) return Statekey;

        // Dash finished — fall through to appropriate state
        return _player.isGrounded ? PlayerStateEnum.Idle : PlayerStateEnum.Fall;
    }

    public override void ExitState()
    {
        // Restore gravity and bleed off dash velocity
        _player.rb.gravityScale   = 1f;
        _player.rb.linearVelocity = new Vector2(0f, _player.rb.linearVelocity.y);
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }
}
