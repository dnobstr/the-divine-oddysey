using System.Collections;
using UnityEngine;

public class OrderAirAttack : MonoBehaviour
{
    private PlayerController player;

    public void init(PlayerController player) => this.player = player;

    public IEnumerator execute()
    {
        // 1. Initial Stall - Freeze momentum for weightiness
        player.canAirAttack = false;
        player.rb.linearVelocity = Vector2.zero;
        player.rb.gravityScale = 0;

        // player.anim.SetTrigger("airAttackStart");
        yield return new WaitForSeconds(0.1f); // Brief "hang time" before the drop

        // 2. The Descent - High speed downward
        player.rb.gravityScale = player.stats.order.jumpAttack.; // Use a high value here
        player.rb.AddForceY(-player.stats.order.jumpAttack.slamForce, ForceMode2D.Impulse);

        // Buffer to ensure we leave any platforms we were hovering over
        yield return new WaitForSeconds(0.05f);

        // 3. Wait for Impact
        yield return new WaitUntil(() => player.isGrounded);

        // 4. Landing Effect
        ExecuteImpact();

        // 5. Cleanup
        player.rb.gravityScale = player.defaultGravityScale;
        // player.anim.SetTrigger("airAttackLand");
        Destroy(this);
    }

    private void ExecuteImpact()
    {
        float radius = player.stats.order.jumpAttack.staggerRadius;
        Debug.Log($"[Order] Air Slam Impact - Radius: {radius}");

        Collider2D[] hits = Physics2D.OverlapCircleAll(player.transform.position, radius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag(player.tag)) continue;

            // Apply damage or stagger logic here
            // hit.GetComponent<EnemyHp>()?.TakeDamage(player.stats.order.jumpAttack.damage);
            // hit.GetComponent<EnemyHp>()?.Stagger(0.5f);
        }

        // Optional: Trigger a camera shake or particle effect here
    }
}