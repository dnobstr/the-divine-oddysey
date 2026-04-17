// ChaosAttack.cs
using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class ChaosAttack : MonoBehaviour
{
    private PlayerController player;

    public void init(PlayerController player) => this.player = player;

    public IEnumerator execute()
    {
        player.attackPressed = true;
        player.stats.chaos.chain.count = 0;

        while (player.attackPressed || player.stats.chaos.chain.count == 0)
        {
            player.stats.chaos.chain.count++;

            // Speed climbs with each chain — interval shrinks
            float interval = Mathf.Max(
                player.stats.chaos.chain.minInterval,
                player.stats.chaos.chain.startInterval - (player.stats.chaos.chain.count * player.stats.chaos.chain.speedStep)
            );

            player.anim.SetTrigger("attack");
            spawnHitbox(player.stats.chaos.attack.damage); // lower per hit, high total

            // Meter spikes per chain hit
            player.stateMeter.AddChaos(player.stats.chaos.attack.meterGain);

            yield return new WaitForSeconds(interval);

            // Wait for next attack input — auto-chains only if pressed again quickly
            float inputWindow = 0.3f;
            float waited = 0f;
            bool chained = false;

            while (waited < inputWindow)
            {
                if (player.attackPressed) { chained = true; break; }
                waited += Time.deltaTime;
                yield return null;
            }

            if (!chained) break;
        }

        player.stats.chaos.chain.count = 0;
        player.attackPressed= false;
        Destroy(this);
    }

    private void spawnHitbox(float damageMultiplier)
    {
        if (player.stats.chaos.attack.attackHb == null || player.transform.position == null) return;
        GameObject hitGO = Object.Instantiate(
            player.stats.chaos.attack.attackHb,
            player.transform.position,
            player.transform.rotation
        );
        AttackHitbox hitbox = hitGO.GetComponent<AttackHitbox>();
        hitbox.init(player, player.stats.chaos.attack.duration, player.stats.chaos.attack.damage);
        //player.RegisterHitbox(hitbox);
        //hitbox.SetLifetime(player.attackDuration * 0.5f);
    }
}