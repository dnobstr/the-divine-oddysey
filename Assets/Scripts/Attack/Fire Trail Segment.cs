// FireTrailSegment.cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTrailSegment : MonoBehaviour
{
    public float dps;
    public bool isBlastProc;              // Divine Chaos only
    public float blastProcThreshold = 1; // seconds before blast

    private PlayerController player;
    private Dictionary<Collider2D, float> _enemyEnterTimes = new();

    public void Init(PlayerController player, bool blastProc)
    {
        this.player = player;
        dps = player.stats.chaos.attack.damage * player.stats.chaos.trailDOTMultiplier;
        isBlastProc = blastProc;
        Destroy(gameObject, player.stats.chaos.trailLifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(player.tag)) return;

        IEffectable effectable = other.GetComponent<IEffectable>();
        effectable?.Ignite(dps);

        if (isBlastProc)
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
            PlayerHp hp = hit.GetComponent<PlayerHp>();
            hp?.takeDmg(player.stats.chaos.attack.damage * 1.5f);
        }

        // Optional: spawn VFX prefab here
        Destroy(gameObject); // trail segment consumed on blast
    }
}