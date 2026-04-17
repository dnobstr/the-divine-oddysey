using UnityEngine;

// ─── EnemyDespawnState ────────────────────────────────────────────────────────
// Entered when the player escapes deaggroRange.
// Waits despawnDelay seconds (for fade/VFX), then destroys the GameObject.
// This is a terminal state — no transitions out.
// ─────────────────────────────────────────────────────────────────────────────

public class EnemyDespawnState : BaseState<EnemyState>
{
    private readonly EnemyStateManager _ctx;

    private float _despawnTimer;

    public EnemyDespawnState(EnemyState key, EnemyStateManager ctx) : base(key)
    {
        _ctx = ctx;
    }

    public override void EnterState()
    {
        _despawnTimer          = _ctx.despawnDelay;
        _ctx.Rb.linearVelocity = Vector2.zero;

        // ── Trigger your despawn VFX / animation here ─────────────────────────
        // e.g. GetComponent<Animator>().SetTrigger("Despawn");
        Debug.Log($"[{_ctx.name}] despawning in {_ctx.despawnDelay}s.");
    }

    public override void UpdateState()
    {
        _despawnTimer -= Time.deltaTime;
        if (_despawnTimer <= 0f)
            Object.Destroy(_ctx.gameObject);
    }

    public override void ExitState() { }

    // Terminal — never leaves this state on its own
    public override EnemyState GetNextState() => EnemyState.Despawn;

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }
}
