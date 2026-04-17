// ChaosDash.cs
using System.Collections;
using UnityEngine;

public class ChaosDash : MonoBehaviour
{
    [Header("Trail")]
    public GameObject trailSegmentPrefab; // a trigger zone that applies DOT

    private PlayerController player;

    public void init(PlayerController player)
    {
        this.player = player;
        this.trailSegmentPrefab = player.stats.chaos.ignition.trailSegmentPrefab;

        Destroy(gameObject, player.stats.chaos.dash.duration + player.stats.chaos.ignition.trailLifetime);
    }

    public IEnumerator execute()
    {
        float elapsed = 0f;
        float dashEndTime = Time.time + player.stats.normal.dash.duration;

        player.rb.AddForceX(player.stats.chaos.dash.speed * player.direction, ForceMode2D.Impulse);

        while (Time.time < dashEndTime)
        {
            player.rb.gravityScale = 0;
            spawnTrailSegment();
            elapsed += player.stats.chaos.ignition.trailTickRate;
            yield return new WaitForSeconds(player.stats.chaos.ignition.trailTickRate);
        }

        player.stateMeter.AddChaos(player.stats.chaos.dash.meterGain);

        // DOT ticks on self immediately on land
        StartCoroutine(applySelfDOT());
    }

    private void spawnTrailSegment()
    {
        if (trailSegmentPrefab == null) return;
        GameObject seg = Instantiate(trailSegmentPrefab, player.transform.position, Quaternion.identity);
        seg.GetComponent<FireTrailSegment>()?.Init(player, false);
    }

    private IEnumerator applySelfDOT()
    {
        int ticks = 3;
        float dps = player.stats.chaos.attack.damage * 0.1f;
        PlayerHp selfHp = player.GetComponent<PlayerHp>();

        for (int i = 0; i < ticks; i++)
        {
            selfHp?.takeDmg(dps);
            yield return new WaitForSeconds(0.5f);
        }
    }
}