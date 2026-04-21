using Unity.Collections;
using UnityEngine;

public class ProjectileHitbox : BaseHitbox
{
    [Header("Projectile Settings")]
    public float speed = 5f;

    public float customLifetime = 2.0f;

    private Vector2 moveDirection;

    protected override void Start()
    {
        lifetime = customLifetime;

        base.Start(); 

        float dirX = transform.localScale.x > 0 ? 1f : -1f;
        moveDirection = new Vector2(dirX, 0);
    }

    void Update()
    {
        transform.Translate(moveDirection * speed * Time.deltaTime, Space.World);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        int groundLayer = LayerMask.NameToLayer("Ground");
        int playerLayer = LayerMask.NameToLayer("Player");

        // Compare the integer ID of the hit object to our target IDs
        if (other.gameObject.layer == groundLayer || other.gameObject.layer == playerLayer)
        {
            Destroy(gameObject);
        }
    }
    protected override void DealDamage(Transform target)
    {
        Health hp = target.GetComponent<Health>();

        if (hp != null)
        {
            hp.applyDOT(damage, 2f, 4);
        }
    }
}