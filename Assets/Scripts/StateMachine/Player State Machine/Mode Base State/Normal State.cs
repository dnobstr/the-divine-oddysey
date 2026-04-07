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

    public override void ExitState() => Debug.Log("[Normal] Exited");

    public override PlayerStateKey GetNextState() => nextState;

    public override void UpdateState()
    {
        if (player.moveInput != 0) player.move();
        else player.stopMove();

        if (player.jumpPressed && player.isGrounded)
        {
            player.rb.linearVelocityY = 0f;
            player.rb.AddForceY(player.jumpForce, ForceMode2D.Impulse);
            player.anim.SetTrigger("jump");
        }

        if (player.dashPressed && Time.time >= player.lastDashTime + player.dashCooldown)
        {
            player.lastDashTime = Time.time;
            float dir = player.isFacingRight ? 1f : -1f;
            player.rb.AddForceX(player.dashSpeed * dir, ForceMode2D.Impulse);
            player.anim.SetTrigger("dash");
        }

        if (player.attackPressed)
        {
            GameObject hitGO = Object.Instantiate(player.hitboxPrefab, player.attackPoint.position, Quaternion.identity);
            AttackHitbox hitbox = hitGO.GetComponent<AttackHitbox>();
            hitbox.Init(player);

            player.anim.SetTrigger("attack");
        }
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other) => player.isGrounded = true;
    public override void OnTriggerExit2D(Collider2D other) => player.isGrounded = false;
}