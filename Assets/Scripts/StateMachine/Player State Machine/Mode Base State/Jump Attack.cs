// JumpAttack.cs — handles all states, reads correct stats block
using UnityEngine;

public class JumpAttack : MonoBehaviour
{
    private PlayerController player;

    public void Init(PlayerController player) => this.player = player;

    public void Execute(PlayerStateKey state)
    {
        if (!player.canAirAttack || player.isGrounded) return;
        player.canAirAttack = false; // one aerial attack per jump

        AirAttackStats s = state switch
        {
            PlayerStateKey.Normal => player.stats.normal.jumpAttack,
            PlayerStateKey.Order => player.stats.order.jumpAttack,
            PlayerStateKey.Chaos => player.stats.chaos.jumpAttack,
            PlayerStateKey.DivineOrder => player.stats.order.jumpAttack,
            PlayerStateKey.DivineChaos => player.stats.chaos.jumpAttack,
            _ => player.stats.normal.jumpAttack
        };

        // Downward force
        player.rb.linearVelocityY = 0f;
        float downForce = state == PlayerStateKey.Order ? player.stats.order.ascent.force * 1.2f
                        : state == PlayerStateKey.Chaos ? player.stats.chaos.launch.launchForce * 0.6f
                        : player.stats.normal.jump.force * 0.5f;
        player.rb.AddForceY(-downForce, ForceMode2D.Impulse);

        // Spawn hitbox
        //if (s.hitboxPrefab == null || player.stats. == null) return;
        //GameObject hitGO = Object.Instantiate(s.hitboxPrefab, player.jumpAtkPoint.position, player.jumpAtkPoint.rotation);
        //AttackHitbox hitbox = hitGO.GetComponent<AttackHitbox>();
        //hitbox.Init(player, s.multiplier);
        //player.RegisterHitbox(hitbox);
        //hitbox.SetLifetime(s.duration);

        //// Chaos — AOE splash
        //if (state == PlayerStateKey.Chaos || state == PlayerStateKey.DivineChaos)
        //{
        //    Collider2D[] hits = Physics2D.OverlapCircleAll(player.jumpAtkPoint.position, s.splashRadius);
        //    foreach (var hit in hits)
        //    {
        //        if (hit.CompareTag(player.tag)) continue;
        //        hit.GetComponent<EnemyHp>()?.takeDmg(player.atkDmg * s.multiplier * 0.5f);
        //    }
        //}

        // Order — wide stagger radius tracked until landing
        if (state == PlayerStateKey.Order || state == PlayerStateKey.DivineOrder)
            player.StartCoroutine(OrderLandingStagger(s.staggerRadius));

        string trigger = state switch
        {
            PlayerStateKey.Chaos => "jumpAtkChaos",
            PlayerStateKey.Order => "jumpAtkOrder",
            PlayerStateKey.DivineChaos => "jumpAtkDivineChaos",
            PlayerStateKey.DivineOrder => "jumpAtkDivineOrder",
            _ => "jumpAttack"
        };
        player.anim.SetTrigger(trigger);

        Destroy(this);
    }

    private System.Collections.IEnumerator OrderLandingStagger(float radius)
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