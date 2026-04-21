using UnityEngine;

public class DashState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;
    private MoveVariant variant;

    private float dashTimer;
    private bool dashComplete;

    public DashState(PlayerStateKey key, PlayerController player) : base(key)
    {
        this.player = player;
    }

    public override void EnterState()
    {
        dashComplete = false;
        variant = player.getCurrentVariant();

        player.GetComponent<Health>().isVulnerable = false;
        player.rb.gravityScale = 0f;
        player.rb.linearVelocity = Vector2.zero;

        player.anim?.SetTrigger("dash");

        StartDash();

        // ChaosDash handles its own coroutine and cleanup via init()
        if (variant == MoveVariant.Chaos || variant == MoveVariant.DivineChaos)
        {
            player.gameObject.AddComponent<ChaosDash>().init(player, variant);
        }
    }

    private void StartDash()
    {
        float dir = player.facingRight ? 1f : -1f;
        float speed = player.stats.normal.dash.speed;
        dashTimer = player.stats.normal.dash.duration;

        switch (variant)
        {
            case MoveVariant.Order:
                speed = player.stats.order.orderDash.speed;
                dashTimer = player.stats.order.orderDash.duration;
                player.stateMeter.addOrder(8f);
                break;

            case MoveVariant.Chaos:
                speed = player.stats.chaos.chaosDash.speed;
                dashTimer = player.stats.chaos.chaosDash.duration;
                player.stateMeter.addChaos(8f);
                break;

            case MoveVariant.DivineOrder:
                dashTimer = player.stats.order.orderDash.duration;
                Time.timeScale = player.stats.divineOrder.divineDash.divineOrderTimeSlow;
                Time.fixedDeltaTime = 0.02f * Time.timeScale;
                break;

            case MoveVariant.DivineChaos:
                speed = player.stats.divineChaos.divineDash.speed;
                dashTimer = player.stats.divineChaos.divineDash.duration;
                player.stateMeter.addChaos(8f);
                break;
        }

        player.rb.linearVelocity = new Vector2(dir * speed, 0f);
    }

    public override void UpdateState()
    {
        float dt = (variant == MoveVariant.DivineOrder)
            ? Time.unscaledDeltaTime
            : Time.deltaTime;

        dashTimer -= dt;

        if (dashTimer <= 0f)
            dashComplete = true;
    }

    public override void ExitState()
    {
        player.rb.gravityScale = 1f;
        player.rb.linearVelocity = new Vector2(0f, player.rb.linearVelocity.y);

        if (variant == MoveVariant.DivineOrder)
        {
            Time.timeScale = 1f;
            Time.fixedDeltaTime = 0.02f;
        }

        player.GetComponent<Health>().isVulnerable = true;
    }

    public override PlayerStateKey GetNextState()
    {
        if (!dashComplete) return StateKey;

        return !player.isGrounded
            ? PlayerStateKey.Fall
            : (Mathf.Abs(player.HorizontalInput) > 0.01f
                ? PlayerStateKey.Move
                : PlayerStateKey.Idle);
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other) { }
    public override void OnTriggerExit2D(Collider2D other) { }
}