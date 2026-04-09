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
            player.rb.linearVelocityY = 0f;
            player.rb.AddForceY(player.jumpForce * 1.5f, ForceMode2D.Impulse); // explosive
            player.anim.SetTrigger("jump");
        }

        if (player.dashPressed)
        {
            player.rb.AddForceX(player.dashSpeed * player.direction, ForceMode2D.Impulse);

            ChaosDash ignition = player.gameObject.AddComponent<ChaosDash>();
            ignition.Init(player);
            player.StartCoroutine(ignition.Execute());
        }

        if (player.attackPressed && player.isGrounded)
        {
            float randomMult = Random.Range(0.5f, 2.5f);
            var hitGO = Object.Instantiate(player.hitboxPrefab, player.attackPoint.position, Quaternion.identity);
            AttackHitbox hitbox = hitGO.GetComponent<AttackHitbox>();
            hitbox.init(player, 1.5f, randomMult);

            player.anim.SetTrigger("attack");
            Debug.Log($"[Chaos] Strike — {randomMult:F2}x damage");
        }
        else if (player.attackPressed && !player.isGrounded)
        {

        }
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other) => player.isGrounded = true;
    public override void OnTriggerExit2D(Collider2D other) => player.isGrounded = false;
}