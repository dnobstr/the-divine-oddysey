// PlayerStats.cs
// Attach to Player GameObject. Each state reads its own block.
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public NormalStateStats normal;
    public OrderStateStats order;
    public ChaosStateStats chaos;
}

// ─── Shared Base Stats ────────────────────────────────────────────────────────

[System.Serializable]
public class JumpStats
{
    public float force;
    public float gravityScale = 1f;
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
public class AttackStats
{
    public float damage;
    public float duration;
    public float meterGain;
    public float jumpAtkMultiplier;
    public float jumpAtkDownForce;
    public GameObject attackHb;
}

[System.Serializable]
public class AirAttackStats
{
    public float damage;
    public float duration;
    public float downForce;
    public float meterGain;
    public GameObject jumpAttackhb;
}

// ─── Normal State ─────────────────────────────────────────────────────────────

[System.Serializable]
public class NormalStateStats
{
    public JumpStats jump;
    public DashStats dash;
    public AttackStats attack;
    public AirAttackStats jumpAttack;
}

// ─── Order State ──────────────────────────────────────────────────────────────

[System.Serializable]
public class OrderStateStats
{
    public JumpStats jump;
    public DashStats dash;

    [Header("Attack")]
    public AttackStats attack;
    public float windupDuration;   // delay before hit lands
    public float resolveDuration;  // lockout after hit

    [Header("Jump Attack")]
    public AirAttackStats jumpAttack;
    public OrderAirAttackStats orderAirAttack;   // wide stagger on land

    [Header("Special — Ascent")]
    public OrderAscentStats ascent;

    [Header("Divine Order — Attack")]
    public DivineOrderAttackStats divineAttack;

    [Header("Divine Order — Dash (Freeze)")]
    public float freezeRadius;

    [Header("Divine Order — Jump Attack")]
    public AirAttackStats divineJumpAttack;
}

[System.Serializable]
public class DivineOrderAttackStats
{
    public float windupDuration;
    public float resolveDuration;
    public float meterGain;
    public float timerDamagePerSecond; // patience-scaling rate
}

[System.Serializable]
public class OrderAscentStats
{
    public float force;
    public float gravityScale = 0.3f; // slowed rise
    public float pulseRadius = 4f;
    public float pulseDuration = 1.5f;
}

[System.Serializable]
public class OrderAirAttackStats
{
    public float staggerRadius;
}
// ─── Chaos State ──────────────────────────────────────────────────────────────

[System.Serializable]
public class ChaosStateStats
{
    public JumpStats jump;
    public DashStats dash;

    [Header("Attack — Rapid Chain")]
    public AttackStats attack;
    public ChainAttackStats chain;

    [Header("Special — Dash Ignition (Trail)")]
    public IgnitionStats ignition;

    [Header("Jump Attack")]
    public AirAttackStats jumpAttack;
    public float splashRadius;    // AOE on hit

    [Header("Divine Chaos — Attack")]
    public DivineChaosDashStats divineAttack;

    [Header("Divine Chaos — Dash (Blast Proc)")]
    public BlastProcStats blastProc;

    [Header("Special — Launch")]
    public ChaosLaunchStats launch;

    [Header("Special — Eruption")]
    public EruptionStats eruption;

    [Header("Divine Chaos — Jump Attack")]
    public AirAttackStats divineJumpAttack;

}

[System.Serializable]
public class ChainAttackStats
{
    public float startInterval; // first swing timing
    public float minInterval;   // fastest possible swing
    public float speedStep;     // acceleration per swing
    public float inputWindow;   // re-tap window
    public float count;
}

[System.Serializable]
public class IgnitionStats
{
    public float trailTickRate;
    public float trailLifetime;
    public float trailDOTMultiplier; // fraction of atkDmg per tick
    public float selfDOTMultiplier;  // fraction of atkDmg on land
    public int selfDOTTicks;
    public float selfDOTInterval;
    public GameObject trailSegmentPrefab;
}

[System.Serializable]
public class DivineChaosDashStats
{
    public float chainStartInterval;
    public float chainMinInterval;
    public float chainSpeedStep;
    public float selfDOTMultiplier; // compounds per stack
}

[System.Serializable]
public class BlastProcStats
{
    public float threshold;  // seconds in trail before explosion
    public float radius;
    public float multiplier;
}

[System.Serializable]
public class ChaosLaunchStats
{
    public float launchForce;
    public float hangGravityScale = 0.1f;      // float at peak
    public float hangDuration = 0.4f;          // beat at top
    public float airtimeDOTRate = 2f;          // dmg per second on self
    public float airtimeDOTDelay = 0.5f;       // grace before DOT starts
}

[System.Serializable]
public class EruptionStats
{
    public float force = 40f;
    public float arenaYMin = -5f;
    public float arenaYMax = 10f;
    public float selfDamagePerBounce = 3f;
    public float enemyDamagePerCollision = 15f;
    public float criticalHealthThreshold = 0.2f;
}