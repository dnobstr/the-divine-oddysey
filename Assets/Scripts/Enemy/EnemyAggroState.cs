using UnityEngine;

// ─── EnemyAggroState ──────────────────────────────────────────────────────────
// Chases the player.
//   → Attack  : player is within attackRange
//   → Despawn : player has gone beyond deaggroRange
// ─────────────────────────────────────────────────────────────────────────────

public class EnemyAggroState : BaseState<EnemyState>
{
    private readonly EnemyStateManager _ctx;

    public EnemyAggroState(EnemyState key, EnemyStateManager ctx) : base(key)
    {
        _ctx = ctx;
    }

    public override void EnterState() { }

    public override void UpdateState()
    {
        _ctx.Rb.linearVelocity = _ctx.DirectionToPlayer() * _ctx.chaseSpeed;
    }

    public override void ExitState()
    {
        _ctx.Rb.linearVelocity = Vector2.zero;
    }

    public override EnemyState GetNextState()
    {
        float dist = _ctx.DistanceToPlayer();

        if (dist <= _ctx.attackRange)  return EnemyState.Attack;
        if (dist >= _ctx.deaggroRange) return EnemyState.Despawn;

        return EnemyState.Aggro;
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }
}
