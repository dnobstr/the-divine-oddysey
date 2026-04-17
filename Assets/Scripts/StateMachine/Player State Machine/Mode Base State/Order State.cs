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
            OrderJump jump= player.gameObject.AddComponent<OrderJump>();
            jump.init(player);
            player.StartCoroutine(jump.execute());
        }

        if (player.dashPressed)
        {
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