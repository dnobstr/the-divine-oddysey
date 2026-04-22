using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public PolygonCollider2D atkHitbox;
    private PlayerController player;
    private Health hp;
    private float dmg;

    public void init(PlayerController playerController, float duration, float damage)
    {
        player = playerController;
        this.dmg = damage;

        Destroy(gameObject, duration);
    }

    void Awake()
    {
        atkHitbox = GetComponent<PolygonCollider2D>();
        player = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.CompareTag(collision.gameObject.tag))
            return;

        hp = collision.GetComponent<Health>();
        
        if (hp == null) 
            return;

        hp.takeDamage(dmg);
    }
}