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

    // ── Runtime state shared across state objects ───────────────────────────
    [HideInInspector] public float   HorizontalInput;
    [HideInInspector] public bool    jumpPressed;
    [HideInInspector] public bool    dashPressed;
    [HideInInspector] public bool    attackPressed;
    [HideInInspector] public bool    isGrounded;
    [HideInInspector] public bool    FacingRight = true;

    public bool cutscene;

    // ── Awake: build the state dictionary ───────────────────────────────────
    private void Awake()
    {
        rb   = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        States[PlayerStateKey.Idle]           = new IdleState(PlayerStateKey.Idle,           this);
        States[PlayerStateKey.Move]           = new MoveState(PlayerStateKey.Move,           this);
        States[PlayerStateKey.Jump]           = new JumpState(PlayerStateKey.Jump,           this);
        States[PlayerStateKey.Dash]           = new DashState(PlayerStateKey.Dash,           this);
        States[PlayerStateKey.Attack]         = new AttackState(PlayerStateKey.Attack,       this);
        States[PlayerStateKey.AttackAirborne] = new AttackAirborneState(PlayerStateKey.AttackAirborne, this);

        CurrentState = States[PlayerStateKey.Idle];
    }

    // ── Update: gather input before the base Update ticks ───────────────────
    private new void Update()     // 'new' because base uses void Update()
    {
        GatherInput();
        base.Update();            // runs GetNextState → UpdateState / Transition
        ClearFrameInput();
    }

    // ── Helpers ─────────────────────────────────────────────────────────────
    private void GatherInput()
    {
        HorizontalInput = Input.GetAxisRaw("Horizontal");
        if (Input.GetButtonDown("Jump"))   jumpPressed   = true;
        if (Input.GetKeyDown(KeyCode.LeftShift))   dashPressed   = true;   // map "Dash" in Input Manager
        if (Input.GetKeyDown(KeyCode.Mouse0)) attackPressed = true;   // map "Attack" in Input Manager
    }

    private void ClearFrameInput()
    {
        jumpPressed   = false;
        dashPressed   = false;
        attackPressed = false;
    }

    // ── Variant helper: resolve which variant applies right now ─────────────
    public MoveVariant GetCurrentVariant()
    {
        if (stateMeter == null) return MoveVariant.Normal;

        if (stateMeter.isDivineOrder)  return MoveVariant.DivineOrder;
        if (stateMeter.isDivineChaos)  return MoveVariant.DivineChaos;

        float n = stateMeter.Normalized; // -1 … 1
        if      (n >=  0.5f) return MoveVariant.Order;
        else if (n <= -0.5f) return MoveVariant.Chaos;
        else                  return MoveVariant.Normal;
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

    // ── Gizmo for ground check ───────────────────────────────────────────────
    private void OnTriggerStay2D(Collider2D collision)
    {
        isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isGrounded = false;
    }
}
