using UnityEngine;

// ─── EnemyDespawnState ────────────────────────────────────────────────────────
// Terminal state — entered when the player escapes deaggroRange.
// Halts movement, waits despawnDelay seconds (for fade/VFX), then destroys.
// ─────────────────────────────────────────────────────────────────────────────

public class EnemyDespawnState : BaseState<EnemyState>
{
    private readonly EnemyStateManager _ctx;

    private float _timer;

    public EnemyDespawnState(EnemyState key, EnemyStateManager ctx) : base(key)
    {
        _ctx = ctx;
    }

    public override void EnterState()
    {
        _timer = _ctx.stats.despawnDelay;
        _ctx.SetXVelocity(0f);

        // ── Trigger your despawn VFX / animation here ─────────────────────────
        // e.g. GetComponent<Animator>().SetTrigger("Despawn");
        Debug.Log($"[{_ctx.name}] despawning in {_ctx.stats.despawnDelay}s.");
    }

    public override void UpdateState()
    {
        _timer -= Time.deltaTime;
        if (_timer <= 0f)
            Object.Destroy(_ctx.gameObject);
    }

    public override void ExitState() { }

    // Terminal — never transitions out
    public override EnemyState GetNextState() => EnemyState.Despawn;

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }
}
