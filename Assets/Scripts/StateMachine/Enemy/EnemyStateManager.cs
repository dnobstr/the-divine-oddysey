using UnityEngine;

// ─── EnemyStateManager ────────────────────────────────────────────────────────
// Attach to the enemy prefab root.
// All shared references and tunable values live here; states read from this.
// Only X velocity is ever set by states — gravity and Y are left to physics.
// ─────────────────────────────────────────────────────────────────────────────

public class EnemyStateManager : StateManager<EnemyState>
{
    // ── Shared References (set at runtime) ────────────────────────────────────
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Transform playerTransform;
    [HideInInspector] public Animator anim;
    [HideInInspector] public Vector2 spawnPosition;

    public EnemyStats stats;
    public GameObject player;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        stats = GetComponent<EnemyStats>();
        spawnPosition = transform.position;

        // Swap "Player" if your tag differs
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            playerTransform = player.transform;
        else
            Debug.LogWarning($"[{name}] No GameObject tagged 'Player' found.");

        States[EnemyState.Wander] = new EnemyWanderState(EnemyState.Wander, this);
        States[EnemyState.Aggro] = new EnemyAggroState(EnemyState.Aggro, this);
        States[EnemyState.Attack] = new EnemyAttackState(EnemyState.Attack, this);
        States[EnemyState.Despawn] = new EnemyDespawnState(EnemyState.Despawn, this);

        CurrentState = States[EnemyState.Wander];
    }

    // ── Helpers used by multiple states ───────────────────────────────────────

    // Horizontal-only distance — sidescroller doesn't care about Y for aggro
    public float HorizontalDistanceToPlayer()
    {
        if (playerTransform == null) return Mathf.Infinity;
        return Mathf.Abs(playerTransform.position.x - transform.position.x);
    }

    // +1 if player is to the right, -1 if to the left
    public float HorizontalDirectionToPlayer()
    {
        if (playerTransform == null) return 0f;
        return Mathf.Sign(playerTransform.position.x - transform.position.x);
    }

    // Preserve Y so gravity / platform physics are never overridden
    public void SetXVelocity(float x)
    {
        rb.linearVelocity = new Vector2(x, rb.linearVelocity.y);
    }

    public void faceDirection(float direction)
    {
        if (direction == 0f) return;
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * Mathf.Sign(direction);
        transform.localScale = scale;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            if (Time.time >= stats.lastDamageTime + stats.contactCooldown)
            {
                ApplyContactDamage(other.gameObject);

                // 3. Reset the timer to the current time
                stats.lastDamageTime = Time.time;
            }
        }
    }

    private void ApplyContactDamage(GameObject player)
    {
        Debug.Log("Dealing contact damage to player.");
        // Your damage logic here, e.g.:
        player.GetComponent<Health>().takeDamage(stats.attackDamage);
    }
}
