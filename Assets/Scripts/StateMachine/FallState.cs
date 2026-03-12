using UnityEngine;

public class FallState : BaseState<PlayerStateEnum>
{
    readonly PlayerController _player;

    public FallState(PlayerStateEnum key, PlayerController player) : base(key)
    {
        _player = player;
    }

    public override void EnterState()
    {
        _player.anim.SetBool("isFalling", true);
    }

    public override void UpdateState()
    {
        // Preserve air-steering while falling
        _player.HandleFlip(_player.moveInput);
        _player.rb.linearVelocity = new Vector2(
            _player.moveInput * _player.moveSpeed,
            _player.rb.linearVelocity.y);
    }

    public override PlayerStateEnum GetNextState()
    {
        if (_player.dashPressed && CanDash()) return PlayerStateEnum.Dash;

        if (_player.isGrounded)
        {
            _player.anim.SetBool("isFalling", false);
            // Land into Run or Idle depending on whether the player is moving
            return Mathf.Abs(_player.moveInput) > 0.01f
                ? PlayerStateEnum.Run
                : PlayerStateEnum.Idle;

        }

        return Statekey;
    }

    public override void ExitState() 
    {
        _player.anim.SetBool("isFalling", false);
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }

    bool CanDash() =>
        Time.time >= _player.lastDashTime + _player.dashCooldown;
}
