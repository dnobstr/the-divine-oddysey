using NUnit.Framework.Interfaces;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerController : PlayerStateManager<PlayerStateEnum>
{
    // ── Components ────────────────────────────────────────────────────────────
    public Rigidbody2D rb;
    public Animator anim;

    // ── Movement ──────────────────────────────────────────────────────────────
    [Header("Movement")]
    public float moveSpeed;
    public float jumpForce;

    // ── Dash ──────────────────────────────────────────────────────────────────
    [Header("Dash")]
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldown;
    public float lastDashTime;

    // ── Attack ────────────────────────────────────────────────────────────────
    [Header("Attack")]
    public float attackDuration;
    public GameObject hitboxPrefab;
    public Transform attackPoint;
    public float atkDmg;

    // ── Ground Check ──────────────────────────────────────────────────────────
    [Header("Ground Check")]
    public bool isGrounded;

    // ── Runtime Input (populated each frame, read by states) ──────────────────
    public float moveInput;
    public bool jumpPressed;
    public bool dashPressed;
    public bool attackPressed;
    public bool isFacingRight;

    // ─────────────────────────────────────────────────────────────────────────
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Register all states
        States[PlayerStateEnum.Idle] = new IdleState(PlayerStateEnum.Idle, this);
        States[PlayerStateEnum.Run] = new RunState(PlayerStateEnum.Run, this);
        States[PlayerStateEnum.Jump] = new JumpState(PlayerStateEnum.Jump, this);
        States[PlayerStateEnum.Fall] = new FallState(PlayerStateEnum.Fall, this);
        States[PlayerStateEnum.Dash] = new DashState(PlayerStateEnum.Dash, this);
        States[PlayerStateEnum.Attack] = new AttackState(PlayerStateEnum.Attack, this);

        // Set the initial state (Start() in base will call EnterState)
        CurrentState = States[PlayerStateEnum.Idle];
    }

    // NOTE: base class Start() calls CurrentState.EnterState() — keep Awake/Start split.

    void Update()
    {
        // Collect raw input before the base state-machine Update runs
        moveInput = Input.GetAxisRaw("Horizontal");
        jumpPressed = Input.GetButtonDown("Jump");
        dashPressed = Input.GetKeyDown(KeyCode.LeftShift);
        attackPressed = Input.GetMouseButtonDown(0);

        base.Update();
    }

    // ── Helpers called by states ──────────────────────────────────────────────

    private void OnTriggerStay2D(Collider2D collision)
    {
        isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isGrounded = false;
    }

    /// <summary>Flip sprite when the player changes direction.</summary>
    public void HandleFlip(float horizontal)
    {
        if (horizontal > 0 && !isFacingRight) Flip();
        else if (horizontal < 0 && isFacingRight) Flip();
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

}