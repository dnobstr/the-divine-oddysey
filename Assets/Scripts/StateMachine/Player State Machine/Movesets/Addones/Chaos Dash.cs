using System.Collections;
using UnityEngine;

public class ChaosDash : MonoBehaviour
{
    [Header("Trail")]
    public GameObject trailSegmentPrefab;

    private PlayerController player;
    private MoveVariant variant;

    public void init(PlayerController player, MoveVariant variant)
    {
        this.player = player;
        this.variant = variant;
        trailSegmentPrefab = player.stats.chaos.chaosDash.trailSegmentPrefab;

        // Self-destructs once dash + any lingering trail lifetime has passed
        Destroy(this, player.stats.chaos.chaosDash.duration + player.stats.chaos.chaosDash.trailLifetime);

        // Start own coroutine — no longer relies on DashState to kick it off
        StartCoroutine(SpawnTrail());
    }

    private IEnumerator SpawnTrail()
    {
        float tickRate = player.stats.chaos.chaosDash.trailTickRate;
        float dashEndTime = Time.time + player.stats.chaos.chaosDash.duration; // use chaos duration, not normal

        while (Time.time < dashEndTime)
        {
            SpawnTrailSegment();
            yield return new WaitForSeconds(tickRate);
        }
    }

    private void SpawnTrailSegment()
    {
        if (trailSegmentPrefab == null) return;
        GameObject seg = Instantiate(trailSegmentPrefab, new Vector3(player.transform.position.x, player.stats.chaos.chaosDash.trailSpawnOffset, player.transform.position.z), Quaternion.identity);
        seg.GetComponent<FireTrail>()?.init(player, variant);
    }
}