// PlayerStats.cs
// Attach to Player GameObject. Each state reads its own block.
using UnityEngine;
public class PlayerStats : MonoBehaviour
{
    public NormalStateStats normal;

    public OrderStateStats order;

    public ChaosStateStats chaos;
}

[System.Serializable]
public class AttackStats
{
    public float damage;
    public float duration;
    public float multiplier;
    public float meterGain;
    public float jumpAtkMultiplier;
    public float jumpAtkDownForce;
    public GameObject attackHb;
    public GameObject jumpAttackHb;
}

[System.Serializable]
public class DashStats
{
    public float speed;
    public float duration;
    public float lastDashTime;
    public float meterGain;
}

[System.Serializable]
public class JumpStats
{
    public float force;
    public float gravityScale = 1f; // restored after dash
}
// ─────────────────────────────────────────
[System.Serializable]
public class NormalStateStats
{
    [Header("Jump")]
    public JumpStats jump;

    [Header("Dash")]
    public DashStats dash;

    [Header("Attack")]
    public AttackStats attack;
}

// ─────────────────────────────────────────
[System.Serializable]
public class OrderStateStats
{
    [Header("Jump")]
    public JumpStats jump;

    [Header("Dash")]
    public DashStats dash;

    [Header("Attack — slow windup, heavy hit")]
    public AttackStats attack;
    public float windupDuration;   // how long before hit lands
    public float resolveDuration;  // lockout after hit

    [Header("Divine Order Attack")]
    public float divineWindupDuration;
    public float divineResolveDuration;
    public float divineMeterGain;
    public float divineTimerDamagePerSecond; // patience scale rate

    [Header("Divine Order Dash — Freeze")]
    public float freezeRadius;
}

// ─────────────────────────────────────────
[System.Serializable]
public class ChaosStateStats
{
    [Header("Jump")]
    public JumpStats jump;

    [Header("Dash")]
    public DashStats dash;
   
    [Header("Chaos Dash — Ignition")]
    public float trailTickRate;
    public float trailLifetime;
    public float trailDOTMultiplier;  // fraction of atkDmg per tick
    public float selfDOTMultiplier;   // fraction of atkDmg on land
    public int selfDOTTicks;
    public float selfDOTInterval;
    public GameObject trailSegmentPrefab;
    
    [Header("Attack — rapid chain")]
    public AttackStats attack;
    public float chainStartInterval;  // first swing timing
    public float chainMinInterval;    // fastest possible swing
    public float chainSpeedStep;      // how much faster per swing
    public float chainInputWindow;    // how long player has to re-tap
    public float chainCount;

    [Header("Divine Chaos Attack")]
    public float divineChainStartInterval;
    public float divineChainMinInterval;
    public float divineChainSpeedStep;
    public float divineSelfDOTMultiplier; // compounds per stack
    public float divineMeterTopUp;        // meter added per hit to resist drain

    [Header("Divine Chaos Dash — Blast Proc")]
    public float blastProcThreshold; // seconds in trail before explosion
    public float blastProcRadius;
    public float blastProcMultiplier;
}

// ─────────────────────────────────────────