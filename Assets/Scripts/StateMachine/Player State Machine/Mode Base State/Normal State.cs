using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class NormalState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;
    private PlayerStateKey nextState;

    public NormalState(PlayerStateKey key, PlayerController p) : base(key)
    {
        player = p;
        nextState = key;
    }

    public override void EnterState()
    {
        nextState = Statekey;
        Debug.Log("[Normal] Entered");
    }

    public override void ExitState()
    {
        Debug.Log("[Normal] Exited");
    }

    public override PlayerStateKey GetNextState() => nextState;

    public override void UpdateState()
    {
        if (player.jumpPressed && player.isGrounded)
        {
            player.rb.linearVelocityY = 0f;
            player.rb.AddForceY(player.stats.normal.jump.force, ForceMode2D.Impulse);
            player.anim.SetTrigger("jump");
        }

        if (player.dashPressed)
        {
            player.rb.AddForceX(player.stats.normal.dash.speed * player.direction, ForceMode2D.Impulse);
        }

        if (player.attackPressed && player.isGrounded )
        {
            GameObject hitGO = Object.Instantiate(player.stats.normal.attack.attackHb, player.transform.position, Quaternion.identity);
            AttackHitbox hitbox = hitGO.GetComponent<AttackHitbox>();
            hitbox.init(player, 0.5f, 1);

            player.anim.SetTrigger("attack");
        }
        else if (player.attackPressed && !player.isGrounded)
        {
            GameObject hitGO = Object.Instantiate(player.stats.normal.attack.attackHb, player.transform.position, Quaternion.Euler(0,0,90));
            AttackHitbox hitbox = hitGO.GetComponent<AttackHitbox>();
            player.rb.gravityScale = 0;
            hitbox.init(player, 0.5f, 1);

            player.anim.SetTrigger("attack");
        }
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other) => player.isGrounded = true;
    public override void OnTriggerExit2D(Collider2D other) => player.isGrounded = false;
}