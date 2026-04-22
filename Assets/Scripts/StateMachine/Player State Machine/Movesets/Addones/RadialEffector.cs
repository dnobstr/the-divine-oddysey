using System.Collections;
using UnityEngine;

/// <summary>
/// Spawned by airborne attacks to push or pull nearby enemies.
/// push=true  → outward (Order slam)
/// push=false → inward  (Chaos slam)
/// Moves enemy transforms directly since enemies use Transform.Translate.
/// </summary>
public class RadialEffector : MonoBehaviour
{
    private PlayerController player;
    private float radius;
    private float duration;
    private float forceMag;
    private bool push;

    /// <param name="push">true = push outward (Order), false = pull inward (Chaos)</param>
    /// <param name="force">units per second to move enemies</param>
    public void init(PlayerController player, float radius, float duration, bool push, float force)
    {
        this.player = player;
        this.radius = radius;
        this.duration = duration;
        this.push = push;
        this.forceMag = force;

        StartCoroutine(applyEffect());
    }

    private IEnumerator applyEffect()
    {
        float elapsed = 0f;

        Transform t = GetComponentInParent<Transform>();
        Vector3 scale = t.localScale;
        scale.x = radius;
        scale.y = radius / 2;
        t.localScale = scale;

        while (elapsed < duration)
        {
            Collider2D[] hits = Physics2D.OverlapCircleAll(
                transform.position, radius, LayerMask.GetMask("Enemy"));

            foreach (Collider2D hit in hits)
            {
                Vector2 dir = (hit.transform.position - transform.position).normalized;

                // push=true  → outward (Order)
                // push=false → inward  (Chaos, negate direction)
                Vector2 move = (push ? dir : -dir) * forceMag * Time.deltaTime;
                hit.transform.Translate(move);
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