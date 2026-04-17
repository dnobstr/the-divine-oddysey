using UnityEngine;

// ─── EnemyStateManager ────────────────────────────────────────────────────────
// Attach to the enemy prefab root.
// All shared references and thresholds live here; states read from this context.
// ─────────────────────────────────────────────────────────────────────────────

public class EnemyStateManager : StateManager<EnemyState>
{
    // ── Shared References (set at runtime) ────────────────────────────────────
    [HideInInspector] public Rigidbody2D Rb;
    [HideInInspector] public Transform   PlayerTransform;
    [HideInInspector] public Vector2     SpawnPosition;

    // ── Detection Ranges ──────────────────────────────────────────────────────
    [Header("Detection")]
    public float aggroRange   = 8f;    // player enters  → Aggro
    public float deaggroRange = 20f;   // player exits   → Despawn

    [Header("Combat")]
    public float attackRange    = 4f;
    public float attackCooldown = 1.5f;
    public float attackDamage   = 10f;

    [Header("Wander")]
    public float wanderSpeed    = 2f;
    public float wanderRadius   = 6f;  // max distance from spawn
    public float wanderInterval = 3f;  // seconds between new destinations

    [Header("Chase")]
    public float chaseSpeed = 5f;

    [Header("Despawn")]
    public float despawnDelay = 1f;    // seconds before Destroy (for anim/vfx)

    // ─────────────────────────────────────────────────────────────────────────

    void Awake()
    {
        Rb            = GetComponent<Rigidbody2D>();
        SpawnPosition = transform.position;

        // Find player — swap tag if yours differs
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
            PlayerTransform = player.transform;
        else
            Debug.LogWarning($"[{name}] No GameObject tagged 'Player' found.");

        // Wire up states
        States[EnemyState.Wander]  = new EnemyWanderState (EnemyState.Wander,  this);
        States[EnemyState.Aggro]   = new EnemyAggroState  (EnemyState.Aggro,   this);
        States[EnemyState.Attack]  = new EnemyAttackState (EnemyState.Attack,  this);
        States[EnemyState.Despawn] = new EnemyDespawnState(EnemyState.Despawn, this);

        CurrentState = States[EnemyState.Wander];
    }

    // ── Helpers used by multiple states ───────────────────────────────────────

    public float DistanceToPlayer()
    {
        if (PlayerTransform == null) return Mathf.Infinity;
        return Vector2.Distance(transform.position, PlayerTransform.position);
    }

    public Vector2 DirectionToPlayer()
    {
        if (PlayerTransform == null) return Vector2.zero;
        return ((Vector2)PlayerTransform.position - (Vector2)transform.position).normalized;
    }
}
