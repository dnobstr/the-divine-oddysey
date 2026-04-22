// PlayerStats.cs
// Attach to Player GameObject. Each state reads its own block.
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public NormalStats normal;
    public OrderStats order;
    public ChaosStats chaos;
    public DivineOrderStats divineOrder;
    public DivineChaosStats divineChaos;
}

// ─── Shared Base Stats ────────────────────────────────────────────────────────

[System.Serializable]
public class JumpStats
{
    public float force = 8;
}

[System.Serializable]
public class DashStats
{
    public float speed = 22f;
    public float duration = 0.18f;
    public float meterGain = 10f;
}

[System.Serializable]
public class AttackStats
{
    public float damage = 10f;
    public float windupDuration = .1f;   // delay before hit lands
    public float hbDuration = .5f;
    public float resolveDuration = .1f;  // lockout after hit
    public float meterGain = 20f;
    public GameObject attackHb;
}

[System.Serializable]
public class AirAttackStats
{
    public float damage = 10f;
    public float windupDuration = .2f;   // delay before hit lands
    public float hbDuration = .3f;
    public float resolveDuration = .1f;
    public float force;
    public float meterGain = 15f;
    public GameObject jumpAttackhb;
}

// ─── Normal State ─────────────────────────────────────────────────────────────

[System.Serializable]
public class NormalStats
{
    public DashStats dash;
    public JumpStats jump;
    public AttackStats attack;
    public AirAttackStats airAttack;
}

// ─── Order State ──────────────────────────────────────────────────────────────

[System.Serializable]
public class OrderStats
{
    public OrderDashStats orderDash;
    public OrderJumpStats orderJump;
    public AttackStats orderAttack;
    public OrderAirAttackStats orderAirAttack;
}

[System.Serializable]
public class OrderDashStats : DashStats
{
    public float invisDuration = 2f;
}

[System.Serializable]
public class OrderJumpStats : JumpStats
{
    public JumpStats jump;
    public float gravityScale = 0.3f; // slowed rise
}

[System.Serializable]
public class OrderAirAttackStats : AirAttackStats
{
    public float pulseRadius = 4f;
    public float pulseDuration = 1.5f;
    public float invincibiltyWindow = 2f;
}

[System.Serializable]
public class DivineOrderStats
{
    public DivineOrderDashStats divineDash;
    public OrderJumpStats divineJump;
    public AttackStats divineAttack;
    public OrderAirAttackStats divineAirAttack;
}

[System.Serializable]
public class DivineOrderDashStats : DashStats
{
    public float divineOrderTimeSlow = .35f;
    public float divineOrderTimeSlowDuration = 2f;
}
// ─── Chaos State ──────────────────────────────────────────────────────────────

[System.Serializable]
public class ChaosStats
{
    public ChaosDashStats chaosDash;
    public JumpStats chaosJump;
    public AttackStats chaosAttack;
    public OrderAirAttackStats chaosAirAttack;
}

[System.Serializable]
public class ChaosDashStats : DashStats
{
    public float trailTickRate = .25f;
    public float trailLifetime = 2f;
    public float trailDOTMultiplier = .25f; // fraction of atkDmg per tick
    public float trailSpawnOffset;
    public GameObject trailSegmentPrefab;
}

[System.Serializable]
public class DivineChaosStats 
{
    public DivineChaosDashStats divineDash;
    public DivineChaosJumpStats divineJump;
    public DivineChaosAttackStats divineAttack;
    public OrderAirAttackStats divineAirAttack;
}

[System.Serializable]
public class DivineChaosDashStats : ChaosDashStats
{
    public float threshold;  // seconds in trail before explosion
    public float radius;
    public float multiplier;
}

[System.Serializable]
public class DivineChaosJumpStats : JumpStats
{
    public float hangGravityScale = 0.1f;      // float at peak
    public float hangDuration = 0.4f;          // beat at top
}

[System.Serializable]
public class DivineChaosAttackStats : AttackStats
{
    public float attackCount = 2;
}