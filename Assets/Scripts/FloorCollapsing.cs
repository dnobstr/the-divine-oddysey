using UnityEngine;
using System.Collections;

public class FloorCollapsing : MonoBehaviour
{
    private Rigidbody2D rb;
    private Collider2D col;

    public float collapseDelay = 0.2f;
    public float fallGravity = 10f;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        rb.bodyType = RigidbodyType2D.Static;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Collapse());
        }
    }

    IEnumerator Collapse()
    {
        yield return new WaitForSeconds(collapseDelay);

        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = fallGravity;

        // Disable collision so player falls
        yield return new WaitForSeconds(0.1f);
        col.enabled = false;
    }
}