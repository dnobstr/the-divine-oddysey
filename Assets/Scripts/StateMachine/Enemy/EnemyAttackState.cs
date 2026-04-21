using UnityEngine;
public class EnemyAttackState : BaseState<EnemyState>
{
    private readonly EnemyStateManager ctx;

    private float timer;
    private bool hitboxOpen;
    private bool hasSpawnedThisSwing;

    private float Windup => ctx.stats.attackWindup;
    private float Duration => ctx.stats.attackDuration;
    private float Cooldown => ctx.stats.attackCooldown;

    public EnemyAttackState(EnemyState key, EnemyStateManager ctx) : base(key)
    {
        this.ctx = ctx;
    }

    public override void EnterState()
    {
        ctx.SetXVelocity(0f);
        timer = 0f;   // begin windup immediately on entry
        hitboxOpen = false;
        hasSpawnedThisSwing = false;

        float dir = ctx.HorizontalDirectionToPlayer();
        ctx.faceDirection(dir);
        ctx.anim.SetTrigger("attack");
    }

    public override void UpdateState()
    {
        float dir = ctx.HorizontalDirectionToPlayer();
        ctx.faceDirection(dir);

        timer += Time.deltaTime;

        if (!hasSpawnedThisSwing && timer >= Windup)
            OpenHitbox();

        // ── DISABLE LOGIC ──
        if (hitboxOpen && timer >= Windup + Duration)
            CloseHitbox();

        // ── RESET LOGIC: Reset the guard when the cooldown is over ──
        if (timer >= Cooldown)
        {
            timer = 0f;
            hasSpawnedThisSwing = false;
            hitboxOpen = false;
            ctx.anim.SetTrigger("attack");
        }
    }

    public override void ExitState()
    {
        CloseHitbox();
    }

    public override EnemyState GetNextState()
    {
        float xDist = ctx.HorizontalDistanceToPlayer();
        // Calculate absolute vertical distance
        float yDist = Mathf.Abs(ctx.transform.position.y - ctx.player.transform.position.y);

        if (xDist >= ctx.stats.deaggroRange) return EnemyState.Despawn;

        // If player moves out of horizontal range OR vertical height, go back to Aggro
        if (xDist > ctx.stats.attackRange || yDist > ctx.stats.attackHeight)
        {
            return EnemyState.Aggro;
        }

        return EnemyState.Attack;
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other) { }
    public override void OnTriggerExit2D(Collider2D other) { }

    // ── hitbox control ────────────────────────────────────────────────────────

    private void OpenHitbox()
    {
        hasSpawnedThisSwing = true;
        hitboxOpen = true;

        if (ctx.stats.attackHitboxPrefab != null)
        {
            float direction = ctx.transform.localScale.x > 0 ? 1 : -1;

            Vector3 spawnPos = ctx.transform.position + new Vector3(ctx.stats.attackOffset * direction, 0, 0);

            GameObject hbObj = Object.Instantiate(ctx.stats.attackHitboxPrefab, spawnPos, Quaternion.identity);

            hbObj.transform.localScale = new Vector3(ctx.stats.attackHitboxPrefab.transform.localScale.x * direction, ctx.stats.attackHitboxPrefab.transform.localScale.y, ctx.stats.attackHitboxPrefab.transform.localScale.z);

            BaseHitbox hb = hbObj.GetComponent<BaseHitbox>();
            if (hb != null)
            {
                hb.damage = ctx.stats.attackDamage;
                hb.lifetime = ctx.stats.attackDuration;
            }
        }

    }

    private void CloseHitbox()
    {
        hitboxOpen = false;
    }
}