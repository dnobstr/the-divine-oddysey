using UnityEngine;

/// <summary>
/// Ground Attack variants:
///   Normal      – single hit, no meter effect
///   Order       – slow powerful swing, +Order on hit
///   Chaos       – fast flurry (3 quick hits), +Chaos on each hit
///   DivineOrder – wide holy AoE burst
///   DivineChaos – rapid random multi-hit explosion
/// </summary>
public class AttackState : BaseState<PlayerStateKey>
{
    private readonly PlayerController player;
    private MoveVariant variant;

    private float attackTimer;
    private bool  attackDone;

    // Attack durations per variant (seconds – match to your animation lengths)
    private static readonly float[] Durations =
    {
        0.35f, // Normal
        0.55f, // Order
        0.45f, // Chaos
        0.65f, // DivineOrder
        0.50f  // DivineChaos
    };

    public AttackState(PlayerStateKey key, PlayerController player) : base(key)
    {
        this.player = player;
    }

    public override void EnterState()
    {
        variant     = player.GetCurrentVariant();
        attackDone  = false;
        attackTimer = Durations[(int)variant];

        // Lock horizontal movement during the attack swing
        player.rb.linearVelocity = new Vector2(0f, player.rb.linearVelocity.y);

        player.anim?.SetTrigger($"attack");
        //player.anim?.SetTrigger($"Attack_{variant}");
        PerformAttackLogic();
    }

    private void PerformAttackLogic()
    {
        // TODO: replace with your hitbox / hit-detection calls.
        // The switch below shows where variant-specific behaviour lives.
        switch (variant)
        {
            case MoveVariant.Normal:
                player.stateMeter?.AddOrder(0f); // neutral – no meter change on normal attack
                break;

            case MoveVariant.Order:
                player.stateMeter?.AddOrder(10f);
                break;

            case MoveVariant.Chaos:
                player.stateMeter?.AddChaos(10f);
                break;

            case MoveVariant.DivineOrder:
                // Wide AoE – trigger your special VFX / hitbox here
                player.stateMeter?.AddOrder(20f);
                break;

            case MoveVariant.DivineChaos:
                // Multi-hit explosion – trigger your special VFX / hitbox here
                player.stateMeter?.AddChaos(20f);
                break;
        }
    }

    public override void UpdateState()
    {
        attackTimer -= Time.deltaTime;
        if (attackTimer <= 0f)
            attackDone = true;
    }

    public override void ExitState() { }

    public override PlayerStateKey GetNextState()
    {
        if (!attackDone) return StateKey;

        // Allow chaining: if attack is still pressed queue another
        if (player.attackPressed) return PlayerStateKey.Attack;
        if (player.jumpPressed)   return PlayerStateKey.Jump;
        if (player.dashPressed)   return PlayerStateKey.Dash;

        return Mathf.Abs(player.HorizontalInput) > 0.01f
            ? PlayerStateKey.Move
            : PlayerStateKey.Idle;
    }

    public override void OnTriggerEnter2D(Collider2D other) { }
    public override void OnTriggerStay2D(Collider2D other)  { }
    public override void OnTriggerExit2D(Collider2D other)  { }
}
