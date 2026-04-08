using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public PolygonCollider2D atkHitbox;
    private PlayerController pc;
    private PlayerHp hp;
    private float dmgMultiplier;
    public void init(PlayerController playerController, float duration, float multiplier)
    {
        pc = playerController;
        dmgMultiplier = multiplier;

        Destroy(gameObject, duration);
    }
    void Awake()
    {
        atkHitbox = GetComponent<PolygonCollider2D>();
        pc = GetComponentInParent<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.CompareTag(collision.gameObject.tag)) return;

        hp = collision.GetComponent<PlayerHp>();
        if (hp == null) return;

        hp.takeDmg(pc.atkDmg * dmgMultiplier);
    }
}