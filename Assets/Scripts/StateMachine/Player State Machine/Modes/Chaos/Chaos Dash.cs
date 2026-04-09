// ChaosDash.cs
using System.Collections;
using UnityEngine;

public class ChaosDash : MonoBehaviour
{
    [Header("Trail")]
    public GameObject trailSegmentPrefab; // a trigger zone that applies DOT
    public float trailTickRate = 0.05f;   // how often a segment is placed

    private PlayerController player;

    public void Init(PlayerController player)
    {
        this.player = player;
        this.trailSegmentPrefab = player.trailSegmentPrefab;

    }

    public IEnumerator Execute()
    {
        float originalGravity = player.rb.gravityScale;

        float elapsed = 0f;

        while (player.dashPressed)
        {
            player.rb.gravityScale = 0;
            SpawnTrailSegment();
            elapsed += trailTickRate;
            yield return new WaitForSeconds(trailTickRate);
        }
        originalGravity = player.rb.gravityScale;

        // DOT ticks on self immediately on land
        StartCoroutine(ApplySelfDOT());
    }

    private void SpawnTrailSegment()
    {
        if (trailSegmentPrefab == null) return;
        GameObject seg = Instantiate(trailSegmentPrefab, player.transform.position, Quaternion.identity);
        seg.GetComponent<FireTrailSegment>()?.Init(player, false);
    }

    private IEnumerator ApplySelfDOT()
    {
        int ticks = 3;
        float dps = player.atkDmg * 0.1f;
        PlayerHp selfHp = player.GetComponent<PlayerHp>();

        for (int i = 0; i < ticks; i++)
        {
            selfHp?.takeDmg(dps);
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator DashRoutine()
    {
        float originalGravity = player.rb.gravityScale;

        // Start Dash
        player.rb.gravityScale = 0;
        player.rb.linearVelocity = new Vector2(player.rb.linearVelocity.x, 0); // Freeze Y

        yield return new WaitForSeconds(player.dashDuration);

        // End Dash - Restore normal physics
        player.rb.gravityScale = originalGravity;
        player.anim.SetBool("isDashing", false);
    }
}