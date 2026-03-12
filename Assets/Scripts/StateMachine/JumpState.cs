using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class JumpState : BaseState<PlayerStateEnum>
{
    readonly PlayerController _player;

    public JumpState(PlayerStateEnum key, PlayerController player) : base(key)
    {
        _player = player;
    }

    public override void EnterState()
    {
        _player.anim.SetTrigger("jump");

        // Apply jump impulse — override vertical velocity for consistent height
        _player.rb.linearVelocity = new Vector2(_player.rb.linearVelocity.x, _player.jumpForce);
    }

    public override void UpdateState()
    {
        // Allow air-steering at full speed
        _player.HandleFlip(_player.moveInput);
        _player.rb.linearVelocity = new Vector2(
            _player.moveInput * _player.moveSpeed,
            _player.rb.linearVelocity.y);
    }

    public override PlayerStateEnum GetNextState()
    {
        if (_player.dashPressed && CanDash()) return PlayerStateEnum.Dash;

        // Once the apex is passed, switch to fall
        if (_player.rb.linearVelocity.y < 0f) return PlayerStateEnum.Fall;

        return Statekey;
    }

    public override void ExitState() { }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }

    bool CanDash() =>
        Time.time >= _player.lastDashTime + _player.dashCooldown;
}
