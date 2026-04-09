using UnityEngine;

public class AttackHitbox : MonoBehaviour
{
    public PolygonCollider2D atkHitbox;
    private PlayerController player;
    private PlayerHp hp;
    private float dmgMultiplier;
    public void init(PlayerController playerController, float duration, float multiplier)
    {
        player = playerController;
        dmgMultiplier = multiplier;

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

        if (player.dashPressed && Time.time < Time.time + player.dashDuration && (player.state.Equals("Order") || player.state.Equals("DivineOrder")))
            return;

        hp = collision.GetComponent<PlayerHp>();
        
        if (hp == null) 
            return;

        hp.takeDmg(player.atkDmg * dmgMultiplier);
    }
}