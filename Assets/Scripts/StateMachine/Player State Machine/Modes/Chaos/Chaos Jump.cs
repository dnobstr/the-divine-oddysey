// ChaosJump.cs
using System.Collections;
using UnityEngine;

public class ChaosJump : MonoBehaviour
{
    private PlayerController player;
    private PlayerHp selfHp;

    public void init(PlayerController player)
    {
        this.player = player;
        selfHp = player.GetComponent<PlayerHp>();
    }

    public IEnumerator execute()
    {
        player.canAirAttack = true;

        // Violent upward rocket
        player.rb.linearVelocityY = 0f;
        player.rb.AddForceY(player.stats.chaos.launch.launchForce, ForceMode2D.Impulse);
        player.anim.SetTrigger("jump");

        // Wait for peak — when Y velocity flips negative
        yield return new WaitUntil(() => player.rb.linearVelocityY <= 0f);

        if (player.attackPressed)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, player.stats.chaos.jumpAttack.splashRadius);
            foreach (var hit in hits)
            {
                if (hit.CompareTag(player.tag)) continue;
                hit.GetComponent<PlayerHp>()?.takeDmg(player.stats.chaos.jumpAttack.damage);
            }

            player.stateMeter.AddChaos(player.stats.chaos.jumpAttack.meterGain);
        }

        // Hang at peak
        player.rb.gravityScale = player.stats.chaos.launch.hangGravityScale;
        yield return new WaitForSeconds(player.stats.chaos.launch.hangDuration);

        // Restore gravity — start falling
        player.rb.gravityScale = player.defaultGravityScale;

        // DOT ticks on self after grace period
        StartCoroutine(AirtimeDOT());

        yield return new WaitUntil(() => player.isGrounded);

        player.rb.gravityScale = player.defaultGravityScale;
        Destroy(this);
    }

    private IEnumerator AirtimeDOT()
    {
        yield return new WaitForSeconds(player.stats.chaos.launch.airtimeDOTDelay);

        while (!player.isGrounded)
        {
            selfHp?.takeDmg(player.stats.chaos.launch.airtimeDOTRate * Time.deltaTime);
            yield return null;
        }
    }
}