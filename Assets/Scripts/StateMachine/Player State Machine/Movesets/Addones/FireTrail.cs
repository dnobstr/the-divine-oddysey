// FireTrailSegment.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrail : MonoBehaviour
{
    public float dps;
    public MoveVariant variant;              // Divine Chaos only
    public float blastProcThreshold = 1; // seconds before blast

    private PlayerController player;
    private Dictionary<Collider2D, float> _enemyEnterTimes = new();

    public void init(PlayerController player, MoveVariant variant)
    {
        this.player = player;
        dps = player.stats.chaos.chaosAttack.damage * player.stats.chaos.chaosDash.trailDOTMultiplier;
        this.variant = variant;
        Destroy(gameObject, player.stats.chaos.chaosDash.trailLifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(player.tag)) return;

        Health hp = other.GetComponent<Health>();
        hp.applyDOT(dps, player.stats.chaos.chaosDash.trailLifetime, player.stats.chaos.chaosDash.trailTickRate * player.stats.chaos.chaosDash.trailLifetime);

        if (variant == MoveVariant.DivineChaos)
        {
            _enemyEnterTimes[other] = Time.time;
            StartCoroutine(CheckBlastProc(other));
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        _enemyEnterTimes.Remove(other);
    }

    private IEnumerator CheckBlastProc(Collider2D other)
    {
        yield return new WaitForSeconds(blastProcThreshold);

        // Enemy still standing in trail after threshold — BOOM
        if (_enemyEnterTimes.ContainsKey(other) && other != null)
        {
            TriggerBlast(other.transform.position);
        }
    }

    private void TriggerBlast(Vector3 position)
    {
        Debug.Log("[Divine Chaos] Blast Proc!");

        // AOE damage to everything nearby
        Collider2D[] hits = Physics2D.OverlapCircleAll(position, 2f);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(player.tag)) continue;
            Health hp = hit.GetComponent<Health>();
            hp.takeDamage(player.stats.chaos.chaosAttack.damage * player.stats.chaos.chaosDash.trailDOTMultiplier);
        }

        // Optional: spawn VFX prefab here
        Destroy(gameObject); // trail segment consumed on blast
    }
}