// OrderState.cs
using UnityEngine;

public class OrderState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;
    private PlayerStateKey nextState;

    public OrderState(PlayerStateKey key, PlayerController p) : base(key)
    {
        player = p;
        nextState = key;
    }

    public override void EnterState()
    {
        nextState = Statekey;
        Debug.Log("[Order] Entered — precision mode");
    }

    public override void ExitState()
    {
        Debug.Log("[Order] Exited");
    }

    public override PlayerStateKey GetNextState() => nextState;

    public override void UpdateState()
    {
        if (player.jumpPressed && player.isGrounded)
        {
            player.rb.linearVelocityY = 0f;
            player.rb.AddForceY(player.stats.order.jump.force, ForceMode2D.Impulse);
            player.anim.SetTrigger("jump");
        }

        if (player.dashPressed)
        {
            player.rb.AddForceX(player.stats.order.dash.speed * 1.5f * player.direction, ForceMode2D.Impulse);

            OrderDash vanish = player.gameObject.AddComponent<OrderDash>();
            vanish.init(player);
            player.StartCoroutine(vanish.Execute());
        }

        if (player.attackPressed && player.isGrounded)
        {
            OrderAttack attack = player.gameObject.AddComponent<OrderAttack>();
            attack.init(player);
            player.StartCoroutine(attack.execute());
        }
        else if (player.attackPressed && !player.isGrounded)
        {

        }
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other) => player.isGrounded = true;
    public override void OnTriggerExit2D(Collider2D other) => player.isGrounded = false;
}