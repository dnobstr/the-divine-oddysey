using UnityEngine;

// ─── EnemyDespawnState ────────────────────────────────────────────────────────
// Entered when the player escapes deaggroRange.
// Counts down despawnDelay then destroys — but cancels back to Wander if the
// player returns within deaggroRange before the timer expires.
// ─────────────────────────────────────────────────────────────────────────────

public class EnemyDespawnState : BaseState<EnemyState>
{
    private readonly EnemyStateManager ctx;

    private float timer;

    public EnemyDespawnState(EnemyState key, EnemyStateManager ctx) : base(key)
    {
        this.ctx = ctx;
    }

    public override void EnterState()
    {
        timer = ctx.stats.despawnDelay;
        ctx.SetXVelocity(0f);

        // ── Trigger your despawn VFX / animation here ─────────────────────────
        // e.g. ctx.GetComponent<Animator>().SetTrigger("Despawn");
        Debug.Log($"[{ctx.name}] despawning in {ctx.stats.despawnDelay}s.");
    }

    public override void UpdateState()
    {
        timer -= Time.deltaTime;
        if (timer <= 0f)
            Object.Destroy(ctx.gameObject);
    }

    public override void ExitState() { }

    public override EnemyState GetNextState()
    {
        // Player came back within range — cancel despawn and return to Wander
        if (ctx.HorizontalDistanceToPlayer() < ctx.stats.deaggroRange)
            return EnemyState.Wander;

        return EnemyState.Despawn;
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other) { }
    public override void OnTriggerExit2D(Collider2D other) { }
}