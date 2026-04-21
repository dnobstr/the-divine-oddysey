using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    [Header("Detection")]
    public float aggroRange   = 8f;    // horizontal — player enters  → Aggro
    public float deaggroRange = 20f;   // horizontal — player exits   → Despawn

    [Header("Combat")]
    public GameObject attackHitboxPrefab; // Drag your hitbox prefab here
    public float attackOffset = 1.2f;   // Distance in front of enemy
    public float attackRange = 4f;
    public float attackHeight = 2f;
    public float attackCooldown = 1.5f;
    public float attackDamage = 10f;
    public float attackWindup = 0.2f;
    public float attackDuration = 0.3f; // How long the prefab lives

    public float contactCooldown = 1f;
    public float lastDamageTime;

    [Header("Wander")]
    public float wanderSpeed    = 2f;
    public float wanderRadius   = 6f;  // max horizontal drift from spawn
    public float wanderInterval = 3f;  // seconds before picking a new direction

    [Header("Chase")]
    public float chaseSpeed = 5f;

    [Header("Despawn")]
    public float despawnDelay = 1f;    // seconds before Destroy (room for VFX)
}
