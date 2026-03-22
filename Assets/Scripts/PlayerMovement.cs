using System.Collections;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 6.5f;

    private Rigidbody2D rb;
    private Animator anim;
    private Vector3 originalScale;

    public static PlayerMovement Instance;

    // local facing direction used by Flip() and WalkIntoNewScene
    private int xAxis = 1;

    // cached reference to the player state controller (used to check cutscene)
    private PlayerController playerController;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        // Save original scale (3,3,1)
        originalScale = transform.localScale;

        // Try to cache a local PlayerController first, fall back to scene search
        playerController = GetComponent<PlayerController>();
        if (playerController == null)
            playerController = Object.FindFirstObjectByType<PlayerController>();
    }

    void Update()
    {
        if (playerController != null && playerController.cutscene) return;

        float horizontal = Input.GetAxisRaw("Horizontal");

        // Apply horizontal movement (physics velocity)
        rb.linearVelocity = new Vector2(horizontal * moveSpeed, rb.linearVelocity.y);

        anim.SetBool("IsRunning", horizontal != 0);

        // Update facing direction and flip sprite while preserving original size
        if (horizontal != 0)
        {
            xAxis = (int)Mathf.Sign(horizontal);
            Flip();
        }

        // Basic ground check using current vertical velocity
        if (Input.GetButtonDown("Jump") && Mathf.Abs(rb.linearVelocity.y) < 0.05f)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    private void Flip()
    {
        transform.localScale = new Vector3(
            xAxis * originalScale.x,
            originalScale.y,
            originalScale.z
        );
    }

    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        // If exit direction points up, give an upward velocity proportional to jumpForce
        if (_exitDir.y > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce * _exitDir.y);
        }

        // If exit direction has horizontal component, walk toward it
        if (Mathf.Abs(_exitDir.x) > 0f)
        {
            xAxis = _exitDir.x > 0f ? 1 : -1;
            rb.linearVelocity = new Vector2(xAxis * moveSpeed, rb.linearVelocity.y);
        }
                    
        // Ensure sprite is facing the direction of travel
        Flip();

        yield return new WaitForSeconds(_delay);

        // Stop horizontal motion after the walk (optional; adjust as needed)
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
    }
}