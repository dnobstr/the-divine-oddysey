using UnityEngine;

// ─── EnemyWanderState ─────────────────────────────────────────────────────────
// Walks left or right randomly. Only sets X velocity — gravity handles Y.
// Reverses direction when it drifts past wanderRadius from spawn, or on timer.
//   → Aggro : player enters aggroRange (horizontal)
// ─────────────────────────────────────────────────────────────────────────────

public class EnemyWanderState : BaseState<EnemyState>
{
    private readonly EnemyStateManager ctx;

    private float direction;    // -1 = left, 1 = right
    private float _wanderTimer;

    public EnemyWanderState(EnemyState key, EnemyStateManager ctx) : base(key)
    {
        this.ctx = ctx;
    }

    public override void EnterState()
    {
        ctx.anim.SetBool("isMoving", true);
        PickNewDirection();
    }

    public override void UpdateState()
    {
        ctx.SetXVelocity(direction * ctx.stats.wanderSpeed);
        ctx.faceDirection(direction);
        TickTimer();
        CheckBounds();
    }

    public override void ExitState()
    {
        ctx.anim.SetBool("isMoving", false);
        ctx.SetXVelocity(0f);
    }

    public override EnemyState GetNextState()
    {
        if (ctx.HorizontalDistanceToPlayer() <= ctx.stats.aggroRange)
            return EnemyState.Aggro;

        return EnemyState.Wander;
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }

    // ── Internals ─────────────────────────────────────────────────────────────
    public void OnCollisionEnter2D(Collision2D collision)
    {
        // Only reverse on side-hits, not floor/ceiling contacts
        foreach (ContactPoint2D contact in collision.contacts)
        {
            // A normal pointing mostly horizontal means a wall/enemy hit
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                ReverseDirection();
                return; // one reversal per collision is enough
            }
        }
    }

    private void TickTimer()
    {
        _wanderTimer -= Time.deltaTime;
        if (_wanderTimer <= 0f)
            PickNewDirection();
    }

    private void ReverseDirection()
    {
        // Flip the direction variable
        direction *= -1f;

        // Reset the wander timer so it doesn't immediately try to flip back
        _wanderTimer = ctx.stats.wanderInterval;

        // Visually flip the sprite immediately
        ctx.faceDirection(direction);
    }

    // Reverse direction before the enemy walks off its leash
    private void CheckBounds()
    {
        float offsetX = ctx.transform.position.x - ctx.spawnPosition.x;

        // Use ReverseDirection if we exceed the wander radius
        if (offsetX > ctx.stats.wanderRadius && direction > 0f)
        {
            ReverseDirection();
        }
        else if (offsetX < -ctx.stats.wanderRadius && direction < 0f)
        {
            ReverseDirection();
        }
    }

    private void PickNewDirection()
    {
        direction   = Random.value > 0.5f ? 1f : -1f;
        _wanderTimer = ctx.stats.wanderInterval;
    }
}
