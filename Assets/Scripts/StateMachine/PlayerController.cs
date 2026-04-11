using UnityEngine;

public class PlayerController : PlayerStateManager<PlayerStateKey>
{
    public Rigidbody2D rb;
    public Animator anim;
    public CapsuleCollider2D cc;
    public BoxCollider2D bc;
    public StateMeter stateMeter;
    public PlayerStats stats;

    [Header("Divinity")]
    public string state;
    public float divinityMeter;

    [Header("Movement")]
    public float moveSpeed;
    public float defaultGravityScale;

    [Header("Flags")]
    public bool isGrounded;
    public bool isDashing;
    public bool isFalling;
    public float direction;
    
    [Header("Inputs")]
    public float moveInput;
    public bool jumpPressed;
    public bool dashPressed;
    public bool attackPressed;
    private int _cycleIndex = 0;

    //[Header("Cutscene")]
    //public bool cutscene;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cc = GetComponent<CapsuleCollider2D>();
        bc = GetComponent<BoxCollider2D>();
        stateMeter = GetComponent<StateMeter>();
        stats = GetComponent<PlayerStats>();

        defaultGravityScale = rb.gravityScale;

        States[PlayerStateKey.Normal] = new NormalState(PlayerStateKey.Normal, this);
        States[PlayerStateKey.Order] = new OrderState(PlayerStateKey.Order, this);
        States[PlayerStateKey.Chaos] = new ChaosState(PlayerStateKey.Chaos, this);
        States[PlayerStateKey.DivineOrder] = new DivineOrderState(PlayerStateKey.DivineOrder, this);
        States[PlayerStateKey.DivineChaos] = new DivineChaosState(PlayerStateKey.DivineChaos, this);

        CurrentState = States[PlayerStateKey.Normal];

        stateMeter.onDivineOrder.AddListener(() => TransitionToState(PlayerStateKey.DivineOrder));
        stateMeter.onDivineOrderBroken.AddListener(() => TransitionToState(PlayerStateKey.Order));

        stateMeter.onDivineChaos.AddListener(() => TransitionToState(PlayerStateKey.DivineChaos));
        stateMeter.onDivineChaosBroken.AddListener(() => TransitionToState(PlayerStateKey.Chaos));
    }

    protected override void Update()
    {
        if (Time.time < stats.normal.dash.duration + stats.normal.dash.lastDashTime)
            return;
        else
            endDash();

        moveInput = Input.GetAxisRaw("Horizontal");
        if (moveInput != 0)
            move();
        else
            anim.SetBool("isMoving", false);

        jumpPressed = Input.GetButtonDown("Jump");
        dashPressed = Input.GetKeyDown(KeyCode.LeftShift);
        if (dashPressed)
            dash();

        attackPressed = Input.GetMouseButtonDown(0);

        if (Input.GetKeyDown(KeyCode.LeftControl)) cycleState();

        falling();

        base.Update();
    }
    private void dash()
    {
        isDashing = true;
        stats.normal.dash.lastDashTime = Time.time;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isDashing", true);

        rb.gravityScale = 0f;
        rb.linearVelocityY = 0f;
    }

    private void endDash()
    {
        isDashing = false;
        rb.gravityScale = defaultGravityScale;
        anim.SetBool("isDashing", false);
        rb.linearVelocityX = 0;
    }


    private void cycleState()
    {
        _cycleIndex = (_cycleIndex + 1) % 3;
        TransitionToState((PlayerStateKey)_cycleIndex);
        state = CurrentState.ToString();
    }

    public void move()
    {
        rb.linearVelocityX = moveSpeed * moveInput;
        handleFlip(moveInput);
        anim.SetBool("isMoving", true);
    }
    private void falling()
    {
        if (rb.linearVelocityY < 0 && !isGrounded)
        {
            isFalling = true;
            anim.SetBool("isFalling", true);
        }
        else
        {
            isFalling = false;
            anim.SetBool("isFalling", false);
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        isGrounded = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        isGrounded = false;
    }

    void handleFlip(float horizontal)
    {
        if (horizontal > 0 && direction == -1) Flip();
        else if (horizontal < 0 && direction == 1) Flip();
    }

    void Flip()
    {
        direction *= -1;
        Vector3 s = transform.localScale;
        s.x *= -1;
        transform.localScale = s;
    }

}