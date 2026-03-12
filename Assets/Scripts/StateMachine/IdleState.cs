using UnityEngine;

public class IdleState : BaseState<PlayerStateEnum>
{
    readonly PlayerController _player;

    public IdleState(PlayerStateEnum key, PlayerController player) : base(key)
    {
        _player = player;
    }

    public override void EnterState()
    {
        // Kill horizontal velocity so the player stops cleanly
        _player.rb.linearVelocity = new Vector2(0f, _player.rb.linearVelocity.y);
    }

    public override void UpdateState()
    {
        // Nothing extra needed — transitions handle everything
    }

    public override PlayerStateEnum GetNextState()
    {
        // Dash has highest priority
        if (_player.dashPressed && CanDash())           return PlayerStateEnum.Dash;
        if (_player.attackPressed)                      return PlayerStateEnum.Attack;
        if (_player.jumpPressed && _player.isGrounded) return PlayerStateEnum.Jump;
        if (!_player.isGrounded)                      return PlayerStateEnum.Fall;
        if (Mathf.Abs(_player.moveInput) > 0.01f)       return PlayerStateEnum.Run;

        return Statekey;
    }

    public override void ExitState() { }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }

    bool CanDash() =>
        Time.time >= _player.lastDashTime + _player.dashCooldown;
}
