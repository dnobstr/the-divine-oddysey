using UnityEngine;

public class PlayerControls : MonoBehaviour
{
    public float hp;
    public float moveSpd;
    public float jumpSpd;
    public bool onGround;

    BoxCollider2D bc;
    Rigidbody2D rb;
    void Start()
    {
        bc = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        rb.linearVelocityX = Input.GetAxisRaw("Horizontal") * moveSpd;

        if (Input.GetButtonDown("Jump") && onGround) rb.AddForceY(jumpSpd, ForceMode2D.Impulse);  
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        onGround = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        onGround = false;
    }

    public void takeDmg(float dmg)
    {
        hp -= dmg;
    }
}
