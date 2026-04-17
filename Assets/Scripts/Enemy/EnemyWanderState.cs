using UnityEngine;

// ─── EnemyWanderState ─────────────────────────────────────────────────────────
// Picks a random point within wanderRadius of the spawn position, moves to it,
// waits, then picks another. Transitions to Aggro if the player gets too close.
// ─────────────────────────────────────────────────────────────────────────────

public class EnemyWanderState : BaseState<EnemyState>
{
    private readonly EnemyStateManager _ctx;

    private Vector2 _destination;
    private float   _destinationTimer;

    public EnemyWanderState(EnemyState key, EnemyStateManager ctx) : base(key)
    {
        _ctx = ctx;
    }

    public override void EnterState()
    {
        PickNewDestination();
    }

    public override void UpdateState()
    {
        MoveTowardDestination();
        TickDestinationTimer();
    }

    public override void ExitState()
    {
        _ctx.Rb.linearVelocity = Vector2.zero;
    }

    public override EnemyState GetNextState()
    {
        if (_ctx.DistanceToPlayer() <= _ctx.aggroRange)
            return EnemyState.Aggro;

        return EnemyState.Wander;
    }

    // ── Physics ───────────────────────────────────────────────────────────────

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }

    // ── Internals ─────────────────────────────────────────────────────────────

    private void MoveTowardDestination()
    {
        Vector2 currentPos = _ctx.transform.position;
        Vector2 direction  = (_destination - currentPos).normalized;
        float   distance   = Vector2.Distance(currentPos, _destination);

        if (distance > 0.1f)
            _ctx.Rb.linearVelocity = direction * _ctx.wanderSpeed;
        else
            _ctx.Rb.linearVelocity = Vector2.zero;
    }

    private void TickDestinationTimer()
    {
        _destinationTimer -= Time.deltaTime;
        if (_destinationTimer <= 0f)
            PickNewDestination();
    }

    private void PickNewDestination()
    {
        Vector2 randomOffset = Random.insideUnitCircle * _ctx.wanderRadius;
        _destination      = _ctx.SpawnPosition + randomOffset;
        _destinationTimer = _ctx.wanderInterval;
    }
}
