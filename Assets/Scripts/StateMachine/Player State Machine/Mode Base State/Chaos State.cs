// ChaosState.cs
using UnityEngine;

public class ChaosState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;
    private PlayerStateKey nextState;

    public ChaosState(PlayerStateKey key, PlayerController p) : base(key)
    {
        player = p;
        nextState = key;
    }

    public override void EnterState()
    {
        nextState = Statekey;
        Debug.Log("[Chaos] Entered — unleashed");
    }

    public override void ExitState()
    {
        Debug.Log("[Chaos] Exited");
    }

    public override PlayerStateKey GetNextState() => nextState;

    public override void UpdateState()
    {
        if (player.jumpPressed && player.isGrounded)
        {
            ChaosJump jump = player.gameObject.AddComponent<ChaosJump>();
            jump.init(player);
            player.StartCoroutine(jump.execute());
        }

        if (player.dashPressed)
        {
            ChaosDash ignition = player.gameObject.AddComponent<ChaosDash>();
            ignition.init(player);
            player.StartCoroutine(ignition.execute());
        }

        if (player.attackPressed && player.isGrounded)
        {
            ChaosAttack attack = player.gameObject.AddComponent<ChaosAttack>();
            attack.init(player);
            player.StartCoroutine(attack.execute());

            player.anim.SetTrigger("attack");   
        }
        else if (player.attackPressed && !player.isGrounded)
        {

        }
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other) => player.isGrounded = true;
    public override void OnTriggerExit2D(Collider2D other) => player.isGrounded = false;
}