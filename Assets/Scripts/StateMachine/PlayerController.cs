using NUnit.Framework.Interfaces;
using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerController : PlayerStateManager<PlayerStateKey>
{
    public Rigidbody2D rb;
    public Animator anim;
    public CapsuleCollider2D cc;
    public BoxCollider2D bc;

    [Header("Divinity")]
    public string state;
    public float divinityMeter;

    [Header("Movement")]
    public float moveSpeed;
    public float jumpForce;
    public float defaultGravityScale;

    [Header("Dash")]
    public float dashSpeed;
    public float dashDuration;
    public float lastDashTime;
    public GameObject trailSegmentPrefab;

    [Header("Attack")]
    public float attackDuration;
    public GameObject hitboxPrefab;
    public Transform attackPoint;
    public float atkDmg;

    [Header("Flags")]
    public bool isGrounded;
    public bool isFalling;
    public float direction;
    
    [Header("Inputs")]
    public float moveInput;
    public bool jumpPressed;
    public bool dashPressed;
    public bool attackPressed;
    private int _cycleIndex = 0;

    [Header("Cutscene")]
    public bool cutscene;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        cc = GetComponent<CapsuleCollider2D>();
        bc = GetComponent<BoxCollider2D>();

        defaultGravityScale = rb.gravityScale;

        States[PlayerStateKey.Normal] = new NormalState(PlayerStateKey.Normal, this);
        States[PlayerStateKey.Order] = new OrderState(PlayerStateKey.Order, this);
        States[PlayerStateKey.Chaos] = new ChaosState(PlayerStateKey.Chaos, this);

        CurrentState = States[PlayerStateKey.Normal];
    }

    protected override void Update()
    {
        if (Time.time < dashDuration + lastDashTime)
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
        lastDashTime = Time.time;
        rb.linearVelocity = Vector2.zero;
        anim.SetBool("isDashing", true);

        // Y by killing gravity and locking vertical velocity
        rb.gravityScale = 0f;
        rb.linearVelocityY = 0f;
    }

    private void endDash()
    {
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