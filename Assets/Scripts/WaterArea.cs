using UnityEngine;

public class WaterArea : MonoBehaviour
{
    [Header("Water Settings")]
    [Tooltip("Higher number = slower movement and slower falling")]
    public float waterDrag = 5f;

    // We use this to remember the player's normal drag so we can restore it later
    private float originalDrag;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the object entering the water is the Player
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                // Save their normal physics drag, then apply the heavy water drag
                originalDrag = rb.linearDamping;
                rb.linearDamping = waterDrag;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // When the player leaves the water, return their drag to normal
        if (collision.CompareTag("Player"))
        {
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.linearDamping = originalDrag;
            }
        }
    }
}