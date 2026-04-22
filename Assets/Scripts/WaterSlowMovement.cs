using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class WaterSlowMovement : MonoBehaviour
{
    [Header("Water slowdown")]
    [Tooltip("Multiplier applied to the player's moveSpeed while inside water. 0.5 = half speed.")]
    [Range(0f, 1f)] [SerializeField] private float speedMultiplier = 0.5f;

    [Tooltip("Optional: set to true to also modify Rigidbody2D.drag while inside water.")]
    [SerializeField] private bool modifyDrag = true;
    [Tooltip("Drag value applied to the player's Rigidbody2D while inside water.")]
    [SerializeField] private float waterDrag = 3f;

    // Runtime state
    private PlayerController playerController;
    private Rigidbody2D playerRb;
    private float originalMoveSpeed;
    private float originalDrag;
    private int playersInsideCount = 0;

    void Reset()
    {
        // Ensure collider is trigger by default (designer convenience)
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var pc = other.GetComponent<PlayerController>();
        if (pc == null)
        {
            // fallback: if object has Player tag, try global instance
            if (other.CompareTag("Player") && PlayerController.Instance != null)
                pc = PlayerController.Instance;
            else
                return;
        }

        // allow nested colliders / multiple triggers; only apply on first
        playersInsideCount++;
        if (playersInsideCount > 1) return;

        playerController = pc;
        playerRb = playerController.rb;

        // store originals (guard against nulls)
        originalMoveSpeed = playerController.moveSpeed;
        if (playerRb != null) originalDrag = playerRb.linearDamping;

        // apply water effects
        playerController.moveSpeed = originalMoveSpeed * speedMultiplier;
        if (modifyDrag && playerRb != null) playerRb.linearDamping = waterDrag;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var pc = other.GetComponent<PlayerController>();
        if (pc == null)
        {
            if (other.CompareTag("Player") && PlayerController.Instance != null)
                pc = PlayerController.Instance;
            else
                return;
        }

        // decrement count, only restore when no colliders remain
        playersInsideCount = Mathf.Max(0, playersInsideCount - 1);
        if (playersInsideCount > 0) return;

        RestorePlayer();
    }

    private void OnDisable()
    {
        // ensure player isn't left slowed if the water object is disabled
        RestorePlayer();
    }

    private void RestorePlayer()
    {
        if (playerController != null)
        {
            playerController.moveSpeed = originalMoveSpeed;
        }
        if (modifyDrag && playerRb != null)
        {
            playerRb.linearDamping = originalDrag;
        }

        playerController = null;
        playerRb = null;
        playersInsideCount = 0;
    }
}
