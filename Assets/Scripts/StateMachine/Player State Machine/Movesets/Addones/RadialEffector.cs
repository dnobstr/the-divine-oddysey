using System.Collections;
using UnityEngine;

/// <summary>
/// Spawned by airborne attacks to push or pull nearby enemies.
/// Set push=true for Order (outward), push=false for Chaos (inward).
/// </summary>
public class RadialEffector : MonoBehaviour
{
    private PlayerController player;
    private float radius;
    private float duration;
    private bool  push;

    public void Init(PlayerController player, float radius, float duration, bool push)
    {
        this.player   = player;
        this.radius   = radius;
        this.duration = duration;
        this.push     = push;

        StartCoroutine(ApplyEffect());
    }

    private IEnumerator ApplyEffect()
    {
        float elapsed   = 0f;
        float forceMag  = push
            ? player.stats.order.orderAirAttack.force
            : player.stats.chaos.chaosAirAttack.force;

        while (elapsed < duration)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, radius, LayerMask.GetMask("Enemy"));

            foreach (Collider2D hit in hits)
            {
                Rigidbody2D rb = hit.GetComponent<Rigidbody2D>();
                if (rb == null) continue;

                Vector2 dir = (hit.transform.position - transform.position).normalized;

                // push=true  → outward (Order slam)
                // push=false → inward  (Chaos slam, negate direction)
                rb.AddForce((push ? dir : -dir) * forceMag, ForceMode2D.Force);
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = push ? Color.blue : Color.red;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
