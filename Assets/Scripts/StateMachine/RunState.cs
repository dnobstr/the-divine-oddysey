using UnityEngine;

public class RunState : BaseState<PlayerStateEnum>
{
    readonly PlayerController _player;

    public RunState(PlayerStateEnum key, PlayerController player) : base(key)
    {
        _player = player;
    }

    public override void EnterState()
    {
        _player.anim.SetBool("isRunning", true);
    }

    public override void UpdateState()
    {
        _player.HandleFlip(_player.moveInput);
        _player.rb.linearVelocity = new Vector2(
            _player.moveInput * _player.moveSpeed,
            _player.rb.linearVelocity.y);
    }

    public override PlayerStateEnum GetNextState()
    {
        if (_player.dashPressed && CanDash())            return PlayerStateEnum.Dash;
        if (_player.attackPressed)                       return PlayerStateEnum.Attack;
        if (_player.jumpPressed && _player.isGrounded) return PlayerStateEnum.Jump;
        if (!_player.isGrounded)                       return PlayerStateEnum.Fall;
        if (Mathf.Abs(_player.moveInput) < 0.01f)        return PlayerStateEnum.Idle;

        return Statekey;
    }

    public override void ExitState() 
    {
        _player.anim.SetBool("isRunning", false);
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }

    bool CanDash() =>
        Time.time >= _player.lastDashTime + _player.dashCooldown;
}
