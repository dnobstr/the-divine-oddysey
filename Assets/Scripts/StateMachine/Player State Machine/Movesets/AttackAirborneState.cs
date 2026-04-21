using System.Collections;
using UnityEngine;

/// <summary>
/// Airborne Attack variants:
///   Normal      – quick aerial slash, slight upward push
///   Order       – slow downward slam, pushes enemies outward (radial effector)
///   Chaos       – rapid downward slam, pulls enemies inward (radial effector) + invulnerability window
///   DivineOrder – holy radial burst, freezes vertical velocity
///   DivineChaos – spinning chaos drill downward
/// </summary>
public class AttackAirborneState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;
    private MoveVariant variant;

    private float attackTimer;
    private bool attackDone;

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
        PerformAttackLogic();
    }

    private void ApplyVariantPhysics()
    {
        switch (variant)
        {
            case MoveVariant.Normal:
                // Quick upward push
                player.rb.linearVelocityY = player.stats.normal.airAttack.force;
                attackTimer = player.stats.normal.airAttack.windupDuration
                            + player.stats.normal.airAttack.hbDuration
                            + player.stats.normal.airAttack.resolveDuration;
                break;

            case MoveVariant.Order:
                // Slow downward slam — gravity stays on, force pushes down
                player.rb.linearVelocityY = -player.stats.order.orderAirAttack.force;
                attackTimer = player.stats.order.orderAirAttack.windupDuration
                            + player.stats.order.orderAirAttack.hbDuration
                            + player.stats.order.orderAirAttack.resolveDuration;
                break;

            case MoveVariant.Chaos:
                // Rapid downward slam
                player.rb.linearVelocityY = -player.stats.chaos.chaosAirAttack.force;
                attackTimer = player.stats.chaos.chaosAirAttack.windupDuration
                            + player.stats.chaos.chaosAirAttack.hbDuration
                            + player.stats.chaos.chaosAirAttack.resolveDuration;
                break;

            case MoveVariant.DivineOrder:
                player.rb.gravityScale = 0f;
                player.rb.linearVelocity = Vector2.zero;
                attackTimer = player.stats.divineOrder.divineAirAttack.windupDuration
                            + player.stats.divineOrder.divineAirAttack.hbDuration
                            + player.stats.divineOrder.divineAirAttack.resolveDuration;
                break;

            case MoveVariant.DivineChaos:
                player.rb.linearVelocityY = -player.stats.divineChaos.divineAirAttack.force * 1.2f;
                attackTimer = player.stats.divineChaos.divineAirAttack.windupDuration
                            + player.stats.divineChaos.divineAirAttack.hbDuration
                            + player.stats.divineChaos.divineAirAttack.resolveDuration;
                break;
        }
    }

    private void PerformAttackLogic()
    {
        switch (variant)
        {
            case MoveVariant.Normal:
                // TODO: activate normal aerial hitbox
                break;

            case MoveVariant.Order:
                player.stateMeter?.addOrder(player.stats.order.orderAirAttack.meterGain);
                SpawnRadialEffector(push: true,
                    player.stats.order.orderAirAttack.pulseRadius,
                    player.stats.order.orderAirAttack.pulseDuration,
                    player.stats.order.orderAirAttack.jumpAttackhb);
                // TODO: activate order aerial hitbox
                break;

            case MoveVariant.Chaos:
                player.stateMeter?.addChaos(player.stats.chaos.chaosAirAttack.meterGain);
                SpawnRadialEffector(push: false,
                    player.stats.chaos.chaosAirAttack.pulseRadius,
                    player.stats.chaos.chaosAirAttack.pulseDuration,
                    player.stats.chaos.chaosAirAttack.jumpAttackhb);
                player.StartCoroutine(ApplyInvulnerability(player.stats.chaos.chaosAirAttack.invincibiltyWindow));
                // TODO: activate chaos aerial hitbox
                break;

            case MoveVariant.DivineOrder:
                player.stateMeter?.addOrder(player.stats.divineOrder.divineAirAttack.meterGain);
                // TODO: activate divine order aerial hitbox
                break;

            case MoveVariant.DivineChaos:
                player.stateMeter?.addChaos(player.stats.divineChaos.divineAirAttack.meterGain);
                // TODO: activate divine chaos aerial hitbox
                break;
        }
    }

    // ── Helpers ──────────────────────────────────────────────────────────────────

    /// <summary>
    /// Spawns a RadialEffector at the player's position.
    /// push=true  → pushes enemies outward (Order slam)
    /// push=false → pulls enemies inward  (Chaos slam)
    /// </summary>
    private void SpawnRadialEffector(bool push, float radius, float duration, GameObject prefab)
    {
        if (prefab == null) return;

        GameObject obj = Object.Instantiate(prefab, player.transform.position, Quaternion.identity);
        obj.GetComponent<RadialEffector>()?.Init(player, radius, duration, push);
    }

    /// <summary>Grants invulnerability for a fixed window then restores it.</summary>
    private IEnumerator ApplyInvulnerability(float duration)
    {
        player.GetComponent<Health>().isVulnerable = false;
        yield return new WaitForSeconds(duration);
        player.GetComponent<Health>().isVulnerable = true;
    }

    // ── State machine ─────────────────────────────────────────────────────────────

    public override void UpdateState()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
            attackDone = true;
    }

    public override void ExitState()
    {
        player.rb.gravityScale = originalGravity;
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