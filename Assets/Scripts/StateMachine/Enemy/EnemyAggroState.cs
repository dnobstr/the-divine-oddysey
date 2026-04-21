using UnityEngine;

// ─── EnemyAggroState ──────────────────────────────────────────────────────────
// Chases the player horizontally. Only sets X velocity — gravity handles Y.
//   → Attack  : player is within attackRange (horizontal)
//   → Despawn : player is beyond deaggroRange (horizontal)
// ─────────────────────────────────────────────────────────────────────────────

public class EnemyAggroState : BaseState<EnemyState>
{
    private readonly EnemyStateManager ctx;

    public EnemyAggroState(EnemyState key, EnemyStateManager ctx) : base(key)
    {
        this.ctx = ctx;
    }

    public override void EnterState() 
    {
        ctx.anim.SetBool("isMoving", true);
        float dir = ctx.HorizontalDirectionToPlayer();
        ctx.faceDirection(dir);
    }

    public override void UpdateState()
    {
        ctx.SetXVelocity(ctx.HorizontalDirectionToPlayer() * ctx.stats.chaseSpeed);
    }

    public override void ExitState()
    {
        ctx.anim.SetBool("isMoving", false);
        ctx.SetXVelocity(0f);
    }

    public override EnemyState GetNextState()
    {
        float xDist = ctx.HorizontalDistanceToPlayer();
        float yDist = Mathf.Abs(ctx.transform.position.y - ctx.player.transform.position.y);

        // Transition to Attack ONLY if in horizontal AND vertical range
        if (xDist <= ctx.stats.attackRange && yDist <= ctx.stats.attackHeight)
        {
            return EnemyState.Attack;
        }

        if (xDist >= ctx.stats.deaggroRange) return EnemyState.Despawn;

        return EnemyState.Aggro;
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }
}
