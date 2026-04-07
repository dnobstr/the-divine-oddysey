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

    public override void ExitState() => Debug.Log("[Order] Exited");

    public override PlayerStateKey GetNextState() => nextState;

    public override void UpdateState()
    {
        if (player.moveInput != 0) player.move();
        else player.stopMove();

        if (player.jumpPressed && player.isGrounded)
        {
            player.rb.linearVelocityY = 0f;
            player.rb.AddForceY(player.jumpForce * 1.2f, ForceMode2D.Impulse); // floatier
            player.anim.SetTrigger("jump");
        }

        if (player.dashPressed && Time.time >= player.lastDashTime + player.dashCooldown)
        {
            player.lastDashTime = Time.time;
            player.rb.linearVelocity = Vector2.zero; // blink feel
            float dir = player.isFacingRight ? 1f : -1f;
            player.rb.AddForceX(player.dashSpeed * 1.5f * dir, ForceMode2D.Impulse);
            player.anim.SetTrigger("dash");
        }

        if (player.attackPressed)
        {
            var hit = Object.Instantiate(player.hitboxPrefab, player.attackPoint.position, Quaternion.identity);
            // hit.GetComponent<Hitbox>().SetDamage(player.atkDmg * 1.75f);
            player.anim.SetTrigger("attack");
        }
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other) => player.isGrounded = true;
    public override void OnTriggerExit2D(Collider2D other) => player.isGrounded = false;
}