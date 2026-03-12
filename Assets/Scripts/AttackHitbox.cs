using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public PolygonCollider2D atkHitbox;
    PlayerController pc;
    PlayerHp hp;
    public void Init(PlayerController playerController)
    {
        pc = playerController;
    }
    void Awake()
    {
        atkHitbox = GetComponent<PolygonCollider2D>();
        pc = GetComponentInParent<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (gameObject.CompareTag(collision.gameObject.tag)) return;

        PlayerHp hp = collision.GetComponent<PlayerHp>();
        if (hp == null) return;

        hp.takeDmg(pc.atkDmg);
    }
}