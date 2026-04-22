using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// Airborne Attack variants:
///   Normal      – quick aerial slash, slight upward push
///   Order       – slow downward slam, spawns push effector on landing
///   Chaos       – rapid downward slam, spawns pull effector + invuln on landing
///   DivineOrder – holy radial burst, freezes vertical velocity
///   DivineChaos – spinning chaos drill downward
/// </summary>
public class AttackAirborneState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;
    private MoveVariant variant;

    private bool attackDone;
    private Coroutine attackCoroutine;
    private Coroutine invulnCoroutine;

    private float originalGravity;

    public AttackAirborneState(PlayerStateKey key, PlayerController player) : base(key)
    {
        this.player = player;
    }

    public override void EnterState()
    {
        variant = player.getCurrentVariant();
        attackDone = false;
        originalGravity = player.rb.gravityScale;

        ApplyVariantPhysics();
        attackCoroutine = player.StartCoroutine(AttackSequence());
    }

    // ── Physics setup (unchanged) ────────────────────────────────────────────────

    private void ApplyVariantPhysics()
    {
        switch (variant)
        {
            case MoveVariant.Normal:
                player.rb.linearVelocityY = player.stats.normal.airAttack.force;
                break;

            case MoveVariant.Order:
                player.rb.linearVelocityY = -player.stats.order.orderAirAttack.force;
                break;

            case MoveVariant.Chaos:
                player.rb.linearVelocityY = -player.stats.chaos.chaosAirAttack.force;
                break;

            case MoveVariant.DivineOrder:
                player.rb.gravityScale = 0f;
                player.rb.linearVelocity = Vector2.zero;
                break;

            case MoveVariant.DivineChaos:
                player.rb.linearVelocityY = -player.stats.divineChaos.divineAirAttack.force * 1.2f;
                break;
        }
    }

    // ── Main sequence ────────────────────────────────────────────────────────────

    private IEnumerator AttackSequence()
    {
        switch (variant)
        {
            case MoveVariant.Normal:
                yield return AerialHit(
                    player.stats.normal.airAttack,
                    player.stats.normal.airAttack.jumpAttackhb,
                    waitForLanding: false);
                break;

            case MoveVariant.Order:
                // Windup fires immediately in the air, effector spawns on landing
                yield return new WaitForSeconds(player.stats.order.orderAirAttack.windupDuration);
                yield return WaitUntilGrounded();
                SpawnRadialEffector(
                    push: false,
                    player.stats.order.orderAirAttack.pulseRadius,
                    player.stats.order.orderAirAttack.pulseDuration,
                    player.stats.order.orderAirAttack.jumpAttackhb,
                    player.stats.order.orderAirAttack.force);
                invulnCoroutine = player.StartCoroutine(
                    ApplyInvulnerability(player.stats.chaos.chaosAirAttack.invincibiltyWindow));
                player.stateMeter?.addOrder(player.stats.order.orderAirAttack.meterGain);
                yield return new WaitForSeconds(player.stats.order.orderAirAttack.resolveDuration);
                break;

            case MoveVariant.Chaos:
                // Windup fires immediately in the air, effector + invuln spawn on landing
                yield return new WaitForSeconds(player.stats.chaos.chaosAirAttack.windupDuration);
                yield return WaitUntilGrounded();
                SpawnRadialEffector(
                    push: true,
                    player.stats.chaos.chaosAirAttack.pulseRadius,
                    player.stats.chaos.chaosAirAttack.pulseDuration,
                    player.stats.chaos.chaosAirAttack.jumpAttackhb,
                    player.stats.chaos.chaosAirAttack.force);
                player.stateMeter?.addChaos(player.stats.chaos.chaosAirAttack.meterGain);
                yield return new WaitForSeconds(player.stats.chaos.chaosAirAttack.resolveDuration);
                break;

            case MoveVariant.DivineOrder:
                yield return AerialHit(
                    player.stats.divineOrder.divineAirAttack,
                    player.stats.divineOrder.divineAirAttack.jumpAttackhb,
                    waitForLanding: false);
                player.stateMeter?.addOrder(player.stats.divineOrder.divineAirAttack.meterGain);
                break;

            case MoveVariant.DivineChaos:
                yield return AerialHit(
                    player.stats.divineChaos.divineAirAttack,
                    player.stats.divineChaos.divineAirAttack.jumpAttackhb,
                    waitForLanding: false);
                player.stateMeter?.addChaos(player.stats.divineChaos.divineAirAttack.meterGain);
                break;
        }

        attackDone = true;
    }

    // ── Shared aerial hit: windup → hb → resolve, optionally waits for landing ───

    private IEnumerator AerialHit(AirAttackStats stats, GameObject hbPrefab, bool waitForLanding)
    {
        yield return new WaitForSeconds(stats.windupDuration);

        if (waitForLanding)
            yield return WaitUntilGrounded();

        SpawnHitbox(hbPrefab, stats.damage);
        yield return new WaitForSeconds(stats.hbDuration);
        yield return new WaitForSeconds(stats.resolveDuration);
    }

    // ── Waits until player.isGrounded is true ────────────────────────────────────

    private IEnumerator WaitUntilGrounded()
    {
        while (!player.isGrounded)
            yield return null;
    }

    // ── Helpers ──────────────────────────────────────────────────────────────────

    private void SpawnHitbox(GameObject prefab, float damage)
    {
        if (prefab == null) return;
        GameObject hb = Object.Instantiate(prefab, player.transform.position, player.transform.rotation);
        var hitbox = hb.GetComponent<BaseHitbox>();
        if (hitbox != null) hitbox.damage = damage;
    }

    private void SpawnRadialEffector(bool push, float radius, float duration, GameObject prefab, float force)
    {
        if (prefab == null) return;
        GameObject obj = Object.Instantiate(prefab, player.transform.position, Quaternion.identity);
        obj.GetComponent<RadialEffector>()?.init(player, radius, duration, push, force);
    }

    private IEnumerator ApplyInvulnerability(float duration)
    {
        player.GetComponent<Health>().isVulnerable = false;
        yield return new WaitForSeconds(duration);
        player.GetComponent<Health>().isVulnerable = true;
    }

    // ── State machine ─────────────────────────────────────────────────────────────

    public override void UpdateState() { }

    public override void ExitState()
    {
        player.rb.gravityScale = originalGravity;

        if (attackCoroutine != null)
        {
            player.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        // If interrupted mid-invuln window, restore vulnerability
        if (invulnCoroutine != null)
        {
            player.StopCoroutine(invulnCoroutine);
            invulnCoroutine = null;
        }
        player.GetComponent<Health>().isVulnerable = true;
    }

    public override PlayerStateKey GetNextState()
    {
        if (!attackDone) return StateKey;

        if (player.attackPressed && !player.isGrounded) return PlayerStateKey.AttackAirborne;
        if (player.isGrounded)
            return Mathf.Abs(player.HorizontalInput) > 0.01f ? PlayerStateKey.Move : PlayerStateKey.Idle;

        return PlayerStateKey.Fall;
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other) { }
    public override void OnTriggerExit2D(Collider2D other) { }
}