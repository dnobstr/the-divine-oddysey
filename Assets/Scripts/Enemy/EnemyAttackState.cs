using UnityEngine;

// ─── EnemyAttackState ─────────────────────────────────────────────────────────
// Holds position at the edge of attackRange and fires on cooldown.
// Backs off if the player closes inside half attackRange (too close).
//   → Aggro   : player moves out of attackRange (but still in deaggroRange)
//   → Despawn : player moves beyond deaggroRange
// ─────────────────────────────────────────────────────────────────────────────

public class EnemyAttackState : BaseState<EnemyState>
{
    private readonly EnemyStateManager _ctx;

    private float _attackTimer;

    // How close the player can get before the enemy backs off
    private float BackoffRange => _ctx.attackRange * 0.5f;

    public EnemyAttackState(EnemyState key, EnemyStateManager ctx) : base(key)
    {
        _ctx = ctx;
    }

    public override void EnterState()
    {
        _attackTimer = 0f; // attack immediately on entry
    }

    public override void UpdateState()
    {
        MaintainRange();
        TickAttack();
    }

    public override void ExitState()
    {
        _ctx.Rb.linearVelocity = Vector2.zero;
    }

    public override EnemyState GetNextState()
    {
        float dist = _ctx.DistanceToPlayer();

        if (dist >= _ctx.deaggroRange) return EnemyState.Despawn;
        if (dist >  _ctx.attackRange)  return EnemyState.Aggro;

        return EnemyState.Attack;
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }

    // ── Internals ─────────────────────────────────────────────────────────────

    // Strafe backward if player is too close, otherwise stand still
    private void MaintainRange()
    {
        float dist = _ctx.DistanceToPlayer();

        if (dist < BackoffRange)
            _ctx.Rb.linearVelocity = -_ctx.DirectionToPlayer() * _ctx.chaseSpeed;
        else
            _ctx.Rb.linearVelocity = Vector2.zero;
    }

    private void TickAttack()
    {
        _attackTimer -= Time.deltaTime;
        if (_attackTimer > 0f) return;

        PerformAttack();
        _attackTimer = _ctx.attackCooldown;
    }

    private void PerformAttack()
    {
        // ── Plug your own attack logic here ───────────────────────────────────
        // e.g. play animation, spawn projectile, call player health component, etc.
        Debug.Log($"[{_ctx.name}] attacked for {_ctx.attackDamage} damage.");

        // Example — damage player directly if they have a health component:
        // _ctx.PlayerTransform.GetComponent<PlayerHealth>()?.TakeDamage(_ctx.attackDamage);
    }
}
