using UnityEngine;

public class AttackState : BaseState<PlayerStateEnum>
{
    readonly PlayerController _player;

    float _attackTimer;

    public AttackState(PlayerStateEnum key, PlayerController player) : base(key)
    {
        _player = player;
    }

    public override void EnterState()
    {
        _player.anim.SetTrigger("attack");
        _attackTimer = _player.attackDuration;

        // Halt horizontal movement so the player "plants" during the attack
        _player.rb.linearVelocity = new Vector2(0f, _player.rb.linearVelocity.y);

        GameObject hitbox = Object.Instantiate(
            _player.hitboxPrefab,
            _player.attackPoint.position,
            _player.attackPoint.rotation
        );

        hitbox.GetComponent<AttackHitbox>().Init(_player);

        // Optional: parent it to the player so it moves with them
        hitbox.transform.SetParent(_player.attackPoint);

        // Optional: auto-destroy after the attack duration
        Object.Destroy(hitbox, _player.attackDuration);
    }

    public override void UpdateState()
    {
        _attackTimer -= Time.deltaTime;
    }

    public override PlayerStateEnum GetNextState()
    {
        // Hold this state for the full attack duration
        if (_attackTimer > 0f) return Statekey;

        if (!_player.isGrounded)                      return PlayerStateEnum.Fall;
        if (Mathf.Abs(_player.moveInput) > 0.01f)       return PlayerStateEnum.Run;

        return PlayerStateEnum.Idle;
    }

    public override void ExitState() 
    {
        
    }

    public override void OnTriggerEnter(Collider other) { }
    public override void OnTriggerStay(Collider other)  { }
    public override void OnTriggerExit(Collider other)  { }
}
