// OrderJump.cs
using System.Collections;
using UnityEngine;

public class OrderJump : MonoBehaviour
{
    private PlayerController player;
    private int _jumpsUsed = 0;
    private const int MaxJumps = 2;

    public void init(PlayerController player) => this.player = player;

    public IEnumerator execute()
    {
        _jumpsUsed = 0;

        // First jump
        performJump();

        yield return new WaitUntil(() => player.isGrounded || player.jumpPressed);

        // Second jump — only if still airborne
        
        if (player.attackPressed)
        {
            player.StartCoroutine(OrderLandingStagger(player.stats.order.jumpAttack.staggerRadius));
        }

        // Wait for landing
        yield return new WaitUntil(() => player.isGrounded);

        // Landing pulse — gravitational freeze
        StartCoroutine(GravitationalPulse());

        player.rb.gravityScale = player.defaultGravityScale;
        Destroy(this);
    }

    private void performJump()
    {
        _jumpsUsed++;
        player.canAirAttack = true;

        player.rb.linearVelocityY = 0f;

        // Second jump is slightly weaker — feels deliberate, not a full relaunch
        float force = _jumpsUsed == 1
            ? player.stats.order.ascent.force
            : player.stats.order.ascent.force * 0.8f;

        player.rb.gravityScale = player.stats.order.ascent.gravityScale;
        player.rb.AddForceY(force, ForceMode2D.Impulse);

        //player.anim.SetTrigger(_jumpsUsed == 1 ? "jump" : "doubleJump");
        Debug.Log($"[Order] Jump {_jumpsUsed} — force: {force}");
    }

    private IEnumerator GravitationalPulse()
    {
        //player.anim.SetTrigger("landingPulse");

        Collider2D[] hits = Physics2D.OverlapCircleAll(
            player.transform.position,
            player.stats.order.ascent.pulseRadius
        );

        foreach (var hit in hits)
        {
            if (hit.CompareTag(player.tag)) continue;
            //hit.GetComponent<EnemyHp>()?.Freeze(player.stats.order.ascent.pulseDuration);


            yield return new WaitForSecondsRealtime(player.stats.order.ascent.pulseDuration * 0.1f);

        }
    }

    private IEnumerator OrderLandingStagger(float radius)
    {
        yield return new WaitUntil(() => player.isGrounded);
        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(player.tag)) continue;
            //hit.GetComponent<EnemyHp>()?.Freeze(0.8f); // brief stagger root
        }
    }
}