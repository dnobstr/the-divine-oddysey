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

    public override void ExitState() => Debug.Log("[Chaos] Exited");

    public override PlayerStateKey GetNextState() => nextState;

    public override void UpdateState()
    {
        if (player.moveInput != 0) player.move();

        if (player.jumpPressed && player.isGrounded)
        {
            player.rb.linearVelocityY = 0f;
            player.rb.AddForceY(player.jumpForce * 1.5f, ForceMode2D.Impulse); // explosive
            player.anim.SetTrigger("jump");
        }

        if (player.dashPressed && Time.time >= player.lastDashTime + player.dashCooldown)
        {
            player.lastDashTime = Time.time;
            float randomDir = Random.value > 0.5f ? 1f : -1f;
            player.rb.linearVelocity = Vector2.zero;
            player.rb.AddForceX(player.dashSpeed * randomDir, ForceMode2D.Impulse);
            player.anim.SetTrigger("dash");
        }

        if (player.attackPressed)
        {
            float randomMult = Random.Range(0.5f, 2.5f);
            var hit = Object.Instantiate(player.hitboxPrefab, player.attackPoint.position, Quaternion.identity);
            // hit.GetComponent<Hitbox>().SetDamage(player.atkDmg * randomMult);
            player.anim.SetTrigger("attack");
            Debug.Log($"[Chaos] Strike — {randomMult:F2}x damage");
        }
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other) => player.isGrounded = true;
    public override void OnTriggerExit2D(Collider2D other) => player.isGrounded = false;
}