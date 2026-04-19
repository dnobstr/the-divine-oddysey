using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class PlayerController : StateManager<PlayerStateKey>
{
    // ── Components ──────────────────────────────────────────────────────────
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public Animator   anim;

    // ── Inspector tunables ──────────────────────────────────────────────────
    [Header("Movement")]
    public float moveSpeed    = 8f;
    public float jumpForce    = 16f;
    public float dashSpeed    = 22f;
    public float dashDuration = 0.18f;

    [Header("Meter Reference")]
    public StateMeter stateMeter;

    [Header("Mode")]
    public ModeManager modeManager;

    // ── Runtime state shared across state objects ───────────────────────────
    [HideInInspector] public float   HorizontalInput;
    [HideInInspector] public bool    jumpPressed;
    [HideInInspector] public bool    dashPressed;
    [HideInInspector] public bool    attackPressed;
    [HideInInspector] public bool    switchMode;
    [HideInInspector] public bool    isGrounded;
    [HideInInspector] public bool    FacingRight = true;

    public bool cutscene;

    // ── Awake: build the state dictionary ───────────────────────────────────
    private void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        stateMeter = GetComponent<StateMeter>();
        modeManager = GetComponent<ModeManager>();

        States[PlayerStateKey.Idle]           = new IdleState(PlayerStateKey.Idle,           this);
        States[PlayerStateKey.Move]           = new MoveState(PlayerStateKey.Move,           this);
        States[PlayerStateKey.Jump]           = new JumpState(PlayerStateKey.Jump,           this);
        States[PlayerStateKey.Dash]           = new DashState(PlayerStateKey.Dash,           this);
        States[PlayerStateKey.Attack]         = new AttackState(PlayerStateKey.Attack,       this);
        States[PlayerStateKey.AttackAirborne] = new AttackAirborneState(PlayerStateKey.AttackAirborne, this);
        States[PlayerStateKey.Fall] = new FallState(PlayerStateKey.Fall, this);

        CurrentState = States[PlayerStateKey.Idle];

        // ── Meter → ModeManager event wiring ────────────────────────────────
        stateMeter.onDivineOrder.AddListener(modeManager.OnDivineOrderReached);
        stateMeter.onDivineChaos.AddListener(modeManager.OnDivineChaosReached);
        stateMeter.onDivineOrderBroken.AddListener(modeManager.OnDivineOrderBroken);
        stateMeter.onDivineChaosBroken.AddListener(modeManager.OnDivineChaosBroken);
    }

    // ── Update: gather input before the base Update ticks ───────────────────
    private new void Update()     // 'new' because base uses void Update()
    {
        GatherInput();
        if (switchMode) modeManager?.cycleStance();
        base.Update();            // runs GetNextState → UpdateState / Transition
        ClearFrameInput();
    }

    // ── Helpers ─────────────────────────────────────────────────────────────
    private void GatherInput()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump")) jumpPressed = true;
        if (Input.GetKeyDown(KeyCode.LeftShift)) dashPressed = true;
        if (Input.GetKeyDown(KeyCode.LeftControl)) switchMode = true;
        if (Input.GetKeyDown(KeyCode.Mouse0)) attackPressed = true;
    }

    private void ClearFrameInput()
    {
        jumpPressed = false;
        dashPressed = false;
        attackPressed = false;
        switchMode = false;
    }

    // ── Variant resolver ─────────────────────────────────────────────────────
    /// <summary>
    /// Returns the effective MoveVariant for this frame.
    ///
    /// While Divine Order is active the player is locked into DivineOrder moves
    /// UNLESS they have switched to Chaos (or DivineChaos) to break out.
    /// The same logic applies symmetrically for Divine Chaos / Order.
    /// </summary>
    public MoveVariant getCurrentVariant()
    {
        MoveVariant stance = modeManager.currentVariant;

        if (stateMeter.isDivineOrder)
        {
            // Allow Chaos/DivineChaos through so the player can break the state.
            if (stance == MoveVariant.Chaos || stance == MoveVariant.DivineChaos)
                return stance;
            return MoveVariant.DivineOrder;
        }

        if (stateMeter.isDivineChaos)
        {
            // Allow Order/DivineOrder through so the player can break the state.
            if (stance == MoveVariant.Order || stance == MoveVariant.DivineOrder)
                return stance;
            return MoveVariant.DivineChaos;
        }

        return stance;
    }

    // ── Flip sprite to match movement direction ──────────────────────────────
    public void FlipTowards(float direction)
    {
        if (direction > 0 && !FacingRight) Flip();
        else if (direction < 0 && FacingRight) Flip();
    }

    private void Flip()
    {
        FacingRight = !FacingRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isGrounded = false;
    }
}
