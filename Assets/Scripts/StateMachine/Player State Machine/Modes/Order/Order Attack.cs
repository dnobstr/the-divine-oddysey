// OrderAttack.cs
using System.Collections;
using UnityEngine;

public class OrderAttack : MonoBehaviour
{
    private PlayerController player;

    public void init(PlayerController player) => this.player = player;

    public IEnumerator execute()
    {
        player.attackPressed = true;
        //player.attackLocked = false;

        // Slow windup — animator handles the visual
        player.anim.SetTrigger("attack");

        // Wait for windup before hit registers
        yield return new WaitForSeconds(player.stats.order.windupDuration);

        // Land the hit — heavy damage, wider stagger
        spawnHitbox();

        player.stateMeter.AddOrder(player.stats.order.attack.meterGain);

        // Wait for full animation to resolve before anything else
        float remainder = 0.4f;
        yield return new WaitForSeconds(remainder);

        player.attackPressed = false;
        Destroy(this);
    }

    private void spawnHitbox()
    {
        if (player.stats.order.attack.attackHb == null) return;
        GameObject hitGO = Object.Instantiate(
            player.stats.order.attack.attackHb,
            player.transform.position,
            player.transform.rotation
        );
        AttackHitbox hitbox = hitGO.GetComponent<AttackHitbox>();
        hitbox.init(player, 1, player.stats.order.attack.multiplier);
        //player.RegisterHitbox(hitbox);
        //hitbox.SetLifetime(player.attackDuration);

        //// Meter gain on hit
        //player.stateMeter.Add(8f);
    }
}