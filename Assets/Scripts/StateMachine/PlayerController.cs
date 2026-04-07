using NUnit.Framework.Interfaces;
using System;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class PlayerController : PlayerStateManager<PlayerStateKey>
{
    public Rigidbody2D rb;
    public Animator anim;
    public PolygonCollider2D pc;
    public BoxCollider2D bc;

    [Header("Movement")]
    public float moveSpeed;
    public float jumpForce;

    [Header("Dash")]
    public float dashSpeed;
    public float dashDuration;
    public float dashCooldown;
    public float lastDashTime;

    [Header("Attack")]
    public float attackDuration;
    public GameObject hitboxPrefab;
    public Transform attackPoint;
    public float atkDmg;

    [Header("Flags")]
    public bool isGrounded;
    public bool isFalling;
    public bool isFacingRight;
    
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
        pc = GetComponent<PolygonCollider2D>();
        bc = GetComponent<BoxCollider2D>();

        States[PlayerStateKey.Normal] = new NormalState(PlayerStateKey.Normal, this);
        States[PlayerStateKey.Order] = new OrderState(PlayerStateKey.Order, this);
        States[PlayerStateKey.Chaos] = new ChaosState(PlayerStateKey.Chaos, this);

        CurrentState = States[PlayerStateKey.Normal];
    }

    protected override void Update()
    {
        moveInput = Input.GetAxisRaw("Horizontal");
        jumpPressed = Input.GetButtonDown("Jump");
        dashPressed = Input.GetKeyDown(KeyCode.LeftShift);
        attackPressed = Input.GetMouseButtonDown(0);

        if (Input.GetKeyDown(KeyCode.LeftControl)) cycleState();

        falling();

        base.Update();
    }

    private void cycleState()
    {
        _cycleIndex = (_cycleIndex + 1) % 3;
        TransitionToState((PlayerStateKey)_cycleIndex);
    }

    public void move()
    {
        rb.linearVelocityX = moveSpeed * moveInput;
        handleFlip(moveInput);
        anim.SetBool("isMoving", true);
    }

    public void stopMove()
    {
        anim.SetBool("isMoving", false);
    }

    private void falling()
    {
        if (rb.linearVelocityY < 0)
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