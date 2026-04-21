using System.Collections;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

/// <summary>
/// Ground Attack variants:
///   Normal      – single hit, no meter effect
///   Order       – slow powerful swing, +Order on hit
///   Chaos       – fast swing, +Chaos on hit
///   DivineOrder – long windup, invincible while casting, wide holy burst
///   DivineChaos – one continuous animation, spawns 2 hitboxes timed to each swing
/// </summary>
public class AttackState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;
    private MoveVariant variant;

    private bool attackDone;
    private Coroutine attackCoroutine;

    public AttackState(PlayerStateKey key, PlayerController player) : base(key)
    {
        this.player = player;
    }

    public override void EnterState()
    {
        variant = player.getCurrentVariant();
        attackDone = false;

        player.rb.linearVelocity = Vector2.zero;

        if (variant == MoveVariant.DivineChaos || variant == MoveVariant.DivineOrder)
            player.anim?.SetTrigger($"attack - {variant}");
        else
            player.anim?.SetTrigger($"attack - Normal");

        attackCoroutine = player.StartCoroutine(AttackSequence());
    }

    // ── Main sequence ────────────────────────────────────────────────────────────

    private IEnumerator AttackSequence()
    {
        switch (variant)
        {
            case MoveVariant.Normal:
                yield return SingleHit(
                    player.stats.normal.attack,
                    player.stats.normal.attack.attackHb,
                    player.stats.normal.attack.hbDuration);
                break;

            case MoveVariant.Order:
                yield return SingleHit(
                    player.stats.order.orderAttack,
                    player.stats.order.orderAttack.attackHb,
                    player.stats.order.orderAttack.hbDuration);
                player.stateMeter?.addOrder(player.stats.order.orderAttack.meterGain);
                break;

            case MoveVariant.Chaos:
                yield return SingleHit(
                    player.stats.chaos.chaosAttack,
                    player.stats.chaos.chaosAttack.attackHb,
                    player.stats.chaos.chaosAttack.hbDuration);
                player.stateMeter?.addChaos(player.stats.chaos.chaosAttack.meterGain);
                break;

            case MoveVariant.DivineOrder:
                yield return DivineOrderAttack();
                break;

            case MoveVariant.DivineChaos:
                yield return DivineChaosAttack();
                break;
        }

        attackDone = true;
    }

    // ── Shared single-hit flow: windup → spawn hb → resolve ─────────────────────

    private IEnumerator SingleHit(AttackStats stats, GameObject hbPrefab, float duration)
    {
        yield return new WaitForSeconds(stats.windupDuration);
        SpawnHitbox(hbPrefab, stats.damage, stats.hbDuration);
        yield return new WaitForSeconds(stats.hbDuration);
        yield return new WaitForSeconds(stats.resolveDuration);
    }

    // ── DivineOrder: long windup with full invincibility, then burst ─────────────

    private IEnumerator DivineOrderAttack()
    {
        AttackStats stats = player.stats.divineOrder.divineAttack;

        player.GetComponent<Health>().isVulnerable = false;
        yield return new WaitForSeconds(stats.windupDuration);
        player.GetComponent<Health>().isVulnerable = true;

        SpawnHitbox(stats.attackHb, stats.damage, stats.hbDuration);
        yield return new WaitForSeconds(stats.hbDuration);
        yield return new WaitForSeconds(stats.resolveDuration);
    }

    // ── DivineChaos: two hitboxes timed to the two swings in the animation ───────

    private IEnumerator DivineChaosAttack()
    {
        AttackStats stats = player.stats.divineChaos.divineAttack;

        yield return new WaitForSeconds(stats.windupDuration);
        SpawnHitbox(stats.attackHb, stats.damage, stats.hbDuration);

        yield return new WaitForSeconds(stats.hbDuration);
        SpawnHitbox(stats.attackHb, stats.damage, stats.hbDuration);

        yield return new WaitForSeconds(stats.resolveDuration);
        player.stateMeter?.addChaos(stats.meterGain);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────────

    private void SpawnHitbox(GameObject prefab, float damage, float duration)
    {
        GameObject hb = GameObject.Instantiate(prefab, player.transform.position, player.transform.rotation);

        var hitbox = hb.GetComponent<BaseHitbox>();
        if (hitbox != null)
        {
            hitbox.damage = damage;
            hitbox.lifetime = duration; // This is the field in Base Hitbox.cs
        }
    }

    // ── Base overrides ───────────────────────────────────────────────────────────

    public override void UpdateState() { }

    public override void ExitState()
    {
        // Stop the coroutine if the state is interrupted mid-sequence —
        // prevents a stale coroutine spawning a hitbox after we've left the state
        if (attackCoroutine != null)
        {
            player.StopCoroutine(attackCoroutine);
            attackCoroutine = null;
        }

        // Safety: restore vulnerability if interrupted during DivineOrder windup
        player.GetComponent<Health>().isVulnerable = true;
    }

    public override PlayerStateKey GetNextState()
    {
        if (!attackDone) return StateKey;

        if (player.attackPressed) return PlayerStateKey.Attack;
        if (player.jumpPressed) return PlayerStateKey.Jump;
        if (player.dashPressed) return PlayerStateKey.Dash;

        return !player.isGrounded
            ? PlayerStateKey.Fall
            : Mathf.Abs(player.HorizontalInput) > 0.01f
            ? PlayerStateKey.Move
            : PlayerStateKey.Idle;
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other) { }
    public override void OnTriggerExit2D(Collider2D other) { }
}