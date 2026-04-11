using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public PolygonCollider2D atkHitbox;
    private PlayerController player;
    private PlayerHp hp;
    private float dmg;
    public void init(PlayerController playerController, float duration, float dmg)
    {
        player = playerController;
        dmg = dmg;

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

        hp = collision.GetComponent<PlayerHp>();
        
        if (hp == null) 
            return;

        hp.takeDmg(dmg);
    }
}